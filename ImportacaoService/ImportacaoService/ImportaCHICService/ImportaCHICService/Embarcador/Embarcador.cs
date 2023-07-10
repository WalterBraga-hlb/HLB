using System;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Net;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using ImportaCHICService.Data;
using ImportaCHICService.Data.CHICDataSetTableAdapters;

namespace ImportaCHICService.Embarcador
{
    public class Embarcador
    {
        //Inicializa o webservice
        private static WebService EmbarcadorAPI = new WebService("http://webservice.softlogbrasil.com.br/index.php");

        #region Unidades

        //insereAtualizaUnidade - passando os parâmetros já definidos em outro local
        public static string insereAtualizaUnidade(OrderedDictionary parametros)
        {
            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("insereAtualizaUnidade", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código da Unidade: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally 
            { 
                EmbarcadorAPI.PosInvoke();
            }
        }

        //insereAtualizaUnidade - buscando os parâmetros na própria função
        public static string insereAtualizaUnidade()
        {
            //Cria um arrayList com os dados da unidade pai
            ArrayList arrayUnidadePai = new ArrayList();
            OrderedDictionary arrayUnidadePaiItens = new OrderedDictionary();
            arrayUnidadePaiItens.Add("cod_unidade", 0);
            arrayUnidadePaiItens.Add("diferenciador", "MATRIZ");
            arrayUnidadePai.Add(arrayUnidadePaiItens);

            //Cria um arrayList com os dados da referencia da unidade
            ArrayList arrayReferencia = new ArrayList();
            OrderedDictionary arrayReferenciaItens = new OrderedDictionary();
            arrayReferenciaItens.Add("lat", -23.5292839);
            arrayReferenciaItens.Add("lon", -46.4105916);
            arrayReferencia.Add(arrayReferenciaItens);

            //Cria um arrayList com os dados do(s) tipo(s) de operacao da unidade
            ArrayList arrayTipoOperacao = new ArrayList();
            OrderedDictionary arrayTipoOperacaoItens = new OrderedDictionary();
            arrayTipoOperacaoItens.Add("codigo", 1783);
            arrayTipoOperacaoItens.Add("origem", false);
            arrayTipoOperacaoItens.Add("destino", true);
            arrayTipoOperacaoItens.Add("passagem", false);
            arrayTipoOperacao.Add(arrayTipoOperacaoItens);

            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("cod_unidade", 577669);
            parametros.Add("diferenciador", "");
            parametros.Add("descricao", "2TESTE INTEGRACAO SOFTLOG");
            parametros.Add("responsavel", "");
            parametros.Add("telefone", "11 963926643");
            parametros.Add("endereco", "RUA CARDON, 1135, SAO PAULO, JARDIM LAJEADO, 08041525");
            parametros.Add("observacao", "");
            parametros.Add("unidade_pai", arrayUnidadePai);
            parametros.Add("cidade", "SAO PAULO");
            parametros.Add("uf", "SP");
            parametros.Add("tipo", 2622);
            parametros.Add("zona", "");
            parametros.Add("regiao", "");
            parametros.Add("referencia", arrayReferencia);
            parametros.Add("tipo_operacao", arrayTipoOperacao);
            parametros.Add("cnpj", "");
            parametros.Add("numero", 1135);
            parametros.Add("bairro", "JARDIM LAJEADO");
            parametros.Add("cep", "08041525");
            parametros.Add("complemento", "Bloco 1");
            parametros.Add("tipo_pessoa", "J");
            parametros.Add("rg_ie", "");

            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("insereAtualizaUnidade", false);
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }

            //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
            return EmbarcadorAPI.ResultString;
        }

        #endregion

        #region Pedidos

        //inserePedidosLote - insere pedidos por lote
        public static XDocument inserePedidosLote(OrderedDictionary parametros)
        {
            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("inserepedidoslote", false);
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }

            //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
            return EmbarcadorAPI.ResponseSOAP;
        }

        //buscaPedido - Método que retorna os pedidos e, caso hover, ID da carga e a placa onde está o pedido
        public static XDocument buscaPedido(OrderedDictionary parametros)
        {
            //prepara a cham    ada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("buscaPedido", false);
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }

            //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
            return EmbarcadorAPI.ResponseSOAP;
        }

        //buscaPedido - Método que retorna os pedidos e, caso hover, ID da carga e a placa onde está o pedido
        public static XDocument buscaPedido(string numeroPedido, string codItem)
        {
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("CODIGO", numeroPedido);
            parametros.Add("NR_ITEM", codItem);

            //prepara a cham    ada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("buscaPedido", false);
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }

            //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
            return EmbarcadorAPI.ResponseSOAP;
        }

        //retornaIDCargaPedido - Método que retorna os pedidos e, caso hover, ID da carga e a placa onde está o pedido
        public static int retornaIDCargaPedido(string numeroPedido, string codItem)
        {
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("CODIGO", numeroPedido);
            parametros.Add("NR_ITEM", codItem);

            //prepara a cham    ada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("buscaPedido", false);
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }

            int idCarga = 0;

            foreach (XElement retorno in EmbarcadorAPI.ResponseSOAP.Descendants("return"))
            {
                var listaItens = retorno.Nodes();

                foreach (XElement item in listaItens)
                {
                    var listaSubItens = item.Nodes()
                        .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                    #region Carrega valores do retorno

                    XElement objIDCarga = (XElement)listaSubItens[0];
                    XElement objPlaca = (XElement)listaSubItens[1];
                    XElement objQuantidade = (XElement)listaSubItens[2];
                    XElement objPeso = (XElement)listaSubItens[3];

                    #endregion

                    if (objIDCarga.Value != "")
                    {
                        idCarga = Convert.ToInt32(objIDCarga.Value);
                    }
                }
            }

            return idCarga;
        }

        //adicionaPedidosCarga - Adiciona pedidos na carga ou viagem
        public static string adicionaPedidosCarga(int idCarga, string placa, int codigoPedido, int numeroItem,
            int qtde, int pesoTotal)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("ID_CARGA", idCarga);
            parametros.Add("PLACA", placa);

            //Cria um arrayList com os dados da origem
            ArrayList arrayPedidos = new ArrayList();
            OrderedDictionary pedido = new OrderedDictionary();
            pedido.Add("COD_PEDIDO", codigoPedido);
            pedido.Add("NR_ITEM", numeroItem);
            pedido.Add("QUANTIDADE", qtde);
            pedido.Add("PESO_TOTAL", pesoTotal);
            arrayPedidos.Add(pedido);
            parametros.Add("PEDIDOS", arrayPedidos);

            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("adicionaPedidosCarga", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    #region Verifica Erro

                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código da Unidade: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;

                    #endregion
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        //removePedidosCarga - Remove pedidos da carga ou viagem
        public static string removePedidosCarga(OrderedDictionary parametros)
        {
            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("removePedidosCarga", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código do Pedido: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        //apagaPedido - Método para apagar pedidos
        public static string apagaPedido(OrderedDictionary parametros)
        {
            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("apagaPedido", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código do Pedido: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        #endregion

        #region Carga

        //buscaCargaCodigo - Retorna o Id da Carga no Embarcador
        public static string buscaCargaCodigo(int numCarga)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("cod_carga", numCarga);

            //prepara a cham    ada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("buscaCargaCodigo", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    if (xmlRetorno != null)
                    {
                        foreach (XElement retorno in xmlRetorno.Descendants())
                        {
                            if (retorno.Name.LocalName == "Fault")
                            {
                                var listaItens = retorno.Nodes();

                                foreach (XElement item in listaItens)
                                {
                                    if (item.Name.LocalName == "Code")
                                    {
                                        var listaItensCode = item.Nodes().ToList();
                                        XElement objCode = (XElement)listaItensCode[0];
                                        msgRetorno = "Código do Erro: " + objCode.Value;
                                    }

                                    if (item.Name.LocalName == "Reason")
                                    {
                                        var listaItensReason = item.Nodes().ToList();
                                        XElement objReason = (XElement)listaItensReason[0];
                                        msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                    }

                                    if (item.Name.LocalName == "Detail")
                                    {
                                        msgRetorno = msgRetorno + " / Código da Carga: " + item.Value;
                                    }
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        //insereCarga - Cria carga sem informar destino
        public static string insereCarga(int numCarga, int codigoIncubatorio, string obsCarga, string alias)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("COD_CARGA", numCarga);
            parametros.Add("UNIDADE_BASE_COD", codigoIncubatorio);
            parametros.Add("DIFERENCIADOR", "");
            parametros.Add("TIPO_OPERACAO", 2623);
            parametros.Add("TRANSBORDO", false);
            parametros.Add("OBSERVACAO", obsCarga);
            parametros.Add("ALIAS", alias);
            
            //Cria um arrayList com os dados da origem
            ArrayList arrayOrigem = new ArrayList();
            OrderedDictionary origem = new OrderedDictionary();
            origem.Add("COD_UNIDADE", codigoIncubatorio);
            origem.Add("DIFERENCIADOR", "");
            origem.Add("ORDEM", 1);
            //origem.Add("CADASTRO_UNIDADE", "");
            arrayOrigem.Add(origem);
            parametros.Add("UNIDADES_ORIGEM", arrayOrigem);

            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("insereCarga", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código da Carga: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        //insereCargaComPedidos - Cria carga a partir dos pedidos passados
        public static string insereCargaComPedidos(int numCarga, int codigoIncubatorio, string obsCarga, string alias,
            ArrayList arrayPedido)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("COD_CARGA", numCarga);
            parametros.Add("UNIDADE_BASE", codigoIncubatorio);
            parametros.Add("DIFERENCIADOR", "");
            parametros.Add("TIPO_OPERACAO", 2623);
            parametros.Add("TRANSBORDO", false);
            parametros.Add("OBSERVACAO", obsCarga);
            parametros.Add("ALIAS", alias);

            //Cria um arrayList com os dados da origem
            ArrayList arrayOrigem = new ArrayList();
            OrderedDictionary origem = new OrderedDictionary();
            origem.Add("COD_UNIDADE", codigoIncubatorio);
            origem.Add("DIFERENCIADOR", "");
            arrayOrigem.Add(origem);
            parametros.Add("UNIDADES_ORIGEM", arrayOrigem);
            parametros.Add("PEDIDOS", arrayPedido);

            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("insereCargaComPedidos", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código da Carga: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        //apagaCarga - Método para apagar cargas
        public static string apagaCarga(int idCarga)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("ID_CARGA", idCarga);

            //prepara a cham    ada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("apagaCarga", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código da Carga: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        //vincularCargaAoVeiculo - Faz o vinculo entre a carga e o veículo que irá trasportá-la
        public static string vincularCargaAoVeiculo(int idCarga, string placa, DateTime dataPrevisaoInicio, DateTime horaPrevisaoInicio,
            DateTime dataPrevisaoFim, DateTime horaPrevisaoFim, bool liberaVeiculo)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("ID_CARGA", idCarga);
            parametros.Add("PLACA", placa);
            parametros.Add("DT_PREVISAO_INICIO", dataPrevisaoInicio.ToShortDateString());
            parametros.Add("HR_PREVISAO_INICIO", horaPrevisaoInicio.ToString("HH:mm"));
            parametros.Add("DT_PREVISAO_FIM", dataPrevisaoFim.ToShortDateString());
            parametros.Add("HR_PREVISAO_FIM", horaPrevisaoFim.ToString("HH:mm"));
            parametros.Add("LIBERA_VEICULO", liberaVeiculo);

            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("vincularCargaAoVeiculo", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Detalhes: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        //removerVinculoCargaVeiculo - Remove o vinculo entre a carga e o veículo
        public static string removerVinculoCargaVeiculo(int idCarga, string placa)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("ID_CARGA", idCarga);
            parametros.Add("PLACA", placa);

            //prepara a cham    ada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("removerVinculoCargaVeiculo", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código da Carga: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        //liberaCarga - Libera o veículo para viajar
        public static string liberaCarga(int idCarga)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("ID_CARGA", idCarga);

            //prepara a chamada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("liberaCarga", false);

                return EmbarcadorAPI.ResultString;
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResultString;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código da Carga: " + item.Value;
                                }
                            }
                        }
                    }

                    return msgRetorno;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }
        }

        //retornaDadosViagem - Método que retorna informações da viagem
        public static XDocument retornaDadosViagem(int idCarga, string placa)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("ID_CARGA", idCarga);
            parametros.Add("PLACA", placa);

            //prepara a cham    ada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("retornaDadosViagem", false);
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResponseSOAP;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    if (xmlRetorno != null)
                    {
                        foreach (XElement retorno in xmlRetorno.Descendants())
                        {
                            if (retorno.Name.LocalName == "Fault")
                            {
                                var listaItens = retorno.Nodes();

                                foreach (XElement item in listaItens)
                                {
                                    if (item.Name.LocalName == "Code")
                                    {
                                        var listaItensCode = item.Nodes().ToList();
                                        XElement objCode = (XElement)listaItensCode[0];
                                        msgRetorno = "Código do Erro: " + objCode.Value;
                                    }

                                    if (item.Name.LocalName == "Reason")
                                    {
                                        var listaItensReason = item.Nodes().ToList();
                                        XElement objReason = (XElement)listaItensReason[0];
                                        msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                    }

                                    if (item.Name.LocalName == "Detail")
                                    {
                                        msgRetorno = msgRetorno + " / Código da Carga: " + item.Value;
                                    }
                                }
                            }
                        }
                    }

                    return EmbarcadorAPI.ResponseSOAP;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }

            return EmbarcadorAPI.ResponseSOAP;
        }

        //retornaVeiculoPassagens - Retorna as passagens da placa informada no dia informado
        public static XDocument retornaVeiculoPassagens(string placa, DateTime data)
        {
            //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
            OrderedDictionary parametros = new OrderedDictionary();
            parametros.Add("PLACA", placa);
            parametros.Add("DATA", data.ToString("dd/MM/yyyy"));

            //prepara a cham    ada
            EmbarcadorAPI.PreInvoke();

            //adiciona os parâmetros
            EmbarcadorAPI.SetParameters(parametros);

            //envia a chamada ao método
            try
            {
                EmbarcadorAPI.Invoke("retornaVeiculoPassagens", false);
            }
            catch
            {
                //string de retorno caso obtenha sucesso ou string de erro caso haja algum problema
                if (EmbarcadorAPI.ResultString != "")
                    return EmbarcadorAPI.ResponseSOAP;
                else
                {
                    string msgRetorno = "";

                    XDocument xmlRetorno = EmbarcadorAPI.ResponseSOAP;

                    foreach (XElement retorno in xmlRetorno.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgRetorno = "Código do Erro: " + objCode.Value;
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgRetorno = msgRetorno + " / Msg. do Erro: " + objReason.Value;
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgRetorno = msgRetorno + " / Código da Carga: " + item.Value;
                                }
                            }
                        }
                    }

                    return EmbarcadorAPI.ResponseSOAP;
                }
            }
            finally
            {
                EmbarcadorAPI.PosInvoke();
            }

            return EmbarcadorAPI.ResponseSOAP;
        }

        #endregion
    }
}
