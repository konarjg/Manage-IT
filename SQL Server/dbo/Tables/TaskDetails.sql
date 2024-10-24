-- Table TaskDetails

CREATE TABLE [TaskDetails]
(
 [UserId] Bigint NOT NULL,
 [TaskId] Bigint NOT NULL
)
GO
-- Create foreign keys (relationships) section ------------------------------------------------- 


ALTER TABLE [TaskDetails] ADD CONSTRAINT [Users_TaskDetails] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
ALTER TABLE [TaskDetails] ADD CONSTRAINT [Tasks_TaskDetails] FOREIGN KEY ([TaskId]) REFERENCES [Tasks] ([TaskId]) ON UPDATE NO ACTION ON DELETE NO ACTION
GO
-- Create indexes for table TaskDetails

CREATE INDEX [IX_Users_TaskDetails] ON [TaskDetails] ([UserId])
GO
CREATE INDEX [IX_Tasks_TaskDetails] ON [TaskDetails] ([TaskId])