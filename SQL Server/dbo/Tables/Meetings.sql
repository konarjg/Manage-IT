/*
Created: 16.10.2024
Modified: 16.10.2024
Model: MS SQL
Database: MS SQL Server 2019
*/


-- Create tables section -------------------------------------------------

-- Table Projects

CREATE TABLE [Meetings]
(
 [MeetingId] Bigint IDENTITY PRIMARY KEY NOT NULL,
 [ProjectId] Bigint NOT NULL,
 [Title] Varchar(30) NOT NULL,
 [Description] Text NOT NULL,
 [Date] Date NOT NULL
)
GO
ALTER TABLE [Meetings] ADD CONSTRAINT [Meetings_Projects] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([ProjectId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
-- Add keys for table Projects
-- Create indexes for table Projects

CREATE INDEX [IX_Meetings_Projects] ON [Meetings] ([ProjectId])