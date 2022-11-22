
create view dbo.[breathtaking view]
as
select s.c1, c2, c3
from (
  select 0 c1, 1 as c2, 2 c3
  from sys.objects
  where 1 = 0
) s
