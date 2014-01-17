using Hospital.Models;
using Hospital.Models.DatabaseEntities;
using Hospital.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
using System.Net;

namespace Hospital.Controllers
{
    [Authorize]
    public class DoctorsController : BaseController
    {
        [Route("Doctors")]
        public async Task<ActionResult> Index()
        {
            var patients = await DatabaseSession
                .Query<ApplicationUser>()
                .Where(u => u.Role == Role.Doctor)
                .ToListAsync();

            var queryTasks = patients
                .Select(async u => new UserViewModel
                {
                    Id = u.IntId,
                    Name = u.FullName,
                    Email = u.UserName,
                    DoctorInfo = await DatabaseSession.Query<DoctorInfo>().FirstOrDefaultAsync(i => i.UserId == u.Id),
                    CanBeEdited = (CurrentUser.Role == Role.Administrator || CurrentUser.Id == u.Id),
                    Role = u.Role
                });

            var model = new List<UserViewModel>();
            foreach (var task in queryTasks)
                model.Add(await task);

            return View("UserList", model);
        }

        [Route("Doctors/{id}")]
        public async Task<ActionResult> View(int? id, bool editable = false)
        {
            if (id.HasValue == false)
            {
                return View("Error", "Error");
            }

            var user = await DatabaseSession.LoadAsync<ApplicationUser>(id.Value);

            if (user == null)
            {
                return View("Error", "Doctor not found");
            }

            var doctorInfo = await DatabaseSession
                .Query<DoctorInfo>()
                .Customize(q => q.WaitForNonStaleResultsAsOfLastWrite())
                .FirstOrDefaultAsync(p => p.UserId == user.Id);


            var model = new UserViewModel
            {
                Id = id.Value,
                Name = user.FullName,
                Email = user.UserName,
                DoctorInfo = doctorInfo,
                CanBeEdited = (CurrentUser.Role == Role.Administrator || CurrentUser.IntId == id),
                Role = Role.Doctor
            };

            model.Editable = editable && model.CanBeEdited;

            return View("UserView", model);
        }

        [Route("Doctors/{id}/Edit")]
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
        [Route("Doctors/{id}/Edit")]
        public async Task<ActionResult> Edit(UserViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await DatabaseSession.LoadAsync<ApplicationUser>(model.Id);

                var doctorInfo = await DatabaseSession
                    .Query<DoctorInfo>()
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                user.FullName = model.Name;
                if (doctorInfo == null)
                {
                    doctorInfo = new DoctorInfo();
                }

                doctorInfo.Types = model.DoctorInfo.Types;
                doctorInfo.UserId = user.Id;
                if (string.IsNullOrEmpty(doctorInfo.Id))
                {
                    await DatabaseSession.StoreAsync(doctorInfo);
                }

                await DatabaseSession.SaveChangesAsync();

                return RedirectToAction("View", new { id = model.Id });
            }

            model.Editable = true;
            return View("UserView", model);
        }


        [Route("Appointments/{id?}")]
        public async Task<ActionResult> Appointments(int? doctorId)
        {
            if (doctorId == null && CurrentUser.Role != Role.Doctor)
            {
                return View("Error", "You don't have your own appointments session");
            }

            int id = doctorId.HasValue ? doctorId.Value : CurrentUser.IntId;

            var doctor = await DatabaseSession.LoadAsync<ApplicationUser>(id);

            var registrations = await DatabaseSession
                .Query<Registration>()
                .Where(r => r.DoctorId == doctor.Id)
                .ToListAsync();

            foreach (var reg in registrations)
            {
                await reg.LoadEntitiesAsync(DatabaseSession, eagerly: false);
            }

            ViewBag.Title = "Appointments";
            return View("~/Views/Registration/Manage.cshtml", registrations);
        }
    }
}