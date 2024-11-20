use popSys
CREATE TABLE Production_Targets (
    ID int primary key identity(1,1),   
    LineID varchar(10) not null,
    TargetProduction int not null,
    OperatingTimeAvailable int not null
);

use popSys
CREATE TABLE Production_Line_Status (
    ID int primary key identity(1,1),         
    LineID varchar(10) not null,       
    ActualProduction int not null,               -- ���� ���귮
    EntryDate date DEFAULT cast(GETDATE() as date),        -- ��� ��¥ 
	Entrytime time DEFAULT cast(GETDATE() as time)       -- ��� �ð�  
);



use popSys
CREATE TABLE Com_Data (     -- �½���
    EntryID int primary key identity(1,1), 
    Temperature float not null,
    Humidity float not null,
    EntryDate date DEFAULT cast(GETDATE() as date),        -- ��� ��¥ 
	Entrytime time DEFAULT cast(GETDATE() as time)       -- ��� �ð�  
);

use popSys
CREATE TABLE Total_pro (     -- �Ѵ� ���귮
    EntryID int primary key identity(1,1), 
	Line1 int,
	Line2 int,
	Line3 int,
	Line4 int,
	EntryDate INT DEFAULT CAST(CONVERT(VARCHAR(6), GETDATE(), 112) AS INT)
);



----------------------------------------------------

use popSys
insert into Com_Data(Temperature, Humidity) values (35.2, 48.3)

insert into Production_Line_Status(LineID, ActualProduction) values ('Line1', 1250)
insert into Production_Line_Status(LineID, ActualProduction) values ('Line2', 1117)
insert into Production_Line_Status(LineID, ActualProduction) values ('Line3', 0)
insert into Production_Line_Status(LineID, ActualProduction) values ('Line4', 610)



insert into Total_pro(Line1, Line2, Line3, Line4, EntryDate) values (2500, 1500, 450, 800, 202411)
update Total_pro set Line1 = Line1 + 100

select
-- �� �� -> ������ ������Ʈ -> �ٸ��� �����


insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line1', 1500, 1)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line2', 1200, 2)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line3', 1800, 8)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line4', 2000, 3)


select * from Production_Targets