using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Core
{
    public abstract class SendTransaction
    {
        public string Receiver { get; set; }
        public Decimal Amount { get; set; }
        public string Message { get; set; }
    }
}
