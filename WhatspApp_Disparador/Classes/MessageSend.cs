using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace WhatspApp_Disparador
{
    public class MessageSend
    {
        private Sessoes sender;
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int IdSuporte { get; set; }
        public int IdCliente { get; set; }
        public Template Template { get; set; }
        public string NumTelefone { get; set; }
        public string[] Message { get; set; }        
        public string Return { get; set; }
        public Sessoes Sender
        {
            get
            {
                if (sender == null)
                {
                    sender = GetSessoes(JsonNode.Parse(Template.Criterios)["Sender"].AsArray());
                }
                return sender;
            }
            set { sender = value; }
        }
        public bool Send { get; set; }
        public DateTime DateTime { get; set; }
        public async void UpdateDbSoftcom()
        {
            string _string = $@"
update vw_whatsDM_envios
set Blip_MsgEnviada = -1, Blip_MsgEnviadaRetorno = '{Return}'
where id = {Id}";

            await SQL.ExeQuerySQL_DbBlip(_string);
        }
        public async void InsertDb()
        {
            string _string = $@"
INSERT INTO `dbwhatsdm`.`messagesend` (
    `Id`, `Guid`, `IdSuporte`, `IdCliente`, `Template`, `NumTelefone`, `Message`, `Return`, `Send`, `DateTime`) 
VALUES (
NULL,
'{Guid}' ,
'{IdSuporte}' ,
'{IdCliente}' ,
'{Template.Nome}' ,
'{NumTelefone}' ,
'{Message}' ,
'{Return}' ,
'{(Send ? '1' : '0')}' ,
NOW()
);";

            await SQL.ExeQueryMySQL(_string);
        }
        public async void InsertLoteDb()
        {
            string _string = $@"
INSERT INTO `dbwhatsdm`.`messagesend` (
    `Id`, `Guid`, `IdSuporte`, `IdCliente`, `Template`, `NumTelefone`, `Message`, `Return`, `Send`, `DateTime`) 
VALUES (
NULL,
'{Guid}' ,
'{IdSuporte}' ,
'{IdCliente}' ,
'{Template.Nome}' ,
'{NumTelefone}' ,
'{Message}' ,
'{Return}' ,
'{(Send ? '1' : '0')}' ,
NOW()
);";

            await SQL.ExeQueryMySQL(_string);
        }
        public static async void InsertLoteDb(List<MessageSend> list_MessageSend)
        {
            string _script = "";
            string _head = $@"INSERT INTO `dbwhatsdm`.`messagesend` (
    `Id`, `Guid`, `IdSuporte`, `IdCliente`, `Template`, `NumTelefone`, `Message`, `Return`, `Send`, `DateTime`) 
VALUES
"; ;
            string _rows = "";            
            if (list_MessageSend != null)
            {
                //tamanho da lista de dados
                int _size = list_MessageSend.Count;
                //comprimento maximo de linha para insert
                int _length = 1000;

                foreach (MessageSend item in list_MessageSend)
                {
                    _length--; _size--;

                    _rows +=
$@",(
NULL,
'{item.Guid}' ,
'{item.IdSuporte}' ,
'{item.IdCliente}' ,
'{item.Template.Nome}' ,
'{item.NumTelefone}' ,
'{item.Message[0]}' ,
'{item.Return}' ,
'{(item.Send ? '1' : '0')}' ,
NOW()
)";
                    if (_length == 0 || _size == 0)
                    {
                        _length = 1000;

                        _script = _script.Trim();
                        _rows = _rows.Length > 0 ? _rows.Remove(0, 1) : "";
                        _rows = _rows.Trim();
                        _rows += ";";
                        _script += _head + _rows;

                        _rows = "";
                    }
                }
                await SQL.ExeQueryMySQL(_script);
                //Console.WriteLine(_script);
            }
        }
        public async void UpdateDb()
        {
            string _string = $@"
UPDATE messagesend 
SET 
`Send` = {(Send ? '1' : '0')},
`Return` = '{Return}'
WHERE Id = '{Id}'
";
            await SQL.ExeQueryMySQL(_string);
        }
        private Sessoes GetSessoes(JsonArray sender)
        {
            Sessoes _sessoes = null;
            foreach (string item in sender)
            {
                if (!string.IsNullOrEmpty(item) && item == "own")
                {
                    _sessoes = Sessoes.GetList().FirstOrDefault(x => x.IdSuporte == IdSuporte);
                }
                else
                {
                    _sessoes = Sessoes.GetList().FirstOrDefault(x => x.Nome == item);
                }

                if( _sessoes != null ) return _sessoes;
            }
            return _sessoes;
        }
        public static List<MessageSend> SelectDb_Soft()
        {
            return SQL.GetListEnvio_DbSoft();
        }
        public static List<MessageSend> SelectDb_DM()
        {
            return SQL.GetListEnvio_DbWhatsDM();
        }

    }
}
