using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Imposto.Core.Service;
using Imposto.Core.Domain;
using System.Collections.Generic;
using System.Configuration;

namespace Imposto.Test
{
    [TestClass]
    public class UnitTest
    {
        private NotaFiscalService nfService;
        private NotaFiscal notaFiscal;
        private Pedido pedido;
        
        public UnitTest()
        {
            nfService = new NotaFiscalService();
            pedido = new Pedido
            {
                EstadoDestino = "SP",
                EstadoOrigem = "MG",
                NomeCliente = "Cliente",
                ItensDoPedido = new List<PedidoItem>
                {
                    new PedidoItem {
                        CodigoProduto = "100.789",
                        NomeProduto = "Tênis",
                        ValorItemPedido = 189.9
                    },
                    new PedidoItem {
                        CodigoProduto = "100.324",
                        NomeProduto = "Sapato",
                        ValorItemPedido = 99.9
                    }
                }
            };
        }

        [TestMethod]
        public void EmitirNota()
        {
            try
            {
                nfService.GerarNotaFiscal(pedido);
            }
            catch (Exception ex)
            {
                throw ex;                
            }
        }

        [TestMethod]
        public void GerarArquivo()
        {
            notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);

            string path = ConfigurationManager.AppSettings["SavePath"];
            string filePath = $"{path}\\NF_{notaFiscal.NumeroNotaFiscal}.xml";
            try
            {
                string file = nfService.GerarArquivoXml(filePath, notaFiscal, new string[] { "NotaFiscal" }, new string[] { "ItensDaNotaFiscal" }, new string[] { "Item" });
                Assert.IsNotNull(file);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
