use popSys
CREATE TABLE Production_Targets (
    ID INT PRIMARY KEY IDENTITY(1,1),   
    LineID VARCHAR(10) NOT NULL,
    TargetProduction INT NOT NULL,               -- ��ǥ ���귮
    OperatingTimeAvailable int NOT NULL,         -- �۵� ������ �ð�
);

use popSys
CREATE TABLE Production_Line_Status (
    ID INT PRIMARY KEY IDENTITY(1,1),          
    LineID VARCHAR(10) NOT NULL,          
    ActualProduction INT NOT NULL,               -- ���� ���귮
    EntryDate date DEFAULT cast(GETDATE() as date),        -- ��� ��¥ 
	Entrytime time DEFAULT cast(GETDATE() as time)       -- ��� �ð�  
);



use popSys
CREATE TABLE Com_Data (     -- �½���
    EntryID INT PRIMARY KEY IDENTITY(1,1), 
    Temperature FLOAT NOT NULL,               -- �µ� (����/ȭ��)
    Humidity FLOAT NOT NULL,                  -- ���� (%)
    EntryDate date DEFAULT cast(GETDATE() as date),        -- ��� ��¥ 
	Entrytime time DEFAULT cast(GETDATE() as time)       -- ��� �ð�  
);

----------------------------------------------------

use popSys
insert into Com_Data(Temperature, Humidity) values (35.2, 48.3)

insert into Production_Line_Status(LineID, ActualProduction) values ('Line1', 1200)
insert into Production_Line_Status(LineID, ActualProduction) values ('Line2', 1100)
insert into Production_Line_Status(LineID, ActualProduction) values ('Line3', 0)
insert into Production_Line_Status(LineID, ActualProduction) values ('Line4', 600)


insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line1', 1500, 1)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line2', 1200, 2)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line3', 1800, 8)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line4', 2000, 3)
