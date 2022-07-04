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
        public string Nome { get; set; }
        public string Setor { get; set; }
        public string Porta { get; set; }
        public string NumSessao { get; set; }
        public bool Ativo { get; set; }
        public static List<Sessoes> GetList()
        {
            return SQL.GetSessoes();
        }
        public async void UpdateStatus()
        {
            string _script =$@"
UPDATE TB_Sessoes SET
Ativo = {Ativo}
WHERE Id = {Id}";
            await SQL.ExeQueryAccess(_script);
        }
    }

}
