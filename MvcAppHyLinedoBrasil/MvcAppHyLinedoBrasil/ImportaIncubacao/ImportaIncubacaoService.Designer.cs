namespace ImportaIncubacao
{
    partial class ImportaIncubacaoService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Atualizacao = new System.Windows.Forms.Timer(this.components);
            this.AtualizaMinuto = new System.Windows.Forms.Timer(this.components);
            // 
            // Atualizacao
            // 
            this.Atualizacao.Tick += new System.EventHandler(this.Atualizacao_Tick);
            // 
            // AtualizaMinuto
            // 
            this.AtualizaMinuto.Tick += new System.EventHandler(this.AtualizaMinuto_Tick);
            // 
            // ImportaIncubacaoService
            // 
            this.ServiceName = "ImportaIncubacao";

        }

        #endregion

        private System.Windows.Forms.Timer Atualizacao;
        private System.Windows.Forms.Timer AtualizaMinuto;
    }
}
