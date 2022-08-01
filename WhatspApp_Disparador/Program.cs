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
        public static bool Homologacao = true;
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Api_DM.SendTeste(); 

            //new MessageSend().teste();
            while (true)
            {
                Console.Clear();
                //Select mensagens e tratamenta do copor da mensagem insere o MySQL DB_WhatsDM
                WhatsSoft();
                //Envio das mensagens para API
                WhatsDM();
                Console.WriteLine("Sleep...");
                Thread.Sleep(TimeSpan.FromSeconds(Homologacao ? 5 : 30));
            }
        }
        /// <summary>
        /// Faz a requisação do banco Blip e trata a mensagem de acordo com template
        /// </summary>
        private static void WhatsSoft() 
        {
            
            List<MessageSend> _messageSends = new MessageSend().GetListMessageSends();

            int _countMessage = _messageSends.Count;

            if (_messageSends != null && _countMessage > 0)
            {                
                Console.WriteLine($"Novo lote({_countMessage}):{DateTime.Now}...");
                foreach (var item in _messageSends)
                {
                    if (item.Template != null)
                    {
                        string[] msg = SQL.BodyMessage(item);
                        if (msg != null)
                        {
                            Console.Write($"{_countMessage--} ");
                            for (int i = 0; i < msg.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(msg[i]))
                                {                                    
                                    msg[i] = msg[i].Trim();
                                }
                                //item.InsertDb();
                                //Console.Write("Insert WhatsDM ");                              
                                
                                Console.Write("Update Agenda");
                                Console.WriteLine();
                            }
                            //item.Json = msg;
                        }
                        else
                        {
                            item.Return = "Erro BodyMessage";                            
                        }
                        item.UpdateDbSoftcom();
                    }
                    else
                    {
                        Console.WriteLine("Template NULL");
                    }
                }                
                MessageSend.InsertLoteDb(_messageSends);
                Console.Write("Insert WhatsDM ");

                Console.WriteLine("Insert OK!");
            }
        }
        /// <summary>
        /// Envia as messagens do banco MySQL do WhatsDM para seus Sender
        /// </summary>
        private static void WhatsDM()
        {
            List<MessageSend> _Sends = MessageSend.SelectDb_DM();

            if (_Sends != null)
            {
                API.TesteSend();
                int _cont = _Sends.Count;                
                Console.WriteLine($"{_cont,5} {"ID",6} {"Suporte",7} {"Cliente",7} {"WhatsDM",12} {"Template",-26} {"Status",5}");
                foreach (var item in _Sends)
                {
                    if (item.Sender != null && item.Sender.Ativo)
                    {
                        API.SendDM(item);
                        Console.WriteLine($"{_cont--,5} {item.Id,6} {item.IdSuporte,7} {item.IdCliente,7} {item.NumTelefone,12} {item.Template.Nome,-26} {item.Return.Length,5}");
                    }
                    else 
                    {
                        item.Return = "Sender_OFF";
                        item.UpdateDb();
                        //Console.WriteLine($"{_cont--,5} Sender_OFF={item.Sender.NumSessao}:{item.Sender.Porta}");
                        Console.WriteLine($"{_cont--,5} {item.Id,6} {item.IdSuporte,7} {item.IdCliente,7} {item.NumTelefone,12} {item.Template.Nome,-26} {item.Return,5}");
                    }                    
                }
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
