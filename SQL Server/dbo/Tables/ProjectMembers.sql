-- Table ProjectMembers

CREATE TABLE [ProjectMembers]
(
 [UserId] Bigint NOT NULL,
 [ProjectId] Bigint NOT NULL
)
GO
ALTER TABLE [ProjectMembers] ADD CONSTRAINT [Users_ProjectMembers] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
ALTER TABLE [ProjectMembers] ADD CONSTRAINT [Projects_ProjectMembers] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([ProjectId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
-- Create indexes for table ProjectMembers

CREATE INDEX [IX_Projects_ProjectMembers] ON [ProjectMembers] ([ProjectId])
GO
CREATE INDEX [IX_Users_ProjectMembers] ON [ProjectMembers] ([UserId])