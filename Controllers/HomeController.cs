using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
namespace connectingToDBTESTING.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbConnector _dbConnector;
 
        public HomeController(DbConnector connect)
        {
            _dbConnector = connect;
        }
        // GET: /Home/
        [HttpGet]
        [Route("delete")]
        public IActionResult Index()
        {
            List<Dictionary<string, object>> Allq = _dbConnector.Query("SELECT * FROM quotes");
            ViewBag.q = Allq;
            return View();
        }
        
        [HttpPost]
        [Route("create")]
        public IActionResult Create(string name)
        {   
            string quote = " ";
            string query = $"INSERT INTO quotes (name, quote) VALUES ('{name}', '{quote}')";
            _dbConnector.Execute(query);
            Console.WriteLine("+++++++++++++++++++++++++++");
            Console.WriteLine(name);
            // Console.WriteLine(quote);
            Console.WriteLine("+++++++++++++++++++++++++++");
            return Json(name);
        }
    }
}
