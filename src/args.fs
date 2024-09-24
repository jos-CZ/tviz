namespace jos.tviz

module args =

  open Argu

  type
    T =
    | [<AltCommandLine("-f")>] File of path:string
    | [<AltCommandLine("-q")>] Query of ``T-SQL``:string
    | [<AltCommandLine("-t")>] Token of index:int
    | All
    | Empty
    | Stream
    | [<Hidden>] Debug

    interface IArgParserTemplate with
      member this.Usage =
        match this with
        | File _ -> "T-SQL file to parse"
        | Query _ -> "string to parse (or use <stdin>)"
        | Token _ -> "unfold fragments containing given token index"
        | All -> "unfold all fragments"
        | Empty -> "show empty/null properties"
        | Stream -> "instead of AST, print token stream"
        | Debug -> "show debug output"

  let sql (parsed:ParseResults<T>) =
    match parsed.TryGetResult T.File, parsed.TryGetResult T.Query with
    | Some f, _ -> tools.readFile f
    | _, Some q -> q
    | _, _ -> tools.readStdin()

  let (|PrintAST|_|) (parsed:ParseResults<T>) =
    if parsed.Contains T.Stream then
      None
    else
      Some (
        parsed.Contains T.Debug
      , parsed.Contains T.Empty
      , parsed.Contains T.All
      , parsed.GetResults T.Token
      , sql parsed
      )

  let (|PrintStream|_|) (parsed:ParseResults<T>) =
    if parsed.Contains T.Stream then
      Some (parsed.GetResults T.Token, sql parsed)
    else
      None

  let parse argv =
    let parser = ArgumentParser.Create<T>()
    parser.ParseCommandLine argv
