using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Runtime.CompilerServices;

public class MeetingManager
{
    public static MeetingManager Instance { get; private set; }

    public static void Instantiate()
    {
        Instance = new MeetingManager();
    }

    public bool GetAllMeetings(out List<Meeting> meetings)
    {
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Meetings");
        return DatabaseAccess.Instance.ExecuteQuery(query, out meetings);
    }

    public bool DeleteAllMeetings(long projectId)
    {
        List<Meeting> meetings;
        FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Meetings WHERE ProjectId = {projectId}");
        return DatabaseAccess.Instance.ExecuteQuery(query, out meetings);
    }

    public bool GetMeeting(long meetingId, out Meeting meeting)
    {
        List<Meeting> meetings;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Meetings WHERE MeetingId = {meetingId}");
        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out meetings) && meetings != null && meetings.Count != 0;

        if (!success)
        {
            meeting = null;
            return false;
        }

        meeting = meetings[0];
        return true;
    }

    public bool UpdateMeeting(Meeting data)
    {
        List<Meeting> meetings;
        FormattableString query = FormattableStringFactory.Create($"UPDATE dbo.Meetings SET ProjectId = {data.ProjectId}, Date = '{data.Date.ToString("yyyy-MM-dd")}', Title = '{data.Title}', Description = '{data.Description}' WHERE MeetingId = {data.MeetingId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out meetings);
    }

    public bool CreateMeeting(Meeting data)
    {
        List<Meeting> meetings;
        FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.Meetings (ProjectId, Title, Description, Date) VALUES ({data.ProjectId}, '{data.Title}', '{data.Description}', '{data.Date.ToString("yyyy-MM-dd")}')");

        return DatabaseAccess.Instance.ExecuteQuery(query, out meetings);
    }
    public bool GetMeetingsRelatedToProject(long projectID, out List<Meeting> result)
    {
        result = new();
        List<Meeting> meetings;
        FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Meetings WHERE ProjectId = {projectID}");

        bool success = DatabaseAccess.Instance.ExecuteQuery(query, out meetings);

        if (!success || meetings == null || meetings.Count == 0)
        {
            return false;
        }

        result.AddRange(meetings);

        return true;
    }
    public bool GetAllMeetings(User user, out List<Meeting> result)
    {
        List<Project> projects;
        result = new();

        bool projectsExist = ProjectManager.Instance.GetAllProjects(user.UserId, out projects);

        if (!projectsExist)
        {
            return false;
        }

        foreach (Project project in projects)
        {
            List<Meeting> meetings;
            FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Meetings WHERE ProjectId = {project.ProjectId}");

            bool success = DatabaseAccess.Instance.ExecuteQuery(query, out meetings);

            if (!success || meetings == null || meetings.Count == 0)
            {
                continue;
            }

            result.AddRange(meetings);
        }

        return true;
    }

    public bool DeleteMeeting(long meetingId)
    {
        List<Meeting> meetings;
        FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Meetings WHERE MeetingId = {meetingId}");

        return DatabaseAccess.Instance.ExecuteQuery(query, out meetings);
    }
}
