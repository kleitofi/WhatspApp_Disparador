using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WhatspApp_Disparador
{
    public static class SQL
    {
        private static SqlConnection connBaseSoftcom = new SqlConnection
    (@"Persist Security Info=False;User ID=sa;
Password=zaq321;Initial Catalog=BaseSoftcom;
Data Source=lordvader.softcomtec.info\SQL2017STD,5433");

        private static SqlConnection connBaseWhatsapp = new SqlConnection
    (@"Persist Security Info=False;User ID=sa;
Password=qaz@123;Initial Catalog=dbBlip;
Data Source=ragnar\SQLEXPRESS,5433");

        private static OleDbConnection connBaseLocal = new OleDbConnection
            ($"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\\\\ragnar\\WhatsDM_Base\\Base.mdb");


        private static MySqlConnection connBaseWhatsAPI = new MySqlConnection(
            "Server=ragnar; port=3306 ;Database=dbWhatsDM; Uid=softcom; Pwd=qaz123;convert zero datetime=True");
        public static Template GetTemplate(string tamplateNome)
        {
            string _script = $@"
SELECT [Id]
      ,[Gerador_id]
      ,[Tipo]
      ,[Nome]
      ,[ParameterQuant]
      ,[Conteudo]
      ,[Criterios]
  FROM [dbo].[vw_whatsDM_templates]
  WHERE [Nome] LIKE '{tamplateNome}'";
            try
            {
                SqlCommand cmd = new SqlCommand(_script, connBaseWhatsapp);
                SqlDataAdapter data = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                data.Fill(tb);
                Template template = null;
                
                foreach (var item in tb.AsEnumerable())
                {
                    template = new Template()
                    {
                        Id = item.Field<int>("Id"),
                        Gerador_id = item.Field<int>("Gerador_id"),
                        Tipo = item.Field<string>("Tipo"),
                        Nome = item.Field<string>("Nome"),
                        ParameterQuant = item.Field<int>("ParameterQuant"),
                        Conteudo = item.Field<string>("Conteudo"),
                        Criterios = item.Field<string>("Criterios")
                    };
                }
                return template;
            }
            catch (Exception ex)
            {
                Program.Log($@"Erro Function: ProcedureFiananceiroExtra
{ex.Message}
_scriptParameter:
{_script}");
                return null;
            }
            finally
            {
                connBaseWhatsapp.Close();
            }
        }
        public static List<MessageSend> GetListEnvio_DbSoft()
        {
            try
            {
                string _script = $@"select * from vw_whatsDM_envios";

                connBaseWhatsapp.Open();

                SqlCommand _cmd = new SqlCommand(_script, connBaseWhatsapp);
                SqlDataAdapter _data = new SqlDataAdapter(_cmd);
                DataTable _tb = new DataTable();
                _data.Fill(_tb);
                List<MessageSend> _list = new List<MessageSend>();

                connBaseWhatsapp.Close();

                foreach (var item in _tb.AsEnumerable())
                {
                    MessageSend _msgTemp = new MessageSend
                    {
                        Id = item.Field<int>("Id"),
                        Guid = Guid.NewGuid(),
                        IdSuporte = item.Field<int>("Id_Suporte"),
                        IdCliente = item.Field<int>("Id_Cliente"),
                        NumTelefone = "55" + item.Field<string>("NumeroWhatsapp"),
                        Template = new Template().GetTemplate(item.Field<string>("TipoTemplete")),
                        Return = "",
                        Message = "",
                        Send = false
                    };

                    _list.Add(_msgTemp);
                }
                return _list;
            }
            catch (Exception ex)
            {
                Program.Log($"GetListEnvio_DbSoft:{ex.Message}");
                //Task.Factory.StartNew(() => { MessageBox.Show($"GetListEnvio_DbSoft|{ex.Message}"); });
                return null;
            }
            finally
            {
                connBaseWhatsapp.Close();
            }
        }
        public static List<MessageSend> GetListEnvio_DbWhatsDM()
        {
            try
            {
                string _scriptSQL = "SELECT * FROM `messagesend` WHERE `Send` = false";
                //Console.WriteLine("ExeQuerysSelectSQL:" + scriptSQL);
                connBaseWhatsAPI.Open();
                MySqlCommand _cmd = new MySqlCommand(_scriptSQL, connBaseWhatsAPI);

                MySqlDataAdapter _sqlDataAdapter = new MySqlDataAdapter(_cmd);
                DataTable _tb = new DataTable();
                _tb.Load(_cmd.ExecuteReader());
                MySqlDataAdapter _data = _sqlDataAdapter;

                List<MessageSend> _list = new List<MessageSend>();

                foreach (var item in _tb.AsEnumerable())
                {
                    _list.Add(new MessageSend
                    {
                        Id = item.Field<int>("Id"),
                        Guid = item.Field<Guid>("Guid"),
                        IdSuporte = item.Field<int>("IdSuporte"),
                        IdCliente = item.Field<int>("IdCliente"),
                        Message = item.Field<string>("message"),
                        NumTelefone = item.Field<string>("numTelefone"),
                        Template = new Template().GetTemplate(item.Field<string>("Template"))
                    });
                }
                return _list.Count > 0 ? _list : null;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => { MessageBox.Show($"GetListEnvio_DbWhatsDM ERRO:{ex.Message}"); });
                return null;
            }
            finally
            {
                connBaseWhatsAPI.Close();
            }
        }
        public static Task ExeQueryMySQL(string scriptSQL)
        {
            try
            {
                connBaseWhatsAPI.Open();
                MySqlCommand _cmd = new MySqlCommand(scriptSQL, connBaseWhatsAPI);
                MySqlDataAdapter _sqlDataAdapter = new MySqlDataAdapter(_cmd);
                _cmd.ExecuteReader();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.Factory.StartNew(() => { Program.Log($"ExeQueryMySQL:{ex.Message}\n{scriptSQL}"); });
            }
            finally
            {
                connBaseWhatsAPI.Close();
            }
        }
        public static string[] BodyMessage(MessageSend message)
        {
            string _script = $@"SELECT * FROM [vw_whatsDM_templates] WHERE [Nome] like '{message.Template.Nome}'";
            try
            {
                connBaseWhatsapp.Open();

                SqlCommand _cmd = new SqlCommand(_script, connBaseWhatsapp);
                SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(_cmd);
                DataTable _tb = new DataTable();

                _sqlDataAdapter.Fill(_tb);

                List<object> listObj = new List<object>();

                foreach (var item in _tb.AsEnumerable())
                {
                    string
                        _conteudo = item.Field<string>("conteudo")
                        ;
                    int
                        _id = item.Field<int>("id"),
                        _parameterquant = item.Field<int>("parameterquant")
                        ;

                    List<Parametros> _listParametros = GetParametros(_id);
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
                }
                return null;
            }
            catch (Exception ex)
            {
                Program.Log($"BodyMessage ERRO:{ex.Message}");
                //Task.Factory.StartNew(() => { MessageBox.Show($"BodyMessage ERRO:{ex.Message}"); });
                return null;
            }
            finally
            {
                connBaseWhatsapp.Close();
            }
        }
        public static string ExeQuerysProcedure(string strSQL, int registroCliente, SqlConnection sqlConnection = null)
        {
            try
            {
                sqlConnection = connBaseSoftcom;
                sqlConnection.Open();
                SqlCommand _cmd = new SqlCommand(strSQL + " " + registroCliente, sqlConnection);
                SqlDataAdapter _data = new SqlDataAdapter(_cmd);
                DataTable _tb = new DataTable();
                _data.Fill(_tb);
                if (_tb.Rows.Count > 0)
                {
                    return _tb.AsEnumerable().FirstOrDefault().ItemArray[0].ToString();
                }
                return "";
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => { MessageBox.Show($"ExeQuerysProcedure | {ex.Message}"); });
                return "Erro!";
            }
            finally
            {
                sqlConnection.Close();
            };
        }
        public static Task ExeQuerySQL(string strSQL, SqlConnection conn)
        {
            try
            {
                conn.Open();
                SqlCommand _cmd = new SqlCommand(strSQL, conn);
                _cmd.ExecuteReader();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.Factory.StartNew(() => { MessageBox.Show($"ExeQuerySQL | {ex.Message}"); });
            }
            finally
            {
                conn.Close();
            };
        }
        public static Task ExeQuerySQL_DbBlip(string strSQL)
        {
            try
            {
                connBaseWhatsapp.Open();
                SqlCommand _cmd = new SqlCommand(strSQL, connBaseWhatsapp);
                _cmd.ExecuteReader();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.Factory.StartNew(() => { MessageBox.Show($"ExeQuerySQL | {ex.Message}"); });
            }
            finally
            {
                connBaseWhatsapp.Close();
            };
        }
        public static void ExeQueryAccess(string[] strSQL, OleDbConnection conn = null)
        {
            foreach (var item in strSQL)
            {
                conn = conn == null ? connBaseLocal : conn;
                // Create a command and set its connection    
                OleDbCommand cmdQry = new OleDbCommand(item, conn);
                // Open the connection and execute the select command.    
                try
                {
                    // Open connecton    
                    conn.Open();
                    // Execute command    
                    cmdQry.ExecuteReader();
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() => { MessageBox.Show($"ExeQueryAccess ERRO:{ex.Message}"); });
                }
                finally
                {
                    conn.Close();
                }
                // The connection is automatically closed becasuse of using block.
            }
        }
        public static Task ExeQueryAccess(string strSQL, OleDbConnection conn = null)
        {
            conn = conn == null ? connBaseLocal : conn;
            // Create a command and set its connection    
            OleDbCommand cmdQry = new OleDbCommand(strSQL, conn);
            // Open the connection and execute the select command.    
            try
            {
                // Open connecton    
                conn.Open();
                // Execute command    
                cmdQry.ExecuteReader();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.Factory.StartNew(() => { MessageBox.Show($"ExeQueryAccess ERRO:{ex.Message}"); });
            }
            finally
            {
                conn.Close();
            }
            // The connection is automatically closed becasuse of using block.
        }
        public static List<string> SelectAccess(string strSQL, string campo, OleDbConnection conn = null)
        {
            List<string> _list = new List<string>();
            conn = conn == null ? connBaseLocal : conn;
            OleDbCommand cmdQry = new OleDbCommand(strSQL, conn);
            try
            {
                conn.OpenAsync();
                OleDbDataReader reader = cmdQry.ExecuteReader();
                while (reader.Read())
                {
                    _list.Add(reader[campo].ToString());
                }
                return _list;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => { MessageBox.Show($"SelectAccess ERRO:{ex.Message}"); });
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
        public static List<Sessoes> GetSessoes()
        {
            try
            {
                string _script = $@"
SELECT Id, IdSuporte, NomeSuporte, Setor, Porta, WhatsSuporte, Ativo
FROM TB_Sessoes WHERE WhatsSuporte is not null;";
                List<Sessoes> _list = new List<Sessoes>();                
                OleDbCommand _cmd = new OleDbCommand(_script, connBaseLocal);
                OleDbDataAdapter _data = new OleDbDataAdapter(_cmd);
                DataTable _tb = new DataTable();
                _data.Fill(_tb);

                foreach (var item in _tb.AsEnumerable())
                {
                    _list.Add(new Sessoes
                    {
                        Id = item.Field<int>("Id"),
                        IdSuporte = item.Field<int>("IdSuporte"),
                        Nome = item.Field<string>("NomeSuporte"),
                        Setor = item.Field<string>("Setor"),
                        NumSessao = item.Field<string>("WhatsSuporte"),
                        Porta = item.Field<string>("Porta"),
                        Ativo = item.Field<bool>("Ativo")
                    });
                }
                return _list;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => { MessageBox.Show($"SelectAccess ERRO:{ex.Message}"); });
                return null;
            }
            finally
            {
                connBaseLocal.Close();
            }
        }
        private static Extras ProcedureFiananceiroExtra(int registroCliente, SqlConnection sqlConnection = null)
        {
            string _scriptParameter = $@"exec pa_softdm_get_financeiro {registroCliente}";
            try
            {
                sqlConnection = connBaseSoftcom;
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(_scriptParameter, sqlConnection);
                SqlDataAdapter data = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                data.Fill(tb);
                Extras extras = null;
                //                sqlConnection.Close();
                foreach (var item in tb.AsEnumerable())
                {
                    extras = new Extras()
                    {
                        valor = item.Field<string>("ParcelaValor").ToString(),
                        pix = item.Field<string>("CopiaColaPIX").ToString(),
                        vencimento = item.Field<string>("ParcelaVencimento").ToString(),
                        pdv = item.Field<string>("PDV").ToString(),
                        bloqueado = item.Field<int>("Bloqueado").ToString()
                    };
                }
                return extras;
            }
            catch (Exception ex)
            {
                Program.Log($@"Erro Function: ProcedureFiananceiroExtra
{ex.Message}
_scriptParameter:
{_scriptParameter}");
                return null;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        private static List<Parametros> GetParametros(int Id)
        {
            List<Parametros> _list_parametros = new List<Parametros>();
            string _script = $@"
SELECT [id]
      ,[text]
      ,[posicao]
  FROM [dbo].[vw_whatsDM_parametros]
  WHERE [id] = {Id}
";
            try
            {
                //connBaseSoftcom.Open();
                SqlCommand _cmd = new SqlCommand(_script, connBaseWhatsapp);
                SqlDataAdapter _data = new SqlDataAdapter(_cmd);
                DataTable _tb = new DataTable();
                _data.Fill(_tb);
                connBaseWhatsapp.Close();
                foreach (var item in _tb.AsEnumerable())
                {
                    _list_parametros.Add(new Parametros()
                    {
                        id_template = item.Field<int>("id"),
                        posicao = item.Field<int>("posicao"),
                        text = item.Field<string>("text").ToString()
                    });
                }
                return _list_parametros;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    MessageBox.Show($@"Erro Function: GetParametros
{ex.Message}
");
                });
                return null;
            }
            finally
            {
                connBaseWhatsapp.Close();
            }
        }
        public static async void SetEvento(EventoLog eventoLog)
        {
            string _script = "";

            await ExeQueryAccess(_script);
        }
    }
}
