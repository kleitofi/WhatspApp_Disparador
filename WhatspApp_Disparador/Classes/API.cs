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
using System.Text.Json;

namespace WhatspApp_Disparador
{
    public static class API
    {
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
                Program.Log($"MessageSendWhatsDM:{ex.Message}\n{message}");
                return "Erro !";
            }
        }
        public static async void TesteSend()
        {
            List<Sessoes> _ListSessoes = Sessoes.GetList();
            if (_ListSessoes != null )
            {
                foreach (var item in Sessoes.GetList())
                {
                    string _retorno = await WhatDM_Post(new MessageSend
                    {
                        NumTelefone = "558387183158",
                        Sender = item
                    });
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
                    Console.WriteLine(item.NumSessao + ":" + _status);
                    SQL.ExeQueryAccess(
                    $@"UPDATE TB_Sessoes SET TB_Sessoes.Ativo = {_status}
                WHERE WhatsSuporte = '{item.NumSessao}';");

                }
            }
        }
    }
}
