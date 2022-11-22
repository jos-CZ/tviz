SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

create function [big fun] (@i int, @c char(1) null)
returns @rv table (d datetime not null unique, i int null, c char(1))
with schemabinding
begin
  insert into @rv (d, i)
  select current_timestamp, @i + 1, coalesce(@c, 'c')
  return
end
