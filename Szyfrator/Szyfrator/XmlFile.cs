using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

namespace Szyfrator
{
    interface IXmlFile
    {
        void CreateXml(ListView receivers, string algorithm, string cipherMode);
        void SaveXml(byte[] encryptedData);
    }
    class XmlFile : IXmlFile
    {
        public string Path { get; set; }
        public string Name { get; set; }

        public XmlFile(string path, string name)
        {
            this.Name = name;
            this.Path = path;
        }

        public void CreateXml(ListView receivers, string algorithm, string cipherMode)
        {
            XmlTextWriter Writer = new XmlTextWriter(this.Path, null);

            Writer.Formatting = Formatting.Indented;
            Writer.Indentation = 4;
            Writer.WriteStartDocument();
            Writer.WriteStartElement("EncryptedFileHeader");

            Writer.WriteStartElement("Algorithm");
            Writer.WriteString(algorithm);
            Writer.WriteEndElement();
            Writer.WriteStartElement("CipherMode");
            Writer.WriteString(cipherMode);
            Writer.WriteEndElement();
            Writer.WriteStartElement("ApprovedUsers");
            foreach (PublicKey rec in receivers.SelectedItems)
            {
                Writer.WriteStartElement("User");
                Writer.WriteStartElement("Name");
                Writer.WriteString(rec.Name);
                Writer.WriteEndElement();
                Writer.WriteEndElement();
                Writer.WriteEndElement();

                Writer.WriteEndElement();
                Writer.WriteEndDocument();
                Writer.Close();

            }
        }

        public void SaveXml(byte[] encryptedData)
        {
            using (StreamWriter writer = new StreamWriter(this.Path, true))
            {
                writer.Write("\n\n");
            }

            using (StreamWriter writer = new StreamWriter(this.Path, true))
            {
                writer.BaseStream.Write(encryptedData, 0, encryptedData.Length);
            }
        }

    }
         
}
