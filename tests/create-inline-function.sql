
create function dbo.[funny fun] (@i int, @c char(1))
returns table
with schemabinding
return
  with d as (
    select @i c1, @c c2
    union
    select 1, 'x'
  )
  select c1, dd.c2
  from d as dd
