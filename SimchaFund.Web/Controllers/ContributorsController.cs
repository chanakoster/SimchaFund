using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using System.Runtime.Intrinsics.X86;

namespace SimchaFund.Web.Controllers
{
    public class ContributorsController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;TrustServerCertificate=true;";

        public IActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            SimchaFundManager mgr = new SimchaFundManager(_connectionString);
            ContributorsViewModel vm = new ContributorsViewModel();
            vm.Contributors = mgr.GetContributors();
            vm.Total = 0;

            foreach (Contributor contributor in vm.Contributors)
            {
                contributor.Balance = mgr.GetContributorBalance(contributor.Id);
                vm.Total += contributor.Balance;
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult New(Contributor contributor, Deposit deposit)
        {
            SimchaFundManager mgr = new SimchaFundManager(_connectionString);
            deposit.ContributorId = mgr.AddContributor(contributor);
            mgr.AddDeposit(deposit);
            TempData["Message"] = $"New Contributor Created! {contributor.Id} {contributor.FirstName} {contributor.LastName} {deposit.Amount}";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Edit(Contributor contributor)
        {
            SimchaFundManager mgr = new SimchaFundManager(_connectionString);
            mgr.UpdateContributor(contributor);
            return RedirectToAction("Index");
        }

        public IActionResult History(int contributorId)
        {
            SimchaFundManager mgr = new SimchaFundManager(_connectionString);
            HistoryViewModel vm = new HistoryViewModel();
            vm.Contributor = mgr.GetContributor(contributorId);
            vm.Contributor.Balance = mgr.GetContributorBalance(contributorId);
            vm.Transactions.AddRange(mgr.GetDepositsByContributor(contributorId));
            vm.Transactions.AddRange(mgr.GetContributionsByContributor(contributorId));
            vm.Transactions = vm.Transactions.OrderByDescending(t => t.Date).ToList();

            return View(vm);
        }

        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            SimchaFundManager mgr = new SimchaFundManager(_connectionString);
            mgr.AddDeposit(deposit);
            return RedirectToAction("Index");
        }
    }
}
