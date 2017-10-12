using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using connectingToDBTESTING.Models;
using System.Linq;
using Newtonsoft.Json;
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
        [Route("")]
        public IActionResult Index()
        {
            List<object> Retrieve = HttpContext.Session.GetObjectFromJson<List<object>>("TheList");
            ViewBag.logerr = null;
            ViewBag.errors = Retrieve;
            List<Dictionary<string, object>> User = HttpContext.Session.GetObjectFromJson<List<Dictionary<string, object>>>("cur_user");
            if(User!=null){
                return RedirectToAction("Quotes");
            }
            return View();
        }

        [HttpPost]
        [Route("store")]
        public IActionResult Register(User model)
        {
            if(ModelState.IsValid){
                User CurrentUser = new User(){
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password,
                    Email = model.Email 
                };
                List<Dictionary<string, object>> User = _dbConnector.Query($"INSERT INTO users (first_name, last_name, email, password, age) VALUES ('{model.FirstName}', '{model.LastName}', '{model.Email}', '{model.Password}', 0)");
                HttpContext.Session.SetObjectAsJson("cur_user", User);
                return RedirectToAction("Quotes");
            }else{
                HttpContext.Session.SetObjectAsJson("TheList", ModelState.Values);
                return RedirectToAction("Index");
            }
            
            //List<Dictionary<string, object>> Allq = _dbConnector.Query("SELECT * FROM quotes ORDER BY created_at Desc");
            
        }
        [HttpGet]
        [Route("quotes")]
        public IActionResult Quotes()
        {       
                List<Dictionary<string, object>> User = HttpContext.Session.GetObjectFromJson<List<Dictionary<string, object>>>("cur_user");
                if(User==null){
                    return RedirectToAction("Index");
                }
                List<Dictionary<string, object>> AllMessages = _dbConnector.Query("SELECT * FROM messages JOIN users ON messages.user_id = users.id");
                List<Dictionary<string, object>> AllUsers = _dbConnector.Query("SELECT * FROM users");
                List<Dictionary<string, object>> AllComments = _dbConnector.Query("SELECT * FROM comments JOIN users ON comments.user_id_com = users.id");
                
                @ViewBag.AM = AllMessages;
                @ViewBag.AC = AllComments;
                @ViewBag.AU = AllUsers;
                Console.WriteLine(User[0]["first_name"]);
                    
                @ViewBag.cur_user = User[0]["first_name"];
                List<object> m_errors = HttpContext.Session.GetObjectFromJson<List<object>>("messageerror");
                @ViewBag.m_errors = m_errors;
                return View();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(string email, string password)
        {
            if(email!=null && password!=null){
                List<Dictionary<string, object>> User = _dbConnector.Query($"SELECT * FROM users WHERE email = '{email}'");
                Console.WriteLine(User[0]["first_name"]);
                string pas = (string)User[0]["password"];
                if(pas==password){
                    HttpContext.Session.SetObjectAsJson("cur_user", User);
                    return RedirectToAction("Quotes");
                }
            }
                string errors = "Invalid email or password";
                ViewBag.errors = null;
                ViewBag.logerr = errors;
                return View("Index");
            }
            
            [HttpPost]
            [Route("newmessage")]
            public IActionResult NewMessage(Message me)
            {
                if(ModelState.IsValid){
                    List<Dictionary<string, object>> User = HttpContext.Session.GetObjectFromJson<List<Dictionary<string, object>>>("cur_user");
                    Console.WriteLine(User[0]["first_name"]);
                    _dbConnector.Execute($"INSERT INTO messages (text, user_id) VALUES ('{me.Text}', '{User[0]["id"]}')");
                    HttpContext.Session.SetObjectAsJson("messageerror", null);
                    return RedirectToAction("Quotes");
                }else{
                    HttpContext.Session.SetObjectAsJson("messageerror", ModelState.Values);
                    return RedirectToAction("Quotes");
                }
                
            }
            [HttpPost]
            [Route("newcomment")]
            public IActionResult NewComment(string comment, int mes_id)
            {
                    Console.WriteLine($"{comment} is comment for {mes_id}");
                    List<Dictionary<string, object>> User = HttpContext.Session.GetObjectFromJson<List<Dictionary<string, object>>>("cur_user");
                    Console.WriteLine(User[0]["first_name"]);
                    _dbConnector.Execute($"INSERT INTO comments (text_com, user_id_com, message_id) VALUES ('{comment}', '{User[0]["id"]}', {mes_id})");
                    HttpContext.Session.SetObjectAsJson("messageerror", null);
                    return RedirectToAction("Quotes");
            }
            [HttpGet]
            [Route("logout")]
            public IActionResult logout()
            {
                    HttpContext.Session.SetObjectAsJson("cur_user", null);
                    return RedirectToAction("Index");
            }
                
            
            
        }
    }

public static class SessionExtensions
{
    // We can call ".SetObjectAsJson" just like our other session set methods, by passing a key and a value
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        // This helper function simply serializes theobject to JSON and stores it as a string in session
        session.SetString(key, JsonConvert.SerializeObject(value));
    }
       
    // generic type T is a stand-in indicating that we need to specify the type on retrieval
    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        string value = session.GetString(key);
        // Upon retrieval the object is deserialized based on the type we specified
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}