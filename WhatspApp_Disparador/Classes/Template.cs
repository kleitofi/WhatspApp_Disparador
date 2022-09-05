using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WhatspApp_Disparador
{
    public class Template
    {
        public ICollection<Parametros> Parametros
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
        public string File { get; set; }        
        public List<string> Message { get; set; }
        public string Criterios { get; set; }
        public JsonValue Json { get; set; }
        public Template Get(string nome, int registroCliente, int ocorrencia) 
        {
            Template _template = SQL.GetTemplate(nome);
            {
                _template.Message = _template.ReplaceConteudoSetParameter(registroCliente, ocorrencia);
            };
            
            return _template;
        }
        private List<string> ReplaceConteudoSetParameter(int registroCliente, int ocorrencia) 
        {
            Dictionary<int, string> _returnReplace = GetParameterPairs(registroCliente, ocorrencia);
            
            if (_returnReplace == null) return null;

            foreach (var item in _returnReplace)
            {
                Conteudo = Conteudo.Replace("{{" + item.Key + "}}", item.Value);
            }
            return Dividir_Conteudo(Conteudo);
        }
        private Dictionary<int, string> GetParameterPairs(int registroCliente, int ocorrencia) 
        {
            //var _listKeyValuePair = new List<KeyValuePair<int, string>>();

            Dictionary<int, string> _dictionary = new Dictionary<int, string>();

            List<string> _parametros = new List<string>();

            try
            {
                for (int i = 1; i <= ParameterQuant; i++)
                {
                    string _parametroValor =
                Parametros.Where(x => x.posicao == i).First().text;

                    if (_parametroValor.Contains("pa_get_cliente_financeiro"))
                    {
                        string[] _parametroSplit = _parametroValor.Split('.');

                        Extras _extras = SQL.ProcedureFiananceiroExtra(registroCliente) ??
                            throw new Exception(new JsonObject
                            {
                                ["Date"] = DateTime.Now,
                                ["Erro"] = $"exec pa_get_cliente_financeiro {registroCliente}"
                            }.ToString());

                        switch (_parametroSplit[1])
                        {
                            case "pix":
                                _dictionary.Add(i, _extras.pix);                                
                                break;
                            case "valor":
                                _dictionary.Add(i, _extras.valor);                                
                                break;
                            case "PDV":
                                break;
                            default:
                                _dictionary.Add(i, "null");
                                break;
                        }
                    }
                    else if (_parametroValor.Contains("pa_get_cliente"))
                    {
                        _dictionary.Add(i, SQL.ExeQuerysProcedure(_parametroValor, registroCliente));                        
                    }
                    else if (_parametroValor.Contains("pa_get_oc"))
                    {
                        _dictionary.Add(i, SQL.ExeQuerysProcedure(_parametroValor, ocorrencia));                        
                    }
                    else if (_parametroValor.Contains("file"))
                    {
                        string[] _parametroSplit = _parametroValor.Split(':');
                        File = _parametroSplit[1];
                    }
                }
/*                foreach (var item in _listKeyValuePair)
                {
                    Console.WriteLine($"{item.Key} : {item.Value}");
                }*/
                return _dictionary;
            }
            catch (Exception ex)
            {
                Program.Log(ex.ToString());
                return null;
            }
        }
        private List<string> Dividir_Conteudo(string conteudo) 
        {
            List<string> _conteudoRetorno = new List<string>();
            //string[] _conteudoRetorno;
            if (conteudo.Contains("\\"))
            {
                string[] _slitConteudo = conteudo.Split('\\');
                for (int i = 0; i < _slitConteudo.Length; i++)
                {
                    _slitConteudo[i] = _slitConteudo[i].Trim();

                    if (!string.IsNullOrEmpty(_slitConteudo[i]) && _slitConteudo[i] != "")
                    {
                        //Console.WriteLine(_slitConteudo[i]);
                        _conteudoRetorno.Add(_slitConteudo[i].Trim());
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
