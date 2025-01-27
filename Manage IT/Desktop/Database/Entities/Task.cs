using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("Tasks")]
public class Task
{
    [Key]
    public long TaskId { get; set; }
    public string Name { get; set; }
    [ForeignKey("TaskListId")]
    public long TaskListId { get; set; }
    public DateTime Deadline { get; set; }
    public string Description { get; set; }
    public bool? Accepted { get; set; }
    public bool HandedIn { get; set; }

    public string DaysLeft
    {
        get
        {
            int diff = (int)(Deadline - DateTime.Now).TotalDays;

            if (diff < 0)
            {
                diff = 0;
            }

            return $"{diff}d left";
        }
    }

    public string HandedInText
    {
        get
        {
            return HandedIn ? "Handed in" : "";
        }
    }

    public SolidColorBrush ReviewColor
    {
        get
        {
            switch (Accepted)
            {
                case null:
                    return Brushes.White;

                case false:
                    return Brushes.Red;

                case true:
                    return Brushes.Green;
            }
        }
    }

    public string ReviewText
    {
        get
        {
            switch (Accepted)
            {
                case null:
                    return "";

                case false:
                    return "Rejected";

                case true:
                    return "Accepted";
            }
        }
    }

    public string AcceptedSql
    {
        get
        {
            switch (Accepted)
            {
                case null:
                    return "NULL";

                case false:
                    return "0";

                case true:
                    return "1";
            }
        }
    }
}