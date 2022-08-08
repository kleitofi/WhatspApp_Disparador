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
        public static async void TesteTemp1()
        {
            string _json = @"{""number"":""558387183158"",""message"":""teste"",""sender"":""83987183158""}";
            try
            {
                var responseString = await $@"http://ragnar:{"8100"}/send-message"
                    .ConfigureRequest(settings => settings.Timeout = TimeSpan.FromSeconds(4))
                    .PostUrlEncodedAsync(_json.ToString());                   

                await Task.CompletedTask;



                //return responseString;
            }
            catch (Exception ex)
            {
                Program.Log($"WhatDM_Post:{ex.Message}\n{_json}");
                //return "Erro !";
            }

        }
        public static async void TesteTemp2()
        {
            //string _json = @"{""number"":""558387183158"",""message"":""teste"",""sender"":""83987183158""}";
            try
            {
                var responseString = await $@"http://ragnar:{"8100"}/send-message"
                    .ConfigureRequest(settings => settings.Timeout = TimeSpan.FromSeconds(4))
                     .PostUrlEncodedAsync(new
                     {
                         number = "558387183158",
                         message = "TesteTemp2",
                         sender = "83987183158"
                     })
                     .ReceiveString();

                await Task.CompletedTask;

                //return responseString;
            }
            catch (Exception ex)
            {
                Program.Log($"TesteTemp2:{ex.Message}");
                //return "Erro !";
            }

        }

        public static async Task<string> Send(MessageSend messageSend)
        {
            string _jsonRetorno = null;
            bool _status;
            try
            {
                _jsonRetorno = await $@"http://ragnar:{messageSend.Sender.Porta}/send-message"
                     .ConfigureRequest(settings => settings.Timeout = TimeSpan.FromSeconds(4))
                     .PostJsonAsync(messageSend.Json)
                     .ReceiveString();

                JsonNode _jsonNode = JsonNode.Parse(_jsonRetorno);
                _status = (bool)_jsonNode["status"];
            }
            catch
            {
                _status = false;
            }

            return _jsonRetorno;
        }
        public static async void SenderTestAll()
        {
            List<Sessoes> sessoes = Sessoes.GetList();
            string _jsonRetorno;
            bool _status = false;

            foreach (var item in sessoes)
            {
                Api_DM api_DM = new Api_DM
                {
                    number = "558387183158",
                    message = new List<string> { "" },
                    file = "\\\\ragnar\\WhatsDM_FTP\\Imagens\\Softcom.png",
                    sender = item.NumSessao
                };

                string jsonSendString = JsonSerializer.Serialize(api_DM);
                try
                {
                    _jsonRetorno = await $@"http://ragnar:{item.Porta}/send-message"
     .ConfigureRequest(settings => settings.Timeout = TimeSpan.FromSeconds(4))
     .PostJsonAsync(jsonSendString)
     .ReceiveString();

                    JsonNode _jsonNode = JsonNode.Parse(_jsonRetorno);
                    _status = (bool)_jsonNode["status"];
                    item.Ativo = _status;
                    item.UpdateStatus();
                }
                catch
                {
                    _status = false;
                }
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
        private static async Task<string> WhatDM_Post(MessageSend message)
        {
            try
            {

                var x1 = message.Json;
                var x2 = message.Json.Replace(@"\", @"\\");
                message.Json = message.Json.Replace(@"\", @"\\");

                var responseString = await $@"http://ragnar:{message.Sender.Porta}/send-message"
                     .ConfigureRequest(settings => settings.Timeout = TimeSpan.FromSeconds(4))
                     .PostStringAsync(message.Json)
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
        public static void SenderTest(string numSessao = null)
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
