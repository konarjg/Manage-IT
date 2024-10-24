-- Table UserPermissions

CREATE TABLE [UserPermissions]
(
 [UserId] Bigint NOT NULL,
 [ProjectId] Bigint NOT NULL,
 [Editing] Bit NOT NULL,
 [InvitingMembers] Bit NOT NULL,
 [KickingMembers] Bit NOT NULL
)
GO
ALTER TABLE [UserPermissions] ADD CONSTRAINT [Users_UserPermissions] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
ALTER TABLE [UserPermissions] ADD CONSTRAINT [Projects_UserPermissions] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([ProjectId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
-- Create indexes for table UserPermissions

CREATE INDEX [IX_Users_UserPermissions] ON [UserPermissions] ([UserId])
GO
CREATE INDEX [IX_Projects_UserPermissions] ON [UserPermissions] ([ProjectId])