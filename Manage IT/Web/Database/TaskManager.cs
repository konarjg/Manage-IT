using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Runtime.CompilerServices;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public class TaskManager
{
    public static TaskManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new TaskManager();
    }

    public bool GetAllTasks(out List<Task> tasks)
    {
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Tasks");
        return DatabaseAccess.Instance.ExecuteQuery(query, out tasks);
    }

    public bool DeleteMembers(long taskId)
    {
        List<TaskDetails> details;
        FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.TaskDetails WHERE TaskId = {taskId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out details);
    }

    public bool DeleteTask(long taskId)
    {
        List<Task> tasks;
        DeleteMembers(taskId);
        FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Tasks WHERE TaskId = {taskId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out tasks);
    }

    public bool DeleteAllTasks(long projectId)
    {
        List<Task> tasks;
        FormattableString query = FormattableStringFactory.Create($"WITH TaskIdsToDelete AS ( SELECT t.TaskId FROM dbo.Tasks t JOIN dbo.TaskLists tl ON t.TaskListId = tl.TaskListId ) DELETE FROM dbo.TaskDetails WHERE TaskId IN (SELECT TaskId FROM TaskIdsToDelete)");
        FormattableString query1 = FormattableStringFactory.Create($"WITH ProjectIdsToDelete AS (SELECT tl.TaskListId FROM dbo.TaskLists tl WHERE tl.ProjectId = {projectId}) DELETE FROM dbo.Tasks WHERE TaskListId IN (SELECT TaskListId FROM ProjectIdsToDelete)");

        return DatabaseAccess.Instance.ExecuteQuery(query, out tasks) && DatabaseAccess.Instance.ExecuteQuery(query1, out tasks);
    }

    public bool GetTaskId(long taskListId, string name, out long taskId)
    {
        List<Task> tasks;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Tasks WHERE TaskListId = {taskListId} AND Name LIKE '{name}'");
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
        FormattableString queryTasks = FormattableStringFactory.Create($"SELECT * FROM dbo.Tasks WHERE TaskListId = {taskListId}");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out tasks);
    }

    public bool CreateTask(Task data)
    {
        List<Task> tasks;
        FormattableString queryTasks = FormattableStringFactory.Create($"INSERT INTO dbo.Tasks (Name, TaskListId, Description, Deadline, HandedIn) VALUES ('{data.Name}', {data.TaskListId}, '{data.Description}', '{data.Deadline.ToString("yyyy-MM-dd")}', 0)");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out tasks);
    }
}
