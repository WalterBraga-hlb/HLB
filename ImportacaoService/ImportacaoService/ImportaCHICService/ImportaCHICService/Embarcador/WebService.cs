using System;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Web.Services.Protocols;

namespace ImportaCHICService.Embarcador
{
    class WebService
    {
        public string Url { get; private set; }
        public string Method { get; private set; }
        public OrderedDictionary Params = new OrderedDictionary();
        public XDocument ResponseSOAP = XDocument.Parse("<root/>");
        public XDocument ResultXML = XDocument.Parse("<root/>");
        public string ResultString = String.Empty;

        private Cursor InitialCursorState;

        public WebService()
        {
            Url = String.Empty;
            Method = String.Empty;
        }
        public WebService(string baseUrl)
        {
            Url = baseUrl;
            Method = String.Empty;
        }
        public WebService(string baseUrl, string methodName)
        {
            Url = baseUrl;
            Method = methodName;
        }

        /// Adiciona os parametros ao método chamado
        public void AddParameter(string name, string value)
        {
            Params.Add(name, value);
        }

        /// Seta os parametros ao método chamado
        public void SetParameters(OrderedDictionary parametros)
        {
            Params = (OrderedDictionary)parametros;
        }

        // Adiciona o SOAP ENCODING nas Tags
        private static string GetSoapEncodeString(Type tipo)
        {

            String strRetorno = "";
            if (tipo == typeof(ArrayList))
            {
                strRetorno = "SOAP-ENC:arrayType=\"SOAP-ENC:Array[1]\" xsi:type=\"SOAP-ENC:Array\"";
            }
            else if (tipo == typeof(OrderedDictionary))
            {
                strRetorno = "SOAP-ENC:arrayType=\"xsd:ur-type[2]\" xsi:type=\"SOAP-ENC:Array\"";

            }
            else if (tipo == typeof(bool))
            {
                strRetorno = "xsi:type=\"SOAP-ENC:Boolean\"";
            }
            return strRetorno;
        }

        // Limpa dados da ultima chamada
        public void CleanLastInvoke()
        {
            ResponseSOAP = ResultXML = null;
            ResultString = Method = String.Empty;
            Params = new OrderedDictionary();
        }

        // Verifica se foi passao URL e Método
        private void AssertCanInvoke(string methodName = "")
        {
            if (Url == String.Empty)
                throw new ArgumentNullException("Especifique uma URL.");
            if ((methodName == "") && (Method == String.Empty))
                throw new ArgumentNullException("Especifique um Metodo.");
        }

        // Extrai xml resultado em caso de erro (SOAP FAULT)
        private void ExtractResult()
        {
            var faultString = "";
            var strTag = "//faultstring";
            XElement webMethodResultString = ResponseSOAP.XPathSelectElement(strTag);
            if (webMethodResultString != null) faultString = webMethodResultString.FirstNode.ToString();

            strTag = "//faultcode";
            var faultCode = "";
            XElement webMethodResultCode = ResponseSOAP.XPathSelectElement(strTag);
            if (webMethodResultCode != null) faultCode = webMethodResultCode.FirstNode.ToString();

            ResultString = faultString + "(" + faultCode + ")";
        }

        // Extrai xml resultado em caso de sucesso na chamada
        private void ExtractResult(string methodName)
        {
            XmlNamespaceManager namespMan = new XmlNamespaceManager(new NameTable());
            namespMan.AddNamespace("prefix", "http://webservice.softlogbrasil.com.br/index.php");

            var strTag = "//prefix:" + methodName + "Response/return";

            XElement webMethodResult = ResponseSOAP.XPathSelectElement(strTag, namespMan);
            if (webMethodResult.FirstNode != null)
            {
                if (webMethodResult.FirstNode.NodeType != XmlNodeType.Element)
                {
                    ResultString = webMethodResult.FirstNode.ToString();
                }
            }
        }

        // Chama o método especificado
        // <param name="methodName">Nome do Método (case sensitive)</param>
        // <param name="encode">Encode parameters? </param>
        public void Invoke(string methodName, bool encode)
        {

            //verifica url e metodo
            AssertCanInvoke(methodName);

            //Monta cabeçalho xml com a Authkey fornecida
            string soapStr =
//                @"<?xml version=""1.0"" encoding=""utf-8""?>
//	                <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
//	                   xmlns:xsd=""http://www.w3.org/2003/05/soap-envelope""
//	                   xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
//	                   <soap:Header>
//					      <AuthKey>#HyL1ne8r@S1L*</AuthKey>
//					   </soap:Header>
//					   <soap:Body>	                  
//					   <{0}>
//	                      {1}
//	                   </{0}>					  
//	                  </soap:Body>
//	                </soap:Envelope>";
                @"<?xml version=""1.0"" encoding=""utf-8""?>
	                <soap12:Envelope xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope""
	                   xmlns:SOAP-ENC=""http://www.w3.org/2003/05/soap-envelope""
	                   xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
	                   <soap12:Header>
					      <AuthKey>#HyL1ne8r@S1L*</AuthKey>
					   </soap12:Header>
					   <soap12:Body>	                  
					   <{0}>
	                      {1}
	                   </{0}>					  
	                  </soap12:Body>
	                </soap12:Envelope>";

            // Cria requisição
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Accept = "text/xml";
            req.Method = "POST";

            using (Stream stm = req.GetRequestStream())
            {

                string postValues = "";
                string strSoapEncode = "";

                // percorre e adiciona parametros parâmatros a chamada
                foreach (DictionaryEntry param in Params)
                {
                    if (param.Value.GetType() == typeof(ArrayList))
                    {
                        strSoapEncode = GetSoapEncodeString(param.Value.GetType());
                        postValues += string.Format("<{0} {1}>", param.Key, strSoapEncode);

                        ArrayList paramList = (ArrayList)param.Value;
                        foreach (var item in paramList)
                        {
                            OrderedDictionary itemParams = (OrderedDictionary)item;
                            strSoapEncode = GetSoapEncodeString(itemParams.GetType());
                            postValues += string.Format("<item {0}>", strSoapEncode);
                            foreach (DictionaryEntry itemParam in itemParams)
                            {
                                if (itemParam.Value.GetType() == typeof(ArrayList))
                                {
                                    strSoapEncode = GetSoapEncodeString(itemParam.Value.GetType());
                                    postValues += string.Format("<{0} {1}>", itemParam.Key, strSoapEncode);

                                    ArrayList itemParamList = (ArrayList)itemParam.Value;
                                    foreach (var subItem in itemParamList)
                                    {
                                        OrderedDictionary subitemParams = (OrderedDictionary)subItem;
                                        strSoapEncode = GetSoapEncodeString(subitemParams.GetType());
                                        postValues += string.Format("<item {0}>", strSoapEncode);

                                        foreach (DictionaryEntry subitemParam in subitemParams)
                                        {
                                            strSoapEncode = GetSoapEncodeString(subitemParam.Value.GetType());
                                            string strKey = subitemParam.Key.ToString();
                                            string strValor = "";

                                            if (subitemParam.Value.GetType() == typeof(bool))
                                            {
                                                strValor = subitemParam.Value.ToString().ToLower();
                                            }
                                            else if (subitemParam.Value.GetType() == typeof(double))
                                            {
                                                strValor = subitemParam.Value.ToString().Replace(",", ".");
                                            }
                                            else
                                            {
                                                strValor = subitemParam.Value.ToString();
                                            }

                                            if (encode)
                                            {
                                                postValues += string.Format("<{0} {1}>{2}</{0}>", Uri.EscapeDataString(strKey), strSoapEncode, Uri.EscapeDataString(strValor));
                                            }
                                            else
                                            {
                                                postValues += string.Format("<{0} {1}>{2}</{0}>", strKey, strSoapEncode, strValor);
                                            }
                                        }
                                        postValues += "</item>";
                                    }
                                    postValues += string.Format("</{0}>", itemParam.Key);
                                }
                                else
                                {
                                    strSoapEncode = GetSoapEncodeString(itemParam.Value.GetType());
                                    string strKey = itemParam.Key.ToString();
                                    string strValor = "";

                                    if (itemParam.Value.GetType() == typeof(bool))
                                    {
                                        strValor = itemParam.Value.ToString().ToLower();
                                    }
                                    else if (itemParam.Value.GetType() == typeof(double))
                                    {
                                        strValor = itemParam.Value.ToString().Replace(",", ".");
                                    }
                                    else
                                    {
                                        strValor = itemParam.Value.ToString();
                                    }

                                    if (encode)
                                    {
                                        postValues += string.Format("<{0} {1}>{2}</{0}>", Uri.EscapeDataString(strKey), strSoapEncode, Uri.EscapeDataString(strValor));
                                    }
                                    else
                                    {
                                        postValues += string.Format("<{0} {1}>{2}</{0}>", strKey, strSoapEncode, strValor);
                                    }
                                }
                            }
                            postValues += "</item>";
                        }

                        postValues += string.Format("</{0}>", param.Key);
                    }
                    else
                    {
                        strSoapEncode = GetSoapEncodeString(param.Value.GetType());
                        string strKey = param.Key.ToString();
                        string strValor = "";

                        if (param.Value.GetType() == typeof(bool))
                        {
                            strValor = param.Value.ToString().ToLower();
                        }
                        else if (param.Value.GetType() == typeof(double))
                        {
                            strValor = param.Value.ToString().Replace(",", ".");
                        }
                        else
                        {
                            strValor = param.Value.ToString();
                        }
                        if (encode)
                        {
                            postValues += string.Format("<{0} {1}>{2}</{0}>", Uri.EscapeDataString(strKey), strSoapEncode, Uri.EscapeDataString(strValor));
                        }
                        else
                        {
                            postValues += string.Format("<{0} {1}>{2}</{0}>", strKey, strSoapEncode, strValor);
                        }
                    }

                }

                soapStr = string.Format(soapStr, methodName, postValues);
                using (StreamWriter stmw = new StreamWriter(stm))
                {
                    stmw.Write(soapStr);
                }
            }

            // valida e extrai resuldado
            try
            {

                using (StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    string result = responseReader.ReadToEnd();
                    ResponseSOAP = XDocument.Parse(result);
                    ExtractResult(methodName);
                }

            }
            catch (WebException e)
            {

                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    StreamReader reader = new StreamReader(((HttpWebResponse)e.Response).GetResponseStream());
                    string failureReason = reader.ReadToEnd().ToString();
                    ResponseSOAP = XDocument.Parse(failureReason);
                    ExtractResult();
                }
            }
        }

        // Inicializa Cursor.
        internal void PreInvoke()
        {
            CleanLastInvoke();
            InitialCursorState = Cursor.Current;
        }

        /// Seta Cursor
        internal void PosInvoke()
        {
            Cursor.Current = InitialCursorState;
        }
    }
}
