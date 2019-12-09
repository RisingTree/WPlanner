using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext; 
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Register")]
        public  IActionResult Register(LogRegWrapperModel newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(user => user.Email == newUser.RegUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.RegUser.Password= Hasher.HashPassword(newUser.RegUser, newUser.RegUser.Password);
                    dbContext.Add(newUser.RegUser);
                    dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else 
            {
                return View("Index");
            }
        }
        [HttpPost("Login")]
        public  IActionResult Login(LogRegWrapperModel  confirmUser)
        {
            User UserInDb = dbContext.Users.FirstOrDefault(user => user.Email == confirmUser.LogUser.LoginEmail);
            if (UserInDb == null)
            {
                ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                return View("Index");
            }
            var hasher = new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(confirmUser.LogUser, UserInDb.Password, confirmUser.LogUser.LoginPassword);
            if(result == 0 )
            {
                ModelState.AddModelError("LoginPassword", "Password Doesnt Match");
                return View("Index");
            }
            else
            {
                HttpContext.Session.SetInt32("UserId", UserInDb.UserId);
                return RedirectToAction("Dashboard");
            }
        }
        
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? LoginId = HttpContext.Session.GetInt32("UserId");
            User LoggedInUser = dbContext.Users.FirstOrDefault(user => user.UserId ==LoginId );
            if (LoggedInUser == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                List<Wedding> AllWeddings = dbContext.Weddings.Include(wed => wed.InAttendance).ToList();
                ViewBag.User= LoggedInUser;
                return View("Dashboard", AllWeddings);
            }  
        }
        [HttpGet("NewWedding")]
        public IActionResult NewWedding()
        {

            return View("NewWedding");
        }
        [HttpPost("ProcessWedding")]
        public IActionResult ProcessWedding(Wedding newWedding)
        {
            if(ModelState.IsValid)
            {
                int? UserId = HttpContext.Session.GetInt32("UserId");
                newWedding.UserId = (int)UserId;
                dbContext.Add(newWedding);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("NewWedding");
            }
        }
        [HttpGet("RSVP/{WeddingId}")]
        public IActionResult RSVP(int WeddingId)
        {
                int? UserId = HttpContext.Session.GetInt32("UserId");
                Wedding  ThisWedding = dbContext.Weddings.Include(w=>w.InAttendance).ThenInclude(u => u.User).FirstOrDefault(wed => wed.WeddingId==WeddingId);
                User UserToAdd= dbContext.Users.Include(a => a.WeddingAttending).ThenInclude(b => b.Wedding).FirstOrDefault(use => use.UserId == UserId);
                Guest WantsToRsvp = dbContext.Guests.FirstOrDefault(w => w.WeddingId == WeddingId && w.UserId == UserToAdd.UserId);
                if(WantsToRsvp != null)
                {
                    ThisWedding.InAttendance.Remove(WantsToRsvp);
                }
                else
                {
                    Guest NewGuest = new Guest();
                    
                    NewGuest.UserId = (int)UserId;
                    NewGuest.WeddingId = WeddingId;  
                    dbContext.Add(NewGuest);
                }
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
        }
        [HttpDelete("Delete/{WeddingId}")]
        public IActionResult Delete(int WeddingId)
        {
            if( HttpContext.Session.GetInt32("UserId") != null)
            {
                Wedding WeddingDelete = dbContext.Weddings.FirstOrDefault(w => w.WeddingId == WeddingId);
                dbContext.Weddings.Remove(WeddingDelete);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("Dashboard");
        }
        [HttpGet("{WeddingId}")]
        public IActionResult WeddingInfo(int WeddingId)
        {
            int? LoginId = HttpContext.Session.GetInt32("UserId");
            User LoggedInUser = dbContext.Users.FirstOrDefault(user => user.UserId ==LoginId );
            if (LoggedInUser == null)
            {
                return RedirectToAction("Index");
            }
            else{
            ViewBag.ThisUser= LoggedInUser;
            Wedding  ShowInfo = dbContext.Weddings.Include(w=>w.InAttendance).ThenInclude(u => u.User).FirstOrDefault(wed => wed.WeddingId==WeddingId);
            return View("WeddingInfo",ShowInfo);
            }
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
                public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
