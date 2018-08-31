using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Core
{
    public class Transaction
    {
        public string Hash { get; set; }
        public string Text { get; set; }
        public decimal Value { get; set; }
        public int Count { get; set; }
    }    
}
