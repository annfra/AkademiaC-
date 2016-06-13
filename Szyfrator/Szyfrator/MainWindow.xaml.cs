using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;

namespace Szyfrator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum Algorithms {DES, IDEA, AES}
        Stream inputStream = null;
        Stream outputStream = null;
        ObservableCollection<PublicKey> pubKeys = new ObservableCollection<PublicKey>();
        string keysPath = System.AppDomain.CurrentDomain.BaseDirectory + "Keys";

        public MainWindow()
        {
            InitializeComponent();
            foreach (var alg in Enum.GetValues(typeof(Algorithms)))
                comboBoxAlg.Items.Add(alg);

            if (Directory.Exists(keysPath))
            {
                string[] namesOfKeys = Directory.GetFiles(keysPath + "\\PublicKeys\\")
                .Select(path => System.IO.Path.GetFileName(path))
                .ToArray();

                foreach (string keyName in namesOfKeys)
                    pubKeys.Add(new PublicKey(keyName));
            }
                
            
        }

        private void buttonOpenE_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog window = new OpenFileDialog();
            if (window.ShowDialog() == true)
            {
                inputStream = File.OpenRead(window.FileName);
                textBoxInputE.Text = window.FileName;
            }
            
            if(pubKeys != null)
            {
                this.listViewReceivers.ItemsSource = pubKeys;
            }
            
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            PrivateKey newPrivKey = new PrivateKey(textBoxName.Text, keysPath, rsa.ExportParameters(true));
            PublicKey newPubKey = new PublicKey(textBoxName.Text,keysPath,rsa.ExportParameters(false));
            try
            {
                StreamWriter sw = new StreamWriter(newPubKey.Path);
                sw.Write(rsa.ToXmlString(false));
                sw.Close();

                IBufferedCipher cipherTmp;
                cipherTmp = CipherUtilities.GetCipher("IDEA/ECB/PKCS7Padding");
                byte[] passBytes = System.Text.Encoding.UTF8.GetBytes(passBox.Password.ToString());
                cipherTmp.Init(true, new KeyParameter(passBytes));

                var sw2 = new System.IO.StringWriter();
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                xs.Serialize(sw2, newPrivKey.Value);

                byte[] pivKeyBytes = System.Text.Encoding.UTF8.GetBytes(sw2.ToString());
                newPrivKey.EncryptedKey = cipherTmp.DoFinal(pivKeyBytes);
                FileStream fs = new System.IO.FileStream(newPrivKey.Path, FileMode.Create);
                fs.Write(newPrivKey.EncryptedKey, 0, newPrivKey.EncryptedKey.Length);
                fs.Close();

                pubKeys.Add(newPubKey);

                textBoxName.Text = "";
                passBox.Password = "";
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                textBlockOutput.Text = "Field the blanks";
            }
        }

        private void buttonSaveE_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog window = new SaveFileDialog();

            window.ShowDialog();
            textBoxOutputE.Text = window.FileName;
        }

        private void buttonEncrypt_Click(object sender, RoutedEventArgs e)
        {
            Cipher cipher;
            if (comboBoxAlg.SelectedIndex == (int)Algorithms.DES)
                cipher = new Cipher(64, "DES", comboBoxCipherMode.Text);
            else
                cipher = new Cipher(128, comboBoxAlg.Text, comboBoxCipherMode.Text);
            cipher.Encryption(inputStream, int.Parse(subBlockSize.Content.ToString()));
            XmlFile xmlFile = new XmlFile(textBoxOutputE.Text,"xmlFileEncryption");
            xmlFile.CreateXml(listViewReceivers, comboBoxAlg.Text, comboBoxCipherMode.Text);
            xmlFile.SaveXml(cipher.EncryptedData);

            textBlockOutput.Text = "End of encryption";
        }
    }
}
