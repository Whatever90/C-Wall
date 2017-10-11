using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
namespace connectingToDBTESTING.Controllers
{
    public class QuotesController : Controller
    {
        // GET: /Home/
        private readonly DbConnector _dbConnector;
 
        public QuotesController(DbConnector connect)
        {
            _dbConnector = connect;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Quotes()
        {
            List<Dictionary<string, object>> Allq = _dbConnector.Query("SELECT * FROM quotes ORDER BY created_at Desc");
            ViewBag.q = Allq;
            return View();
        }

        [HttpGet]
        [Route("gimmeall")]
        public JsonResult gimmeall()
        {
            List<Dictionary<string, object>> Allq = _dbConnector.Query("SELECT * FROM quotes ORDER BY created_at Desc");
            return Json(Allq);
        }
        [HttpGet]
        [Route("gimme/{id}")]
        public JsonResult gimmeall(int id)
        {
            Console.WriteLine("+++++++++++++++++++++++++++++");
            Console.WriteLine($"++++++++++++++{id}+++++++++++++");
            Console.WriteLine("+++++++++++++++++++++++++++++");
            List<Dictionary<string, object>> Allq = _dbConnector.Query($"SELECT * FROM quotes WHERE id = {id}");
            return Json(Allq);
        }
        [HttpPost]
        [Route("update/{id}")]
        public JsonResult update(int id, string quote)
        {
            Console.WriteLine("+++++++++++++++++++++++++++++");
            Console.WriteLine(quote);
            Console.WriteLine($"+++++++++++++++++++++++++++");
            Console.WriteLine($"++++++++++++++++++++++++++");
            int y = 1;
            _dbConnector.Execute($"UPDATE quotes SET quote = '{quote}' WHERE id = {id}");
            return Json(y);
        }
        [HttpGet]
        [Route("delete/{id}")]
        public JsonResult delete(int id)
        {
            Console.WriteLine("+++++++++++++++++++++++++++++");
            Console.WriteLine($"++++++++++++++{id}+++++++++++++");
            Console.WriteLine("+++++++++++++++++++++++++++++");
            _dbConnector.Execute($"DELETE FROM quotes WHERE id = {id}");
            return Json(id);
        }
    }
}