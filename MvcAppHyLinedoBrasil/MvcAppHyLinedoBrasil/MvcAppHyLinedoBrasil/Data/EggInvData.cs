using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OracleClient;
using MvcAppHyLinedoBrasil.Classe;

namespace MvcAppHyLinedoBrasil.Data
{
    public class EggInvData : Conexao
    {
        #region Campos

        public enum Status { D, EX, M, MV, O, S, XX };

        #endregion

        #region Construtor

        public EggInvData(string connString)
            : base(connString)
        {
            
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método para Retornar o Inventario de Ovos por Status e Incubatorio.
        /// </summary>
        /// <param name="status">Status do Ovo</param>
        /// <returns>Estoque de Ovos</returns>
        public List<EggInv> Lista(Status status, String incubatorio)
        {
            List<EggInv> lista = new List<EggInv>();
                     
            OracleConnection conexao = OpenConexaoORA();

            OracleCommand comando = new OracleCommand();
            comando.Connection = conexao;

            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = "SELECT E.*, F.Variety FROM EGGINV_DATA E, FLOCKS F WHERE E.STATUS = '" + status + 
                "' AND E.FLOCK_KEY = F.FLOCK_KEY AND E.HATCH_LOC = '" + incubatorio + "' ORDER BY F.VARIETY, E.LAY_DATE";

            conexao.Open();

            OracleDataReader reader = comando.ExecuteReader();

            while (reader.Read())
            {
                EggInv EggInv = new EggInv();

                EggInv.Company = (string)reader["Company"];
                EggInv.Egg_Units = (decimal)reader["Egg_Units"];
                EggInv.Farm_ID = (string)reader["Farm_ID"];
                EggInv.Flock_ID = (string)reader["Flock_ID"];
                EggInv.Flock_Key = (string)reader["Flock_Key"];
                EggInv.Hatch_Loc = (string)reader["Hatch_Loc"];
                EggInv.Lay_Date = (DateTime)reader["Lay_Date"];
                EggInv.Location = (string)reader["Location"];
                EggInv.Region = (string)reader["Region"];
                EggInv.Status = (string)reader["Status"];
                EggInv.Track_NO = (string)reader["Track_NO"];
                EggInv.Variety = (string)reader["Variety"];
                
                lista.Add(EggInv);
            }

            CloseConexaoORA();

            return lista;
        }

        public void ExecutaProcedureMapaIncubacao(string hatchLoc, DateTime setDate)
        {
            OracleConnection conexao = OpenConexaoORA();

            OracleCommand comando = new OracleCommand();
            comando.Connection = conexao;

            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.CommandText = "MAPAINCUBACAO";
            comando.Parameters.Clear();
            comando.Parameters.AddWithValue("plocal", hatchLoc);
            comando.Parameters.AddWithValue("psetdate", setDate);

            conexao.Open();

            comando.ExecuteNonQuery();

            conexao.Close();
        }

        #endregion
    }
}