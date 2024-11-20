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
    ActualProduction int not null,               -- 실제 생산량
    EntryDate date DEFAULT cast(GETDATE() as date),        -- 기록 날짜 
	Entrytime time DEFAULT cast(GETDATE() as time)       -- 기록 시간  
);



use popSys
CREATE TABLE Com_Data (     -- 온습도
    EntryID int primary key identity(1,1), 
    Temperature float not null,
    Humidity float not null,
    EntryDate date DEFAULT cast(GETDATE() as date),        -- 기록 날짜 
	Entrytime time DEFAULT cast(GETDATE() as time)       -- 기록 시간  
);

use popSys
CREATE TABLE Total_pro (     -- 한달 생산량
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
-- 달 비교 -> 같으면 업데이트 -> 다르면 지우기


insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line1', 1500, 1)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line2', 1200, 2)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line3', 1800, 8)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line4', 2000, 3)


select * from Production_Targets