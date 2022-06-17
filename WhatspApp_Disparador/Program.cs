using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WhatspApp_Disparador
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //API.TesteSend();
            while (true)
            {                               
                List<MessageSend> _messageSends = SQL.GetListEnvio_DbSoft();                
                if (_messageSends != null && _messageSends.Count > 0)
                {
                    Console.Write($"Novo lote({_messageSends.Count}):{DateTime.Now}...");
                    foreach (var item in _messageSends)
                    {
                        string[] msg = SQL.BodyMessage(item);
                        if (msg != null)
                        {
                            for (int i = 0; i < msg.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(msg[i]))
                                {
                                    item.Message = msg[i].Trim();
                                    item.InsertDb();
                                    item.UpdateDbSoftcom();
                                }
                            }
                        }
                        else
                        {
                            item.Return = "Erro BodyMessage";
                            item.UpdateDbSoftcom();
                        }
                    }
                    Console.WriteLine("Insert OK!");
                }
                WhatsDM();
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
            Application.Run();
        }
        public static void WhatsDM()
        {
            List<MessageSend> _Sends = SQL.GetListEnvio_DbWhatsDM();

            if (_Sends != null)
            {
                int _cont = _Sends.Count;
                //API.TesteSend();
                Console.WriteLine($"{_cont,5} {"ID",6} {"Suporte",7} {"Cliente",7} {"WhatsDM",12} {"Template",16} {"Status",5}");
                foreach (var item in _Sends)
                {                    
                    MessageSend(item);
                    Console.WriteLine($"{_cont--,5} {item.Id,6} {item.IdSuporte,7} {item.IdCliente,7} {item.NumTelefone,12} {item.Template,16} {item.Return.Length,5}");
                }
            }
        }
        /// <summary>
        /// Função principal onde passa a classe messageSend e converte em messageSendWhats para setar no Disparo do WhatsDM
        /// </summary>
        /// <param name="messageSend"></param>
        public static void MessageSend(MessageSend messageSend)
        {    
            //################################|ENVIO|################################
            string retornoWhatsDM_Post = MessageSendWhatsDM(messageSend).Result;

            messageSend.Send = true;
            messageSend.Return = retornoWhatsDM_Post;
            messageSend.UpdateDb();
        }
        public static async Task<string> MessageSendWhatsDM(MessageSend message)
        {
            try
            {
                //API.TesteSend();
                var responseString = await $@"http://ragnar:{message.Sender.Porta}/send-message"
                     .ConfigureRequest(settings => settings.Timeout = TimeSpan.FromSeconds(4))
                     .PostUrlEncodedAsync(new
                     {
                         number = message.NumTelefone,
                         message = message.Message,
                         sender = message.Sender.NumSessao
                     })
                     .ReceiveString();

                await Task.CompletedTask;

                return responseString;
            }
            catch (Exception ex)
            {
                Log($"MessageSendWhatsDM:{ex.Message}\n{message}");
                return "Erro !";
            }

        }
        public static bool Log(string strMensagem, string strNomeArquivo = "Log")
        {
            try
            {
                string caminhoArquivo = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), strNomeArquivo);
                if (!File.Exists(caminhoArquivo))
                {
                    FileStream arquivo = File.Create(caminhoArquivo);
                    arquivo.Close();
                }
                using (StreamWriter w = File.AppendText(caminhoArquivo))
                {
                    AppendLog(strMensagem, w);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static void AppendLog(string logMensagem, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entrada : ");
                txtWriter.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                txtWriter.WriteLine($"{logMensagem}");
                txtWriter.WriteLine("------------------------------------");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
