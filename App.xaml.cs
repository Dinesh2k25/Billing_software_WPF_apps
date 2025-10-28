using System.Configuration;
using System.Data;
using System.Windows;

namespace BS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"unhandled Exceptionoccured:{e.Exception.Message}\n\n{e.Exception.StackTrace}","ApplicationError E)!!!",MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }

}
