tviz: T-SQL AST visualiser
==========================

`tviz` is simple commandline T-SQL abstract syntax tree visualiser,
based on DacFx/ScriptDom library (https://github.com/microsoft/DacFx)
and reflection.

Inspired by https://github.com/GoEddie/ScriptDomVisualizer

    USAGE: tviz.exe [--help] [--file <path>] [--query <T-SQL>] [--token <index>] [--all] [--empty] [--stream]

    OPTIONS:

        --file, -f <path>     T-SQL file to parse
        --query, -q <T-SQL>   string to parse (or use <stdin>)
        --token, -t <index>   unfold fragments containing given token index
        --all                 unfold all fragments
        --empty               show empty/null properties
        --stream              instead of AST, print token stream
        --help                display this list of options.

Source precedence is `--file`, `--query`, `<stdin>`.

Parser is TSql160Parser.

Examples
--------

Top level statements are listed by default:

    $ dotnet run --project tviz.fsproj -- --query "select 0; exec [some proc]"
    batch #0 [TSqlStatement[2]] [0..7]
    ├ #0 [SelectStatement] [0..3] line 1, col 1, offset 0, len 9
    └ #1 [ExecuteStatement] [5..7] line 1, col 11, offset 10, len 16

Two statements in separated batches:

    $ dotnet run --project tviz.fsproj -- << EOF
    select 0
    GO
    exec [some proc]
    EOF
    batch #0 [TSqlStatement[1]] [0..2]
    └ #0 [SelectStatement] [0..2] line 1, col 1, offset 0, len 8
    batch #1 [TSqlStatement[1]] [6..8]
    └ #0 [ExecuteStatement] [6..8] line 3, col 1, offset 12, len 16

Notation [TSqlStatement[2]] represents collection of TSqlStatement with length 2,
[0..7] is span of token indexes in parsed token stream.

Use `--all` to display full AST:

    $ dotnet run --project tviz.fsproj -- --query "select distinct 1" --all
    batch #0 [TSqlStatement[1]] [0..4]
    └ #0 [SelectStatement] [0..4] line 1, col 1, offset 0, len 17
      └ QueryExpression [QuerySpecification] [0..4] line 1, col 1, offset 0, len 17
        ├ UniqueRowFilter [UniqueRowFilter] Distinct
        └ SelectElements [SelectElement[1]] [4..4]
          └ #0 [SelectScalarExpression] [4..4] line 1, col 17, offset 16, len 1
            └ Expression [IntegerLiteral] [4..4] line 1, col 17, offset 16, len 1
              ├ LiteralType [LiteralType] Integer
              └ Value [String] 1

Use `--token <index>` (or `-t` <index>) to display only parts of AST.
`--token` can be specified multiple times.

    $ dotnet run --project tviz.fsproj -- --query "select 0, 1, 2" --token 0
    batch #0 [TSqlStatement[1]] [0..8]
    └ #0 [SelectStatement] [0..8] line 1, col 1, offset 0, len 14
      └ QueryExpression [QuerySpecification] [0..8] line 1, col 1, offset 0, len 14
        ├ UniqueRowFilter [UniqueRowFilter] NotSpecified
        └ SelectElements [SelectElement[3]] [2..8]

If given token index points to first item in a list, whole list is printed:

    $ dotnet run --project tviz.fsproj -- --query "select 0, 1, 2" --token 2
    batch #0 [TSqlStatement[1]] [0..8]
    └ #0 [SelectStatement] [0..8] line 1, col 1, offset 0, len 14
      └ QueryExpression [QuerySpecification] [0..8] line 1, col 1, offset 0, len 14
        ├ UniqueRowFilter [UniqueRowFilter] NotSpecified
        └ SelectElements [SelectElement[3]] [2..8]
          ├ #0 [SelectScalarExpression] [2..2] line 1, col 8, offset 7, len 1
          | └ Expression [IntegerLiteral] [2..2] line 1, col 8, offset 7, len 1
          |   ├ LiteralType [LiteralType] Integer
          |   └ Value [String] 0
          ├ #1 [SelectScalarExpression] [5..5] line 1, col 11, offset 10, len 1
          └ #2 [SelectScalarExpression] [8..8] line 1, col 14, offset 13, len 1

Same statement, displaying second and third element of select list:

    $ dotnet run --project tviz.fsproj -- --query "select 0, 1, 2" --token 5 --token 8
    batch #0 [TSqlStatement[1]] [0..8]
    └ #0 [SelectStatement] [0..8] line 1, col 1, offset 0, len 14
      └ QueryExpression [QuerySpecification] [0..8] line 1, col 1, offset 0, len 14
        ├ UniqueRowFilter [UniqueRowFilter] NotSpecified
        └ SelectElements [SelectElement[3]] [2..8]
          ├ #1 [SelectScalarExpression] [5..5] line 1, col 11, offset 10, len 1
          | └ Expression [IntegerLiteral] [5..5] line 1, col 11, offset 10, len 1
          |   ├ LiteralType [LiteralType] Integer
          |   └ Value [String] 1
          └ #2 [SelectScalarExpression] [8..8] line 1, col 14, offset 13, len 1
            └ Expression [IntegerLiteral] [8..8] line 1, col 14, offset 13, len 1
              ├ LiteralType [LiteralType] Integer
              └ Value [String] 2

Some TSqlFragments can display unexpected values, like StatementList here:

    $ dotnet run --project tviz.fsproj -- --token 0 << EOF
    begin
    exec [some proc]
    end
    EOF
    batch #0 [TSqlStatement[1]] [0..6]
    └ #0 [BeginEndBlockStatement] [0..6] line 1, col 1, offset 0, len 26
      └ StatementList [StatementList] [-1..-1] line -1, col -1, offset -1, len -1
        └ Statements [TSqlStatement[1]] [2..4]

Empty lists and <null>s are not displayed by default, use `--empty` to show them.

Basic listing:

    $ dotnet run --project tviz.fsproj -- --query "select 0, 1, 2" --token 0
    batch #0 [TSqlStatement[1]] [0..8]
      └ #0 [SelectStatement] [0..8] line 1, col 1, offset 0, len 14
        └ QueryExpression [QuerySpecification] [0..8] line 1, col 1, offset 0, len 14
          ├ UniqueRowFilter [UniqueRowFilter] NotSpecified
          └ SelectElements [SelectElement[3]] [2..8]

With `--empty`:

    $ dotnet run --project tviz.fsproj -- --query "select 0, 1, 2" --token 0 --empty
    batch #0 [TSqlStatement[1]] [0..8]
    └ #0 [SelectStatement] [0..8] line 1, col 1, offset 0, len 14
      ├ QueryExpression [QuerySpecification] [0..8] line 1, col 1, offset 0, len 14
      | ├ UniqueRowFilter [UniqueRowFilter] NotSpecified
      | ├ TopRowFilter [TopRowFilter] NULL
      | ├ SelectElements [SelectElement[3]] [2..8]
      | ├ FromClause [FromClause] NULL
      | ├ WhereClause [WhereClause] NULL
      | ├ GroupByClause [GroupByClause] NULL
      | ├ HavingClause [HavingClause] NULL
      | ├ WindowClause [WindowClause] NULL
      | ├ OrderByClause [OrderByClause] NULL
      | ├ OffsetClause [OffsetClause] NULL
      | └ ForClause [ForClause] NULL
      ├ Into [SchemaObjectName] NULL
      ├ On [Identifier] NULL
      ├ ComputeClauses [ComputeClause[0]]
      ├ WithCtesAndXmlNamespaces [WithCtesAndXmlNamespaces] NULL
      └ OptimizerHints [OptimizerHint[0]]

Use `--stream` to print token stream:

    $ dotnet run --project tviz.fsproj -- --stream --query "/* comment */ select 'c'"
    [TSqlBatch] [2..4] line 1, col 15, offset 14, len 10
    [SelectStatement] [2..4] line 1, col 15, offset 14, len 10
    #0 line 1, col 1, offset 0 [MultilineComment] '/* comment */'
    #1 line 1, col 14, offset 13 [WhiteSpace] ' '
    #2 line 1, col 15, offset 14 [Select] 'select'
    #3 line 1, col 21, offset 20 [WhiteSpace] ' '
    #4 line 1, col 22, offset 21 [AsciiStringLiteral] ''c''
    #5 line 1, col 25, offset 24 [EndOfFile] ''

You can limit printed tokens by passing `--token`:

    $ dotnet run --project tviz.fsproj -- --stream --query "/* just a comment */ select 'c'" -t 0 -t 2 -t 4
    [TSqlBatch] [2..4] line 1, col 22, offset 21, len 10
    [SelectStatement] [2..4] line 1, col 22, offset 21, len 10
    #0 line 1, col 1, offset 0 [MultilineComment] '/* just a comment */'
    #2 line 1, col 22, offset 21 [Select] 'select'
    #4 line 1, col 29, offset 28 [AsciiStringLiteral] ''c''

Browse directory `tests` for more examples.
