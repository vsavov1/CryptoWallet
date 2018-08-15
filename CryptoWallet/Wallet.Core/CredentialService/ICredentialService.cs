using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Core.CredentialService
{
    public interface ICredentialService
    {
        bool CreateAccount(string password, string accountName);
        string UnlockAccount(string password, string accountName);
    }
}
