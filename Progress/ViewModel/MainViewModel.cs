using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows;

namespace Progress.ViewModel
{
    [POCOViewModel(ImplementIDataErrorInfo = true, ImplementINotifyPropertyChanging = true)]
    public class MainViewModel
    {
        private Stopwatch stopWatch;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        protected MainViewModel()
        {
            URL = "https://library.princeton.edu/special-collections/sites/default/files/Creating_PDFA.pdf";
            if (POCOViewModelExtensions.IsInDesignMode(this))
            {
                Path = "-";
                Progress = 60;
                DownloadTime = "-";
            }
            else
            {
                stopWatch = new Stopwatch();
            }
        }

        public static MainViewModel Create()
        {
            return ViewModelSource.Create(() => new MainViewModel());
        }

        public bool CanOpenFile()
        {
            return OpenEnabled;
        }

        public void OpenFile()
        {
            DefaultLaunch(Path);
        }

        public bool CanStartDownload()
        {
            return true;
        }

        /// <summary>
        /// Start download
        /// </summary>
        public void StartDownload()
        {
            OpenEnabled = false;
            stopWatch.Start();
            DownloadTime = "00:00:00.00";
            Path = "...";
            string tmp = System.IO.Path.GetTempFileName();
            if (System.IO.Path.HasExtension(URL))
            {
                int idx = 0;
                int foundIdx = 0;
                bool found = false;
                while (!found)
                {
                    idx = URL.IndexOf('.', idx);
                    if (idx == -1)
                    {
                        found = true;
                    }
                    else
                    {
                        foundIdx = idx;
                        idx += 1;
                    }
                }
                //Console.WriteLine("Last found index of . found on " + foundIdx);
                string ext = URL.Substring(foundIdx + 1, URL.Length - foundIdx - 1);
                //Console.WriteLine("Extension is '" + ext + "'");
                tmp = System.IO.Path.ChangeExtension(tmp, ext);
            }
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(
                    new Uri(URL),
                    tmp
                );
                Path = tmp;
            }
        }

        /// <summary>
        /// Launch file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private bool DefaultLaunch(string filepath)
        {
            bool result = false;
            try
            {
                Process p = Process.Start(filepath);
                if (p != null)
                {
                    result = p.Id > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Event to track the progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
            if (Progress == 100)
            {
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                DownloadTime = elapsedTime;
                Console.WriteLine("[" + Thread.CurrentThread.ManagedThreadId + "] RunTime " + elapsedTime);
                OpenEnabled = true;
                this.RaiseCanExecuteChanged(c => c.StartDownload());
            }
        }

        /// <summary>
        /// Sets and gets the URL property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>       
        public virtual string URL { get; set; } = "";

        /// <summary>
        /// Sets and gets the Progress property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public virtual int Progress { get; set; } = 0;

        /// <summary>
        /// Sets and gets the Path property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public virtual string Path { get; set; } = "";

        /// <summary>
        /// Sets and gets the DownloadTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public virtual string DownloadTime { get; set; } = "";

        /// <summary>
        /// Property DownloadTime was changed.
        /// </summary>
        /// <param name="oldValue"></param>
        protected void OnDownloadTimeChanged(string oldValue)
        {
            //MessageBox.Show(string.Format("DownloadTimeChanged from {0} to {1}", oldValue, DownloadTime));
        }

        /// <summary>
        /// Property DownloadTime is changing.
        /// </summary>
        /// <param name="newValue"></param>
        protected void OnDownloadTimeChanging(string newValue)
        {
            //MessageBox.Show(string.Format("DownloadTimeChanging from {0} to {1}", DownloadTime, newValue));
        }

        /// <summary>
        /// Sets and gets the OpenEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public virtual bool OpenEnabled { get; set; } = false;

    }
}