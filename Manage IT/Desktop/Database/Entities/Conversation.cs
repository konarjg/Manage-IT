using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFModeling.EntityProperties.DataAnnotations.Annotations;

[Table("dbo.Conversations")]
public class Conversation
{
    [Key]
    public long ConversationId { get; set; }
    [ForeignKey("UserId")]
    public long User1Id { get; set; }
    [ForeignKey("UserId")]
    public long User2Id { get; set; }
}