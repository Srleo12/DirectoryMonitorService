using System;
using System.IO;
using System.ServiceProcess;
using System.Diagnostics;

namespace SystemMonitoDirectory
{
    public partial class SystemMonit : ServiceBase
    {
        private FileSystemWatcher watcher;

        // Construtor do serviço, onde as componentes são inicializadas
        public SystemMonit()
        {
            InitializeComponent();
        }

        // Método executado quando o serviço é iniciado
        protected override void OnStart(string[] args)
        {
            WriteToLog("Serviço de Monitoramento de Diretório Iniciado.");

            // Inicia o script Python que mostra a notificação
            StarPythonNotificacao();

            // Define o caminho do diretório a ser monitorado
            string pathToWatch = @"C:\\caminho\\para\\seu\\diretorio";

            // Verifica se o diretório existe; se não, encerra o serviço
            if (!Directory.Exists(pathToWatch))
            {
                WriteToLog($"Diretório '{pathToWatch}' não encontrado.");
                return;
            }

            // Cria um novo FileSystemWatcher para monitorar o diretório
            watcher = new FileSystemWatcher();
            watcher.Path = pathToWatch;

            // Define os tipos de alterações no arquivo a serem monitoradas
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.CreationTime | NotifyFilters.Size;

            // Monitora todos os arquivos do diretório
            watcher.Filter = "*.*";

            // Adiciona eventos que serão disparados quando arquivos forem criados, deletados, alterados ou renomeados
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Changed += OnChanged;
            watcher.Renamed += OnRenamed;

            // Habilita o monitoramento do diretório
            watcher.EnableRaisingEvents = true;
        }

        // Método executado quando o serviço é parado
        protected override void OnStop()
        {
            // Desabilita o monitoramento do diretório e libera os recursos
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            WriteToLog("Serviço Finalizado!");
        }

        // Evento disparado quando um arquivo é criado, alterado ou deletado
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // Registra a mudança no log
            string message = $"Arquivo: {e.FullPath} {e.ChangeType}";
            WriteToLog(message);
        }

        // Evento disparado quando um arquivo é renomeado
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            // Registra a renomeação no log
            string message = $"Arquivo renomeado: {e.OldFullPath} para {e.FullPath}";
            WriteToLog(message);
        }

        // Método para iniciar o script Python que mostra a notificação
        private void StarPythonNotificacao()
        {
            try
            {
                // Define o caminho do script Python
                string pythonScript = @"C:\\caminho\\para\\seu\\script\\notificação.py";

                // Define o caminho completo do executável do Python
                string pythonExePath = @"C:\Users\SeuUsuario\AppData\Local\Programs\Python\Python311\python.exe"; // Atualize para o caminho correto

                // Configura as informações do processo para executar o script Python
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = pythonExePath; // Define o executável do Python
                psi.Arguments = $"\"{pythonScript}\""; // Define o script a ser executado
                psi.UseShellExecute = false; // Não usa o shell do Windows para executar o processo
                psi.RedirectStandardOutput = true; // Redireciona a saída padrão
                psi.RedirectStandardError = true; // Redireciona o erro padrão
                psi.CreateNoWindow = true; // Não cria uma janela para o processo

                // Inicia o processo para rodar o script Python
                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit(); // Aguarda a finalização do processo
                    string output = process.StandardOutput.ReadToEnd(); // Captura a saída do processo
                    string errors = process.StandardError.ReadToEnd(); // Captura os erros do processo

                    // Registra a saída do Python no log, se houver
                    if (!string.IsNullOrEmpty(output))
                    {
                        WriteToLog($"Output do Python: {output}");
                    }

                    // Registra os erros do Python no log, se houver
                    if (!string.IsNullOrEmpty(errors))
                    {
                        WriteToLog($"Erros do Python: {errors}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Registra qualquer exceção que ocorra ao tentar executar o script Python
                WriteToLog($"Erro ao iniciar o script Python: {ex.Message}");
            }
        }

        // Método para registrar mensagens em um arquivo de log
        public void WriteToLog(string message)
        {
            // Define o caminho onde os logs serão armazenados
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); // Cria o diretório de logs, se não existir
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            
            // Escreve a mensagem de log no arquivo
            using (StreamWriter sw = new StreamWriter(filepath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}
