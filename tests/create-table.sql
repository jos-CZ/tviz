
create table dbo.[coffee table] (
  id int not null
, c char(2)
, d datetime null constraint df default current_timestamp
, p as coalesce(c, 'n/a') persisted not null
, g uniqueidentifier rowguidcol not null
, constraint pk primary key nonclustered (id)
, constraint fk foreign key (id) references dbo.[coffee table] (id)
)

GO

create index ix on dbo.[coffee table] (id, p desc)
