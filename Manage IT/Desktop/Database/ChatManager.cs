﻿using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Desktop.Database
{
    public class ChatManager
    {
        public static ChatManager Instance { get; private set; }

        public static void Instantiate()
        {
            Instance = new();
        }

        public bool GetAllConversations(long userId, out List<Conversation> conversations)
        {
            System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Conversations WHERE User1Id = {userId} OR User2Id = {userId}");
            return DatabaseAccess.Instance.ExecuteQuery(query, out conversations);
        }

        public bool GetAllMessages(long conversationId, out List<Message> messages)
        {
            System.FormattableString query = FormattableStringFactory.Create($"SELECT * FROM dbo.Messages WHERE ConversationId = {conversationId}");
            return DatabaseAccess.Instance.ExecuteQuery(query, out messages);
        }

        public bool CreateConversation(Conversation data)
        {
            List<Conversation> conversations;
            System.FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.Conversations(User1Id, User2Id) VALUES({data.User1Id}, {data.User2Id})");

            return DatabaseAccess.Instance.ExecuteQuery(query, out conversations);
        }

        public bool AddMessage(Message data)
        {
            List<Message> messages;
            System.FormattableString query = FormattableStringFactory.Create($"INSERT INTO dbo.Messages(ConversationId, UserId, MessageBody) VALUES({data.ConversationId}, {data.UserId}, '{data.MessageBody}')");

            return DatabaseAccess.Instance.ExecuteQuery(query, out messages);
        }

        public bool DeleteAllMessages(long conversationId)
        {
            List<Message> messages;
            System.FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Messages WHERE ConversationId = {conversationId}");

            return DatabaseAccess.Instance.ExecuteQuery(query, out messages);
        }

        public bool DeleteConversation(long conversationId)
        {
            List<Conversation> conversations;
            System.FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Conversations WHERE ConversationId = {conversationId}");

            return DeleteAllMessages(conversationId) && DatabaseAccess.Instance.ExecuteQuery(query, out conversations);
        }

        public bool DeleteAllConversations(long userId)
        {
            List<Conversation> conversations;
            bool success = GetAllConversations(userId, out conversations);

            if (!success)
            {
                return false;
            }

            foreach (Conversation conversation in conversations)
            {
                DeleteAllMessages(conversation.ConversationId);
            }

            System.FormattableString query = FormattableStringFactory.Create($"DELETE FROM dbo.Conversations WHERE User1Id = {userId} OR User2Id = {userId}");

            return DatabaseAccess.Instance.ExecuteQuery(query, out conversations);
        }
    }
}
