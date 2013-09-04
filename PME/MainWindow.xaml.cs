using System.Configuration;
using System.Windows;
using Autofac;
using Communication;

namespace PME
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];

            var scraper = App.AutoFacContainer.Resolve<IWebScraper>(new NamedParameter("username", username), new NamedParameter("password", password));
            DataContext = new MainWindowViewModel(scraper);
        }
    }
}
