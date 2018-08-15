using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wallet.Presentation.ViewModel;

namespace Wallet.Presentation.Model
{
    public class NewAccount  : Account
    {
        public string RepeatPassword { get; set; }
    }
}
