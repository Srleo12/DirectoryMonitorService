using System;
using System.IO;
using System.Net;
using System.Net.Mail;
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
            string message = $"Arquivo: {e.FullPath} {e.ChangeType}";
            WriteToLog(message);
            SendEmailNotification("Mudança no Diretório", message);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            string message = $"Arquivo renomeado: {e.OldFullPath} para {e.FullPath}";
            WriteToLog(message);
            SendEmailNotification("Arquivo Renomeado", message);
        }

        private void SendEmailNotification(string subject, string body)
        {
            string toEmail = "seu-email-destino@example.com";  // Destinatário
            string fromEmail = "seu-email-origem@example.com"; // Remetente
            string smtpServer = "smtp.example.com"; // Servidor SMTP
            string smtpUsername = "seu-usuario-smtp"; // Usuário SMTP
            string smtpPassword = "sua-senha-smtp"; // Senha SMTP

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;

                    using (SmtpClient smtp = new SmtpClient(smtpServer, 587))
                    {
                        smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                WriteToLog("Notificação por e-mail enviada com sucesso.");
            }
            catch (Exception ex)
            {
                WriteToLog($"Erro ao enviar notificação por e-mail: {ex.Message}");
            }
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
