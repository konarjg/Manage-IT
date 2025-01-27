using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public class TaskListManager
{
    public static TaskListManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new TaskListManager();
    }

    public TaskList GetTaskList(long taskListId)
    {
        List<TaskList> taskLists;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.TaskLists WHERE TaskListId = {taskListId}");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out taskLists) && taskLists != null && taskLists.Count != 0;
    
        if (!success)
        {
            return null;
        }

        return taskLists.FirstOrDefault();
    }

    public bool GetAllTaskLists(long projectId, out List<TaskList> taskLists)
    {
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.TaskLists WHERE ProjectId = {projectId}");
        return DatabaseAccess.Instance.ExecuteQuery(query, out taskLists);
    }

    public bool CreateTaskList(TaskList data)
    {
        List<TaskList> taskLists;
        var query = FormattableStringFactory.Create($"INSERT INTO dbo.TaskLists (Name, Description, ProjectId) VALUES ('{data.Name}', '{data.Description}', {data.ProjectId})");
        return DatabaseAccess.Instance.ExecuteQuery(query, out taskLists);
    }

    public bool UpdateTaskList(TaskList data)
    {
        List<TaskList> taskLists;
        var query = FormattableStringFactory.Create($"UPDATE dbo.TaskLists SET Name = '{data.Name}', Description = '{data.Description}' WHERE TaskListId = {data.TaskListId}");
        return DatabaseAccess.Instance.ExecuteQuery(query, out taskLists);
    }

    public bool DeleteTaskList(long taskListId)
    {
        List<Task> tasks;
        bool success = TaskManager.Instance.GetAllTasks(taskListId, out tasks);

        if (tasks != null && tasks.Count != 0)
        {
            foreach (var task in tasks)
            {
                List<TaskDetails> tempDetails;
                List<Task> tempTasks;

                var queryDetails = FormattableStringFactory.Create($"DELETE FROM dbo.TaskDetails WHERE TaskId = {task.TaskId}");
                var queryTasks = FormattableStringFactory.Create($"DELETE FROM dbo.Tasks WHERE TaskId = {task.TaskId}");

                DatabaseAccess.Instance.ExecuteQuery(queryDetails, out tempDetails);
                DatabaseAccess.Instance.ExecuteQuery(queryTasks, out tempTasks);
            }
        }

        List<TaskList> taskLists;
        var query = FormattableStringFactory.Create($"DELETE FROM dbo.TaskLists WHERE TaskListId = {taskListId}");
        return DatabaseAccess.Instance.ExecuteQuery(query, out taskLists);
    }

    public bool DeleteAllTaskLists(long projectId)
    {
        TaskManager.Instance.DeleteAllTasks(projectId);

        List<TaskList> temp;
        var query = FormattableStringFactory.Create($"DELETE FROM dbo.TaskLists WHERE ProjectId = {projectId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out temp);
    }
}
