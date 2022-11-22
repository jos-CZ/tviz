namespace jos.tviz

module tools =

  open System

  exception NoSuchFile of string

  let printError (message:string) = Console.Error.WriteLine(message)

  let print (message:string) = Console.WriteLine(message)

  let join glue (strings:string seq) = String.Join(glue, strings)

  let newlineJoin strings = join "\n" strings

  let readFile f =
    if IO.File.Exists f then
      IO.File.ReadAllText f
    else
      raise (NoSuchFile f)

  let readStdin _ =
    Seq.initInfinite (fun _ -> Console.ReadLine())
    |> Seq.takeWhile (function null -> false | _ -> true)
    |> Seq.toList
    |> newlineJoin
