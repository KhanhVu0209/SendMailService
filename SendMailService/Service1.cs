using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SendMailService
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        private static readonly HttpClient client = new HttpClient();
        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            WriteTextToFile("Service started at " + DateTime.Now);
            schedule_Timer();
        }
        protected override void OnStop()
        {
            WriteTextToFile("Service stop at " + DateTime.Now);
        }
        public void schedule_Timer()
        {
            WriteTextToFile("Timer Start at " + DateTime.Now);

            DateTime nowTime = DateTime.Now;
            DateTime scheduledTimeMorning = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 22, 45, 0, 0);
            DateTime scheduledTimeAfter = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 22, 48, 0, 0);

            if (nowTime < scheduledTimeMorning && nowTime < scheduledTimeAfter)
            {
                //send at 8h
                double tickTime = (double)(scheduledTimeMorning - DateTime.Now).TotalMilliseconds;
                timer = new System.Timers.Timer(tickTime);
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Start();
            }
            if (nowTime > scheduledTimeMorning && nowTime < scheduledTimeAfter)
            {
                //send at 15h
                double tickTime = (double)(scheduledTimeAfter - DateTime.Now).TotalMilliseconds;
                timer = new System.Timers.Timer(tickTime);
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Start();
            }
            if (nowTime > scheduledTimeMorning && nowTime > scheduledTimeAfter)
            {
                //set up time for 8h
                scheduledTimeMorning = scheduledTimeMorning.AddDays(1);

                //set at 8h tomorrow
                double tickTime = (double)(scheduledTimeMorning - DateTime.Now).TotalMilliseconds;
                timer = new System.Timers.Timer(tickTime);
                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                timer.Start();
            }
        }
        public async void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            WriteTextToFile("SendMail!!!");
            //await ProcessRepositories();
            WriteTextToFile("Task Finished at " + DateTime.Now);
            schedule_Timer();
        }
        public void WriteTextToFile(string Message)
        {
            string checkPath = AppDomain.CurrentDomain.BaseDirectory + "\\LogsFile";
            if (!Directory.Exists(checkPath))
            {
                Directory.CreateDirectory(checkPath);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\LogsFile\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }

            }
        }
        //call api
        private static async Task ProcessRepositories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = client.GetStringAsync("http://115.74.201.161:99/api/RemindTask/SendMailAuto?SecretKey=vt~)4/%26]AugM@gNw[q63%26ps-]sP*gd_~2Ga$%3C$2U%3EacHckN~Jd5=G4R)GV%3EC");

            var msg = await stringTask;
            Console.Write(msg);
        }
    }
}
