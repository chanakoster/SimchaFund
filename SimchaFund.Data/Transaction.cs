using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string TransactionMemo { get; set; }
    }
}
