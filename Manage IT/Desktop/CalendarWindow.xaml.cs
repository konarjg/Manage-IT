using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Task = EFModeling.EntityProperties.DataAnnotations.Annotations.Task;

namespace Desktop
{
    
    public class DateModel
    {
        public string DateId { get; set; }
        public DateTime Date { get; set; }
        public List<Meeting> Meetings { get; set; }
        public List<Task> Tasks { get; set; }

        public ObservableCollection<DateItem> DateItems { get; set; }

        public bool IsToday
        {
            get
            {
                return Date.Day == DateTime.Now.Day && Date.Month == DateTime.Now.Month && Date.Year == DateTime.Now.Year;
            }
        }

        public bool IsOccupied
        {
            get
            {
                return Meetings.Count != 0 || Tasks.Count != 0;
            }
        }
    }

    public class DateItem
    {
        public string Name
        {
            get
            {
                if (Task != null)
                {
                    return Task.Name;
                }

                if (Meeting != null)
                {
                    return Meeting.Title;
                }

                return "";
            }
        }

        public Task? Task { get; set; }
        public Meeting? Meeting { get; set; }

        public object Item
        {
            get
            {
                if (Task != null)
                {
                    return Task;
                }

                if (Meeting != null)
                {
                    return Meeting;
                }

                return null;
            }
        }
    }


    public class CalendarModel : INotifyPropertyChanged
    {
        private ObservableCollection<DateItem> _items;
        private object _currentItem;

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public string TimeLeftText { get; set; }
        public bool IsEditor { get; set; }

        public Visibility EditorVisible
        {
            get
            {
                return IsEditor ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ObservableCollection<DateItem> Items
        {
            get
            {
                return _items;
            }

            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }

        public object CurrentItem
        {
            get
            {
                return _currentItem;
            }

            set
            {
                if (value == null)
                {
                    _currentItem = null;
                    return;
                }

                _currentItem = value;

                if (_currentItem.GetType() == typeof(Meeting))
                {
                    var meeting = _currentItem as Meeting;
                    Title = meeting.Title; 
                    Description = meeting.Description;
                    Date = meeting.Date;
                    var daysLeft = (int)((meeting.Date -  DateTime.Now).TotalDays);

                    var user = UserManager.Instance.CurrentSessionUser;
                    UserPermissions permissions;
                    Project project;
                    ProjectManager.Instance.GetProject(meeting.ProjectId, out project);
                    UserManager.Instance.GetCurrentUserPermissions(meeting.ProjectId, out permissions);

                    IsEditor = permissions.Editing;

                    TimeLeftText = $"Meeting in: {daysLeft}d";
                }
                else
                {
                    var task = _currentItem as Task;
                    Title = task.Name;

                    if (task.HandedIn)
                    {
                        Title += "(Handed in)";
                    }

                    Description = task.Description;
                    var daysLeft = (task.Deadline - DateTime.Now).Days;
                    var hoursLeft = (task.Deadline - DateTime.Now).Hours;

                    TimeLeftText = $"Deadline in: {daysLeft}d, {hoursLeft}h";
                }

                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(Date));
                OnPropertyChanged(nameof(TimeLeftText));
                OnPropertyChanged(nameof(IsEditor));
            }
        }

        public void SaveItem()
        {
            if (CurrentItem == null)
            {
                return;
            }

            if (CurrentItem.GetType() == typeof(Meeting))
            {
                var item = CurrentItem as Meeting;
                Title = item.Title;
                Description = item.Description;
                Date = item.Date;

                var daysLeft = (int)(((DateTime)Date - DateTime.Now).TotalDays);

                TimeLeftText = $"Meeting in: {daysLeft}d";

                var user = UserManager.Instance.CurrentSessionUser;
                UserPermissions permissions;
                Project project;
                ProjectManager.Instance.GetProject(item.ProjectId, out project);
                UserManager.Instance.GetCurrentUserPermissions(item.ProjectId, out permissions);

                IsEditor = permissions.Editing;

                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(Date));
                OnPropertyChanged(nameof(TimeLeftText));
                OnPropertyChanged(nameof(IsEditor));
            }
            else
            {
                var item = CurrentItem as Task;
                Title = $"{item.Name}(Handed in)";

                OnPropertyChanged(nameof(Title));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class CalendarWindow : Window
    {
        private DateTime Date;
        private string TemplateKey;
        private DateModel ActiveDate;
        private List<Meeting> Meetings;
        private List<Task> Tasks;

        public CalendarWindow()
        {
            InitializeComponent();
            SwitchPageTemplate("Main");
            TemplateKey = "Main";
            Date = DateTime.Now;
            InitializeData();
            DataContext = new CalendarModel();
        }

        private Style GetStyle(DateModel model)
        {
            string active;

            if (ActiveDate == null)
            {
                active = "";
            }
            else
            {
                active = model.DateId == ActiveDate.DateId ? "Active" : "";
            }

            string occupied = model.IsOccupied ? "Occupied" : "";
            string today = model.IsToday ? "Today" : "Normal";

            string key = $"{active}{occupied}{today}";
            return Resources[key] as Style;
        }

        private void InitializeData()
        {
            MeetingManager.Instance.GetAllMeetings(UserManager.Instance.CurrentSessionUser, out Meetings);
            TaskManager.Instance.GetAllAssignedTasks(UserManager.Instance.CurrentSessionUser, out Tasks);
            (DataContext as CalendarModel)?.Items?.Clear();
        }

        private void SwitchPageTemplate(string name)
        {
            Template = Resources[name] as ControlTemplate;
            TemplateKey = name;
        }

        private T GetTemplateControl<T>(string name) where T : class
        {
            return Template.FindName(name, this as FrameworkElement) as T;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            switch (TemplateKey)
            {
                case "Main":
                    UpdateCalendarDisplay();
                    break;

                case "Task":
                    var context = DataContext as CalendarModel;
                    var task = context.CurrentItem as Task;

                    if (task.HandedIn)
                    {
                        GetTemplateControl<Button>("TaskFinish").Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        GetTemplateControl<Button>("TaskFinish").Visibility = Visibility.Visible;
                    }

                    break;
            }
        }
        
        public void PrevMonthClick(object sender, RoutedEventArgs e)
        {
            Date = Date.AddMonths(-1);
            UpdateCalendarDisplay();
        }
        public void NextMonthClick(object sender, RoutedEventArgs e)
        {
            Date = Date.AddMonths(1);
            UpdateCalendarDisplay();
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

        public void UpdateCalendarDisplay()
        {
            GetTemplateControl<TextBlock>("MonthYear").Text = GetMonthName(Date.Month) + " " + Date.Year;

            var firstDayOfTheMonth = new DateTime(Date.Year, Date.Month, 1);
            var lastDayOfTheMonth = firstDayOfTheMonth.AddMonths(1).AddDays(-1);
            var numberOfDays = Convert.ToInt16(lastDayOfTheMonth.ToString("dd"));

            // Oblicz pierwszy widoczny dzień na kalendarzu
            var startDate = firstDayOfTheMonth.AddDays((int)DayOfWeek.Monday - (int)firstDayOfTheMonth.DayOfWeek);

            if (startDate > firstDayOfTheMonth)
            {
                startDate = startDate.AddDays(-7);
            }

            for (int i = 0; i < 42; i++)
            {
                DateTime dateModified = startDate.AddDays(i);

                string controlName = "Day" + (i + 1); // Kontrolki są nazwane "Day1", "Day2", ... "Day42"

                Button button = GetTemplateControl<Button>(controlName);
                List<Meeting> meetings = Meetings.Where(x => x.Date.Day == dateModified.Day && x.Date.Month == dateModified.Month && x.Date.Year == dateModified.Year).ToList();
                List<Task> tasks = Tasks.Where(x => x.Deadline.Day == dateModified.Day && x.Deadline.Month == dateModified.Month && x.Deadline.Year == dateModified.Year).ToList();
                ObservableCollection<DateItem> items = new();

                foreach (var meeting in meetings)
                {
                    items.Add(new() { Meeting = meeting });
                }

                foreach (var task in tasks)
                {
                    items.Add(new() { Task = task });
                }

                button.Tag = new DateModel()
                {
                    DateId = dateModified.ToShortDateString(),
                    Date = dateModified,
                    Meetings = meetings,
                    Tasks = tasks,
                    DateItems = items
                };

                button.Style = GetStyle(button.Tag as DateModel);

                if (button != null)
                {
                    if (dateModified.Month == Date.Month)
                    {
                        button.Foreground = Brushes.White;
                    }
                    else
                    {
                        button.Foreground = Brushes.Gray;
                    }

                    button.Content = dateModified.Day.ToString();
                }
            }
        }

        public void BackClick(object sender, RoutedEventArgs e)
        {
            var window = new ProjectManagementWindow();
            window.Activate();
            window.Visibility = Visibility.Visible;

            Close();
        }

        public void CalendarDayClick(object sender, RoutedEventArgs e)
        {
            var date = (sender as Button).Tag as DateModel;

            if (ActiveDate == null)
            {
                ActiveDate = date;
            }
            else if (ActiveDate.DateId == date.DateId)
            {
                ActiveDate = null;
            }
            else
            {
                ActiveDate = date;
            }

            var model = DataContext as CalendarModel;
            model.Items = date.DateItems;

            UpdateCalendarDisplay();
        }

        public void ManageDateItem(object sender, RoutedEventArgs e)
        {
            var model = (sender as Button).Tag as DateItem;
            object data = model.Item;

            if (data == null)
            {
                return;
            }

            if (data.GetType() == typeof(Meeting))
            {
                SwitchPageTemplate("Meeting");
            }
            else
            {
                SwitchPageTemplate("Task");
            }

            var context = DataContext as CalendarModel;
            context.CurrentItem = data;
        }

        public void ItemBackClick(object sender, RoutedEventArgs e)
        {
            SwitchPageTemplate("Main");

            var context = DataContext as CalendarModel;
            context.CurrentItem = null;

        }

        public void HandInClick(object sender, RoutedEventArgs e)
        {
            var context = DataContext as CalendarModel;
            var model = context.CurrentItem as Task;

            model.HandedIn = true;
            TaskManager.Instance.UpdateTask(model);
            context.SaveItem();
            InitializeData();
        }

        public void EditMeetingClick(object sender, RoutedEventArgs e)
        {
            var popup = GetTemplateControl<Border>("EditMeeting");
            popup.Visibility = Visibility.Visible;
        }

        public void DeleteMeetingClick(object sender, RoutedEventArgs e)
        {
            var popup = GetTemplateControl<Border>("DeleteMeeting");
            popup.Visibility = Visibility.Visible;
        }

        public void CancelDeleteMeeting(object sender, RoutedEventArgs e)
        {
            var popup = GetTemplateControl<Border>("DeleteMeeting");
            popup.Visibility = Visibility.Collapsed;
        }

        public void ConfirmDeleteMeeting(object sender, RoutedEventArgs e)
        {
            var model = DataContext as CalendarModel;
            var item = model.CurrentItem as Meeting;
            var error = GetTemplateControl<TextBlock>("DeleteError");

            bool success = MeetingManager.Instance.DeleteMeeting(item.MeetingId);

            if (!success)
            {
                error.Text = "There was an unexpected error!";
                return;
            }

            InitializeData();
            var popup = GetTemplateControl<Border>("DeleteMeeting");
            popup.Visibility = Visibility.Collapsed;

            SwitchPageTemplate("Main");
        }

        public void CancelEditMeeting(object sender, RoutedEventArgs e)
        {
            var popup = GetTemplateControl<Border>("EditMeeting");
            popup.Visibility = Visibility.Collapsed;
        }

        public void ConfirmEditMeeting(object sender, RoutedEventArgs e)
        {
            var model = (DataContext as  CalendarModel).CurrentItem as Meeting;
            var error = GetTemplateControl<TextBlock>("EditError");

            var title = GetTemplateControl<TextBox>("TitleEdit").Text;
            var description = GetTemplateControl<TextBox>("DescriptionEdit").Text;
            var date = GetTemplateControl<DateTimePicker>("DateEdit").Value;

            if (title == "" || description == "" || date == null)
            {
                error.Text = "You have to fill in every field!";
                return;
            }

            model.Title = title;
            model.Description = description;
            model.Date = (DateTime)date;
            (DataContext as CalendarModel).SaveItem();

            MeetingManager.Instance.UpdateMeeting(model);
            InitializeData();

            var popup = GetTemplateControl<Border>("EditMeeting");
            popup.Visibility = Visibility.Collapsed;
        }
    }
}