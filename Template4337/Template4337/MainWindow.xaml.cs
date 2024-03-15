using System.Windows;

namespace Template4337
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Kupriyanov4337_Click(object sender, RoutedEventArgs e)
        {
            var a = new Kupriyanov_4337();
            a.Show();
        }
    }
}
