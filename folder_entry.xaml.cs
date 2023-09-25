﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace folder_cacher_net
{
    /// <summary>
    /// Interaction logic for folder_entry.xaml
    /// </summary>
    public partial class folder_entry : UserControl
    {
        public FolderEntry _FolderEntry { get; private set; } = new FolderEntry();

        private MainWindow Outer;
        private byte RunStatus = 0;
        private bool IsPaused = false;

        private string DIRECTORY;
        private float PERCENT;
        private UInt32 WORKER;

        private UInt32 CURRENT_FILE;
        private UInt32 TOTAL_FILES;
        private List<FileInfo> Files = new List<FileInfo>();

        private List<Thread> Threads = new List<Thread>();
        private UInt32 FinishedWorker;
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private string NewStatus;

        public folder_entry(MainWindow _Outer)
        {
            InitializeComponent();
            Outer = _Outer;
            UpdateButtons();

            Application.Current.Exit += OnExit;

            _timer.Interval = 0.1;
            _timer.Start();
            _timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                WriteStatus();
            };
        }

        private void SaveFolderEntry()
        {
            _FolderEntry.DirectoryPath = txt_Path.Text;

            float _percent;
            float.TryParse(txt_Percent.Text, out _percent);
            _FolderEntry.Percent = _percent;

            UInt32 _worker;
            UInt32.TryParse(txt_Worker.Text, out _worker);
            _FolderEntry.Worker = _worker;
        }

        private void LoadFolderEntry()
        {
            txt_Path.Text = _FolderEntry.DirectoryPath;
            txt_Percent.Text = _FolderEntry.Percent.ToString();
            txt_Worker.Text = _FolderEntry.Worker.ToString();
        }

        public void SetFolderEntry(FolderEntry _Entry)
        {
            _FolderEntry = _Entry;
            LoadFolderEntry();
        }

        private void OnExit(object sender, EventArgs e)
        {
            foreach (var _thread in Threads)
            {
                _thread.Abort();
            }
        }

        private void UpdateStatus(string _text)
        {
            NewStatus = _text;
        }

        private void WriteStatus()
        {
            this.Dispatcher.Invoke(() =>
            {
                txt_Status.Text = NewStatus;
            });
        }

        private void UpdateButtons()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (RunStatus == 0)
                {
                    btn_Cache.Content = "Cache";
                    btn_Pause.Content = "Pause";
                    btn_Pause.IsEnabled = false;
                }
                else if (RunStatus == 1)
                {
                }
                else if (RunStatus == 2)
                {
                    btn_Cache.Content = "Stop";
                    btn_Pause.Content = IsPaused ? "Resume" : "Pause";
                    btn_Pause.IsEnabled = true;
                }
            });
        }

        private void Worker(List<FileInfo> _Files)
        {
            foreach (var _file in _Files)
            {
                while (IsPaused || RunStatus != 2)
                {
                    Thread.Sleep(100);
                }

                try
                {
                    var _length = _file.Length * PERCENT;
                    var _file_stream = new FileStream(_file.FullName, FileMode.Open);
                    for (int i = 0; i < _length; i++)
                        _file_stream.ReadByte();
                    _file_stream.Close();
                }
                catch (Exception ex) { }

                CURRENT_FILE++;
                UpdateStatus((CURRENT_FILE * 100 / TOTAL_FILES) + "%: " + CURRENT_FILE.ToString() + "/" + TOTAL_FILES.ToString());
            }
            FinishedWorker++;
            if (FinishedWorker == WORKER)
            {
                RunStatus = 0;
                UpdateButtons();
            }
        }

        private async Task TryToCache()
        {
            // ###############################################
            // ###############################################
            // ####     Parse Parameter
            // ####

            DIRECTORY = txt_Path.Text;

            CURRENT_FILE = 0;

            float.TryParse(txt_Percent.Text, out PERCENT);
            PERCENT = Math.Max(Math.Min(PERCENT, 100), 0);
            PERCENT /= 100;

            Int32 _worker;
            Int32.TryParse(txt_Worker.Text, out _worker);
            WORKER = Math.Max(WORKER, 1);
            WORKER = (UInt32)_worker;
            FinishedWorker = 0;

            Files = new List<FileInfo>();
            Threads = new List<Thread>();

            // ###############################################
            // ###############################################
            // ####     Check directory
            // ####

            if (!Directory.Exists(DIRECTORY))
            {
                UpdateStatus("Error: Directory path is not exist.");
                return;
            }
            Console.WriteLine("Listing");
            Console.WriteLine(PERCENT);
            Console.WriteLine(WORKER);

            await Caching();
        }

        private async Task Caching()
        {
            RunStatus = 1;

            // ###############################################
            // ###############################################
            // ####     List files
            // ####

            UpdateStatus("Listing");

            await Task.Run(() =>
             {
                 var _directory_info = new DirectoryInfo(DIRECTORY);
                 Files.AddRange(_directory_info.GetFiles("*", SearchOption.AllDirectories));
             });

            TOTAL_FILES = (UInt32)Files.Count;

            // Create worker
            await Task.Run(() =>
            {
                for (int i = 0; i < WORKER; i++)
                {
                    int _length = (int)(TOTAL_FILES / WORKER);
                    int _start = i * _length;

                    // Set end to the length if the loop is index is the last index
                    if (i == WORKER - 1)
                        _length = (int)TOTAL_FILES - _start;

                    Thread _thread = new Thread(() => Worker(Files.GetRange(_start, _length)));
                    _thread.Priority = ThreadPriority.Lowest;
                    Threads.Add(_thread);
                }
            });

            // Start worker
            await Task.Run(() =>
            {
                foreach (var _thread in Threads)
                    _thread.Start();
            });

            RunStatus = 2;
            UpdateButtons();
        }

        private async Task PauseCaching()
        {
            IsPaused = true;
            UpdateButtons();
        }

        private async Task ResumeCaching()
        {
            IsPaused = false;
            UpdateButtons();
        }

        private async void StopCaching()
        {
            // Stop worker
            await Task.Run(() =>
            {
                foreach (var _thread in Threads)
                    _thread.Abort();
            });

            RunStatus = 0;
            IsPaused = false;
            UpdateButtons();
        }

        private void btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            Outer.lsb_Folder_List.Items.Remove(this);
            Outer.SaveConfig();
        }

        private void btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (IsPaused)
                ResumeCaching();
            else
                PauseCaching();
        }

        private async void btn_Cache_Click(object sender, RoutedEventArgs e)
        {
            SaveFolderEntry();
            await Outer.SaveConfig();

            switch (RunStatus)
            {
                case 0:
                    TryToCache();
                    break;

                case 1:
                    break;

                case 2:
                    StopCaching();
                    break;
            }
        }
    }
}