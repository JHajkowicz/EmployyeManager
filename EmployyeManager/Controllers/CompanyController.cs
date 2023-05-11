using EmployyeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployyeManager.Controllers
{
    public class CompanyController : Controller
    {
        private EmployeManagerDatabaseEntities db = new EmployeManagerDatabaseEntities();
        // GET: Company
        public ActionResult Index()
        {
            int userId = Convert.ToInt32(Session["UserID"]); 
            using (var db = new EmployeManagerDatabaseEntities()) 
            {
                var companies = db.Companies.Join(db.CompanyEmployees, c => c.CompanyID, ce => ce.CompanyID,
                (c, ce) => new { Company = c, userId = ce.UserID })
                    .Where(x => x.userId == userId)
                    .Select(x => x.Company)
                    .Distinct()
                    .ToList();
                return View(companies);
            } 
        }

        public ActionResult Create()
        {
            List<Subscription> subscriptionType = db.Subscriptions.ToList();
            ViewBag.SubscriptionTypes = new SelectList(subscriptionType, "SubscriptionID", "SubscriptionName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompanyInputModel CompanyInputModel)
        {
            if (ModelState.IsValid)
            {
                // Map view model data to model
                Company company = new Company
                {
                    CompanyName = CompanyInputModel.CompanyName,
                    WorkerAmount = CompanyInputModel.WorkerAmount,
                    SubscriptionType = CompanyInputModel.SubscriptionType,
                    OwnerID = Convert.ToInt32(Session["UserID"]) // Set OwnerID from session ID
                };
                
                db.Companies.Add(company);
                db.SaveChanges();

                CompanyEmployee companyEmployee = new CompanyEmployee
                {
                    UserID = Convert.ToInt32(Session["UserID"]), // Set OwnerID from session ID
                    RoleID = 1,
                    CompanyID = GetCompanyIDFromDBbyName(CompanyInputModel.CompanyName)
                };
                db.CompanyEmployees.Add(companyEmployee);
                db.SaveChanges();
                return RedirectToAction("Index", "Company");

            }
            List<Subscription> subscriptionTypes = db.Subscriptions.ToList();
            ViewBag.SubscriptionTypes = new SelectList(subscriptionTypes, "SubscriptionID", "SubscriptionName");
            return View(CompanyInputModel);
        }
        public ActionResult CompanyDetails(int? id)
        {

            Company company = db.Companies.FirstOrDefault(c => c.CompanyID == id);
            if (company == null)
            { 
                return View("Error"); 
            }
            var users = db.CompanyEmployees
                      .Where(cu => cu.CompanyID == id)
                      .Select(cu => cu.User)
                      .ToList();
            var roles = db.Roles.ToList();
            var companyEmployees = db.CompanyEmployees.Where(ce => ce.CompanyID == id).ToList();
            var viewModel = new CompanyViewModel
            {
            CompanyID = company.CompanyID,
            CompanyName = company.CompanyName,
            WorkerAmount = company.WorkerAmount,           
            OwnerID = company.OwnerID,
            Users = users,
            Roles = roles,
            CompanyEmployees = companyEmployees 
            };
            return View(viewModel);       
        }


        public ActionResult GenerateInviteLink(string emailAddress, int companyId)
        {
            var user = db.Users.FirstOrDefault(u => u.EmailAdress == emailAddress);
            if (user != null)
            {
                bool isInvited = db.CompanyEmployees
                    .Any(ce => ce.CompanyID == companyId && ce.User.EmailAdress == emailAddress);
                if (isInvited)
                {
                    TempData["ErrorMessage"] = "The user with the given email already exists in the company.";
                    return RedirectToAction("CompanyDetails", new { id = companyId });
                }

                var userId = user.UserID;
                var inviteLink = Url.Action("JoinCompany", "Company", new { companyId, userId }, Request.Url.Scheme);
                var invite = new Invite
                {
                    UserID = userId,
                    CompanyID = companyId,
                    EmailAdressReciver = emailAddress,
                    InviteLink = inviteLink
                };
                db.Invites.Add(invite);
                db.SaveChanges();
                ViewBag.InviteLink = inviteLink; // Store the invite link in ViewBag to display in the view
                return View("InviteLink");
            }

            // Handle case where user is not found
            return View("Error");
        }
        [HttpPost]
        public ActionResult RemoveEmployeeFromCompany(int companyId, int userID)
        {
            var companyEmployee = db.CompanyEmployees.FirstOrDefault(u => u.CompanyID == companyId && u.UserID == userID);
            db.CompanyEmployees.Remove(companyEmployee);
            db.SaveChanges();
            return RedirectToAction("CompanyDetails", new { id = companyId });
        }

        public ActionResult JoinCompany(int userId)
        {
            if(userId == Convert.ToInt32(Session["UserID"])) 
            { 
            var user = db.Users.FirstOrDefault(u => u.UserID == userId);
            var companyId = GetCompanyIdFromInvite(userId);
            var company = db.Companies.FirstOrDefault(c => c.CompanyID == companyId);
            var existingCompanyEmployee = db.CompanyEmployees.FirstOrDefault(ce => ce.UserID == userId && ce.CompanyID == companyId.Value);
              if (existingCompanyEmployee != null)
                {
                    
                    ViewBag.Message = "You are already a part of this company!";
                    return View("JoinCompanyFailed");
                }

                var companyEmployee = new CompanyEmployee

                {
                    UserID = userId,
                    CompanyID = companyId.Value,
                    RoleID = 3
                };
            db.CompanyEmployees.Add(companyEmployee);
            db.SaveChanges();
            ViewBag.Message = "You have successfully joined the company!";
            return View("JoinCompanySuccess");
            }
            else
            {
                ViewBag.Message = "Something is wrong!";
                return View("JoinCompanyFailed");
            }

        }

            private int? GetCompanyIdFromInvite(int userId)
            {         
            var invite = db.Invites.FirstOrDefault(i => i.UserID == userId);
            if (invite != null)
            {
                return invite.CompanyID;
            }
            return null;
            }
        private int GetCompanyIDFromDBbyName(string CompanyName)
        {
            Company Companies = db.Companies.FirstOrDefault(u => u.CompanyName == CompanyName);
            return Companies.CompanyID;
        }      
    }
}
