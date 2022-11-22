namespace jos.tviz

module main =

  [<EntryPoint>]
  let main argv =
    try
      let debug, showEmpty, unfoldAll, tokens, sql = args.parse argv
      let print = ast.astPrinter debug showEmpty
      let create = ast.astCreator unfoldAll tokens
      parse.parseBatches sql
      |> Seq.mapi (fun i b -> create $"{ast.firstLevel}{i}" b.Statements)
      |> Seq.iter print
      0
    with
    | tools.NoSuchFile f ->
      tools.printError $"file not found: {f}"
      1
    | :? Argu.ArguParseException as e ->
      tools.printError e.Message
      2
