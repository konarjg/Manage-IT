using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public class TaskManager
{
    public static TaskManager Instance { get; private set; } 

    public static void Instantiate()
    {
        Instance = new TaskManager();
    }

    public bool GetTaskId(long taskListId, string name, out long taskId)
    {
        List<Task> tasks;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Tasks WHERE TaskListId = {taskListId} AND Name LIKE '{name}'");
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
        var queryTasks = FormattableStringFactory.Create($"SELECT * FROM dbo.Tasks WHERE TaskListId = {taskListId}");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out tasks);
    }

    public bool CreateTask(Task data)
    {
        List<Task> tasks;
        var queryTasks = FormattableStringFactory.Create($"INSERT INTO dbo.Tasks (Name, TaskListId, Description, Deadline) VALUES ('{data.Name}', {data.TaskListId}, '{data.Description}', '{data.Deadline.ToString("yyyy-MM-dd")}')");

        return DatabaseAccess.Instance.ExecuteQuery(queryTasks, out tasks);
    }
}
