using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Deposit : Transaction
    {
        public int Id { get; set; }
        public int ContributorId { get; set; }
    }
}
