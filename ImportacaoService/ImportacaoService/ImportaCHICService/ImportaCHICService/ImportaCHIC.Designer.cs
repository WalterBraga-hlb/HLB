﻿namespace ImportaCHICService
{
    partial class ImportaCHIC
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
            this.Atualizacao_02 = new System.Windows.Forms.Timer(this.components);
            // 
            // Atualizacao
            // 
            this.Atualizacao.Tick += new System.EventHandler(this.Atualizacao_Tick);
            // 
            // Atualizacao_02
            // 
            this.Atualizacao_02.Tick += new System.EventHandler(this.Atualizacao_02_Tick);
            // 
            // ImportaCHIC
            // 
            this.ServiceName = "ImportaCHIC";

        }

        #endregion

        private System.Windows.Forms.Timer Atualizacao;
        private System.Windows.Forms.Timer Atualizacao_02;
    }
}
