#!/usr/bin/env sh

dotnet run --project tviz.fsproj -- --file tests/simple-select.sql --all > tests/simple-select.all.txt
dotnet run --project tviz.fsproj -- --file tests/simple-select.sql --all --empty > tests/simple-select.all.empty.txt
dotnet run --project tviz.fsproj -- --file tests/simple-select.sql -t 3 > tests/simple-select.3.txt
dotnet run --project tviz.fsproj -- --file tests/simple-select.sql -t 4 > tests/simple-select.4.txt
dotnet run --project tviz.fsproj -- --file tests/simple-select.sql -t 11 > tests/simple-select.11.txt
dotnet run --project tviz.fsproj -- --file tests/simple-select.sql -t 11 --empty > tests/simple-select.11.empty.txt
dotnet run --project tviz.fsproj -- --file tests/simple-select.sql -t 15 > tests/simple-select.15.txt
dotnet run --project tviz.fsproj -- --file tests/simple-select.sql --stream > tests/simple-select.stream.txt
dotnet run --project tviz.fsproj -- --file tests/simple-select.sql --stream -t 0 -t 1 -t 12 -t 19 > tests/simple-select.stream.txt

dotnet run --project tviz.fsproj -- --file tests/create-table.sql --all > tests/create-table.all.txt
dotnet run --project tviz.fsproj -- --file tests/create-table.sql --all --empty > tests/create-table.all.empty.txt
dotnet run --project tviz.fsproj -- --file tests/create-table.sql --stream > tests/create-table.stream.txt

dotnet run --project tviz.fsproj -- --file tests/create-inline-function.sql --all > tests/create-inline-function.all.txt
dotnet run --project tviz.fsproj -- --file tests/create-inline-function.sql --all --empty > tests/create-inline-function.all.empty.txt
dotnet run --project tviz.fsproj -- --file tests/create-inline-function.sql --stream > tests/create-inline-function.stream.txt

dotnet run --project tviz.fsproj -- --file tests/create-scalar-function.sql --all > tests/create-scalar-function.all.txt
dotnet run --project tviz.fsproj -- --file tests/create-scalar-function.sql --all --empty > tests/create-scalar-function.all.empty.txt
dotnet run --project tviz.fsproj -- --file tests/create-scalar-function.sql --stream > tests/create-scalar-function.stream.txt

dotnet run --project tviz.fsproj -- --file tests/create-table-valued-function.sql --all > tests/create-table-valued-function.all.txt
dotnet run --project tviz.fsproj -- --file tests/create-table-valued-function.sql --all --empty > tests/create-table-valued-function.all.empty.txt
dotnet run --project tviz.fsproj -- --file tests/create-table-valued-function.sql --stream > tests/create-table-valued-function.stream.txt

dotnet run --project tviz.fsproj -- --file tests/create-procedure.sql --all > tests/create-procedure.all.txt
dotnet run --project tviz.fsproj -- --file tests/create-procedure.sql --all --empty > tests/create-procedure.all.empty.txt
dotnet run --project tviz.fsproj -- --file tests/create-procedure.sql --stream > tests/create-procedure.stream.txt

dotnet run --project tviz.fsproj -- --file tests/create-view.sql --all > tests/create-view.all.txt
dotnet run --project tviz.fsproj -- --file tests/create-view.sql --all --empty > tests/create-view.all.empty.txt
dotnet run --project tviz.fsproj -- --file tests/create-view.sql --stream > tests/create-view.stream.txt
