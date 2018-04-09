using Imposto.Core.Domain;
using Imposto.Core.Service;
using System;
using System.Data;
using System.Windows.Forms;

namespace TesteImposto
{
    public partial class FormImposto : Form
    {
        private Pedido pedido = new Pedido();

        public FormImposto()
        {
            InitializeComponent();
            dataGridViewPedidos.AutoGenerateColumns = true;
            dataGridViewPedidos.DataSource = GetTablePedidos();
            ResizeColumns();

            boxEstadoOrigem.Items.AddRange(GetEstados());
            boxEstadoDestino.Items.AddRange(GetEstados());
        }

        private void ResizeColumns()
        {
            double mediaWidth = dataGridViewPedidos.Width / dataGridViewPedidos.Columns.GetColumnCount(DataGridViewElementStates.Visible);

            for (int i = dataGridViewPedidos.Columns.Count - 1; i >= 0; i--)
            {
                var coluna = dataGridViewPedidos.Columns[i];
                coluna.Width = Convert.ToInt32(mediaWidth);
            }
        }

        private object GetTablePedidos()
        {
            DataTable table = new DataTable("pedidos");
            table.Columns.Add(new DataColumn("Nome do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Codigo do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Valor", typeof(decimal)));
            table.Columns.Add(new DataColumn("Brinde", typeof(bool)));

            return table;
        }

        private void buttonGerarNotaFiscal_Click(object sender, EventArgs e)
        {
            NotaFiscalService service = new NotaFiscalService();
            pedido.EstadoOrigem = boxEstadoOrigem.Text;
            pedido.EstadoDestino = boxEstadoDestino.Text;
            pedido.NomeCliente = textBoxNomeCliente.Text;

            DataTable table = (DataTable)dataGridViewPedidos.DataSource;

            foreach (DataRow row in table.Rows)
            {
                pedido.ItensDoPedido.Add(
                    new PedidoItem()
                    {
                        Brinde = string.IsNullOrEmpty(row["Brinde"].ToString()) ? false : Convert.ToBoolean(row["Brinde"]),
                        CodigoProduto = row["Codigo do produto"].ToString(),
                        NomeProduto = row["Nome do produto"].ToString(),
                        ValorItemPedido = Convert.ToDouble(row["Valor"].ToString())
                    });
            }

            service.GerarNotaFiscal(pedido);

            MessageBox.Show("Operação efetuada com sucesso");

            //ActiveForm.ResetText();
            LimparCampos();

        }

        private object[] GetEstados() => new[] { "RJ", "PE", "MG", "PB", "PR", "PI", "RO", "TO", "SE", "PA", "SP" };

        public void LimparCampos()
        {
            textBoxNomeCliente.Text = string.Empty;
            boxEstadoOrigem.SelectedIndex = -1;
            boxEstadoDestino.SelectedIndex = -1;
            dataGridViewPedidos.DataSource = GetTablePedidos();
        }
    }
}
