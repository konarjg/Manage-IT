using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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