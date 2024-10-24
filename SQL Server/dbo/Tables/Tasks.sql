-- Table Tasks

CREATE TABLE [Tasks]
(
 [TaskId] Bigint IDENTITY PRIMARY KEY NOT NULL,
 [TaskListId] Bigint NOT NULL,
 [Deadline] Date NOT NULL,
 [Description] Text NOT NULL
)
GO
ALTER TABLE [Tasks] ADD CONSTRAINT [TaskLists_Tasks] FOREIGN KEY ([TaskListId]) REFERENCES [TaskLists] ([TasklistId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
-- Add keys for table Tasks
GO
-- Create indexes for table Tasks

CREATE INDEX [IX_TaskLists_Tasks] ON [Tasks] ([TaskListId])