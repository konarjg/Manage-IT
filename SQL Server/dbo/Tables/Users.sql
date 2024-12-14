-- Table Users

CREATE TABLE [Users]
(
 [UserId] Bigint IDENTITY PRIMARY KEY NOT NULL,
 [Login] Varchar(20) NOT NULL,
 [Password] Text NOT NULL,
 [Email] Text NOT NULL,
 [Verified] Bit NOT NULL,
 [Admin] Bit NOT NULL,
)
GO