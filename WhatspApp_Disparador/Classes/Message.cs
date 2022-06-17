using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatspApp_Disparador
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DeviceType { get; set; }
        public string IdRemote { get; set; }        
        public string IdMessage { get; set; }
        public string Body { get; set; }
    }    
}
