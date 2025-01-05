using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Calendar : PageModel
{
    public DateTime Date { get; set; }
    public List<Meeting> Meetings { get; set; }

    public IActionResult OnGet(string action)
    {
        if (HttpContext.Session.Get<User>("User") == null)
        {
            return Redirect("~/LoginForm");
        }

        if (HttpContext.Session.Get<DateTime>("Date") == DateTime.MinValue)
        {
            Date = DateTime.Now;
            HttpContext.Session.Set("Date", Date);
        }
        else
        {
            Date = HttpContext.Session.Get<DateTime>("Date");
        }

        if (HttpContext.Session.Get<List<Meeting>>("Meetings") == null)
        {
            var user = HttpContext.Session.Get<User>("User");
            List<Meeting> meetings;
            bool success = MeetingManager.Instance.GetAllMeetings(user, out meetings);

            if (!success || meetings == null)
            {
                Meetings = new();
            }

            Meetings = meetings;
            HttpContext.Session.Set("Meetings", Meetings);
        }
        else
        {
            Meetings = HttpContext.Session.Get<List<Meeting>>("Meetings");
        }

        if (action == null || action == "")
        {
            return null;
        }

        switch (action)
        {
            case "next":
                Date = Date.AddMonths(1);
                HttpContext.Session.Set("Date", Date);
                break;

            case "previous":
                Date = Date.AddMonths(-1);
                HttpContext.Session.Set("Date", Date);
                break;
        }

        return Redirect("~/Calendar");
    }

    public JsonResult OnPostSetActiveDate(string date)
    {
        HttpContext.Session.Set("ActiveDate", DateTime.Parse(date));
        return new(new { success = true });
    }

    public JsonResult OnPostUnsetActiveDate()
    {
        HttpContext.Session.Remove("ActiveDate");
        return new(new { success = true });
    }

    public JsonResult OnPostSetEditedMeetingId(string id)
    {
        HttpContext.Session.Set<long>("EditedMeetingId", long.Parse(id));
        return new(new { success = true });
    }

    public JsonResult OnPostUnsetEditedMeetingId()
    {
        HttpContext.Session.Remove("EditedMeetingId");
        return new(new { success = true });
    }

    public JsonResult OnPostEditMeeting(string id)
    {
        HttpContext.Session.Set<long>("EditId", long.Parse(id));
        return new(new { success = true });
    }

    public JsonResult OnPostEditBack()
    {
        HttpContext.Session.Remove("EditId");
        return new(new { success = true });
    }

    public JsonResult OnPostDeleteMeeting(string id)
    {
        HttpContext.Session.Remove("EditedMeetingId");
        HttpContext.Session.Remove("ActiveDate");
        HttpContext.Session.Remove("Meetings");
        HttpContext.Session.Remove("Date");

        bool success = MeetingManager.Instance.DeleteMeeting(long.Parse(id));

        if (!success)
        {
            return new(new { success = false });
        }

        return new(new { success = true });
    }

    public JsonResult OnPostUpdateMeeting(string meetingId, string projectId, string date, string name, string description)
    {
        if (name == null || description == null)
        {
            return new(new { success = false });
        }

        Meeting data = new();
        data.MeetingId = long.Parse(meetingId);
        data.ProjectId = long.Parse(projectId);
        data.Date = DateTime.Parse(date);
        data.Title = name;
        data.Description = description;

        bool success = MeetingManager.Instance.UpdateMeeting(data);

        if (!success)
        {
            return new(new { success = false });
        }

        HttpContext.Session.Remove("EditId");
        HttpContext.Session.Remove("Meetings");
        HttpContext.Session.Remove("Date");

        return new(new { success = true });
    }

    public string GetMonthName(int month)
    {
        switch (month)
        {
            case 1:
                return "January";

            case 2:
                return "February";

            case 3:
                return "March";

            case 4:
                return "April";

            case 5:
                return "May";

            case 6:
                return "June";

            case 7:
                return "July";

            case 8:
                return "August";

            case 9:
                return "September";

            case 10:
                return "October";

            case 11:
                return "November";

            case 12:
                return "December";
        }

        throw new ArgumentException();
    }
}