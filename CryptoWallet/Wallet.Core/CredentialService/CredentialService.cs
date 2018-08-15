using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityDriven.Inferno;
using SecurityDriven.Inferno.Extensions;
using System.IO;
using System.Security;
using HBitcoin.KeyManagement;
using NBitcoin;

namespace Wallet.Core.CredentialService
{
    public class CredentialService : ICredentialService
    {
        private byte[] Encrypt(string text, string password)
        {
            var keyBytes = text.ToBytes();
            var passwordBytes = password.ToBytes();
            var encryptedKey = SuiteB.Encrypt(passwordBytes, new ArraySegment<byte>(keyBytes));

            return encryptedKey;
        }

        private string Decrypt(string password, string accoutName)
        {
            var encryptedAccount = File.ReadAllBytes($@".\{accoutName}.txt");
            var decryptedBytes = SuiteB.Decrypt(password.ToBytes(), new ArraySegment<byte>(encryptedAccount));

            return decryptedBytes.FromBytes();
        }


        public bool CreateAccount(string password, string accoutName)
        {
            Safe.Create(out var mnemonic, password, "./bitcoin" + accoutName + ".json", Network.TestNet);

            var encryptedMnemonic = Encrypt(mnemonic.ToString(), password);

            File.WriteAllBytes($@".\{accoutName}.txt", encryptedMnemonic);

            return true;
        }

        public string UnlockAccount(string password, string accountName)
        {
            try
            {
                return Decrypt(password, accountName);
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
