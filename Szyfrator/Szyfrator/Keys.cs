using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Szyfrator
{
    public abstract class Keys
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public RSAParameters Value { get; set; }

        public Keys(string name)
        {
            Name = name;
        }

        public Keys(string name, string path, RSAParameters value)
        {
            Name = name;
            Value = value;
        }

        public abstract void CreateDirectory(string path);
    }

    class PublicKey : Keys
    {
        public PublicKey(string name)
            : base(name) { }

        public PublicKey(string name, string path, RSAParameters value)
            : base(name, path, value)
        {
            path += "\\PublicKeys\\";
            if (!Directory.Exists(path))
            {
                CreateDirectory(path);
            }

            this.Path = path + name;
            
        }

        public override void CreateDirectory(string path)
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }

    class PrivateKey : Keys
    {
        private byte[] _encryptedKey;

        public byte[] EncryptedKey
        {
            get { return _encryptedKey; }
            set { _encryptedKey = value; }
        }

        public PrivateKey(string name)
            : base(name) { }

        public PrivateKey(string name, string path, RSAParameters value)
            : base(name, path, value)
        {
            path += "\\PrivateKeys\\";
            if (!Directory.Exists(path))
            {
                CreateDirectory(path);
            }

            this.Path = path + name + ".priv";
        }

        public override void CreateDirectory(string path)
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }
}
