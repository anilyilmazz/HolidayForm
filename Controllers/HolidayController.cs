using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IzinFormu.Data;
using IzinFormu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IzinFormu.Controllers
{
    public class HolidayController : Controller
    {
        private UserManager<ApplicationUser> _usermanager;
        private ApplicationDbContext _ctx;

        public HolidayController( ApplicationDbContext _ctx, UserManager<ApplicationUser> _usermanager)
        {
            this._usermanager = _usermanager;
            this._ctx = _ctx;
        }
        [Authorize]
        public IActionResult Index()
        {
            //var query = _ctx.Holiday.Where(a => String.Equals(a.Department, "ar-ge") && a.CreateDate < DateTime.Now);
            //var holidays = query.ToList();
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var myholidaylist = _ctx.Holiday.Where(a=>a.User.Id == user.Id).Select(s => new HolidayViewModel() { CreateDate = s.CreateDate, Department = s.Department, EndDate = s.EndDate, Manager = s.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id , Id = s.Id}).ToList();
            return View(myholidaylist);
        }
        [Authorize]
        public JsonResult GetsMyHoliday()
        {
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var myholidaylist = _ctx.Holiday.Where(a => a.User.Id == user.Id).Select(s => new HolidayViewModel() { CreateDate = s.CreateDate, Department = s.Department, EndDate = s.EndDate, Manager = s.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id, Id = s.Id ,StartDateString =s.StartDate.ToString("dd-MM-yyyy") ,EndDateString = s.EndDate.ToString("dd-MM-yyyy") ,CreateDateString =s.CreateDate.ToString("dd-MM-yyyy"),HolidayTime ="5"}).ToList();
            foreach (var i in myholidaylist)
            {
                int holidaycount = -1;
                int fridaycount = 0;
                for (DateTime friday = i.StartDate; friday <= i.EndDate; friday = friday.AddDays(1))
                {
                    holidaycount++;
                    if (friday.DayOfWeek == DayOfWeek.Friday)
                    {
                        fridaycount++;
                    }

                }
                i.HolidayTime = (holidaycount + fridaycount).ToString();

            }
            return Json(myholidaylist);
        }

        [Authorize]
        public IActionResult HolidayDelete(int Id)
        {
            var deletingholiday = new Holiday { Id = Id };
             _ctx.Holiday.Attach(deletingholiday);
            _ctx.Holiday.Remove(deletingholiday);
            _ctx.SaveChanges();
            //Delete process succesfull but view not found.
            return RedirectToAction("HolidayIndex", "Admin");
        }

        [Authorize]
        public IActionResult HolidayRegister()
        {
            return View();
        }

        [Authorize]
        public IActionResult HolidayEdit(int Id)
        {
            ViewBag.username = _ctx.Holiday.Where(a => a.Id == Id).Select(s => s.User.Name).FirstOrDefault();  
            ViewBag.startdate = _ctx.Holiday.Where(a => a.Id == Id).Select(s => s.StartDate).FirstOrDefault().Date.ToString("yyyy-MM-dd");
            ViewBag.enddate = _ctx.Holiday.Where(a => a.Id == Id).Select(s => s.EndDate).FirstOrDefault().Date.ToString("yyyy-MM-dd");
            ViewBag.createdate = _ctx.Holiday.Where(a => a.Id == Id).Select(s => s.CreateDate).FirstOrDefault().Date.ToString("dd-MM-yyyy");

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult HolidayEdit(HolidayViewModel model, int Id)
        {
            var editingholiday = _ctx.Holiday.SingleOrDefault(a => a.Id == Id);
            if (editingholiday != null)
            {
                editingholiday.StartDate = model.StartDate;
                editingholiday.EndDate = model.EndDate;
                _ctx.SaveChanges();
            }
            return RedirectToAction("HolidayIndex", "Admin");
        }


        [HttpPost]
        [Authorize]
        public IActionResult HolidayRegister(HolidayViewModel model)
        {
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var holiday = new Holiday();
            holiday.CreateDate = DateTime.Now;
            holiday.StartDate = model.StartDate.Date;
            holiday.EndDate = model.EndDate.Date;
            holiday.RequestDate = DateTime.Now;
            holiday.Department = user.Department;
            holiday.Manager = user.Manager;
            holiday.User = user;

            _ctx.Holiday.Add(holiday);
            _ctx.SaveChanges();

            return RedirectToAction("Index", "Holiday");
        }

        [Authorize]
        public IActionResult GetHolidayHtml(int Id)
        {
            //ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var myholidaylist = _ctx.Holiday.Where(a => a.Id == Id).Select(s => new HolidayViewModel() { CreateDate = s.CreateDate, Department = s.Department, EndDate = s.EndDate, Manager = s.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id, Id = s.Id, StartDateString = s.StartDate.ToString("dd-MM-yyyy"), EndDateString = s.EndDate.ToString("dd-MM-yyyy"), CreateDateString = s.CreateDate.ToString("dd-MM-yyyy"), HolidayTime = "5" }).ToList();
            foreach (var i in myholidaylist)
            {
                int holidaycount = -1;
                int fridaycount = 0;
                for (DateTime friday = i.StartDate; friday <= i.EndDate; friday = friday.AddDays(1))
                {
                    holidaycount++;
                    if (friday.DayOfWeek == DayOfWeek.Friday)
                    {
                        fridaycount++;
                    }

                }
                i.HolidayTime = (holidaycount + fridaycount).ToString();

            }
            return View(myholidaylist);
        }

        




    }
}