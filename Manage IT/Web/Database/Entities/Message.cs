using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("dbo.Messages")]
public class Message
{
    [Key]
    public long MessageId { get; set; }
    [ForeignKey("ConversationId")]
    public long ConversationId { get; set; }
    [ForeignKey("UserId")]
    public long UserId { get; set; }
    public string MessageBody { get; set; }
}