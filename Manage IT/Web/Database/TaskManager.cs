using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

public class TaskManager
{
    //Singleton instance
    public static TaskManager Instance { get; private set; } 

    //Create new instance and store it in Instance
    public static void Instantiate()
    {

    }

    //Add a task to task list and also the database
    //(you must also insert a new record to TaskDetails table)
    public bool AddTaskToTaskList(TaskList list, Task task)
    {
        return true;
    }

    //Select all tasks for given TaskList
    public bool GetTasksForTaskList(TaskList list, out List<Task> tasks)
    {
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
        return true;
    }
}
