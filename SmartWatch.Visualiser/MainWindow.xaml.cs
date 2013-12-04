using System.Windows;
using SmartWatch.Core;
using SmartWatch.Core.Mocks;

namespace SmartWatch.Visualiser
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var arduino = new ArduinoMock();
            //var arduino = new Arduino("COM3");
            ViewModel = new ViewModel(arduino);
            InitializeComponent();
        }

        public ViewModel ViewModel { get; set; }
    }
}