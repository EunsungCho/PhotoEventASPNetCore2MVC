using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhotoEventTest.Models;
using PhotoEventTest.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace PhotoEventTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly PHOTOGRAPHYEVENTContext _context; 

        public HomeController(PHOTOGRAPHYEVENTContext ctx)
        {
            _context = ctx;
        }

        public IActionResult Index()
        {
            return View();
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
