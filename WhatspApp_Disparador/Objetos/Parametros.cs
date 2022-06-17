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
        public Parametros GetParametros(int Id)
        {
            string _script = 
    $@"SELECT [idtemplete]
      ,[text]
      ,[posicao]
      ,[seq]
  FROM [blip_parametros_gerar]
  WHERE [idtemplete] = {Id}";            
            return null;
        }

    }
}
