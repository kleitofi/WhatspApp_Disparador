using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatspApp_Disparador
{
    public class Template
    {
        public int Id { get; set; }
        public int Gerador_id { get; set; }
        public string Tipo { get; set; }
        public string Nome { get; set; }
        public int ParameterQuant { get; set; }
        public string Conteudo { get; set; }
        public string Criterios { get; set; }
        public Template GetTemplate(string templateNome) 
        {
            return SQL.GetTemplate(templateNome);
        }        
    }
    
}
