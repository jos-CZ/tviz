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
    | [<Hidden>] Debug

    interface IArgParserTemplate with
      member this.Usage =
        match this with
        | File _ -> "T-SQL file to parse"
        | Query _ -> "string to parse (or use <stdin>)"
        | Token _ -> "unfold fragments containing given token index"
        | All -> "unfold all fragments"
        | Empty -> "show empty/null properties"
        | Debug -> "show debug output"

  let parse argv =
    let parser = ArgumentParser.Create<T>()
    let parsed = parser.ParseCommandLine argv
    let sql =
      match parsed.TryGetResult T.File, parsed.TryGetResult T.Query with
      | Some f, _ -> tools.readFile f
      | _, Some q -> q
      | _, _ -> tools.readStdin()
    (
      parsed.Contains T.Debug
    , parsed.Contains T.Empty
    , parsed.Contains T.All
    , parsed.GetResults T.Token
    , sql
    )
