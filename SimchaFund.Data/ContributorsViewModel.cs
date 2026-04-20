using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class ContributorsViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public Simcha simcha { get; set; }
        public Decimal Total { get; set; }
    }
}
