using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecurityDriven.Inferno;
using SecurityDriven.Inferno.Extensions;
using System.IO;
using System.Net.Mime;
using System.Security;
using System.Security.Cryptography;
using HBitcoin.KeyManagement;
using NBitcoin;
using Newtonsoft.Json;
using Rijndael256;

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
            byte[] encryptedAccount;
            try
            {
                encryptedAccount = File.ReadAllBytes($@".\{accoutName}.txt");
            }
            catch (Exception e)
            {
                throw new Exception("Wallet not found.");
            }


            try
            {
                var decryptedBytes = SuiteB.Decrypt(password.ToBytes(), new ArraySegment<byte>(encryptedAccount));

                return decryptedBytes.FromBytes();
            }
            catch (Exception e)
            {
                throw new Exception("Wrong Password");
            }
        }

        public void SaveWalletToJsonFile(Nethereum.HdWallet.Wallet wallet, string password, string pathfile)
        {
            var words = string.Join(" ", wallet.Words);
            var encryptedWords = Rijndael256.Rijndael.Encrypt(words, password, KeySize.Aes256);
            var date = DateTime.Now;
            var walletJsonData = new { encryptedWords = encryptedWords, date = date };
            var json = JsonConvert.SerializeObject(walletJsonData);

            File.WriteAllText(pathfile, json);
        }

        public bool CreateAccount(string password, string accoutName)
        {
            //bitcoin wallet
            var bitcoinPath = System.Environment.CurrentDirectory + $"\\bitcoin{accoutName}.json";
            Safe.Create(out var mnemonic, password, bitcoinPath, Network.TestNet);

            //Ethereum wallet
            var ethereumPath = System.Environment.CurrentDirectory + $"\\ethereum{accoutName}.json";
            var wallet = new Nethereum.HdWallet.Wallet(Wordlist.English, WordCount.Twelve); 
            try
            {
                SaveWalletToJsonFile(wallet, password, ethereumPath);
            }
            catch (Exception e)
            {
            }


            var encryptedMnemonic = Encrypt(mnemonic.ToString(), password);

            File.WriteAllBytes($@".\{accoutName}.txt", encryptedMnemonic);

            return true;
        }

        public void CreateSimpleAccount(string password, string accoutName, string mnemonic)
        {
            var encryptedMnemonic = Encrypt(mnemonic, password);

            File.WriteAllBytes($@".\{accoutName}.txt", encryptedMnemonic);
        }

        public string UnlockAccount(string password, string accountName)
        {
            return Decrypt(password, accountName);
        }
    }
}
