using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Wallet.Presentation.Model
{
    public class RecoveryCoinModel
    {
        public string MnemonicPhrase { get; set; }
        public string WalletName { get; set; }
        public PasswordBox PasswordBox { get; set; }
        public RecoveryCoin RecoveryCoin { get; set; }
    }
}
