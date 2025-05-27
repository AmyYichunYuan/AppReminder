using System;
using System.Windows;

namespace ShowMessageApp
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string message = args.Length > 0 ? args[0] : "Hello World!";
            var app = new App();
            var window = new MainWindow(message);
            app.Run(window);
        }
    }
}
