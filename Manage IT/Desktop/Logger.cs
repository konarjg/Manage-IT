using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop
{
    public static class Logger
    {
        public static string LastMessage { get; private set; }

        public static void Log(string message)
        {
            MessageBox.Show(message);
            LastMessage = message;
        }

        public static void LogFormat(string format, params object[] args)
        {
            var message = string.Format(format, args);
            MessageBox.Show(message);
            LastMessage = message;
        }
    }
}
