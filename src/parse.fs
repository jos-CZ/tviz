namespace jos.tviz

module parse =

  open System.IO
  open System.Collections.Generic
  open Microsoft.SqlServer.TransactSql.ScriptDom

  let parseErrorToString (e:ParseError) =
    $"line {e.Line}, column {e.Column}, message: '{e.Message}'"

  let parseErrorsToString errors =
    tools.newlineJoin (Seq.map parseErrorToString errors)

  let parseString s =
    let parser = TSql160Parser(true)
    use reader = new StringReader(s) :> TextReader
    let mutable errors : IList<_> = Unchecked.defaultof<IList<_>>
    match parser.Parse(reader, &errors) with
    | res when errors.Count = 0 -> res
    | _ -> failwith $"parse error\n{parseErrorsToString errors}"
  
  let batchVisitor (rx:ResizeArray<_>) = {
    new TSqlFragmentVisitor() with
    override this.Visit(b:TSqlBatch) = rx.Add(b)
  }

  let parseBatches s =
    let bx = ResizeArray<TSqlBatch>()
    let parsed = parseString s
    parsed.Accept(batchVisitor bx)
    bx :> seq<_>
