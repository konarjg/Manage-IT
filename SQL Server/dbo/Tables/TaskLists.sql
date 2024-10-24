-- Table TaskLists

CREATE TABLE [TaskLists]
(
 [TasklistId] Bigint IDENTITY PRIMARY KEY NOT NULL,
 [Name] Varchar(50) NOT NULL,
 [Description] Text NOT NULL,
 [ProjectId] Bigint NOT NULL
)
GO
ALTER TABLE [TaskLists] ADD CONSTRAINT [Projects_TaskLists] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([ProjectId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
-- Add keys for table TaskLists
GO
-- Create indexes for table TaskLists

CREATE INDEX [IX_Projects_TaskLists] ON [TaskLists] ([ProjectId])