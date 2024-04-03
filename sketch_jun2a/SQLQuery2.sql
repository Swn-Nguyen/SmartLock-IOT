--create database SQL_login
--go

use SQL_login
go

--t///ao bang
create table AccessLog
(
	LogID int primary key identity(1,1),

	TenUser nvarchar(10)  null,
	foreign key(TenUser) references dbo.DataUser(TenUser),
	
	TimeAccess datetime  null,
	AccData text  null

)
create table DataUser
(
	idUser int identity(1,1),
	TenUser nvarchar(10)  not null,
	primary key(TenUser),
	RFID varchar(15)  null
)
alter table DataUser add  RFID varchar(15)

insert into DataUser(TenUser, RFID)
values(N'Master','51 254 103 189')
(N'Mẹ', '41 80 225 163');
delete from DataUser where TenUser = N'Mẹ'
select DataUser.TenUser
from DataUser



WHERE DataUser.RFID = '179 179 235 16';
 --sua bang them cot: alter table ... add...
 --xoa du lieu bang: truncate table ...
 --xoa bang:
 drop table DataUser
 
 