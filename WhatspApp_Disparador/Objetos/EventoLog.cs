using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatspApp_Disparador
{
    public class EventoLog
    {
        public Guid guid { get; set; }
        public DateTime data { get; set; }        
        public string mensagem { get; set; }

        public EventoLog() 
        {
            data = DateTime.Now;
            guid = new Guid();
        }
    }
}
