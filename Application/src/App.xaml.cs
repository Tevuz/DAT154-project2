using no.hvl.DAT154.V23.GROUP14.SpaceModel;
using System.Windows;

namespace DAT154_project2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e) {
            Model.LoadFromFile("Planets.csv");
        }
    }
}