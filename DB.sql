CREATE TABLE USERS(
Id number(10) NOT NULL PRIMARY KEY, 
Full_Name nvarchar2(50) NOT NULL,
Login nvarchar2(50) NOT NULL,
Password nvarchar2(50) NOT NULL,
Status nvarchar2(10)
);


CREATE TABLE Roat(
Id number(10) NOT NULL PRIMARY KEY, 
Id_User number(10) NOT NULL,
Name nvarchar2(50) NOT NULL,
Country nvarchar2(20) NOT NULL,
City nvarchar2(20) NOT NULL,
Duration_Day number NOT NULL,
Type nvarchar2(10),
Description nvarchar2(50),
CONSTRAINT PK_Roat FOREIGN KEY (Id_User) REFERENCES Users(Id)
);

SELECT name,Country,City,Duration_Day,type,Description FROM roat INNER JOIN USERS ON users.ID=ID_User
ORDER BY Duration_Day;
/*
ORDER BY City;
ORDER BY Country;
ORDER BY Type;
*/

CREATE TABLE Waypoint(
Id number(10) NOT NULL PRIMARY KEY, 
id_Roat number(10) NOT NULL,
Name nvarchar2(50),
Coordinates nvarchar2(50) NOT NULL,
Description nvarchar2(50),
Image nvarchar2(50),
CONSTRAINT PK_Waypoint_Roat FOREIGN KEY (id_Roat) REFERENCES Roat(Id) 
);

SELECT name,Coordinates,Description,Image FROM Waypoint INNER JOIN Roat ON Roat.ID=id_Roat;


CREATE TABLE Comments(
Id number(10) NOT NULL PRIMARY KEY, 
Id_User number(10) NOT NULL,
Id_Roat number(10) NOT NULL,
Description nvarchar2(100) NOT NULL,
CONSTRAINT PK_Comments_Id_User FOREIGN KEY (Id_User) REFERENCES Users(Id),
CONSTRAINT PK_Comments_Id_Roat FOREIGN KEY (Id_Roat) REFERENCES Roat(Id)
);

create or replace table Likes(
Id number(10) NOT NULL PRIMARY KEY, 
Id_User number(10) NOT NULL,
Id_Roat number(10) NOT NULL,
[Like] number NOT NULL,
CONSTRAINT PK_Likes_Id_User FOREIGN KEY (Id_User) REFERENCES Users(Id),
CONSTRAINT PK_Likes_Id_Roat FOREIGN KEY (Id_Roat) REFERENCES Roat(Id)
);

CREATE TABLE Reports(
Id number(10) NOT NULL PRIMARY KEY, 
Id_User number(10) NOT NULL,
Id_User_Moderator number(10),
Id_Roat number(10) NOT NULL,
Description nvarchar2(100) NOT NULL,
CONSTRAINT PK_Comments_Id_User FOREIGN KEY (Id_User) REFERENCES Users(Id),
CONSTRAINT PK_Comments_Id_Roat FOREIGN KEY (Id_Roat) REFERENCES Roat(Id),
CONSTRAINT PK_Comments_Id_User_Moderator FOREIGN KEY (Id_User_Moderator) REFERENCES Users(Id)
);
