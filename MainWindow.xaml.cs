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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace ShutdownAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            timePicker.Value = DateTime.Now;
        }

        //public void TimePicker_UserTimeSelected(object sender, RoutedPropertyChangedEventHandler<DateTime> e)
        //{
        //    var picker = sender as TimePicker;
        //    DateTime? time = picker.Value;
        //}

        public void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan LocalTime = DateTime.Now.TimeOfDay;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            process.StartInfo = startInfo;
            if (timePicker.Value.HasValue)
            {
                var UserSelectedTime = timePicker.Value;
                TimeSpan TimeDifference = (UserSelectedTime - LocalTime).TotalSeconds;
                startInfo.Arguments = "/C shutdown -s -t " + TimeDifference;
                System.Windows.MessageBox.Show(startInfo.Arguments);
                process.Start();
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter a valid time", "Error");
            }
        }
        public void Cancel_Shutdown(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            process.StartInfo = startInfo;
            startInfo.Arguments = "/C shutdown -a";
            process.Start();
        }
    }
}
