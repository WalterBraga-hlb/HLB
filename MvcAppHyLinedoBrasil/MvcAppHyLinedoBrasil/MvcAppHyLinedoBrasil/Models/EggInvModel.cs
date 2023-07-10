using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcAppHyLinedoBrasil.Classe;
using MvcAppHyLinedoBrasil.Data;

namespace MvcAppHyLinedoBrasil.Models
{
    public class EggInvModel
    {
        #region Campos
        private string connString = null;
        #endregion

        #region Construtor
        public EggInvModel(string connString)
        {
            this.connString = connString;
            //this.connString = "Data Source=brflocks;Persist Security Info=True;User ID=na;Password=brnaps;Unicode=True";
        }
        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Método para Listar o Inventario de Ovos com Status Open do Incubatorio Comercial.
        /// </summary>
        /// <returns>Inventario de Ovos</returns>
        public List<EggInv> InventarioOvosParaIncubacao()
        {
            EggInvData data = new EggInvData(connString);

            return data.Lista(EggInvData.Status.O, "CH");
        }

        #endregion
    }
}