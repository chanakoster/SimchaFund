using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class ContributionViewModel
    {
        public List<Contributor> Contributors { get; set; } = new List<Contributor>();
        public Simcha Simcha { get; set; }
    }
}
