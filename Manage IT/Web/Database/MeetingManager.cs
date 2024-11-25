using EFModeling.EntityProperties.DataAnnotations.Annotations;
public class TaskListManager
{
//Singleton instance
    public static MeetingManager Instance { get; private set; } 

    //Create new instance and store it in Instance
    public static void Instantiate()
    {
        Instance = new MeetingManager();
    }
  //C
     public bool AddMeeting(Meeting meeting,string error){
      List<Meeting> meeting;
      var query = FormattableStringFactory.Create($"INSERT INTO dbo.Meetings (Name, Description, Date, ProjectID) VALUES ('{meeting.Name}', '{meeting.Description}','{meeting.Date}',{meeting.ProjectID})");
       
       if(error!="")
         return false;
       return true;
     }
//R
  public bool GetMeeting(Meeting meeting,string error){
      List<Meeting> meeting;
      var query = FormattableStringFactory.Create($"SELECT * FROM dbo.Meetings WHERE MeetingID = '{meeting.MeetingId}'");
       
       if(error!="")
         return false;
       return true;
     }
  

    public bool UpdateMeeting(Meeting meeting,string error)
    {
        List<Meeting> meeting;
        var updateMeetingQuery = FormattableStringFactory.Create($"UPDATE dbo.Meetings SET  Name = '{meeting.Name}',Date = '{meeting.Date}',Description = '{meeting.Description}',ProjectID = {meeting.ProjectID} WHERE MeetingId = {meeting.ID}");                                 
        if(error!="")
            return false;
        return true;
    }
  //D
   public bool DeleteMeeting(Meeting meeting,out string error){
       List<Meeting> meeting;
       
      var query = FormattableStringFactory.Create($"DELETE FROM dbo.Mettings WHERE MeetingID = {dbo.MeetingID}");
      
       
       if(error!="")
         return false;
       return true;
     }
   
}
