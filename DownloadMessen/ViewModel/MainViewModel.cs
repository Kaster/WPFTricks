using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;

namespace DownloadMessen.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private Stopwatch stopWatch;

        public RelayCommand StartDownloadCommand { get; private set; }
        public RelayCommand OpenFileCommand { get; private set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            URL = "https://library.princeton.edu/special-collections/sites/default/files/Creating_PDFA.pdf";
            if (IsInDesignMode)
            {
                Pfad = "-";
                Progress = 60;
                DownloadTime = "-";
            }
            else
            {
                StartDownloadCommand = new RelayCommand(StartDownload, CanStartDownload);
                OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);
                stopWatch = new Stopwatch();
            }
        }

        private bool CanOpenFile()
        {
            return OpenEnabled;
        }

        private void OpenFile()
        {
            DefaultLaunch(Pfad);
        }

        public bool CanStartDownload()
        {
            return true;
        }

        public void StartDownload()
        {
            OpenEnabled = false;
            stopWatch.Start();
            DownloadTime = "00:00:00.00";
            Pfad = "...";
            string tmp = Path.GetTempFileName();
            if (Path.HasExtension(URL))
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
                    } else
                    {
                        foundIdx = idx;
                        idx += 1;
                    }
                }
                Console.WriteLine("Last found index of . found on " + foundIdx);
                string ext = URL.Substring(foundIdx + 1, URL.Length - foundIdx - 1);
                Console.WriteLine("Extension is '" + ext + "'");
                tmp = Path.ChangeExtension(tmp, ext);
            }
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(
                    new Uri(URL),
                    tmp
                );
                Pfad = tmp;
            }
        }

        /// <summary>
        /// Startet übergebene Datei im entsprechenden Programm.
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

        // Event to track the progress
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
                OpenFileCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The <see cref="URL" /> property's name.
        /// </summary>
        public const string URLPropertyName = "URL";

        private string _URL = "";

        /// <summary>
        /// Sets and gets the URL property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string URL
        {
            get
            {
                return _URL;
            }

            set
            {
                if (_URL == value)
                {
                    return;
                }

                _URL = value;
                RaisePropertyChanged(URLPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Progress" /> property's name.
        /// </summary>
        public const string ProgressPropertyName = "Progress";

        private int _Progress = 0;

        /// <summary>
        /// Sets and gets the Progress property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Progress
        {
            get
            {
                return _Progress;
            }

            set
            {
                if (_Progress == value)
                {
                    return;
                }

                _Progress = value;
                RaisePropertyChanged(ProgressPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Pfad" /> property's name.
        /// </summary>
        public const string PfadPropertyName = "Pfad";

        private string _Pfad = "";

        /// <summary>
        /// Sets and gets the Pfad property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Pfad
        {
            get
            {
                return _Pfad;
            }

            set
            {
                if (_Pfad == value)
                {
                    return;
                }

                _Pfad = value;
                RaisePropertyChanged(PfadPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="DownloadTime" /> property's name.
        /// </summary>
        public const string DownloadTimePropertyName = "DownloadTime";

        private string _DownloadTime = "";

        /// <summary>
        /// Sets and gets the DownloadTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DownloadTime
        {
            get
            {
                return _DownloadTime;
            }

            set
            {
                if (_DownloadTime == value)
                {
                    return;
                }

                _DownloadTime = value;
                RaisePropertyChanged(DownloadTimePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="OpenEnabled" /> property's name.
        /// </summary>
        public const string OpenEnabledPropertyName = "OpenEnabled";

        private bool _OpenEnabled = false;

        /// <summary>
        /// Sets and gets the OpenEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool OpenEnabled
        {
            get
            {
                return _OpenEnabled;
            }

            set
            {
                if (_OpenEnabled == value)
                {
                    return;
                }

                _OpenEnabled = value;
                RaisePropertyChanged(OpenEnabledPropertyName);
            }
        }
    }
}