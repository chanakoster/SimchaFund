using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<int> Contributors { get; set; } = new List<int>();
        public List<decimal> Contributions { get; set; } = new List<decimal>();
    }
}
