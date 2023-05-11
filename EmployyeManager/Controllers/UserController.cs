using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployyeManager.Models;
using System.Web.Http;
using System.Web.Http.Description;
using BCrypt.Net;
using System.Net.Http.Formatting;

namespace EmployyeManager.Controllers
{
    public class UserController : Controller
    {
        private EmployeManagerDatabaseEntities db = new EmployeManagerDatabaseEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Create() 
        { 
            return View(); 
        }
        public ActionResult Logout()
        {
            // Clear user session
            Session.Clear();
            return RedirectToAction("Login", "User");
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLoginModel UserLoginModel)
        {
            if (ModelState.IsValid)
            {
                string validationError = IsValidUser(UserLoginModel.Email, UserLoginModel.Password);
                if (string.IsNullOrEmpty(validationError))
                {
                    // Set user session
                    int userid = GetUserIDFromDB(UserLoginModel.Email);
                    Session["UserID"] = userid;
                    User user = db.Users.Find(userid);
                    if (user != null)
                    {
                        user.LastLogin = DateTime.Now;
                        db.SaveChanges();
                    }
                    return RedirectToAction("MainPanel", "Home");
                }
                else
                {
                    ModelState.AddModelError("", validationError);
                }
            }
            return View(UserLoginModel);
        }

        private string IsValidUser(string email, string password) 
        {
            string storedHash = GetStoredHashFromDB(email);
            if (!string.IsNullOrEmpty(storedHash))
            {
                return BCrypt.Net.BCrypt.Verify(password, storedHash) ? null : "Invalid Details.";
            }
            else
            return "Invalid Details";
        }
        private string GetStoredHashFromDB(string email)
        {
            User user = db.Users.FirstOrDefault(u => u.EmailAdress == email);
            if (user != null) 
            {
                return user.Password_Hash;
            }
            return null;
        }
        private int GetUserIDFromDB(string email)
        {
            User user = db.Users.FirstOrDefault(u => u.EmailAdress == email);          
            
        
            return user.UserID;                
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserInputModel userInputModel)
        {
            if (ModelState.IsValid)
            {
                
                var existingUser = db.Users.FirstOrDefault(u => u.EmailAdress == userInputModel.Email);

                if (existingUser != null)
                {
                    
                    ModelState.AddModelError("Email", "Email address already in use.");
                    return View(userInputModel);
                }

                var user = new User
                {
                    EmailAdress = userInputModel.Email,
                    Password_Hash = BCrypt.Net.BCrypt.HashPassword(userInputModel.Password),
                    Password_Salt = "test",
                    FirstName = userInputModel.FirstName,
                    LastName = userInputModel.LastName,
                };
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(userInputModel);
        }
    }
}
