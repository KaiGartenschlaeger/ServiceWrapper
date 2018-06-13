using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;

namespace ServiceWrapper
{
    public class HostEngine
    {
        #region Fields

        private bool _consoleMode;
        private Process _process;
        private BackgroundWorker _worker;

        #endregion

        #region Constructor

        public HostEngine(bool consoleMode)
        {
            _consoleMode = consoleMode;
        }

        #endregion

        #region Properties

        public string StartupFile
        {
            get
            {
                return ConfigurationManager.AppSettings["StartupFile"];
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["WorkingDirectory"];
            }
        }

        public string StartupParameters
        {
            get
            {
                return ConfigurationManager.AppSettings["StartupParameters"];
            }
        }

        public bool HideProcess
        {
            get
            {
                return ConfigurationManager.AppSettings["StartupParameters"].Equals("true");
            }
        }

        public bool AutoRestart
        {
            get
            {
                return ConfigurationManager.AppSettings["AutoRestart"].Equals("true");
            }
        }

        public bool ConsoleMode
        {
            get
            {
                return _consoleMode;
            }
        }

        #endregion

        #region Methods

        public void Start()
        {
            if (!string.IsNullOrWhiteSpace(StartupFile) && _process == null)
            {
                _worker = new BackgroundWorker();
                _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
                _worker.RunWorkerAsync();
            }
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = StartupFile;
            startInfo.Arguments = StartupParameters;
            startInfo.WorkingDirectory = WorkingDirectory;
            startInfo.UseShellExecute = true;

            if (HideProcess)
            {
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            _process = new Process();
            _process.StartInfo = startInfo;

            if (ConsoleMode)
            {
                Console.WriteLine("Start new Instance");
            }
            else
            {
                EventLog.WriteEntry("Service Wrapper", "Start new Instance of " + StartupFile,
                    EventLogEntryType.Information);
            }

            _process.Start();
            _process.WaitForExit();
            _process = null;

            if (AutoRestart && !((BackgroundWorker)sender).CancellationPending)
            {
                Start();
            }
        }

        public void Stop()
        {
            if (_process != null && _worker != null)
            {
                _worker.CancelAsync();
                _process.Kill();
            }
        }

        #endregion
    }
}