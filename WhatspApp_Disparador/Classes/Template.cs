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
        public Parametros[] Parametros { get; set; }
        public string Conteudo { get; set; }
        public string Criterios { get; set; }
        public Template GetTemplate(string templateNome) 
        {
            return SQL.GetTemplate(templateNome);
        }        
        public string JsonBody() 
        {
            List<Parametros> _listParametros = SQL.GetParametros(_id);
            for (int i = 1; i <= _parameterquant; i++)
            {
                string _parametroValor =
                _listParametros.Where(x => x.posicao == i).First().text;

                if (_parametroValor.Contains("pa_get_cliente_financeiro"))
                {
                    string[] _parametroSplit = _parametroValor.Split('.');

                    Extras _extras = ProcedureFiananceiroExtra(message.IdCliente);
                    if (_extras == null)
                    {
                        return null;
                    }
                    switch (_parametroSplit[1])
                    {
                        case "pix":
                            _conteudo = _conteudo.Replace("{{" + i + "}}", _extras.pix);
                            break;
                        case "valor":
                            _conteudo = _conteudo.Replace("{{" + i + "}}", _extras.valor);
                            break;
                        default:
                            break;
                    }
                }
                else if (_parametroValor.Contains("pa_get_cliente"))
                {

                    _conteudo = _conteudo.Replace("{{" + i + "}}", ExeQuerysProcedure(_parametroValor, message.IdCliente));
                }
                else if (_parametroValor.Contains("file"))
                {
                    string[] _parametroSplit = _parametroValor.Split(':');

                    //_conteudo = _conteudo.Replace("{{" + i + "}}", ExeQuerysProcedure(_parametroValor, message.IdCliente));
                }
            }
            string[] _conteudoRetorno;
            if (_conteudo.Contains("\\"))
            {
                _conteudoRetorno = _conteudo.Split('\\');
                for (int i = 0; i < _conteudoRetorno.Length; i++)
                {
                    //Console.WriteLine(_conteudoRetorno[i]);
                }
            }
            else
            {
                _conteudoRetorno = new string[] { _conteudo };
            }
            return _conteudoRetorno;
            return "";
        }
    }
    
}
