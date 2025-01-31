﻿/*
Deployment script for manageit_ManageIT

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "manageit_ManageIT"
:setvar DefaultFilePrefix "manageit_ManageIT"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQL2016\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQL2016\MSSQL\DATA\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET PAGE_VERIFY NONE,
                DISABLE_BROKER 
            WITH ROLLBACK IMMEDIATE;
    END


GO
ALTER DATABASE [$(DatabaseName)]
    SET TARGET_RECOVERY_TIME = 0 SECONDS 
    WITH ROLLBACK IMMEDIATE;


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET QUERY_STORE (CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 367)) 
            WITH ROLLBACK IMMEDIATE;
    END


GO
PRINT N'Creating Table [dbo].[ProjectMembers]...';


GO
CREATE TABLE [dbo].[ProjectMembers] (
    [UserId]    BIGINT NOT NULL,
    [ProjectId] BIGINT NOT NULL
);


GO
PRINT N'Creating Index [dbo].[ProjectMembers].[IX_Projects_ProjectMembers]...';


GO
CREATE NONCLUSTERED INDEX [IX_Projects_ProjectMembers]
    ON [dbo].[ProjectMembers]([ProjectId] ASC);


GO
PRINT N'Creating Index [dbo].[ProjectMembers].[IX_Users_ProjectMembers]...';


GO
CREATE NONCLUSTERED INDEX [IX_Users_ProjectMembers]
    ON [dbo].[ProjectMembers]([UserId] ASC);


GO
PRINT N'Creating Table [dbo].[Projects]...';


GO
CREATE TABLE [dbo].[Projects] (
    [ProjectId]   BIGINT       IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (30) NOT NULL,
    [Description] TEXT         NOT NULL,
    [ManagerId]   BIGINT       NOT NULL,
    PRIMARY KEY CLUSTERED ([ProjectId] ASC)
);


GO
PRINT N'Creating Index [dbo].[Projects].[IX_Users_Projects(manager?)]...';


GO
CREATE NONCLUSTERED INDEX [IX_Users_Projects(manager?)]
    ON [dbo].[Projects]([ManagerId] ASC);


GO
PRINT N'Creating Table [dbo].[TaskDetails]...';


GO
CREATE TABLE [dbo].[TaskDetails] (
    [UserId] BIGINT NOT NULL,
    [TaskId] BIGINT NOT NULL
);


GO
PRINT N'Creating Index [dbo].[TaskDetails].[IX_Users_TaskDetails]...';


GO
CREATE NONCLUSTERED INDEX [IX_Users_TaskDetails]
    ON [dbo].[TaskDetails]([UserId] ASC);


GO
PRINT N'Creating Index [dbo].[TaskDetails].[IX_Tasks_TaskDetails]...';


GO
CREATE NONCLUSTERED INDEX [IX_Tasks_TaskDetails]
    ON [dbo].[TaskDetails]([TaskId] ASC);


GO
PRINT N'Creating Table [dbo].[TaskLists]...';


GO
CREATE TABLE [dbo].[TaskLists] (
    [TasklistId]  BIGINT       IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50) NOT NULL,
    [Description] TEXT         NOT NULL,
    [ProjectId]   BIGINT       NOT NULL,
    PRIMARY KEY CLUSTERED ([TasklistId] ASC)
);


GO
PRINT N'Creating Index [dbo].[TaskLists].[IX_Projects_TaskLists]...';


GO
CREATE NONCLUSTERED INDEX [IX_Projects_TaskLists]
    ON [dbo].[TaskLists]([ProjectId] ASC);


GO
PRINT N'Creating Table [dbo].[Tasks]...';


GO
CREATE TABLE [dbo].[Tasks] (
    [TaskId]      BIGINT IDENTITY (1, 1) NOT NULL,
    [TaskListId]  BIGINT NOT NULL,
    [Deadline]    DATE   NOT NULL,
    [Description] TEXT   NOT NULL,
    PRIMARY KEY CLUSTERED ([TaskId] ASC)
);


GO
PRINT N'Creating Index [dbo].[Tasks].[IX_TaskLists_Tasks]...';


GO
CREATE NONCLUSTERED INDEX [IX_TaskLists_Tasks]
    ON [dbo].[Tasks]([TaskListId] ASC);


GO
PRINT N'Creating Table [dbo].[UserPermissions]...';


GO
CREATE TABLE [dbo].[UserPermissions] (
    [UserId]          BIGINT NOT NULL,
    [ProjectId]       BIGINT NOT NULL,
    [Editing]         BIT    NOT NULL,
    [InvitingMembers] BIT    NOT NULL,
    [KickingMembers]  BIT    NOT NULL
);


GO
PRINT N'Creating Index [dbo].[UserPermissions].[IX_Users_UserPermissions]...';


GO
CREATE NONCLUSTERED INDEX [IX_Users_UserPermissions]
    ON [dbo].[UserPermissions]([UserId] ASC);


GO
PRINT N'Creating Index [dbo].[UserPermissions].[IX_Projects_UserPermissions]...';


GO
CREATE NONCLUSTERED INDEX [IX_Projects_UserPermissions]
    ON [dbo].[UserPermissions]([ProjectId] ASC);


GO
PRINT N'Creating Table [dbo].[Users]...';


GO
CREATE TABLE [dbo].[Users] (
    [UserId]   BIGINT       IDENTITY (1, 1) NOT NULL,
    [Login]    VARCHAR (20) NOT NULL,
    [Password] TEXT         NOT NULL,
    [Email]    TEXT         NOT NULL,
    [Verified] BIT          NOT NULL,
    [Admin]    BIT          NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC)
);


GO
PRINT N'Creating Foreign Key [dbo].[Users_ProjectMembers]...';


GO
ALTER TABLE [dbo].[ProjectMembers] WITH NOCHECK
    ADD CONSTRAINT [Users_ProjectMembers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]);


GO
PRINT N'Creating Foreign Key [dbo].[Projects_ProjectMembers]...';


GO
ALTER TABLE [dbo].[ProjectMembers] WITH NOCHECK
    ADD CONSTRAINT [Projects_ProjectMembers] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([ProjectId]);


GO
PRINT N'Creating Foreign Key [dbo].[Users_Project(manager?)]...';


GO
ALTER TABLE [dbo].[Projects] WITH NOCHECK
    ADD CONSTRAINT [Users_Project(manager?)] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[Users] ([UserId]);


GO
PRINT N'Creating Foreign Key [dbo].[Users_TaskDetails]...';


GO
ALTER TABLE [dbo].[TaskDetails] WITH NOCHECK
    ADD CONSTRAINT [Users_TaskDetails] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]);


GO
PRINT N'Creating Foreign Key [dbo].[Tasks_TaskDetails]...';


GO
ALTER TABLE [dbo].[TaskDetails] WITH NOCHECK
    ADD CONSTRAINT [Tasks_TaskDetails] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks] ([TaskId]);


GO
PRINT N'Creating Foreign Key [dbo].[Projects_TaskLists]...';


GO
ALTER TABLE [dbo].[TaskLists] WITH NOCHECK
    ADD CONSTRAINT [Projects_TaskLists] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([ProjectId]);


GO
PRINT N'Creating Foreign Key [dbo].[TaskLists_Tasks]...';


GO
ALTER TABLE [dbo].[Tasks] WITH NOCHECK
    ADD CONSTRAINT [TaskLists_Tasks] FOREIGN KEY ([TaskListId]) REFERENCES [dbo].[TaskLists] ([TasklistId]);


GO
PRINT N'Creating Foreign Key [dbo].[Users_UserPermissions]...';


GO
ALTER TABLE [dbo].[UserPermissions] WITH NOCHECK
    ADD CONSTRAINT [Users_UserPermissions] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]);


GO
PRINT N'Creating Foreign Key [dbo].[Projects_UserPermissions]...';


GO
ALTER TABLE [dbo].[UserPermissions] WITH NOCHECK
    ADD CONSTRAINT [Projects_UserPermissions] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([ProjectId]);


GO
PRINT N'Checking existing data against newly created constraints';


GO
USE [$(DatabaseName)];


GO
ALTER TABLE [dbo].[ProjectMembers] WITH CHECK CHECK CONSTRAINT [Users_ProjectMembers];

ALTER TABLE [dbo].[ProjectMembers] WITH CHECK CHECK CONSTRAINT [Projects_ProjectMembers];

ALTER TABLE [dbo].[Projects] WITH CHECK CHECK CONSTRAINT [Users_Project(manager?)];

ALTER TABLE [dbo].[TaskDetails] WITH CHECK CHECK CONSTRAINT [Users_TaskDetails];

ALTER TABLE [dbo].[TaskDetails] WITH CHECK CHECK CONSTRAINT [Tasks_TaskDetails];

ALTER TABLE [dbo].[TaskLists] WITH CHECK CHECK CONSTRAINT [Projects_TaskLists];

ALTER TABLE [dbo].[Tasks] WITH CHECK CHECK CONSTRAINT [TaskLists_Tasks];

ALTER TABLE [dbo].[UserPermissions] WITH CHECK CHECK CONSTRAINT [Users_UserPermissions];

ALTER TABLE [dbo].[UserPermissions] WITH CHECK CHECK CONSTRAINT [Projects_UserPermissions];


GO
PRINT N'Update complete.';


GO
