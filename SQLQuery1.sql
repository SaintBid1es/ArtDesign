CREATE DATABASE ARTPROJECT;
USE ARTPROJECT;

CREATE TABLE Clients(
    ID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Surname VARCHAR(30) NOT NULL,
    Name VARCHAR(20) NOT NULL,
    Patronymic VARCHAR(30),
    Email VARCHAR(45) NOT NULL,
    Phone INT NOT NULL,
    HistoryOfProjects VARCHAR(50),
    NameCompany VARCHAR(30) NOT NULL
);

CREATE TABLE Roles(
    ID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    RoleName VARCHAR(30) NOT NULL
);

CREATE TABLE Employees(
    ID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Surname VARCHAR(30) NOT NULL,
    Name VARCHAR(30) NOT NULL,
    Patronymic VARCHAR(30),
    Login VARCHAR(30) NOT NULL,
    Password VARCHAR(30) NOT NULL,
    RoleId INT NOT NULL REFERENCES Roles(ID) ON DELETE CASCADE
);

CREATE TABLE Status(
    ID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    StatusName VARCHAR(30) NOT NULL
);

CREATE TABLE Project(
    ID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    ClientId INT NOT NULL REFERENCES Clients(ID),
    ProjectName VARCHAR(30) NOT NULL,
    Description VARCHAR(150) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    StatusId INT NOT NULL REFERENCES Status(ID) ON DELETE CASCADE
);

CREATE TABLE WorkStages(
    ID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    StageName VARCHAR(30) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    ProjectId INT NOT NULL REFERENCES Project(ID) ON DELETE CASCADE,
    EmployeeId INT NOT NULL REFERENCES Employees(ID) ON DELETE CASCADE
);

CREATE TABLE Files(
    ID INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    FileName VARCHAR(30) NOT NULL,
    FilePath VARCHAR(255) NOT NULL,
    ProjectId INT NULL REFERENCES Project(ID) ON DELETE CASCADE
);

-- 1. �������� ������� Roles (3 ����)
INSERT INTO Roles (RoleName)
VALUES ('��������'), ('������������'), ('��������');

-- 2. �������� ������� Clients (3 �������)
INSERT INTO Clients (Surname, Name, Patronymic, Email, Phone, HistoryOfProjects, NameCompany)
VALUES 
('������', '����', '��������', 'ivanov@test.com', 1234567, '������ �, ������ B', '��� ����'),
('�������', 'ϸ��', '����������', 'sidorov@test.com', 9876543, '������ X', '��� ���������'),
('������', '�������', '���������', 'petrov@test.com', 5555555, '������ Y', '��� �����');

-- 3. �������� ������� Employees (3 ����������, ��������� �� RoleId ����� ��������� SELECT)
INSERT INTO Employees (Surname, Name, Patronymic, Login, Password, RoleId)
VALUES
('����������', '��������', '������������', 'manager', '123', (SELECT ID FROM Roles WHERE RoleName = '��������')),
('���������', '�������', '�����������', 'admin', '123', (SELECT ID FROM Roles WHERE RoleName = '������������')),
('����������', '��������', '������������', 'designer', '123', (SELECT ID FROM Roles WHERE RoleName = '��������'));

-- 4. �������� ������� Status (2 �������)
INSERT INTO Status (StatusName)
VALUES ('�������'), ('��������');

-- 5. �������� ������� Project (3 �������, ��������� �� Clients.ID � Status.ID)
--    ��������, ������ ��� ������� �������, � ������ ��������.
INSERT INTO Project (ClientId, ProjectName, Description, StartDate, EndDate, StatusId)
VALUES
(1, '������ �', '�������� ������� �', '2024-01-01', '2024-03-31', (SELECT ID FROM Status WHERE StatusName = '�������')),
(2, '������ B', '�������� ������� B', '2024-02-01', '2024-05-15', (SELECT ID FROM Status WHERE StatusName = '�������')),
(3, '������ C', '�������� ������� C', '2024-03-10', '2024-06-20', (SELECT ID FROM Status WHERE StatusName = '��������'));

-- 6. �������� ������� WorkStages (��������� ������ �� ������ ��������)
--    ��������� �� Project.ID � Employees.ID.
--    �����������, ���:
--    - id ������� 1 (������ �) ����� ���������� � ID 1 (��������) � 3 (��������)
--    - id ������� 2 (������ B) ����� ���������� � ID 2 (������������) � 1 (��������)
--    - � �.�. � ������ ��� �����������.
INSERT INTO WorkStages (StageName, StartDate, EndDate, ProjectId, EmployeeId)
VALUES
('������������', '2024-01-01', '2024-01-15', 1, 1),
('���������� �������', '2024-01-16', '2024-02-01', 1, 3),
('�����������', '2024-02-01', '2024-02-10', 2, 2),
('������������', '2024-04-01', '2024-04-15', 2, 1),
('����� �������', '2024-06-15', '2024-06-20', 3, 2);

-- 7. �������� ������� Files (��������� ������, ��������� �� Project.ID)
INSERT INTO Files (FileName, VersionFile, ProjectId)
VALUES
('��_������_�', 'v1.0', 1),
('�����_������_B', 'v2.1', 2),
('�����_������_C', 'v1.0', 3);