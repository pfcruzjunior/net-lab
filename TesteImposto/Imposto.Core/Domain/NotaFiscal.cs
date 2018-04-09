using System;
using System.Collections.Generic;
using System.Linq;

namespace Imposto.Core.Domain
{
    public class NotaFiscal
    {
        public int Id { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int Serie { get; set; }
        public string NomeCliente { get; set; }

        public string EstadoDestino { get; set; }
        public string EstadoOrigem { get; set; }

        public IEnumerable<NotaFiscalItem> ItensDaNotaFiscal { get; set; }

        public NotaFiscal()
        {
            ItensDaNotaFiscal = new List<NotaFiscalItem>();
        }

        public void EmitirNotaFiscal(Pedido pedido)
        {
            this.NumeroNotaFiscal = 99999;
            this.Serie = new Random().Next(Int32.MaxValue);
            this.NomeCliente = pedido.NomeCliente;

            this.EstadoDestino = pedido.EstadoOrigem;
            this.EstadoOrigem = pedido.EstadoDestino;
            string[] descontoSudeste = { "SP", "RJ", "MG", "ES" };

            foreach (PedidoItem itemPedido in pedido.ItensDoPedido)
            {
                NotaFiscalItem notaFiscalItem = new NotaFiscalItem();
                notaFiscalItem.Cfop = GetCpof(EstadoDestino);

                if (notaFiscalItem.Cfop == "6.009")
                    notaFiscalItem.BaseIcms = itemPedido.ValorItemPedido * 0.90; //redução de base                
                else
                    notaFiscalItem.BaseIcms = itemPedido.ValorItemPedido;

                if (this.EstadoDestino == this.EstadoOrigem)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;
                }
                else
                {
                    notaFiscalItem.TipoIcms = "10";
                    notaFiscalItem.AliquotaIcms = 0.17;
                }

                notaFiscalItem.ValorIcms = notaFiscalItem.BaseIcms * notaFiscalItem.AliquotaIcms;
                notaFiscalItem.BaseIpi = itemPedido.ValorItemPedido;

                if (itemPedido.Brinde)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;
                    notaFiscalItem.ValorIcms = notaFiscalItem.BaseIcms * notaFiscalItem.AliquotaIcms;
                    notaFiscalItem.AliquotaIpi = 0;
                }
                else
                    notaFiscalItem.AliquotaIpi = 0.1;

                notaFiscalItem.ValorIpi = notaFiscalItem.BaseIpi * notaFiscalItem.AliquotaIpi;
                if (descontoSudeste.Contains(EstadoDestino.ToUpper()))
                    itemPedido.ValorItemPedido = itemPedido.ValorItemPedido - (itemPedido.ValorItemPedido * 0.1);

                notaFiscalItem.NomeProduto = itemPedido.NomeProduto;
                notaFiscalItem.CodigoProduto = itemPedido.CodigoProduto;
                (ItensDaNotaFiscal as List<NotaFiscalItem>).Add(notaFiscalItem);
            }
        }

        private string GetCpof(string destino)
        {
            string cpof = string.Empty;
            switch (destino)
            {
                case "RJ":
                    cpof = "6.000";
                    break;
                case "PE":
                    cpof = "6.001";
                    break;
                case "MG":
                    cpof = "6.002";
                    break;
                case "PB":
                    cpof = "6.003";
                    break;
                case "PR":
                    cpof = "6.004";
                    break;
                case "PI":
                    cpof = "6.005";
                    break;
                case "RO":
                    cpof = "6.006";
                    break;
                case "TO":
                    cpof = "6.008";
                    break;
                case "SE":
                    //*Obs: Haviam duas condições iguais para origem SP e destino SE, optei por considerar a de maior valor
                    cpof = "6.009";
                    break;
                case "PA":
                    cpof = "6.010";
                    break;
            }
            return cpof;
        }
    }
}
