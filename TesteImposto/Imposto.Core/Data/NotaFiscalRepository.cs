using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Imposto.Core.Domain;

namespace Imposto.Core.Data
{
    public class NotaFiscalRepository
    {
        private string connString = ConfigurationManager.ConnectionStrings["SQLConnection"].ConnectionString;
        private SqlConnection conn;
        private SqlCommand cmd;
        private SqlDataReader reader;


        public NotaFiscalRepository()
        {
            conn = new SqlConnection(connString);

            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }

        public int P_NOTA_FISCAL(NotaFiscal nf)
        {
            string query = $@"declare @pId int = 0
                              exec P_NOTA_FISCAL {(nf.Id > 0 ? nf.Id.ToString() : "@pId output")} 
                                                ,{nf.NumeroNotaFiscal}
                                                ,'{nf.Serie}'
                                                ,'{nf.NomeCliente}'
                                                ,'{nf.EstadoDestino}'
                                                ,'{nf.EstadoOrigem}';
                              select @pId";
            using (cmd = new SqlCommand(query, conn))
                return (int)cmd.ExecuteScalar();
        }

        public void P_NOTA_FISCAL_ITEM(NotaFiscalItem itemNf)
        {
            string query = $@"exec P_NOTA_FISCAL_ITEM '{itemNf.Id}'
                                                     ,'{itemNf.IdNotaFiscal}'
                                                     ,'{itemNf.Cfop}'
                                                     ,'{itemNf.TipoIcms}'
                                                     ,@baseIcms
                                                     ,@aliqIcms
                                                     ,@valIcms
                                                     ,@baseIpi
                                                     ,@aliqIpi
                                                     ,@valIpi
                                                     ,'{itemNf.NomeProduto}'
                                                     ,'{itemNf.CodigoProduto}'";
            using (cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(new[] {
                    new SqlParameter("baseIcms", itemNf.BaseIcms),
                    new SqlParameter("aliqIcms", itemNf.AliquotaIcms),
                    new SqlParameter("valIcms", itemNf.ValorIcms),
                    new SqlParameter("baseIpi", itemNf.BaseIpi),
                    new SqlParameter("aliqIpi", itemNf.AliquotaIpi),
                    new SqlParameter("valIpi", itemNf.ValorIpi)
                });

                cmd.ExecuteNonQuery();
            }
        }

        ~NotaFiscalRepository()
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();
        }
    }
}
