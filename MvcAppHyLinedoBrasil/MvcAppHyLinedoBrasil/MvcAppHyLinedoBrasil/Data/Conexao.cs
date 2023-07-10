using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OracleClient;

namespace MvcAppHyLinedoBrasil.Data
{
    public class Conexao
    {
        #region Campos
        private OracleConnection conexaoORA = new OracleConnection();
        #endregion

        #region Construtor
        public Conexao(string ConnStr)
        {
            conexaoORA.ConnectionString = ConnStr;
        }
        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Abre a conexão com o Banco de Dados Oracle
        /// </summary>
        /// <returns>SqlConnection</returns>
        public OracleConnection OpenConexaoORA()
        {
            return conexaoORA;
        }

        /// <summary>
        /// Fecha a conexão com o Banco de Dados Oracle
        /// </summary>
        public void CloseConexaoORA()
        {
            conexaoORA.Close();
        }
        #endregion

    }
}