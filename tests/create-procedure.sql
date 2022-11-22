SET QUOTED_IDENTIFIER OFF
GO

create procedure dbo.[boring procedure] @ro [int] READONLY, @i int = 2, @out int OUTPUT
with execute as caller
as
  set @out = 42
  select @ro c1, @i as c2, @out c3, "quoted" c4
