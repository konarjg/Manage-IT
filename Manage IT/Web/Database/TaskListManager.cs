using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;
using TaskList = EFModeling.EntityProperties.DataAnnotations.Annotations.TaskList;
public class TaskListManager
{
//Singleton instance
    public static TaskListManager Instance { get; private set; } 

    //Create new instance and store it in Instance
    public static void Instantiate()
    {
        Instance = new TaskListManager();
    }
     public bool AddTaskList(TaskList tasklist,string error){
      List<TaskList> tasklist;
       //tasklistID
      var query = FormattableStringFactory.Create($"INSERT INTO dbo.TaskLists (Name,Description,ProjectID) VALUES ('{tasklist.Name}', '{tasklist.Description}',{tasklist.ProjectID})");
       
       if(error!="")
         return false;
       return true;
     }

   public bool DeleteTaskList(TaskList tasklist,out string error){
       List<TaskList> tasklist;
       List<Task> tasks;
       bool getTasksFromTasklists = TaskManager.GetTasksForTaskList(TaskList list, out tasks);
       if(!getTasksFromTasklists){
         error = "There was an error during the process";
         return false;
       }else{
       //tasklistID
         foreach(task in tasks){
          TaskManager.DeleteTask(task);
         }
       }
      var query = FormattableStringFactory.Create($"DELETE FROM dbo.TaskLists WHERE TaskListID = {dbo.TaskListID}");
      
       
       if(error!="")
         return false;
       return true;
     }
   
}
