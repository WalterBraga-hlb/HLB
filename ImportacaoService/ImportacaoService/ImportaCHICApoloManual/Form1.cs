using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImportaCHICService;
using System.Reflection;

namespace ImportaCHICApoloManual
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ImportaCHIC servico = new ImportaCHIC();

                if (comboBox1.SelectedItem.ToString() == "Todos os Pedidos")
                {
                    servico.ImportaPedidosCHIC();
                }

                if (comboBox1.SelectedItem.ToString() == "Somente 01 Pedido")
                {
                    servico.ImportaPedidosCHIC(textBox1.Text);
                }

                textBox1.Visible = false;
                button1.Visible = false;
                label2.Visible = false;

                MessageBox.Show("Importação realizada com sucesso!!!", "AVISO");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;

                    msg = msg + " / " + loaderExceptions[0].Message;
                }

                if (ex.InnerException != null) msg = msg + " / " + ex.InnerException.Message;

                MessageBox.Show("Erro ao realizar importação: " + msg, "AVISO");
            }

            
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Todos os Pedidos")
            {
                label2.Visible = false;
                textBox1.Visible = false;

                button1.Visible = true;
            }

            if (comboBox1.SelectedItem.ToString() == "Somente 01 Pedido")
            {
                label2.Visible = true;
                textBox1.Visible = true;
                button1.Visible = true;
            }
        }
    }
}
