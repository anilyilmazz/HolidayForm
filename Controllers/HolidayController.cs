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

        public IActionResult Index()
        {
            //var query = _ctx.Holiday.Where(a => String.Equals(a.Department, "ar-ge") && a.CreateDate < DateTime.Now);
            //var holidays = query.ToList();
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var myholidaylist = _ctx.Holiday.Where(a=>a.User.Id == user.Id).Select(s => new HolidayViewModel() { CreateDate = s.CreateDate, Department = s.Department, EndDate = s.EndDate, Manager = s.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id , Id = s.Id}).ToList();
            return View(myholidaylist);
        }

        public JsonResult GetsMyHoliday()
        {
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var myholidaylist = _ctx.Holiday.Where(a => a.User.Id == user.Id).Select(s => new HolidayViewModel() { CreateDate = s.CreateDate, Department = s.Department, EndDate = s.EndDate, Manager = s.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id, Id = s.Id }).ToList();
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> HolidayRegister(HolidayViewModel model)
        {
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var holiday = new Holiday();
            holiday.CreateDate = DateTime.Now;
            holiday.StartDate = model.StartDate;
            holiday.EndDate = model.EndDate;
            holiday.RequestDate = DateTime.Now;
            holiday.Department = user.Department;
            holiday.Manager = user.Manager;
            holiday.User = user;


            _ctx.Holiday.Add(holiday);
            _ctx.SaveChanges();
            return View();
        }

       
    }
}