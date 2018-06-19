using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IzinFormu.Data;
using IzinFormu.Models;
using IzinFormu.Models.AccountViewModels;
using IzinFormu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IzinFormu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private UserManager<ApplicationUser> _usermanager;
        private ApplicationDbContext _ctx;
        private RoleManager<IdentityRole> _rolemanager;

        public AdminController(UserManager<ApplicationUser> _usermanager, RoleManager<IdentityRole> _rolemanager, ApplicationDbContext _ctx)
        {
            this._usermanager = _usermanager;
            this._rolemanager = _rolemanager;
            this._ctx = _ctx;
        }


        public IActionResult Index()
        {

            var allusers = _ctx.Users;
            return View();
        }

        public IActionResult UserRegister(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserRegister(RegisterViewModel model,string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email,Name = model.Name,Department=model.Department ,Manager=model.Manager,CreateDate=model.CreateDate};
            var result = await _usermanager.CreateAsync(user, model.Password);
            return View();
        }
        public IActionResult UserIndex()
        {      
            return View(_ctx.Users.ToList());
        }

        public IActionResult HolidayIndex()
        {
            return View();
        }

        public JsonResult GetsHoliday()
        {
            var allholidaylist = _ctx.Holiday.Select(s => new HolidayViewModel() {CreateDate = s.CreateDate, Department = s.User.Department, EndDate = s.EndDate, Manager = s.User.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id, Id=s.Id }).ToList();
            return Json(allholidaylist);
        }
    }
}