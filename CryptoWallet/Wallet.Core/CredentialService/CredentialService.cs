using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityDriven.Inferno;
using SecurityDriven.Inferno.Extensions;
using System.IO;

namespace Wallet.Core.CredentialService
{
    public static class CredentialService
    {
        public static void Encrypt(string privateKey, string password, string walletName)
        {
            var keyBytes = privateKey.ToBytes();
            var passwordBytes = password.ToBytes();
            var encryptedKey = SuiteB.Encrypt(passwordBytes, new ArraySegment<byte>(keyBytes));

            SavePassword(encryptedKey, walletName);
        }

        public static string Decrypt(string password, string walletName)
        {
            var encryptedWallet = File.ReadAllText($@".\{walletName}.txt");
            var decryptedBytes =  SuiteB.Decrypt(password.ToBytes(), new ArraySegment<byte>(encryptedWallet.ToBytes()));

            return decryptedBytes.FromBytes();
        }

        private static void SavePassword(IEnumerable<byte> encrypted, string walletName)
        {
            var text = encrypted.Aggregate("", (current, b) => current + b);

            File.WriteAllText($@".\{walletName}.txt", text);
        }
    }
}
