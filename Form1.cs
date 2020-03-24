using AutoDateTime.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;


namespace AutoDateTime
{
    public partial class Form1 : Form
    {
        static readonly HttpClient client = new HttpClient();
        String Api = "http://worldtimeapi.org/api/timezone/Asia/Ho_Chi_Minh";
        String Api2 = String.Empty;
        System.Timers.Timer aTimer;
        public Form1()
        {
            InitializeComponent();
        }

        Int32 interval = 300000;

        private void Form1_Load(object sender, EventArgs e)


        {
            //this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info; //Shows the info icon so the user doesn't think there is an error.
            //this.notifyIcon.BalloonTipText = "Auto Update Time was minized in the system tray";
            //this.notifyIcon.BalloonTipTitle = "Auto Update Time";
            //Icon myIcon = (Icon)Resources.ResourceManager.GetObject("clock");
            //this.notifyIcon.Icon = myIcon;
            //this.notifyIcon.Text = "Auto Update Time";

            String config = System.Configuration.ConfigurationSettings.AppSettings["IntervalTimeInMilisecond"].ToString();
            try
            {
                interval = Int32.Parse(config);
            }
            catch
            {
                interval = 300000;
            }
            Api2 = System.Configuration.ConfigurationSettings.AppSettings["LocalApi"].ToString();

            SyncTime();

             aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = interval;
            aTimer.Start();

            //this.WindowState = FormWindowState.Minimized;


        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            SyncTime();
        }

        void setDate(string dateInYourSystemFormat)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c date " + dateInYourSystemFormat;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();
        }
        void setTime(string timeInYourSystemFormat)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c time " + timeInYourSystemFormat;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        DateTime date;

        private async void button1_Click(object sender, EventArgs e)
        {
            SyncTime();
        }


        private async void SyncTime()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(Api);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                String datetimeString = responseBody;
                String ptt = "\"unixtime\":";
                int id = datetimeString.IndexOf(ptt);

                datetimeString = datetimeString.Substring(id + ptt.Length, datetimeString.Length - ptt.Length - id);

                int id2 = datetimeString.IndexOf(",");

                datetimeString = datetimeString.Substring(0, id2);

                date = UnixTimeStampToDateTime(Double.Parse(datetimeString));
              //  richTextBox1.Text = (date.ToString() + "\n" + date.ToShortDateString() + "\n" + date.TimeOfDay.ToString());

                try
                {
                    setDate(date.ToShortDateString());
                }
                catch
                { }

                try
                {
                    setTime(date.ToLongTimeString());
                }
                catch
                { }

                lbApi1.Text = "o";
                lbApi2.Text = "x";
            }
            catch (HttpRequestException ex)
            {
                lbApi1.Text = "x";
            
                try
                {
                    HttpResponseMessage response = await client.GetAsync(Api2);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);
                    String datetimeString = responseBody;
                    datetimeString = datetimeString.Replace("\"", String.Empty).Trim();
                    date = UnixTimeStampToDateTime(Double.Parse(datetimeString));
                    //  richTextBox1.Text = (date.ToString() + "\n" + date.ToShortDateString() + "\n" + date.TimeOfDay.ToString());

                    try
                    {
                        setDate(date.ToShortDateString());
                    }
                    catch
                    { }

                    try
                    {
                        setTime(date.ToLongTimeString());
                    }
                    catch
                    { }
                    lbApi2.Text = "o";
                }
                catch
                {
                    lbApi2.Text = "x";
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {

         


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                aTimer.Stop();
            }
            catch
            {

            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                aTimer.Stop();
            }
            catch
            {

            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //if (this.WindowState == FormWindowState.Minimized)
            //{
            //    notifyIcon.Visible = true;
            //    notifyIcon.ShowBalloonTip(3000);
            //    this.ShowInTaskbar = false;
            //}
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //this.WindowState = FormWindowState.Normal;
            //this.ShowInTaskbar = true;
            //notifyIcon.Visible = false;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

        }
    }
}
