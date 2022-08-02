﻿using Flurl.Http;
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
        /// <summary>
        /// Função principal onde passa a classe messageSend e converte em messageSendWhats para setar no Disparo do WhatsDM
        /// </summary>
        /// <param name="messageSend"></param>
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
