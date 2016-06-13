using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szyfrator
{
    public class Cipher
    {
        public string Algorithm { get; set; }
        public int KeySize { get; set; }
        public string CipherMode { get; set; }
        public byte[] EncryptedData { get; set; }
        private byte[] _key;
        private byte[] _ivVector;

        public byte[] IvVector
        {
            get { return _ivVector; }
            set { _ivVector = value; }
        }

        public byte[] Key
        {
          get { return _key; }
          set { _key = value; }
        }


        public Cipher(int keySize, string alg, string cipherMode)
        {
            this.Algorithm = alg;
            this.KeySize = keySize;
            this.CipherMode = cipherMode;
            this.Key = GenerateKey();
        }

        private byte[] GenerateKey()
        {
            CipherKeyGenerator keygen = new CipherKeyGenerator();
            SecureRandom rand = new SecureRandom();
            KeyGenerationParameters keygenParams = new KeyGenerationParameters(rand, this.KeySize);
            keygen.Init(keygenParams);
            return keygen.GenerateKey();
        }

        public void Encryption(Stream inputStream, int subBlockSize)
        {
            bool ifBlock = false;

            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                inputStream.CopyTo(ms);
                data = ms.ToArray();
            }

            IBlockCipher engine = null;

            switch(this.Algorithm)
            {
                case "IDEA":
                    engine = new IdeaEngine();
                    break;
                case "DES":
                    engine = new DesEngine();
                    break;
                case "AES":
                    engine = new AesEngine();
                    break;
            }

            IBufferedCipher cipher = null;
            BufferedBlockCipher blockCipher = null;

            switch (this.CipherMode)
            {
                case "CBC":
                    blockCipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine), new Pkcs7Padding());
                    ifBlock = true;
                    break;
                case "ECB":
                    cipher = CipherUtilities.GetCipher(this.Algorithm + "/ECB/PKCS7Padding");
                    cipher.Init(true, new KeyParameter(this.Key));
                    this.EncryptedData = cipher.DoFinal(data);
                    break;
                case "CFB":
                    blockCipher = new PaddedBufferedBlockCipher(new CfbBlockCipher(engine, subBlockSize), new Pkcs7Padding());
                    ifBlock = true;
                    break;
                case "OFB":
                    blockCipher = new PaddedBufferedBlockCipher(new OfbBlockCipher(engine, subBlockSize), new Pkcs7Padding());
                    ifBlock = true;
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            if (ifBlock)
            {
                ICipherParameters cipherParams = null;

                this.IvVector = new byte[8];
                SecureRandom rand = new SecureRandom();
                rand.NextBytes(this.IvVector);
                KeyParameter kluczParam = new KeyParameter(this.Key);
                cipherParams = new ParametersWithIV(kluczParam, this.IvVector);

                blockCipher.Init(true, cipherParams);
                this.EncryptedData = new byte[blockCipher.GetOutputSize(data.Length)];
                int bytesLength = blockCipher.ProcessBytes(data, 0, data.Length, this.EncryptedData, 0);
                blockCipher.DoFinal(this.EncryptedData, bytesLength);
                ifBlock = false;
            }
        }
            
    }
}
