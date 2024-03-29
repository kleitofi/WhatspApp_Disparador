﻿using Flurl.Http;
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
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace WhatspApp_Disparador
{
    static class Program
    {
        public static bool Homologacao = false;
        public static JavaScriptSerializer js = new JavaScriptSerializer();
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Api_DM.TesteTemp1();
            //Api_DM.TesteTemp2();
            while (true)
            {
                Console.Clear();
                ///Select mensagens e tratamenta do copor da mensagem insere o MySQL DB_WhatsDM
                WhatsSoft();
                ///Envio das mensagens para API
                WhatsDM();
                Console.WriteLine($"{DateTime.Now} - Sleep...");
                Thread.Sleep(TimeSpan.FromSeconds(Homologacao ? 5 : 10));
            }
        }
        /// <summary>
        /// Faz a requisação do banco Blip e trata a mensagem de acordo com template
        /// </summary>
        private static void WhatsSoft()
        {
            List<MessageSend> _messageSends = MessageSend.GetList_dbAgenda();

            int _countMessage = _messageSends.Count;

            if (_messageSends != null && _countMessage > 0)
            {
                Console.WriteLine($"Novo lote({_countMessage})");

                foreach (MessageSend item in _messageSends)
                {
                    Console.WriteLine($"{_countMessage--} ");

                    if (item.Template != null)
                    {
                        List<dynamic> dyn_message = new List<dynamic>();

                        dyn_message.Add(new { 
                            number = item.NumTelefone, 
                            message = item.Template.Message,
                            file = item.Template.File,
                            sender = item.Sender == null ? "" : item.Sender.NumSessao, 
                            porta = item.Sender == null ? "" : item.Sender.Porta 
                        });

                        string json = JsonConvert.SerializeObject(dyn_message[0]);

                        item.Json = json;
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
            List<MessageSend> _Sends = MessageSend.GetList_dbWhatsDM();

            if (_Sends != null)
            {                
                API.TesteSend();
                int _cont = _Sends.Count;
                Console.WriteLine($"{_cont,5} {"ID",6} {"Suporte",7} {"Cliente",7} {"WhatsDM",12} {"Template",-26} {"Status",5}");
                foreach (var item in _Sends)
                {                    
                    if (item.Template != null && item.Sender != null && item.Sender.Ativo)
                    {
                        //API.SendDM(item);
                         API.Post(item);
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
        private static void WhatsDM2()
        {
            List<MessageSend> _Sends = MessageSend.GetList_dbWhatsDM();

            if (_Sends != null)
            {
                API.TesteSend();
                int _cont = _Sends.Count;
                Console.WriteLine($"{_cont,5} {"ID",6} {"Suporte",7} {"Cliente",7} {"WhatsDM",12} {"Template",-26} {"Status",5}");
                foreach (var item in _Sends)
                {
                    if (item.Sender != null && item.Sender.Ativo)
                    {
                        API.Post(item);
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
