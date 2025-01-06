
using Desktop.Database;
using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Task = System.Threading.Tasks.Task;

namespace Desktop
{
    public class ConversationItem
    {
        public string Name { get; set; }
        public Conversation Conversation { get; set; }

        public ConversationItem(Conversation data)
        {
            Conversation = data;
            User user = UserManager.Instance.CurrentSessionUser;
            User user1;
            User user2;

            bool success = UserManager.Instance.GetUser(data.User1Id, out user1);
            success = UserManager.Instance.GetUser(data.User2Id, out user2) && success;
            
            if (!success)
            {
                MessageBox.Show("An unexpected error has occured!");
                return;
            }

            if (user.UserId != user1.UserId)
            {
                Name = user1.Login;
            }
            else
            {
                Name = user2.Login;
            }
        }
    }

    public class MessageItem : INotifyPropertyChanged
    {
        public string Body { get; set; }
        public Message Message { get; set; }
        public SolidColorBrush Color { get; set; }

        public MessageItem(Message data)
        {
            var userId = UserManager.Instance.CurrentSessionUser.UserId;
            Body = data.MessageBody;
            Message = data;
            
            if (data.UserId == userId)
            {
                Color = new SolidColorBrush(new Color()
                {
                    R = 0xa0,
                    G = 0xa0,
                    B = 0xff,
                    A = 0xff
                });

                OnPropertyChanged(nameof(Color));
            }
            else
            {
                Color = new SolidColorBrush(new Color()
                {
                    R = 0xa0,
                    G = 0xa0,
                    B = 0xa0,
                    A = 0xff
                });

                OnPropertyChanged(nameof(Color));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class ChatWindowModel : INotifyPropertyChanged
    {
        private ConversationItem _activeConversation;
        private bool _messageBoxActive;
        private ObservableCollection<ConversationItem> _conversationItems;

        public ObservableCollection<ConversationItem> Conversations { get; set; } 

        public bool MessageBoxActive
        {
            get
            {
                return _messageBoxActive;
            }

            set
            {
                if (_messageBoxActive != value)
                {
                    _messageBoxActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public ConversationItem ActiveConversation 
        {
            get
            {
                return _activeConversation;
            }

            set
            {
                if (_activeConversation != value)
                {
                    _activeConversation = value;
                    MessageBoxActive = _activeConversation != null;
                    LoadMessages(value?.Conversation.ConversationId);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Messages));
                    OnPropertyChanged(nameof(MessageBoxActive));
                }
            }

        }
        public ObservableCollection<MessageItem> Messages { get; set; }

        private void LoadConversations()
        {
            List<Conversation> conversations;
            ObservableCollection<ConversationItem> items = new();
            var userId = UserManager.Instance.CurrentSessionUser.UserId;

            if (!ChatManager.Instance.GetAllConversations(userId, out conversations))
            {
                conversations = new();
            }

            foreach (var conversation in conversations)
            {
                items.Add(new ConversationItem(conversation));
            }

            _conversationItems = items;
        }

        private async void LoadConversationsAsync()
        {
            await Task.Run(() =>
            {
                List<Conversation> conversations;
                ObservableCollection<ConversationItem> items = new();
                var userId = UserManager.Instance.CurrentSessionUser.UserId;

                if (!ChatManager.Instance.GetAllConversations(userId, out conversations))
                {
                    conversations = new();
                }

                foreach (var conversation in conversations)
                {
                    items.Add(new ConversationItem(conversation));
                }

                _conversationItems = items;
            });
        }

        private void LoadMessages(long? conversationId)
        {
            List<Message> messages;
            ObservableCollection<MessageItem> items = new();

            if (conversationId == null)
            {
                Messages.Clear();
                return;
            }

            if (!ChatManager.Instance.GetAllMessages((long)conversationId, out messages))
            {
                messages = new();
            }

            foreach (var message in messages)
            {
                items.Add(new MessageItem(message));
            }

            Messages = items;
        }


        public void FetchDataAsync(Dispatcher dispatcher)
        {
            LoadConversationsAsync();

            dispatcher.Invoke(() =>
            {
                var currentId = ActiveConversation?.Conversation.ConversationId;

                Conversations = _conversationItems;

                if (currentId != null)
                {
                    ActiveConversation = Conversations.FirstOrDefault(x => x.Conversation.ConversationId == currentId);
                }
                else
                {
                    ActiveConversation = null;
                }

                OnPropertyChanged(nameof(Conversations));
                OnPropertyChanged(nameof(ActiveConversation));
            });
        }

        public void FetchData(Dispatcher dispatcher)
        {
            LoadConversations();

            dispatcher.Invoke(() =>
            {
                var currentId = ActiveConversation?.Conversation.ConversationId;

                Conversations = _conversationItems;

                if (currentId != null)
                {
                    ActiveConversation = Conversations.FirstOrDefault(x => x.Conversation.ConversationId == currentId);
                }
                else
                {
                    ActiveConversation = null;
                }

                OnPropertyChanged(nameof(Conversations));
                OnPropertyChanged(nameof(ActiveConversation));
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged; 
        protected void OnPropertyChanged([CallerMemberName]string? name = null) 
        { 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); 
        }
    }

    public partial class ChatWindow : Window
    {
        private DispatcherTimer Timer;

        public ChatWindow()
        {
            InitializeComponent();
            DataContext = new ChatWindowModel();
            Timer = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            Timer.Tick += TimerTick;
            Timer.Start();

            (DataContext as ChatWindowModel).FetchData(Dispatcher);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            (DataContext as ChatWindowModel).FetchDataAsync(Dispatcher);
        }

        public void CreateConversation(object sender, RoutedEventArgs e)
        {
            var credential = Credential.Text;
            
            if (credential == string.Empty)
            {
                Error.Text = "You have to specify a user!";
                ErrorPopup.Visibility = Visibility.Visible;
                return;
            }

            User data = new()
            {
                Email = credential,
                Login = credential
            };
            User user;

            if (!UserManager.Instance.UserExists(data, out user))
            {
                Error.Text = "User doesn't exist!";
                ErrorPopup.Visibility = Visibility.Visible;
                return;
            }

            var conversation = new Conversation()
            {
                User1Id = UserManager.Instance.CurrentSessionUser.UserId,
                User2Id = user.UserId
            };

            if (ChatManager.Instance.CreateConversation(conversation))
            {
                (DataContext as ChatWindowModel).FetchData(Dispatcher);
                return;
            }

            Error.Text = "There was an unexpected error!";
            ErrorPopup.Visibility = Visibility.Visible;
        }

        public void DeleteConversation(object sender, RoutedEventArgs e)
        {
            var data = (sender as Button).Tag as Conversation;

            if (ChatManager.Instance.DeleteConversation(data.ConversationId))
            {
                (DataContext as ChatWindowModel).FetchData(Dispatcher);
                return;
            }

            Error.Text = "There was an unexpected error!";
            ErrorPopup.Visibility = Visibility.Visible;
        }

        public void CloseErrorPopup(object sender, RoutedEventArgs e)
        {
            ErrorPopup.Visibility = Visibility.Collapsed;
        }

        private void MessagesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void MessageBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (MessageBox.Text != "")
            {
                MessageBoxPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBoxPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void CredentialTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Credential.Text != "")
            {
                CredentialPlaceholder.Visibility = Visibility.Collapsed;    
            }
            else
            {
                CredentialPlaceholder.Visibility = Visibility.Visible;
            }
        }

        public void SendMessage(object sender, RoutedEventArgs e)
        {
            var body = MessageBox.Text;

            if (body == "")
            {
                Error.Text = "You have to type something!";
                ErrorPopup.Visibility = Visibility.Visible;
                return;
            }

            var data = new Message()
            {
                ConversationId = (DataContext as ChatWindowModel).ActiveConversation.Conversation.ConversationId,
                MessageBody = body,
                UserId = UserManager.Instance.CurrentSessionUser.UserId
            };

            if (ChatManager.Instance.AddMessage(data))
            {
                (DataContext as ChatWindowModel).FetchData(Dispatcher);
                return;
            }

            Error.Text = "There was an unexpected error!";
            ErrorPopup.Visibility = Visibility.Visible;
        }

        public void BackClick(object sender, RoutedEventArgs e)
        {
            var window = new ProjectManagementWindow();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
        }
    }
}
