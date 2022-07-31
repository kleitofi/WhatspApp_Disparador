using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WhatspApp_Disparador
{
    public class Api_DM
    {
        public string number { get; set; }
        public List<string> message { get; set; }
        public string file { get; set; }
        public string sender { get; set; }

        public static async Task<string> SendTeste()
        {
            Api_DM api_DM = new Api_DM
            {
                number = "558387183158",
                message = new List<string> { "c# teste 1", "c# teste 2" },
                file = "\\\\ragnar\\WhatsDM_FTP\\Imagens\\Softcom.png",
                sender = "83987183158"
            };

            string jsonString = JsonSerializer.Serialize(api_DM);
            try
            {
                var responseString = await $@"http://ragnar:{"8999"}/send-message"
                     .ConfigureRequest(settings => settings.Timeout = TimeSpan.FromSeconds(4))
                     .PostJsonAsync(jsonString)
                     .ReceiveString();
                await Task.CompletedTask;

                return responseString;
            }
            catch (Exception ex)
            {
                Program.Log($"SendTeste:{ex.Message}\n{jsonString}");
                return "Erro !";
            }            
        }
        public static async void SendDM(MessageSend messageSend)
        {
            //################################|ENVIO|################################
            string retornoWhatsDM_Post = WhatDM_Post(messageSend).Result;

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
                     .PostJsonAsync(message.Json)
                     .ReceiveString();

                await Task.CompletedTask;

                return responseString;
            }
            catch (Exception ex)
            {
                Program.Log($"WhatDM_Post:{ex.Message}\n{message.Json}");
                return "Erro !";
            }
        }
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
                }
            }
        }
    }
}
