using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class HistoryViewModel
    {
        public Contributor Contributor { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
