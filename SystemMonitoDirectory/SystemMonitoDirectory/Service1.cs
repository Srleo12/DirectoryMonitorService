using System;
using System.IO;
using System.ServiceProcess;

namespace SystemMonitoDirectory
{
    public partial class SystemMonit : ServiceBase
    {
        private FileSystemWatcher watcher;

        public SystemMonit()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToLog("Serviço de Monitoramento de Diretório Iniciado.");

            string pathToWatch = @"C:\temp\NAS";

            if (!Directory.Exists(pathToWatch))
            {
                WriteToLog($"Diretório '{pathToWatch}' não encontrado.");
                return;
            }

            watcher = new FileSystemWatcher();
            watcher.Path = pathToWatch;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.Size;
            watcher.Filter = "*.*";

            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Changed += OnChanged;
            watcher.Renamed += OnRenamed;

            watcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            WriteToLog("Serviço Finalizado!");
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            WriteToLog($"Arquivo: {e.FullPath} {e.ChangeType}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            WriteToLog($"Arquivo renomeado: {e.OldFullPath} para {e.FullPath}");
        }

        public void WriteToLog(string message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}
