use popSys
CREATE TABLE Production_Targets (
    ID INT PRIMARY KEY IDENTITY(1,1),     -- ������ ���ڵ� �ĺ���
    LineID VARCHAR(10) NOT NULL,                -- ���� ���� �ĺ��� (���� ��ȣ)
    TargetProduction INT NOT NULL,               -- ��ǥ ���귮
    OperatingTimeAvailable int NOT NULL,         -- �۵� ������ �ð�
);

use popSys
CREATE TABLE Production_Line_Status (
    ID INT PRIMARY KEY IDENTITY(1,1),          -- ������ ���ڵ� �ĺ���
    LineID VARCHAR(10) NOT NULL,                -- ���� ���� �ĺ��� (���� ��ȣ)
    ActualProduction INT NOT NULL,               -- ���� ���귮
    EntryDate date DEFAULT cast(GETDATE() as date),        -- ��� ��¥ 
	Entrytime time DEFAULT cast(GETDATE() as time)       -- ��� �ð�  
);



use popSys
CREATE TABLE Com_Data (
    EntryID INT PRIMARY KEY IDENTITY(1,1),  -- ������ ���ڵ� �ĺ���
    Temperature FLOAT NOT NULL,               -- �µ� (����/ȭ��)
    Humidity FLOAT NOT NULL,                  -- ���� (%)
    EntryDate date DEFAULT cast(GETDATE() as date),        -- ��� ��¥ 
	Entrytime time DEFAULT cast(GETDATE() as time)       -- ��� �ð�  
);
-- ���̺� ����
----------------------------------------------------
-- ������ �߰�
use popSys
insert into Com_Data(Temperature, Humidity) values (35.2, 48.3)

insert into Production_Line_Status(LineID, ActualProduction) values ('Line1', 1200)
insert into Production_Line_Status(LineID, ActualProduction) values ('Line2', 1100)
insert into Production_Line_Status(LineID, ActualProduction) values ('Line3', 800)
insert into Production_Line_Status(LineID, ActualProduction) values ('Line4', 600)


insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line1', 1500, 8)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line2', 1200, 10)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line3', 1800, 6)
insert into Production_Targets(LineID, TargetProduction, OperatingTimeAvailable) values ('Line4', 2000, 11)



select LineID, max(ActualProduction) as LineMaxPro from Production_Line_Status group by LineID

use popSys
select * from Production_Line_Status  --���귮
select * from Production_Targets	-- �޼���
select * from Com_Data

truncate table Production_Line_Status
drop table Production_Line_Status


delete from Com_Data where EntryDate != '2024-09-26'

-- T.targetproduction, T.OperatingTimeAvailable, L.actualproduction
select Top 1 T.targetproduction, T.OperatingTimeAvailable from production_targets as T inner join production_line_status as L on T.LineID = L.LineID where T.Lineid = 'line2' order by L.actualproduction desc




select min(Entrytime) as star, max(Entrytime) as cur from Production_Line_Status where LineID = 'Line1' group by LineID


select top 1 Temperature, Humidity from Com_Data order by EntryDate Desc


select TOP 1 ActualProduction, EntryDate from Production_Line_Status where LineID = 'Line1' order by Entrytime DESC


select * from Production_Line_Status  --���귮