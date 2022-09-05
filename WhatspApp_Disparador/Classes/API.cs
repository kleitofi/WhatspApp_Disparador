using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Web;
using System.Dynamic;
using Newtonsoft.Json.Converters;

namespace WhatspApp_Disparador
{
    public static class API
    {
        /// <summary>
        /// Função principal onde passa a classe messageSend e converte em messageSendWhats para setar no Disparo do WhatsDM
        /// </summary>
        /// <param name="messageSend"></param>
        public static async void Post2(MessageSend messageSend)
        {
            //################################|ENVIO|################################
            string retornoWhatsDM_Post = WhatDM_Post(messageSend).Result;

            messageSend.Send = true;
            messageSend.Return = retornoWhatsDM_Post;
            messageSend.UpdateDb();

            await Task.CompletedTask;
        }
        public static async void Post(MessageSend messageSend)
        {
            //################################|ENVIO|################################
            string retornoWhatsDM_Post = WhatDM_Post(jsonString: messageSend.Json).Result;

            messageSend.Send = true;
            messageSend.Return = retornoWhatsDM_Post;
            messageSend.UpdateDb();

            await Task.CompletedTask;
        }
        public static async Task<string> WhatDM_Post(MessageSend message)
        {
            try
            {
                var responseString = await $@"http://ragnar:{message.Sender.Porta}/send-message"
                    .ConfigureRequest(settings => settings.Timeout = TimeSpan.FromSeconds(4))
                     .PostUrlEncodedAsync(new
                     {
                         number = message.NumTelefone,
                         //message = ,
                         sender = message.Sender.NumSessao
                     })
                     .ReceiveString();

                await Task.CompletedTask;

                return responseString;
            }
            catch (Exception ex)
            {
                Program.Log($"MessageSendWhatsDM:{ex.Message}\n{message}");
                return "Erro !";
            }
        }
<<<<<<< HEAD
        public static async Task<string> WhatDM_Post(string jsonString)
        {
            try
            {
                dynamic _data = JsonConvert.DeserializeObject<ExpandoObject>
                    (jsonString.Replace(@"\",@"\\"), new ExpandoObjectConverter());

                string _rota = "";

                if (string.IsNullOrEmpty(_data.file))
                {
                    _rota = "send-message";
                }
                else
                {
                    _rota = "send-media";                    
                }

                string _json = jsonString.Replace(@"\", @"\\");

                //Console.WriteLine(_json);

                var responseString = await $@"http://ragnar:{_data.porta}/{_rota}"
                    .ConfigureRequest(settings => settings.Timeout = TimeSpan.FromSeconds(4))
                    .PostUrlEncodedAsync(new 
                    {
                         number = _data.number,
                         message = _data.message,
                         sender = _data.sender,
                         file = _data.file,
                    })
                     .ReceiveString();
                await Task.CompletedTask;

                return responseString;
            }
            catch (Exception ex)
            {
                Program.Log($"MessageSendWhatsDM:{ex.Message}\n{jsonString}");
                return "Erro !";
            }
        }
=======
>>>>>>> 9f332b6d7660b074efe680bf3da3a39c2e3ae30e
        public static void TesteSend(string numSessao = null)
        {
            List<Sessoes> _ListSessoes = Sessoes.GetList();

            if (numSessao != null)
            {
                _ListSessoes = _ListSessoes.Where(x => x.NumSessao == numSessao).ToList();
            }

            if (_ListSessoes != null)
            {
                foreach (var item in _ListSessoes)
                {
                    string _retorno = WhatDM_Post(new MessageSend
                    {
                        NumTelefone = "558387183158",
                        Sender = item
                    }).Result;
                    bool _status;
                    try
                    {
                        JsonNode _jsonNode = JsonNode.Parse(_retorno);
                        _status = (bool)_jsonNode["status"];
                    }
                    catch
                    {
                        _status = false;
                    }
                    item.Ativo = _status;
                    item.UpdateStatus();

                    Console.WriteLine($"{item.NumSessao} {_status}");
                }
                Console.Clear();
            }
        }
    }
}
