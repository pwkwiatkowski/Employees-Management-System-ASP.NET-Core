using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ProjektZaliczeniowy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektZaliczeniowy.Controllers
{
    [Authorize(Roles = "Admin, User")] //trzeba być przypisanym do roli Admin lub User
    public class EmployeesController : Controller
    {
        //wielojezykowosc
        private readonly IStringLocalizer<EmployeesController> _localizer;
        /*
        public EmployeesController(IStringLocalizer<EmployeesController> localizer)
        {
            _localizer = localizer;
        }
        */
        //.
        CompanyContext db;
        //drugi argument konstruktora - wielojezykowosc
        public EmployeesController(CompanyContext db, IStringLocalizer<EmployeesController> localizer)
        {
            this.db = db;
            _localizer = localizer;
        }
        [AllowAnonymous] //można anonimowo, bez logowania
        [ResponseCache(Duration = 90)] // <- DO WSTAWIENIA
        public IActionResult Index()
        {
            return View(db.Employee2s.OrderBy(e => e.LoginID));
        }

        //Employees/Details/20
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return View("IDNotFound", id.Value);
            }

            var employee = await db.Employee2s.SingleOrDefaultAsync(e => e.BusinessEntityID == id);
            if (employee == null)
                return View("IDNotFound", id.Value);

            return View(employee);
        }

        //Create
        //get
        [ResponseCache(Duration = 90)] // <- DO WSTAWIENIA
        public IActionResult Create()
        {
            return View();
        }

        //post
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //AntiForgeryToken, zabezpiecza przed Cross-Site Request Forgery
        //zabezpieczenie na forumlarzach, żeby ktoś nie posiadając tokena nie puścił żądania w naszym kontekście bezpieczeństwa, 
        //np. spreparowanym linkiem. Bez tokena nic nie zrobi
        //Cross-Site, czyli próby z innego serwisu wykonania akcji w naszym serwisie.
        public async Task<IActionResult> Create(Employee2 employee)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    db.Employee2s.Add(employee);
                    await db.SaveChangesAsync();
                }
                catch(DbUpdateException ex)
                {
                    throw ex;
                }

                TempData["Info"] = _localizer["Employee added:"] + " " + employee.LoginID;
                return RedirectToAction(nameof(Create));
            }

            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id) //int?, bo null
        {
            if (id == null)
            {
                return View("IDNotFound", id.Value);
            }
            var employee = await db.Employee2s.FindAsync(id);

            if (employee == null)
            {
                //Response.StatusCode = 404;
                return View("IDNotFound", id.Value);
            }

            return View(employee);
        }

        [HttpPost, ActionName("Edit")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, [Bind("BusinessEntityID, LoginID, JobTitle, BirthDate, Gender, HireDate, ModifiedDate")] Employee2 employee) //moze bez binda przejdzie po wszystkich
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Employee2s.Update(employee);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", _localizer["Data editing error"]);
                }
            }
            TempData["Info"] = _localizer["Employee edited:"] + " " + employee.LoginID;
            return View(employee); //dostajemy edytowanego pracownika, a nie niezmienionego
        }

        //get - dostanę z serwera widok
        public IActionResult Delete()
        {
            return View();
        }

        //post - wyślę do serwera requesta z usunięciem
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id) //moze znowu int? bo null
        {
            try
            {
                var employee = await db.Employee2s.FindAsync(id);
                if (employee == null)
                {
                    return View("IDNotFound", id.Value);
                }
                db.Employee2s.Remove(employee);
                await db.SaveChangesAsync();
                //TempData["Info"] = "Usunięto pracownika: " + employee.LoginID; //jest to zbędne, od razu przerzuci nas na Index.cshtml
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException err)
            {
                Console.WriteLine(err);
                return RedirectToAction("Delete", new
                {
                    Id = id,
                    saveChangesError = true
                });
            }
        }
        //akcja ustawiająca język
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
