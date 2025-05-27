using System.Windows;

namespace ShowMessageApp
{
    public partial class MainWindow : Window
    {
        public string Message { get; set; }

        public MainWindow(string message)
        {
            InitializeComponent();
            Message = string.IsNullOrWhiteSpace(message) ? "Hello World" : message;
            this.DataContext = this;
        }
    }
}
