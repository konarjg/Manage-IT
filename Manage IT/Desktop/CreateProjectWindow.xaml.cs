using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Desktop
{
    /// <summary>
    /// Logika interakcji dla klasy CalendarWindow.xaml
    /// </summary>
    public partial class CalendarWindow : Window
    {
        DateTime Date = DateTime.Now;
        public CalendarWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            this.Title = "ManageIT Calendar";
            UpdateCalendarDisplay(Date);

        }


        public void Mon1Click(object sender, RoutedEventArgs e)
        {

        }
        public void Tue1Click(object sender, RoutedEventArgs e)
        {

        }
        public void Wed1Click(object sender, RoutedEventArgs e)
        {

        }
        public void Thu1Click(object sender, RoutedEventArgs e)
        {

        }
        public void Fri1Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sat1Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sun1Click(object sender, RoutedEventArgs e)
        {

        }
        public void Mon2Click(object sender, RoutedEventArgs e)
        {

        }
        public void Tue2Click(object sender, RoutedEventArgs e)
        {

        }
        public void Wed2Click(object sender, RoutedEventArgs e)
        {

        }
        public void Thu2Click(object sender, RoutedEventArgs e)
        {

        }
        public void Fri2Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sat2Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sun2Click(object sender, RoutedEventArgs e)
        {

        }
        public void Mon3Click(object sender, RoutedEventArgs e)
        {

        }
        public void Tue3Click(object sender, RoutedEventArgs e)
        {

        }
        public void Wed3Click(object sender, RoutedEventArgs e)
        {

        }
        public void Thu3Click(object sender, RoutedEventArgs e)
        {

        }
        public void Fri3Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sat3Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sun3Click(object sender, RoutedEventArgs e)
        {

        }
        public void Mon4Click(object sender, RoutedEventArgs e)
        {

        }
        public void Tue4Click(object sender, RoutedEventArgs e)
        {

        }
        public void Wed4Click(object sender, RoutedEventArgs e)
        {

        }
        public void Thu4Click(object sender, RoutedEventArgs e)
        {

        }
        public void Fri4Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sat4Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sun4Click(object sender, RoutedEventArgs e)
        {

        }
        public void Mon5Click(object sender, RoutedEventArgs e)
        {

        }
        public void Tue5Click(object sender, RoutedEventArgs e)
        {

        }
        public void Wed5Click(object sender, RoutedEventArgs e)
        {

        }
        public void Thu5Click(object sender, RoutedEventArgs e)
        {

        }
        public void Fri5Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sat5Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sun5Click(object sender, RoutedEventArgs e)
        {

        }
        public void Mon6Click(object sender, RoutedEventArgs e)
        {

        }
        public void Tue6Click(object sender, RoutedEventArgs e)
        {

        }
        public void Wed6Click(object sender, RoutedEventArgs e)
        {

        }
        public void Thu6Click(object sender, RoutedEventArgs e)
        {

        }
        public void Fri6Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sat6Click(object sender, RoutedEventArgs e)
        {

        }
        public void Sun6Click(object sender, RoutedEventArgs e)
        {

        }
        public void PrevMonthClick(object sender, RoutedEventArgs e)
        {
            Date = Date.AddMonths(-1);
            UpdateCalendarDisplay(Date);

        }
        public void NextMonthClick(object sender, RoutedEventArgs e)
        {
            Date = Date.AddMonths(1);
            UpdateCalendarDisplay(Date);
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

        public void UpdateCalendarDisplay(DateTime date)
        {
            MonthYear.Text = GetMonthName(date.Month) + " " + date.Year;

            var firstDayOfTheMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfTheMonth = firstDayOfTheMonth.AddMonths(1).AddDays(-1);
            var numberOfDays = Convert.ToInt16(lastDayOfTheMonth.ToString("dd"));

            // Oblicz pierwszy widoczny dzień na kalendarzu
            var startDate = firstDayOfTheMonth;
            if (firstDayOfTheMonth.DayOfWeek != DayOfWeek.Sunday)
            {
                startDate = firstDayOfTheMonth.AddDays(-(int)firstDayOfTheMonth.DayOfWeek);
            }
            else
            {
                startDate = firstDayOfTheMonth.AddDays(-6);
            }

            for (int i = 0; i < 42; i++)
            {
                DateTime dateModified = startDate.AddDays(i);

                string controlName = "Day" + (i + 1); // Kontrolki są nazwane "Day1", "Day2", ... "Day42"

                Button btn = this.FindName(controlName) as Button;

                if (btn != null)
                {
                    if (dateModified.Month == date.Month)
                        btn.Foreground = new SolidColorBrush(Colors.White);
                    else
                        btn.Foreground = new SolidColorBrush(Colors.Gray);

                    btn.Content = dateModified.Day.ToString();
                }
                else
                {
                    Console.WriteLine($"Control '{controlName}' not found.");
                }
            }
        }



    }
}
