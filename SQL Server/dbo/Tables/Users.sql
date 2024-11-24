-- Table Users

CREATE TABLE [Users]
(
 [UserId] Bigint IDENTITY PRIMARY KEY NOT NULL,
 [Login] Varchar(20) NOT NULL,
 [Password] Text NOT NULL,
 [Email] Text NOT NULL,
 [PrefixId] Smallint NOT NULL,
 [PhoneNumber] Text NOT NULL,
 [Verified] Bit NOT NULL,
 [Admin] Bit NOT NULL,
)
GO
ALTER TABLE [Users] ADD CONSTRAINT [Users_Prefixes] FOREIGN KEY ([PrefixId]) REFERENCES [Prefixes] ([PrefixId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
-- Add keys for table Users
GO
-- Create indexes for table Users

CREATE INDEX [IX8_Users_Prefixes] ON [Users] ([PrefixId])