/* Author: Emma Haskins
 * Xceed Extended WPF Toolkit developed by Xceed Software, Inc.
 * Distributed under the Xceed Community License agreement(for non-commercial use)
 * See /licenses on Github for more
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public string Action_Argument { get; private set; }
        public string Action_Title { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            timePicker.Value = DateTime.Now;
            Shutdown_Radio.IsChecked = true;

            if (!HibernateEnabled())
            {
                Hibernate_Radio.IsEnabled = false;
                Log_Block.Text += "Hibernate is disabled on this system." + Environment.NewLine;
                Log_Block.ScrollToEnd();
            } else
            {
                Log_Block.Text += "Hibernate is enabled on this system." + Environment.NewLine;
                Log_Block.ScrollToEnd();
            }
        }
        public void Shutdown_Click(object sender, RoutedEventArgs e)
        {

            // Prepare default value
            DateTime LocalTime = DateTime.Now;
            TimeSpan OneHour = new TimeSpan(0, 1, 0, 0);
            DateTime OneHourIntoFuture = (LocalTime + OneHour);
            Boolean IsForceChecked = Force_Checkbox.IsChecked ?? false;

            // Connect to command prompt
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            process.StartInfo = startInfo;
            DateTime UserSelectedTime = timePicker.Value ?? OneHourIntoFuture;

            // Check for valid scheduled time and pass arguments to Windows command
            if (UserSelectedTime >= DateTime.Now)
            {
                // Calculate time difference between local time and selected scheduled time
                TimeSpan TimeDifference = (UserSelectedTime - LocalTime);

                //Convert difference into seconds, rounding down
                int TimeDiffSeconds = (int)(Math.Floor(TimeDifference.TotalSeconds));

                // Pass arguments to Windows command
                // Logic for if "Force?" checkbox is enabled
                if (IsForceChecked)
                {
                    startInfo.Arguments = "/C shutdown -f " + Action_Argument + " -t " + TimeDiffSeconds;
                    process.Start();
                    Log_Block.AppendText(Action_Title + " (forced)" + " scheduled for " + UserSelectedTime  + Environment.NewLine);
                    System.Windows.MessageBox.Show(Action_Title + " (forced)" + " scheduled for " + UserSelectedTime, "Success");
                    Log_Block.ScrollToEnd();
                }
                else
                {
                    startInfo.Arguments = "/C shutdown " + Action_Argument + " -t " + TimeDiffSeconds;
                    process.Start();
                    Log_Block.AppendText(Action_Title + " scheduled for " + UserSelectedTime + Environment.NewLine);
                    System.Windows.MessageBox.Show(Action_Title + " scheduled for " + UserSelectedTime, "Success");
                    Log_Block.ScrollToEnd();
                }           
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter a valid time", "Error");
                Log_Block.AppendText("Invalid time entered." + Environment.NewLine);
                Log_Block.ScrollToEnd();
            }
        }

        // Logic to cancel scheduled action
        // Is principally the same as above, just with a different argument.
        // This is probably redundant, and can likely be improved in the future
        public void Cancel_Shutdown(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            process.StartInfo = startInfo;
            startInfo.Arguments = "/C shutdown -a";
            process.Start();
            System.Windows.MessageBox.Show("Scheduled action canceled.", "Notice");
            Log_Block.AppendText("Scheduled action canceled." + Environment.NewLine);
            Log_Block.ScrollToEnd();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {

                About_Window About_Window = new About_Window();
                About_Window.Show();
        }

        private void Shutdown_Checked(object sender, RoutedEventArgs e)
        {
           Action_Argument = "-s";
           Action_Title = "Shutdown";
           Force_Checkbox.IsChecked = false;
           Force_Checkbox.IsEnabled = true;
        }

        private void Restart_Checked(object sender, RoutedEventArgs e)
        {
           Action_Argument = "-r";
           Action_Title = "Restart";
           Force_Checkbox.IsChecked = false;
           Force_Checkbox.IsEnabled = true;
        }
        private void Hibernate_Checked(object sender, RoutedEventArgs e)
        {
            Action_Argument = "-h";
            Action_Title = "Hibernate";
            Force_Checkbox.IsChecked = true;
            Force_Checkbox.IsEnabled = false;
        }

        // Check if hibernate is enabled on current computer
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct SYSTEM_POWER_CAPABILITIES
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool PowerButtonPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool SleepButtonPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool LidPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS1;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS2;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS3;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS4;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemS5;
            [MarshalAs(UnmanagedType.U1)]
            public bool HiberFilePresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool FullWake;
            [MarshalAs(UnmanagedType.U1)]
            public bool VideoDimPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool ApmPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool UpsPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool ThermalControl;
            [MarshalAs(UnmanagedType.U1)]
            public bool ProcessorThrottle;
            public byte ProcessorMinThrottle;
            public byte ProcessorMaxThrottle;
            [MarshalAs(UnmanagedType.U1)]
            public bool FastSystemS4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            private byte[] spare2;
            [MarshalAs(UnmanagedType.U1)]
            public bool DiskSpinDown;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            private byte[] spare3;
            [MarshalAs(UnmanagedType.U1)]
            public bool SystemBatteriesPresent;
            [MarshalAs(UnmanagedType.U1)]
            public bool BatteriesAreShortTerm;
        }

        [DllImport("powrprof.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES systemPowerCapabilites);
        
        public static bool HibernateEnabled()
        {
            SYSTEM_POWER_CAPABILITIES systemPowerCapabilites;
            bool ok = GetPwrCapabilities(out systemPowerCapabilites);
            return systemPowerCapabilites.SystemS4;
        }
        // End hibernate check
    }
}
