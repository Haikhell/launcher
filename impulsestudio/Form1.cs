using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Rocket.features;
using System.Timers;
using GameAnalyticsSDK.Net;
using Microsoft.Win32;
using System.Web.Script.Serialization;

namespace impulsestudio
{
    public partial class mainGrid : Form
    {

        public void InitializeAnalytics()
        {
            GameAnalytics.SetEnabledInfoLog(true);
            GameAnalytics.SetEnabledVerboseLog(true);

            GameAnalytics.ConfigureBuild("0.10");

            GameAnalytics.Initialize("a0bc3f6566d71e5855ee5973555ddd21", "d881f82c6e32cffe030a822163aba8dda4b9ce15");

            

        }

        public void InitializeCEF()
        {
            ChromiumWebBrowser browser = new ChromiumWebBrowser(Path.Combine(Environment.CurrentDirectory, @"www\index.html"));/*"crimetest.ru/index.html");*/
            this.Controls.Add(browser);

            browser.JavascriptObjectRepository.Register("callbackObject", new JavascriptCallback(ref browser), true);

            browser.IsBrowserInitializedChanged += (s, e) =>
            {
                //browser.ShowDevTools();


            };

            browser.DragHandler = new DragDropHandler();

            browser.IsBrowserInitializedChanged += (sender, args) =>
            {
                if (browser.IsBrowserInitialized)
                {
                    ChromeWidgetMessageInterceptor.SetupLoop(browser, (message) =>
                    {
                        if (message.Msg == 0x0201) //left button down
                        {
                            Point point = new Point(message.LParam.ToInt32());

                            if (((DragDropHandler)browser.DragHandler).draggableRegion.IsVisible(point))
                            {
                                ReleaseCapture();
                                SendHandleMessage();
                            }
                        }
                    });
                }
            };

            browser.FrameLoadEnd += (sender, args) =>
            {
                if (args.Frame.IsMain)
                {

                    this.Invoke(new Action(() =>
                    {
                        System.Windows.Forms.Timer delay = new System.Windows.Forms.Timer();

                        delay.Interval = 40;
                        delay.Start();

                        delay.Tick += (s, e) =>
                        {
                            if (this.Opacity < 1)
                                this.Opacity += 0.05;
                            else
                                delay.Stop();
                        };
                    }));

                }
            };
        }

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public void SendHandleMessage()
        {
            if (InvokeRequired) { Invoke(new SendHandleMessageDelegate(SendHandleMessage), new object[] { }); return; }

            SendMessage(Handle, 0xA1, 0x2, 0);
        }
        public delegate void SendHandleMessageDelegate();


        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );


        public mainGrid()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 110, 110));

            InitializeAnalytics();

            InitializeCEF();

      

        }

        private void Form_Closed(object sender, System.EventArgs e)
        {


            //GameAnalytics.OnQuit();
            //Environment.Exit(0);
            //Process.Kill();
        }

        

        public class JavascriptCallback 
        {

            ChromiumWebBrowser browser;

            public JavascriptCallback(ref ChromiumWebBrowser browser)
            {
                this.browser = browser;
            }

            struct files_struct
            {
                public string name;
                public string size;
            };

            List<files_struct> files = new List<files_struct>();



            

            public void DownloadFile(int index)
            {

                //MessageBox.Show(files.Count.ToString());

                WebClient client = new WebClient();

                client.DownloadFileAsync(new Uri("https://go2rage.ru/launcher/" + files[index].name), Path.Combine(Environment.CurrentDirectory, "cache\\5fbe535e9a65c5d", files[index].name));

                client.DownloadFileCompleted += (s, e) =>
                {

                    double progress;

                    /*if (index == 0)
                        progress = (100 / files.Count) * (index + 1);
                    else
                        progress = (100 / files.Count) * (index);
                    */

                    progress = (100 / files.Count) * (index + 1);

                    //Math.Round(progress);

                    browser.ExecuteScriptAsync("changePercent('Скачиваем... " + Math.Round(progress) + "%')");
                    //browser.ExecuteScriptAsync("changePercent('" + progress + "')");


                    if (index + 1 < files.Count)
                    {

                        DownloadFile(++index);

                        //MessageBox.Show(index.ToString());

                        
                    }
                    else
                        start_play();

                };
            }

            public void check_files()
            {
                WebClient client = new WebClient();

                string myJSON = client.DownloadString("https://go2rage.ru/files.json");

                dynamic Received = new JavaScriptSerializer().Deserialize<dynamic>(myJSON);

                

                foreach (var i in Received)
                {

                    files_struct file = new files_struct();



                    if (File.Exists(Path.Combine(Environment.CurrentDirectory, @"cache\5fbe535e9a65c5d\") + i["name"].ToString()))
                    {
                        //MessageBox.Show("true");

                        FileInfo info = new FileInfo(Path.Combine(Environment.CurrentDirectory, @"cache\5fbe535e9a65c5d\") + i["name"].ToString());

                        if (info.Length != i["size"])
                        {
                            file.name = i["name"].ToString();
                            file.size = i["size"].ToString();

                            files.Add(file);
                        }



                    }
                    else
                    {
                        file.name = i["name"].ToString();
                        file.size = i["size"].ToString();

                        files.Add(file);
                    }
                }

                if (files.Count != 0)
                    start_download();
                else
                    start_play();
            }

            public void start_play()
            {

                browser.ExecuteScriptAsync("changePercent('Скачивание завершено')");

                files.Clear();

                GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Complete, "Click Play: " + 1);

                var altv = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv");

                altv.SetValue(string.Empty, "alt:V Multiplayer");
                altv.SetValue("URL Protocol", "");



                Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv\\DefaultIcon");
                Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv\\shell");
                Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv\\shell\\open");
                var command = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv\\shell\\open\\command");



                command.SetValue(string.Empty, Path.Combine(Environment.CurrentDirectory, "altv.exe -connecturl %1"));

                command.Close();

                altv.Close();

                System.Diagnostics.Process.Start("altv.exe", "-connecturl altv://connect/139.162.169.165:22005");
            }

            public void start_download()
            {

                //List<files_struct> files = new List<files_struct>();

                /*WebClient client = new WebClient();

                string myJSON = client.DownloadString("https://go2rage.ru/files.json");

                dynamic Received = new JavaScriptSerializer().Deserialize<dynamic>(myJSON);*/

                //MessageBox.Show(Received);

                /* foreach (var i in Received)
                 {
                     //MessageBox.Show(i["size"].ToString());

                     files_struct file = new files_struct();

                     file.name = i["name"].ToString();
                     file.size = i["size"].ToString() ;

                     files.Add(file);



                 }*/

                //MessageBox.Show(files.Count.ToString());

                browser.ExecuteScriptAsync("showDownload()");

                //MessageBox.Show("Start Download");

                DownloadFile(0);


            }

            mainGrid MainWindow = null;

            public void message(string text)
            {

                if (MainWindow == null)
                    foreach (mainGrid f in Application.OpenForms)
                        if (f.GetType() == typeof(mainGrid))
                            MainWindow = (mainGrid)f;

                if (String.Compare(text, "appexit") == 0)
                    Environment.Exit(0);

                if (String.Compare(text, "startplay") == 0)
                {
                    /*Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = "C://altv//altv.exe -connecturl altv://connect/172.105.77.14:22005";
                    p.Start();*/

                    //GameAnalytics.SetCustomDimension01("Levl");




                    /*var altv = Registry.ClassesRoot.CreateSubKey("altv");
                    Registry.Users.CreateSubKey("S-1-5-21-2992709421-1073072368-1369039414-1001\\SOFTWARE\\Classes\\testing\\DefaultIcon");
                    Registry.Users.CreateSubKey("S-1-5-21-2992709421-1073072368-1369039414-1001\\SOFTWARE\\Classes\\testing\\shell");
                    Registry.Users.CreateSubKey("S-1-5-21-2992709421-1073072368-1369039414-1001\\SOFTWARE\\Classes\\testing\\shell\\open");

                    altv.SetValue(string.Empty, "alt:V Multiplayer");
                    altv.SetValue("URL Protocol", "");

                    altv.Close();*/
                    /*
                    GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Complete, "Click Play: " + 1);

                    var altv = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv");

                    altv.SetValue(string.Empty, "alt:V Multiplayer");
                    altv.SetValue("URL Protocol", "");

                    

                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv\\DefaultIcon");
                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv\\shell");
                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv\\shell\\open");
                    var command = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\altv\\shell\\open\\command");



                    command.SetValue(string.Empty, Path.Combine(Environment.CurrentDirectory, "altv.exe -connecturl %1"));

                    command.Close();

                    altv.Close();


                    WebClient cef = new WebClient();

                    cef.DownloadFile("", "");

                    cef.DownloadFileCompleted += (s, e) =>
                    {
                        

                    };

                    System.Diagnostics.Process.Start("altv.exe", "-connecturl altv://connect/139.162.169.165:22005");*/

                    check_files();
                }

                if (String.Compare(text, "mainsite_button") == 0)
                    Process.Start("https://go2rage.ru/?utm_source=lc");

                if (String.Compare(text, "secretpromo_button") == 0)
                    Process.Start("https://go2rage.ru/promo/?utm_source=lc");
            }
        }
    }
}
