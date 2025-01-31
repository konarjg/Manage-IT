using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public class TaskManager
{
    public static TaskManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new TaskManager();
    }

    public bool CreateTaskDetails(TaskDetails data)
    {
        List<TaskDetails> taskDetails;
        System.FormattableString queryTasks = FormattableStringFactory.Create($"INSERT INTO dbo.TaskDetails (UserId, TaskId) VALUES ('{data.UserId}','{data.TaskId}')");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out taskDetails);
    }

    public bool ClearTaskDetails(TaskDetails data)
    {
        List<TaskDetails> taskDetails;
        System.FormattableString queryTasks = FormattableStringFactory.Create($"DELETE FROM dbo.TaskDetails WHERE UserId = '{data.UserId}' AND TaskId = '{data.TaskId}'");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out taskDetails);
    }

    public bool DeleteAllTasks(long projectId)
    {
        List<Task> tasks;
        System.FormattableString query = FormattableStringFactory.Create($"WITH TaskIdsToDelete AS ( SELECT t.TaskId FROM dbo.Tasks t JOIN dbo.TaskLists tl ON t.TaskListId = tl.TaskListId ) DELETE FROM dbo.TaskDetails WHERE TaskId IN (SELECT TaskId FROM TaskIdsToDelete)");
        System.FormattableString query1 = FormattableStringFactory.Create($"WITH ProjectIdsToDelete AS (SELECT tl.TaskListId FROM dbo.TaskLists tl WHERE tl.ProjectId = {projectId}) DELETE FROM dbo.Tasks WHERE TaskListId IN (SELECT TaskListId FROM ProjectIdsToDelete)");

        return DatabaseAccess.Instance.ExecuteQuery(query, out tasks) && DatabaseAccess.Instance.ExecuteQuery(query1, out tasks);
    }

    public Task GetTask(long taskId)
    {
        List<Task> tasks;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Tasks WHERE TaskId = {taskId}");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out tasks) && tasks != null && tasks.Count != 0;

        if (!success)
        {
            return null;
        }

        return tasks.FirstOrDefault();
    }

    public bool DeleteMembers(long taskId)
    {
        List<TaskDetails> details;
        System.FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.TaskDetails WHERE TaskId = {taskId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out details);
    }

    public bool AssignMembers(long taskId, List<User> members)
    {
        bool success = DeleteMembers(taskId);

        if (!success)
        {
            return false;
        }

        foreach (User member in members)
        {
            List<TaskDetails> details;
            System.FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.TaskDetails(TaskId, UserId) VALUES({taskId}, {member.UserId})");

            DatabaseAccess.Instance.ExecuteQuery(query, out details);
        }

        return true;
    }

    public List<User> GetMembers(long taskId)
    {
        List<User> users = new();

        List<TaskDetails> details;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.TaskDetails WHERE TaskId = {taskId}");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out details) && details != null && details.Count != 0;

        if (!success)
        {
            return users;
        }

        foreach (TaskDetails detail in details)
        {
            User user;
            success = UserManager.Instance.GetUser(detail.UserId, out user) && user != null;

            if (!success)
            {
                continue;
            }

            users.Add(user);
        }

        return users;
    }

    public bool GetTaskId(long taskListId, string name, out long taskId)
    {
        List<Task> tasks;
        System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Tasks WHERE TaskListId = {taskListId} AND Name LIKE '{name}'");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out tasks);

        if (!success || tasks == null || tasks.Count == 0)
        {
            taskId = -1;
            return false;
        }

        taskId = tasks[0].TaskId;
        return true;
    }

    public bool GetAllTasks(long taskListId, out List<Task> tasks)
    {
        System.FormattableString queryTasks = FormattableStringFactory.Create($"SELECT * FROM dbo.Tasks WHERE TaskListId = {taskListId}");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out tasks);
    }

    public bool CreateTask(Task data)
    {
        List<Task> tasks;
        System.FormattableString queryTasks = FormattableStringFactory.Create($"INSERT INTO dbo.Tasks (Name, TaskListId, Description, Deadline, HandedIn) VALUES ('{data.Name}', {data.TaskListId}, '{data.Description}', '{data.Deadline.ToString("yyyy-MM-dd HH:mm:ss")}', 0)");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out tasks);
    }
    public bool CreateTaskDetails(TaskDetails data)
    {
        List<TaskDetails> taskDetails;
        System.FormattableString queryTasks = FormattableStringFactory.Create($"INSERT INTO dbo.TaskDetails (UserId, TaskId) VALUES ('{data.UserId}','{data.TaskId}')");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out taskDetails);
    }

    public bool ClearTaskDetails(TaskDetails data)
    {
        List<TaskDetails> taskDetails;
        System.FormattableString queryTasks = FormattableStringFactory.Create($"DELETE FROM dbo.TaskDetails WHERE UserId = '{data.UserId}' AND TaskId = '{data.TaskId}'");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out taskDetails);
    }
    public bool DeleteTask(long taskId)
    {
        List<Task> tasks;
        DeleteMembers(taskId);
        System.FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Tasks WHERE TaskId = {taskId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out tasks);
    }

    public bool UpdateTask(Task data)
    {
        List<Task> tasks;

        System.FormattableString query = FormattableStringFactory.Create($"UPDATE dbo.Tasks SET TaskListId = {data.TaskListId}, Name = '{data.Name}', Description = '{data.Description}', Deadline = '{data.Deadline.ToString("yyyy-MM-dd HH:mm:ss")}', Accepted = {data.AcceptedSql}, HandedIn = {(data.HandedIn ? 1 : 0)} WHERE TaskId = {data.TaskId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out tasks);
    }

    public bool GetAllAssignedTasks(long taskListId, long userId, out List<Task> tasks)
    {
        System.FormattableString query = FormattableStringFactory.Create($"SELECT t.* FROM dbo.Tasks t JOIN TaskDetails td ON td.TaskId = t.TaskId WHERE t.TaskListId = {taskListId} AND td.UserId = {userId}");
        return DatabaseAccess.Instance.ExecuteQuery(query, out tasks);
    }

    public bool GetAllAssignedTasks(User user, out List<Task> tasks)
    {
        System.FormattableString query = FormattableStringFactory.Create($"SELECT t.* FROM dbo.Tasks t JOIN TaskDetails td ON td.TaskId = t.TaskId WHERE td.UserId = {user.UserId}");
        return DatabaseAccess.Instance.ExecuteQuery(query, out tasks);
    }
}
