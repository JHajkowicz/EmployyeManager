using EmployyeManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployyeManager.Controllers
{
    public class CaseRequestsController : Controller
    {

        private EmployeManagerDatabaseEntities db = new EmployeManagerDatabaseEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DisplayEmployeesLeaves(int companyId)
        {
            var today = DateTime.Today;

            var usersOnLeave = db.LeaveCaseAllocations
                .Where(lca => lca.CompanyID == companyId && lca.EndTime >= DateTime.Today)
                .Select(lca => new UserWithLeaveDates
                {
                    UserID = lca.User.UserID,
                    FirstName = lca.User.FirstName,
                    LastName = lca.User.LastName,
                    EmailAdress = lca.User.EmailAdress,
                    StartDate = lca.StartTime,
                    EndDate = lca.EndTime
                })
                .Distinct()
                .ToList();

            var usersReturnedFromLeave = db.LeaveCaseAllocations
                .Where(lca => lca.CompanyID == companyId && lca.EndTime < DateTime.Today)
                .GroupBy(lca => lca.UserID)
                .Select(g => g.OrderByDescending(lca => lca.EndTime).FirstOrDefault())
                .Select(lca => new UserWithLeaveDates
                {
                    UserID = lca.User.UserID,
                    FirstName = lca.User.FirstName,
                    LastName = lca.User.LastName,
                    EmailAdress = lca.User.EmailAdress,
                    StartDate = lca.StartTime,
                    EndDate = lca.EndTime
                })
                .ToList();

            var viewModel = new OnLeaveViewModel
            {
                UsersOnLeave = usersOnLeave,
                UsersReturnedFromLeave = usersReturnedFromLeave,
                CompanyId = companyId

            };

            return View(viewModel);
        }

        public ActionResult GetLeaveCaseAllocations(int? id)
        {
            var leaveCaseAllocations = db.LeaveCaseAllocations
                .Where(lca => lca.CompanyID == id)
                .ToList();

            var events = leaveCaseAllocations.Select(l => new
            {
                title = GetLeaveCaseTitle(l.UserID, l.StartTime, l.EndTime),
                start = l.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = l.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                color = "blue",
                className = "fc-event-blue"
            });

            return Json(events, JsonRequestBehavior.AllowGet);
        }

        private string GetLeaveCaseTitle(int userId, DateTime startDate, DateTime endDate)
        {
            var user = db.Users.FirstOrDefault(u => u.UserID == userId);
            if (user == null)
            {
                return " on leave";
            }

            var overlappingCases = db.LeaveCaseAllocations
                .Where(lca => lca.UserID == userId && lca.StartTime < endDate && startDate < lca.EndTime)
                .ToList();

            var overlappingUserNames = overlappingCases
                .Select(l => $"{db.Users.FirstOrDefault(u => u.UserID == l.UserID)?.FirstName} {db.Users.FirstOrDefault(u => u.UserID == l.UserID)?.LastName}")
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct();

            var userNames = string.Join(", ", overlappingUserNames);

            return $"{userNames} on leave";
        }
        public ActionResult ListCaseRequests(int companyId)
        {
            var caseRequests = db.CaseRequests
                         .Where(c => c.CompanyID == companyId)
                         .Join(db.CompanyEmployees,
                               c => new { c.CompanyID, c.UserID },
                               e => new { e.CompanyID, e.UserID },
                               (c, e) => new { c, e })
                         .Join(db.Users,
                               ce => ce.e.UserID,
                               u => u.UserID,
                               (ce, u) => new { ce, u })
                         .Join(db.LeaveCases,
                               ceu => ceu.ce.c.CaseType,
                               lc => lc.LeaveCaseID,
                               (ceu, lc) => new CaseViewModel
                               {
                                   CaseID = ceu.ce.c.CaseID,
                                   CompanyID = ceu.ce.c.CompanyID,
                                   FirstName = ceu.u.FirstName,
                                   LastName = ceu.u.LastName,
                                   CaseType = lc.LeaveName,
                                   StartDate = ceu.ce.c.StartDate,
                                   EndDate = ceu.ce.c.EndDate
                               })
                         .ToList();

            return View(caseRequests);
        }
        public ActionResult AcceptRequest( int id)
        {
            var caseRequest = db.CaseRequests.Find(id);

            if (caseRequest == null)
            {
                return HttpNotFound();
            }
            var ApprovedByID = (int)Session["UserId"];
            var leaveAllocation = new LeaveCaseAllocation
            {
                CompanyID = caseRequest.CompanyID,
                UserID = caseRequest.UserID,
                StartTime = caseRequest.StartDate,
                EndTime = caseRequest.EndDate,       
                ApprovedBy = ApprovedByID
            };

            db.LeaveCaseAllocations.Add(leaveAllocation);
            db.CaseRequests.Remove(caseRequest);
            db.SaveChanges();

            return RedirectToAction("ListCaseRequests", new { companyId = caseRequest.CompanyID });
        }
        public ActionResult RejectRequest(int id)
        {
            var caseRequest = db.CaseRequests.Find(id);
            int TemporaryCompanyID = caseRequest.CompanyID;
            db.CaseRequests.Remove(caseRequest);
            db.SaveChanges();
            return RedirectToAction("ListCaseRequests", new { companyId = TemporaryCompanyID });
        }


        public ActionResult RequestLeave(int companyId)
        {
            // Get the currently logged in user's ID from the session
            int userId = (int)Session["UserId"];

            
            var viewModel = new CaseRequestViewModel
            {
                CompanyID = companyId,
                UserID = (int)Session["UserId"],

                CaseTypes = db.LeaveCases.Select(ct => new SelectListItem
                {
                    Value = ct.LeaveCaseID.ToString(),
                    Text = ct.LeaveName
                })
            };

            
            var allocations = db.LeaveCaseAllocations.Where(a => a.UserID == userId).ToList();
            var events = new FullCalendarEvent[allocations.Sum(a => (a.EndTime.Date - a.StartTime.Date).Days + 1)];
            int i = 0;
            foreach (var allocation in allocations)
            {
                for (var day = allocation.StartTime.Date; day <= allocation.EndTime.Date; day = day.AddDays(1))
                {
                    events[i] = new FullCalendarEvent
                    {
                        Title = "Leave",
                        Start = day,
                        End = day.AddHours(23).AddMinutes(59), 
                        AllDay = true,
                        ClassName = "blue" 
                    };
                    i++;
                }
            }
            viewModel.LeaveAllocations = events;

            return View(viewModel);
        }
        public ActionResult Create(CaseRequestViewModel viewModel)
        {
            ViewBag.CaseTypes = db.LeaveCases.Select(ct => new SelectListItem
            {
                Value = ct.LeaveCaseID.ToString(),
                Text = ct.LeaveName
            });

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
   
            var user = (int)Session["UserId"];
            var companyId = viewModel.CompanyID;
            var caseType = viewModel.CaseType;

            // Create a new case request view model
            var caseRequest = new CaseRequest
            {
                CompanyID = companyId,
                UserID = user,
                CaseType = caseType,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate
            };

            db.CaseRequests.Add(caseRequest);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

    }

 }   
