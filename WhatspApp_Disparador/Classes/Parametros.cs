using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatspApp_Disparador
{
    public class Parametros
    {
        public int seq { get; set; }
        public int id_template { get; set; }
        public string text { get; set; }
        public int posicao { get; set; }        
        public Parametros(int IdTemplate) 
        {
            id_template = IdTemplate;
        }
        public List<Parametros> GetParametros()
        {
            return SQL.GetParametros(id_template);
        }

    }
}
