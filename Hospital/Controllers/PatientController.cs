using Hospital.Models;
using Hospital.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
using Hospital.Models.DatabaseEntities;
using System.Net;
using System.Transactions;

namespace Hospital.Controllers
{
    [Authorize]
    public class PatientController : BaseController
    {
        
        [Route("Patients")]
        public async Task<ActionResult> Index()
        {
            var patients = await DatabaseSession
                .Query<ApplicationUser>()
                .Where(u => u.Role == Role.Patient)
                .ToListAsync();

            var queryTasks = patients
                .Select(async u => new UserViewModel
                {
                    Id = u.IntId,
                    Name = u.FullName,
                    Email = u.UserName,
                    PatientInfo = await DatabaseSession.Query<PatientInfo>().FirstOrDefaultAsync(i => i.UserId == u.Id),
                    CanBeEdited = (CurrentUser.Role == Role.Administrator || CurrentUser.Id == u.Id),
                    Role = u.Role
                });

            var model = new List<UserViewModel>();
            foreach (var task in queryTasks)
                model.Add(await task);

            return View("UserList", model);
        }

        [Route("Patients/{id}")]
        public async Task<ActionResult> View(int id, bool editable = false)
        {
            var user = await DatabaseSession.LoadAsync<ApplicationUser>(id);

            var patientInfo = await DatabaseSession
                .Query<PatientInfo>()
                .Customize(q => q.WaitForNonStaleResultsAsOfLastWrite())
                .FirstOrDefaultAsync(p => p.UserId == user.Id);


            var model = new UserViewModel
            {
                Id = id,
                Name = user.FullName,
                Email = user.UserName,
                PatientInfo = patientInfo,
                CanBeEdited = (CurrentUser.Role == Role.Administrator || CurrentUser.IntId == id),
                Role = user.Role
            };

            model.Editable = editable && model.CanBeEdited;

            return View("UserView", model);
        }

        [Route("Patients/{id}/Edit")]
        public async Task<ActionResult> Edit(int id)
        {
            if (CurrentUser.Role == Role.Administrator || CurrentUser.IntId == id)
            {
                return await View(id, editable: true);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Patients/{id}/Edit")]
        public async Task<ActionResult> Edit(UserViewModel model) 
        {

            if (ModelState.IsValid)
            {
                var user = await DatabaseSession.LoadAsync<ApplicationUser>(model.Id);

                var patientInfo = await DatabaseSession
                    .Query<PatientInfo>()
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                user.FullName = model.Name;
                if (patientInfo == null)
                {
                    patientInfo = new PatientInfo();
                }

                patientInfo.Address = model.PatientInfo.Address;
                patientInfo.BirthdayDate = model.PatientInfo.BirthdayDate;
                patientInfo.UserId = user.Id;
                if (string.IsNullOrEmpty(patientInfo.Id))
                {
                    await DatabaseSession.StoreAsync(patientInfo);
                }

                await DatabaseSession.SaveChangesAsync();

                return RedirectToAction("View", new { id = model.Id });
            }

            model.Editable = true;
            return View("UserView", model);
        }
    }
}