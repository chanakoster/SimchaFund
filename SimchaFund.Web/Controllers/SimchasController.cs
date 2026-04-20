using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;

namespace SimchaFund.Web.Controllers
{
    public class SimchasController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;TrustServerCertificate=true;";
        public IActionResult Index()
        {
            SimchaFundManager mgr = new SimchaFundManager(_connectionString);
            SimchasViewModel vm = new SimchasViewModel();
            vm.Simchas = mgr.GetSimchas();
            vm.TotalContributors = mgr.GetTotalContributors();
            return View(vm);
        }

        [HttpPost]
        public IActionResult New(Simcha simcha)
        {
            SimchaFundManager mgr = new SimchaFundManager(_connectionString);
            int simchaId = mgr.AddSimcha(simcha);
            var contributors = mgr.GetContributors().Where(c => c.AlwaysInclude).ToList();
            var contributions = new List<Contribution>();
            foreach (Contributor contributor in contributors)
            {
                contributions.Add(new Contribution
                {
                    ContributorId = contributor.Id,
                    SimchaId = simchaId,
                    Amount = 5,
                    Included = true
                });
            }
            mgr.AddContributions(contributions);
            return Redirect("/simchas/index");
        }

        public IActionResult Contributions(int simchaId)
        {
            SimchaFundManager mgr = new SimchaFundManager(_connectionString);
            ContributionViewModel vm = new ContributionViewModel();
            var contributions = mgr.GetContributions(simchaId);
            var contributors = mgr.GetContributors();

            foreach (var contribution in contributions)
            {
                contributors.Where(c => c.Id == contribution.ContributorId).FirstOrDefault().AmountIncluded = contribution.Amount;
                if (contribution.Amount > 0)
                {
                    contributors.Where(c => c.Id == contribution.ContributorId).FirstOrDefault().Included = true;
                }

            }

            foreach (Contributor contributor in contributors)
            {
                contributor.Balance = mgr.GetContributorBalance(contributor.Id);
            }
            vm.Contributors = contributors;
            vm.Simcha = mgr.GetSimcha(simchaId);
            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateContributions(List<Contribution> contributions, int simchaId)
        {
            SimchaFundManager mgr = new SimchaFundManager(_connectionString);
            foreach (var c in contributions)
            {
                c.SimchaId = simchaId;
            }
            mgr.DeleteContributionsBySimcha(simchaId);
            mgr.AddContributions(contributions);
            return Redirect($"/simchas/contributions?simchaId={simchaId}");
        }
    }
}
