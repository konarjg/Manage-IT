using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Calendar : PageModel
{
    public DateTime Date { get; set; }
    public List<Meeting> Meetings { get; set; }

    public IActionResult OnGet(string action, string idDate)
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

            case "deleteMeeting":
                if (idDate == null || idDate == "")
                {
                    return null;
                }

                long id = long.Parse(idDate.Split('!')[0]);
                string date = idDate.Split('!')[1];
                var dateTime = DateTime.Parse(date);

                if (!MeetingManager.Instance.DeleteMeeting(id, dateTime))
                {
                    return null;
                }

                HttpContext.Session.Remove("Meetings");
                break;
        }

        return Redirect("~/Calendar");
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