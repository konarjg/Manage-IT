﻿-- Table Users

CREATE TABLE [Users]
(
 [UserId] Bigint IDENTITY PRIMARY KEY NOT NULL,
 [Login] Varchar(20) NOT NULL,
 [Password] Text NOT NULL,
 [Email] Varchar(50) NOT NULL,
 [PrefixId] Smallint NOT NULL,
 [PhoneNumber] Varchar(15) NOT NULL
)
GO
ALTER TABLE [Users] ADD CONSTRAINT [Users_Prefixes] FOREIGN KEY ([PrefixId]) REFERENCES [Prefixes] ([PrefixId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
-- Add keys for table Users
GO
-- Create indexes for table Users

CREATE INDEX [IX8_Users_Prefixes] ON [Users] ([PrefixId])