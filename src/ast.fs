namespace jos.tviz

module ast =

  open Microsoft.SqlServer.TransactSql.ScriptDom

  type AST =
  | Leaf of string
  | Empty of string
  | Debug of string
  | Node of string * AST list

  let astFilter debug showEmpty =
    function
    | Empty _ when not showEmpty -> false
    | Debug _ when not debug -> false
    | Node (_, lst) when List.length lst = 0 && not showEmpty -> false
    | _ -> true

  let rec printItems printer prefix =
    function
    | [] -> ()
    | [last] -> printer $"{prefix}└ " last
    | head::rest ->
      printer $"{prefix}├ " head
      printItems printer prefix rest

  let astPrinter debug showEmpty =
    let filter = astFilter debug showEmpty
    let mkPrefix = String.map (function | '└' -> ' ' | '├' -> '|' | c -> c)
    let rec printAst prefix =
      function
      | Leaf l | Empty l | Debug l ->
        tools.print $"{prefix}{l}"
      | Node (l, tree) ->
        tools.print $"{prefix}{l}"
        List.filter filter tree |> printItems printAst (mkPrefix prefix)
    printAst ""

  let firstLevel = "batch #"

  let skippedProps = [
    "StartLine"
    "StartColumn"
    "StartOffset"
    "FragmentLength"
    "FirstTokenIndex"
    "LastTokenIndex"
    "ScriptTokenStream"
  ]

  let tokenStreamFilter tokens =
    if List.length tokens = 0 then
      fun _ -> true
    else
      fun (i, _) -> List.contains i tokens

  let tokenDescription (t:TSqlParserToken) =
    $"line {t.Line}, col {t.Column}, offset {t.Offset} [{t.TokenType}] '{t.Text}'"

  let fragmentDescription (f:TSqlFragment) =
    $"[{f.GetType().Name}] [{f.FirstTokenIndex}..{f.LastTokenIndex}] "
    + $"line {f.StartLine}, col {f.StartColumn}, "
    + $"offset {f.StartOffset}, len {f.FragmentLength}"

  let (|Frag|_|) (o:System.Object) =
    match o with
    | :? TSqlFragment as f -> Some (fragmentDescription f, f)
    | _ -> None

  let (|List|_|) (o:System.Object) =
    match o with
    | :? System.Collections.IEnumerable as items when o.GetType().GenericTypeArguments.Length > 0 ->
      let lst = Seq.cast items |> Seq.map (fun i -> i :> System.Object) |> List.ofSeq
      Some ($"[{o.GetType().GenericTypeArguments[0].Name}[{List.length lst}]]", lst)
    | _ -> None

  let (|FragList|_|) (o:System.Object) =
    match o with
    | List (label, items) when List.length items > 0 ->
      match List.head items with
      | :? TSqlFragment ->
        let fx = items |> List.map (fun f -> f :?> TSqlFragment)
        let first, last = (List.head fx).FirstTokenIndex, (List.last fx).LastTokenIndex
        Some ($"{label} [{first}..{last}]", first, last, fx)
      | _ -> None
    | _ -> None

  let walkFrag astCreator (f:TSqlFragment) = [
    for p in f.GetType().GetProperties() do
      if not p.CanRead then
        yield Debug $"{p.Name} [{p.PropertyType.Name}] UNREADABLE"
      elif p.GetIndexParameters().Length > 0 then
        yield Debug $"{p.Name} [{p.PropertyType.Name}[]] INDEXER"
      elif not (List.contains p.Name skippedProps) then
        match Option.ofObj (p.GetValue(f)) with
        | Some value -> yield astCreator p.Name value
        | None -> yield Empty $"{p.Name} [{p.PropertyType.Name}] NULL"
  ]

  let unfoldFrag tokens (f:TSqlFragment) =
    (f.FirstTokenIndex = -1 && f.LastTokenIndex = -1)
    || List.exists (fun i -> f.FirstTokenIndex <= i && f.LastTokenIndex >= i) tokens

  let unfoldList first last tokens =
    List.exists (fun i -> first <= i && last >= i) tokens

  let walkList' filter astCreator items =
    items |> List.indexed |> List.filter filter |> List.map (fun (i, f) -> astCreator $"#{i}" f)

  let walkList astCreator items = walkList' (fun _ -> true) astCreator items

  let astCreator unfoldAll tokens =
    let rec createAst (label:string) (o:System.Object) =
      match o with
      | Frag (fragLabel, f) when unfoldAll || unfoldFrag tokens f ->
        Node ($"{label} {fragLabel}", walkFrag createAst f)
      | Frag (fragLabel, _) ->
        Leaf $"{label} {fragLabel}"
      | FragList (listLabel, first, _, items) when unfoldAll || List.contains first tokens || label.StartsWith(firstLevel) ->
        Node ($"{label} {listLabel}", walkList createAst items)
      | FragList (listLabel, first, last, items) when unfoldList first last tokens ->
        let filter (_, f) = unfoldAll || unfoldFrag tokens f
        Node ($"{label} {listLabel}", walkList' filter createAst items)
      | FragList (listLabel, _, _, _) ->
        Leaf $"{label} {listLabel}"
      | List (listLabel, items) ->
        Node ($"{label} {listLabel}", walkList createAst items)
      | o ->
        Leaf $"{label} [{o.GetType().Name}] {o}"
    createAst
