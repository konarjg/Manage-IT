/*
Created: 16.10.2024
Modified: 16.10.2024
Model: MS SQL
Database: MS SQL Server 2019
*/


-- Create tables section -------------------------------------------------

-- Table Projects

CREATE TABLE [Projects]
(
 [ProjectId] Bigint IDENTITY PRIMARY KEY NOT NULL,
 [Name] Varchar(30) NOT NULL,
 [Description] Text NOT NULL,
 [ManagerId] Bigint NOT NULL
)
GO
ALTER TABLE [Projects] ADD CONSTRAINT [Users_Project(manager?)] FOREIGN KEY ([ManagerId]) REFERENCES [Users] ([UserId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
-- Add keys for table Projects
-- Create indexes for table Projects

CREATE INDEX [IX_Users_Projects(manager?)] ON [Projects] ([ManagerId])