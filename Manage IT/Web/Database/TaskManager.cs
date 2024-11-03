﻿using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public class TaskManager
{
    //Singleton instance
    public static TaskManager Instance { get; private set; } 

    //Create new instance and store it in Instance
    public static void Instantiate()
    {
        Instance = new TaskManager();
    }

    //Add a task to task list and also the database
    //(you must also insert a new record to TaskDetails table)
    public bool AddTaskToTaskList(TaskList list, Task task)
    {
        
       var queryTasks = FormattableStringFactory.Create($"INSERT INTO dbo.Tasks (TaskListID,Deadline,Description) VALUES ({list.TaskListID}, '{task.Deadline}','{task.Description}')");
        var queryTasklist = FormattableStringFactory.Create($"INSERT INTO dbo.TaskDetails (TaskID,UserID) VALUES ({task.TaskID}, '{task.Deadline}','{task.Description}')");
         var success = DatabaseAccess.Instance.ProcessQuery(query, out TaskList);
        
        if (!success)
        {
            error = "There was an unexpected error! Could not save the task.";
            return false;
        }

        return true;
    }

    //Select all tasks for given TaskList
    public bool GetTasksForTaskList(TaskList list, out List<Task> tasks)
    {
        List<User> tasks;
        var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Tasks WHERE TaskListID = '{tasks.TaskListID}'");
        bool success = DatabaseAccess.Instance.ProcessQuery(query, out tasks)
        tasks = null;
        return true;
    }

    //Update task info with new values
    public bool UpdateTask(Task task)
    {
        return true;
    }

    //Delete task from its list and from database
    //(you must delete it from TaskDetails table too)
    public bool DeleteTask(Task task)
    {
        var queryTask = FormattableStringFactory.Create($"DELETE FROM dbo.Tasks WHERE TaskId = '{task.ID}');
        bool success = DatabaseAccess.Instance.ProcessQuery(query, out TaskList);

        var queryTaskDetails = FormattableStringFactory.Create($"DELETE FROM dbo.TaskDetails WHERE TaskId = '{task.ID}');
        bool success = DatabaseAccess.Instance.ProcessQuery(query, out TaskList);                                              

        return success;
        return true;
    }
}
