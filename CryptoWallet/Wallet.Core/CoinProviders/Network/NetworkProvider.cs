using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Core.CoinProviders
{
    public class NetworkProvider
    {
        public string DefaultPath { get; set; }
        public Dictionary<string, string> Networks { get; set; }
        public NBitcoin Network { get; set; }
        public NetworkProvider()
        {
            LoadSetting();
        }


        public void SetNetwork(string networkName)
        {
            //Check networks
        }

        public void LoadSetting()
        {

        }
    }
}
