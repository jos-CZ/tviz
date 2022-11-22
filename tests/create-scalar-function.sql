
create function [just for fun] (@i int)
returns bigint
with schemabinding, inline = off, execute as 'guest', returns null on null input
begin
  return cast(@i as bigint)
end
