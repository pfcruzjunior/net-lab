using Imposto.Core.Data;
using Imposto.Core.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Imposto.Core.Service
{
    public class NotaFiscalService
    {
        public void GerarNotaFiscal(Domain.Pedido pedido)
        {
            NotaFiscal notaFiscal = new NotaFiscal();
            notaFiscal.EmitirNotaFiscal(pedido);

            string path = ConfigurationManager.AppSettings["SavePath"];
            string filePath = $"{path}\\NF_{notaFiscal.NumeroNotaFiscal}.xml";
            string file = GerarArquivoXml(filePath, notaFiscal, new string[] { "NotaFiscal" }, new string[] { "ItensDaNotaFiscal" }, new string[] { "Item" });

            if (File.Exists(file))
            {
                var repository = new NotaFiscalRepository();
                notaFiscal.Id = repository.P_NOTA_FISCAL(notaFiscal);

                foreach (var item in notaFiscal.ItensDaNotaFiscal)
                {
                    item.IdNotaFiscal = notaFiscal.Id;
                    repository.P_NOTA_FISCAL_ITEM(item);
                }
            }
        }

        public string GerarArquivoXml(string filePath, object obj, string[] parents, string[] children, string[] grandChildren)
        {
            using (XmlWriter writer = XmlWriter.Create(filePath))
            {
                //Inicia a escrita do arquivo
                writer.WriteStartDocument();

                foreach (var parent in parents)
                {
                    writer.WriteStartElement(parent);

                    //Cria as propriedades do arquivo dinamicamente usando reflection
                    foreach (var _prop in obj.GetType().GetProperties())
                    {
                        if (_prop.Name.Equals("Id"))
                            continue;

                        else if (!_prop.PropertyType.Name.ToLower().Contains("list") &&
                               !_prop.PropertyType.Name.ToLower().Contains("ienumerable"))
                            writer.WriteElementString(_prop.Name, _prop.GetValue(obj).ToString());

                        //Verifica se a propriedade é um elemento filho
                        if (children.Contains(_prop.Name))
                        {
                            var child = _prop.GetValue(obj);

                            //Verifica se o filho é uma lista
                            if (_prop.PropertyType.Name.ToLower().Contains("list") ||
                                _prop.PropertyType.Name.ToLower().Contains("ienumerable"))
                            {
                                var list = (child as IList);

                                writer.WriteStartElement(_prop.Name);
                                if (!ReferenceEquals(child, null))
                                {
                                    //Cria as propriedades do elemento filho dinamicamente para cada item da lista
                                    foreach (var item in list)
                                    {
                                        foreach (var grandChild in grandChildren)
                                        {
                                            writer.WriteStartElement(grandChild);
                                            foreach (var _propChild in item.GetType().GetProperties())
                                            {
                                                if (_propChild.Name.Equals("Id"))
                                                    continue;

                                                writer.WriteElementString(_propChild.Name, _propChild.GetValue(item)?.ToString());
                                            }
                                            writer.WriteEndElement();
                                        }
                                    }
                                }
                                writer.WriteEndElement();
                            }
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndDocument();
            }
            return filePath;
        }
    }
}
