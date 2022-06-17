using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatspApp_Disparador
{
    public class Sessoes
    {
        public int Id { get; set; }
        public int IdSuporte { get; set; }
        public string Porta { get; set; }
        public string NumSessao { get; set; }
        public bool Ativo { get; set; }
        public Sessoes()
        {
            //API.TesteSend();
        }
        public static List<Sessoes> GetList()
        {
            string _script =
                $@"SELECT Id, IdSuporte, Porta, WhatsSuporte, Ativo
FROM TB_Sessoes;";

            return SQL.GetSessoes(_script);
        }
        public void SetStatus()
        {
            string _script =
                $@"UPDATE SET
Ativo = {Ativo}
WHERE Id = {Id}";
            SQL.ExeQueryAccess(_script);
        }
    }

}
