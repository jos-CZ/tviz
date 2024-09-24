namespace jos.tviz

module main =

  [<EntryPoint>]
  let main argv =
    try
      match args.parse argv with
      | args.PrintAST (debug, showEmpty, unfoldAll, tokens, sql) ->
        let create = ast.astCreator unfoldAll tokens
        let print = ast.astPrinter debug showEmpty
        parse.parseBatches sql
        |> Seq.mapi (fun i b -> create $"{ast.firstLevel}{i}" b.Statements)
        |> Seq.iter print
      | args.PrintStream (tokens, sql) ->
        let batches = parse.parseBatches sql
        for b in batches do
          ast.fragmentDescription b |> tools.print
          for s in b.Statements do
            ast.fragmentDescription s |> tools.print
        match Seq.tryHead batches with
        | Some b ->
          Seq.indexed b.ScriptTokenStream
          |> Seq.filter (ast.tokenStreamFilter tokens)
          |> Seq.iter (fun (i, t) -> tools.print $"#{i} {ast.tokenDescription t}")
        | None -> tools.printError "no batches"
      | _ -> failwith "dead end"
      0
    with
    | tools.NoSuchFile f ->
      tools.printError $"file not found: {f}"
      1
    | :? Argu.ArguParseException as e ->
      tools.printError e.Message
      2
