using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WhatspApp_Disparador
{
    public class MessageSend
    {
        private Sessoes sender;
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int IdSuporte { get; set; }
        public int IdCliente { get; set; }
        public string Template { get; set; }
        public string NumTelefone { get; set; }
        public string Message { get; set; }
        public string Return { get; set; }       
        public Sessoes Sender
        {
            get {
                sender = Sessoes.GetList().FirstOrDefault(x => x.IdSuporte == IdSuporte) ?? 
                    Sessoes.GetList().FirstOrDefault(x => x.IdSuporte == 0);
                return sender; 
            }
            set { sender = value; }
        }

        public bool Send { get; set; }
        public DateTime DateTime { get; set; }

        public MessageSend() 
        {
            
        }
        
        public async void UpdateDbSoftcom()
        {
            string _string = $@"
update vw_whatsDM_envios
set Blip_MsgEnviada = -1, Blip_MsgEnviadaRetorno = '{Return}'
where id = {Id}";

            await SQL.ExeQuerySQL_DbBlip(_string);
        }
        public void InsertDb()
        {
            string _string = $@"
INSERT INTO `dbwhatsdm`.`messagesend` (
    `Id`, `Guid`, `IdSuporte`, `IdCliente`, `Template`, `NumTelefone`, `Message`, `Return`, `Send`, `DateTime`) 
VALUES (
NULL,
'{Guid}' ,
'{IdSuporte}' ,
'{IdCliente}' ,
'{Template}' ,
'{NumTelefone}' ,
'{Message}' ,
'{Return}' ,
'{(Send ? '1' : '0')}' ,
NOW()
);";

            SQL.ExeQueryMySQL(_string);
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
    }
}
