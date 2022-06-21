using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcSample.ViewModels;
using MvcSample.Core.Interfaces;

namespace MvcSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBrainstormSessionRepository sessionRepository;

        public HomeController(IBrainstormSessionRepository sessionRepository)
        {
            this.sessionRepository = sessionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var sessionList = await sessionRepository.ListAsync();
            var models = sessionList.Select(x=>new StormSessionViewModel{
                Id = x.Id,
                Name = x.Name,
                DateCreated=x.DateCreated,
                IdeaCount = x.Ideas.Count
            });
            return View(models);
        }
        [HttpPost]
        public async Task<IActionResult> Index(StormSessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }else
            {
                await sessionRepository.AddAsync(new Core.Model.BrainstormSession(){
                    DateCreated=model.DateCreated,
                    Name = model.Name
                });
                return RedirectToAction(actionName:nameof(Index));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
