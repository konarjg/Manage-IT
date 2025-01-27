using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Chat : PageModel
{
    public User User { get; set; }
    public List<Conversation> Conversations { get; set; }
    public List<Message> Messages { get; set; }
    public Conversation? CurrentConversation;

    public void OnGet()
    {
        User = HttpContext.Session.Get<User>("User");
        CurrentConversation = HttpContext.Session.Get<Conversation>("CurrentConversation");
    }

    public JsonResult OnPostCreateConversation(string credential)
    {
        Conversations = HttpContext.Session.Get<List<Conversation>>("Conversations");

        User user2;
        var data = new User()
        {
            Email = credential,
            Login = credential
        };

        if (!UserManager.Instance.UserExists(data, out user2))
        {
            return new(new { success = false, message = "Specified user doesn't exist!" });
        }

        if (user2.UserId == HttpContext.Session.Get<User>("User").UserId)
        {
            return new(new { success = false, message = "You cannot open a conversation with Yourself!" });
        }

        var conversation = new Conversation()
        {
            User1Id = HttpContext.Session.Get<User>("User").UserId,
            User2Id = user2.UserId
        };

        if (Conversations.Where(x => x.User2Id ==  conversation.User2Id || x.User2Id == conversation.User2Id).Count() != 0)
        {
            return new(new { success = false, message = "You already have an open conversation with the specified user!" });
        }

        bool result = ChatManager.Instance.CreateConversation(conversation);

        return new(new { success = result, message = "There was an unexpected error!" });
    }

    public JsonResult OnPostSelectConversation(long conversationId)
    {
        CurrentConversation = HttpContext.Session.Get<Conversation?>("CurrentConversation");

        if (CurrentConversation?.ConversationId == conversationId)
        {
            CurrentConversation = null;
        }
        else
        {
            Conversations = HttpContext.Session.Get<List<Conversation>>("Conversations");
            CurrentConversation = Conversations.FirstOrDefault(x => x?.ConversationId == conversationId);
        }
        
        HttpContext.Session.Set("CurrentConversation", CurrentConversation);
        return new(new { success = true });
    }

    public JsonResult OnPostDeleteConversation(long conversationId)
    {
        bool result = ChatManager.Instance.DeleteConversation(conversationId);
        HttpContext.Session.Remove("CurrentConversation");

        return new(new { success = result });
    }

    public JsonResult OnPostSendMessage(string message)
    {
        var user = HttpContext.Session.Get<User>("User");
        CurrentConversation = HttpContext.Session.Get<Conversation?>("CurrentConversation");

        var data = new Message()
        {
            UserId = user.UserId,
            ConversationId = CurrentConversation.ConversationId,
            MessageBody = message
        };

        bool result = ChatManager.Instance.AddMessage(data);

        return new(new { success = result });
    }
}
