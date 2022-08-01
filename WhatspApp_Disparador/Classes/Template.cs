using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatspApp_Disparador
{
    public class Template
    {
        private MessageSend MessageSend { get; set; }
        public List<Parametros> Parametros
        {
            get
            {
                return new Parametros(Id).GetParametros();
            }
            //set { parametros = value; }
        }
        public int Id { get; set; }
        public int Gerador_id { get; set; }
        public string Tipo { get; set; }
        public string Nome { get; set; }
        public int ParameterQuant { get; set; }
        public string Conteudo { get; set; }
        public string Criterios { get; set; }
        public Template(MessageSend messageSend)
        {
            MessageSend = messageSend;
        }

        public List<string> TratarConteudo(int registroCliente, int ocorrencia)
        {
            try
            {
                string _body = Conteudo;
                string _file = "";

                if (string.IsNullOrEmpty(_body)) throw new Exception(@"{""Erro"":""Cliente:{" + registroCliente + " conteudo null}");

                for (int i = 1; i <= ParameterQuant; i++)
                {
                    string _parametroValor =
                Parametros.Where(x => x.posicao == i).First().text;

                    if (_parametroValor.Contains("pa_get_cliente_financeiro"))
                    {
                        string[] _parametroSplit = _parametroValor.Split('.');

                        Extras _extras = SQL.ProcedureFiananceiroExtra(registroCliente);
                        if (_extras == null)
                        {
                            _body = null;
                            throw new Exception(@"{""Erro"":""Cliente:{" + registroCliente + " not Financeiro}");
                        }
                        switch (_parametroSplit[1])
                        {
                            case "pix":
                                _body = _body.Replace("{{" + i + "}}", _extras.pix);
                                break;
                            case "valor":
                                _body = _body.Replace("{{" + i + "}}", _extras.valor);
                                break;
                            default:
                                break;
                        }
                    }
                    else if (_parametroValor.Contains("pa_get_cliente"))
                    {
                        _body = _body.Replace("{{" + i + "}}", SQL.ExeQuerysProcedure(_parametroValor, registroCliente));
                    }
                    else if (_parametroValor.Contains("file"))
                    {
                        string[] _parametroSplit = _parametroValor.Split(':');

                        _file = _parametroSplit[1];
                    }
                }
                return Dividir_Conteudo(_body);
            }
            catch (Exception ex)
            {
                Program.Log($"Erro Function: Body \n{ex.Message}");
                return null;
            }
        }
        public List<string> Dividir_Conteudo(string conteudo) 
        {
            List<string> _conteudoRetorno = null;
            //string[] _conteudoRetorno;
            if (conteudo.Contains("\\"))
            {
                string[] _slitConteudo = conteudo.Split('\\');
                for (int i = 0; i < _slitConteudo.Length; i++)
                {
                    _slitConteudo[i] = _slitConteudo[i].Trim();

                    if (!string.IsNullOrEmpty(_slitConteudo[i]) && _slitConteudo[i] != "")
                    {
                        _conteudoRetorno.Add(_slitConteudo[i]);
                    }                    
                }
            }
            else
            {
                _conteudoRetorno.Add(conteudo);
            }


            return _conteudoRetorno;
        }
    }

}
