using Hospital.Models.DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Linq;
using Hospital.Models.Enums;
using Hospital.Models;

namespace Hospital.Controllers
{
    [Authorize]
    public class RegistrationController : BaseController
    {
        public async Task<ActionResult> New()
        {
            if (CurrentUser.Role != Role.Patient)
            {
                return View("Error", model: string.Format("As a {0} you are unable to post new registrations.", CurrentUser.Role.ToString()));
            }

            var model = new Registration
            {
                UserId = CurrentUser.Id
            };

            await model.LoadEntitiesAsync(DatabaseSession, eagerly: false);
            model.ViewMode = RegistrationMode.PatientCreate;
            return View("Registration", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> New(Registration model)
        {
            if (ModelState.IsValid)
            {
                model.Status = RegistrationStatus.SentToRegistrator;
                model.Submitted = DateTime.Now;
                await DatabaseSession.StoreAsync(model);
                await DatabaseSession.SaveChangesAsync();
                return RedirectToAction("List");
            }

            model.ViewMode = RegistrationMode.PatientCreate;
            return View("Registration", model);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var item = await DatabaseSession.LoadAsync<Registration>(id);
            if (CurrentUser.Role == Role.Doctor || 
                (CurrentUser.Role == Role.Patient && item.UserId != CurrentUser.Id)) 
            {
                return View("Error", model: "You don't have permission to do that");
            }

            if (item != null)
            {
                DatabaseSession.Delete(item);
                await DatabaseSession.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }

        [Route("Registrations/")]
        public async Task<ActionResult> List()
        {
            var model = await DatabaseSession
                .Query<Registration>()
                .Customize(q => q.WaitForNonStaleResultsAsOfLastWrite())
                .Where(r => r.UserId == CurrentUser.Id).ToListAsync();
            foreach (var registration in model)
            {
                await registration.LoadEntitiesAsync(DatabaseSession, eagerly: false);
            }
            return View(model);
        }

        public async Task<ActionResult> View(int id)
        {
            var registration = await DatabaseSession.LoadAsync<Registration>(id);
            if (registration == null)
            {
                return View("Error", "Registration not found");
            }

            ViewBag.CanEdit = CurrentUser.Role == Role.Registrator || CurrentUser.Role == Role.Administrator;

            await registration.LoadEntitiesAsync(DatabaseSession, eagerly: true);
            registration.ViewMode = RegistrationMode.View;

            if (CurrentUser.Role == Role.Registrator && 
                (registration.Status == RegistrationStatus.SentToRegistrator || registration.Status == RegistrationStatus.RejectedByDoctor))
            {
                registration.ViewMode = RegistrationMode.RegistratorView;
            }
            else if (CurrentUser.Role == Role.Doctor)
            {
                registration.ViewMode = RegistrationMode.DoctorView;
            }

            return View("Registration", registration);
        }

        public async Task<ActionResult> Manage()
        {
            if (CurrentUser.Role == Role.Doctor ||
                CurrentUser.Role == Role.Patient)
            {
                return View("Error", model: "You don't have permission to do that");
            }

            var model = await DatabaseSession
                .Query<Registration>()
                .ToListAsync();

            foreach (var entry in model)
            {
                await entry.LoadEntitiesAsync(DatabaseSession, eagerly: false);
            }

            return View(model);
        }

        public async Task<ActionResult> Reject(int id)
        {
            if (CurrentUser.Role != Role.Registrator && CurrentUser.Role != Role.Doctor) {
                return View("Error", model: "You don't have permission to do that");
            }
            var registration = await DatabaseSession.LoadAsync<Registration>(id);
            if (registration != null)
            {
                registration.Status = CurrentUser.Role == Role.Doctor ? RegistrationStatus.RejectedByDoctor : RegistrationStatus.Rejected;
            }
            await DatabaseSession.StoreAsync(registration);
            await DatabaseSession.SaveChangesAsync();
            return RedirectToAction("View", new { id = id });
        }

        public async Task<ActionResult> Approve(int id)
        {
            if (CurrentUser.Role != Role.Doctor)
            {
                return View("Error", model: "You don't have permission to do that");
            }
            var registration = await DatabaseSession.LoadAsync<Registration>(id);
            if (registration != null)
            {
                registration.Status = RegistrationStatus.Assigned;
            }
            await DatabaseSession.StoreAsync(registration);
            await DatabaseSession.SaveChangesAsync();
            return RedirectToAction("View", new { id = id });
        }

        public async Task<ActionResult> Register(int id)
        {
            if (CurrentUser.Role != Role.Registrator)
            {
                return View("Error", model: string.Format("As a {0} you are unable to assign registrations.", CurrentUser.Role.ToString()));
            }

            var model = await DatabaseSession.LoadAsync<Registration>(id);
            if (model == null)
            {
                return View("Error", model: "Registration not found");
            }

            await model.LoadEntitiesAsync(DatabaseSession, eagerly: false);
            model.ViewMode = RegistrationMode.RegistratorEdit;
            ViewBag.Doctors = await DatabaseSession.Query<ApplicationUser>().Where(u => u.Role == Role.Doctor).ToListAsync();
            return View("Registration", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Registration model)
        {
            if (ModelState.IsValid)
            {
                bool overlapping = (await DatabaseSession.Query<Registration>()
                    .Where(r => r.DoctorId == model.DoctorId)
                    .ToListAsync())
                    .Select(r => r.StartTime - model.StartTime)
                    .Any(ts => ts.HasValue && ts.Value.Hours >= 2);

                if (overlapping)
                {
                    ModelState.AddModelError("StartTime", "Specified time is already filled for this doctor");
                }
                else
                {
                    if (string.IsNullOrEmpty(model.DoctorId))
                    {
                        return View("Error", "Error");
                    }
                    var registration = await DatabaseSession.LoadAsync<Registration>(model.IntId);
                    registration.DoctorId = model.DoctorId;
                    registration.StartTime = model.StartTime;
                    registration.Status = RegistrationStatus.SentToDoctor;
                    await DatabaseSession.StoreAsync(registration);
                    await DatabaseSession.SaveChangesAsync();
                    return RedirectToAction("View", new { id = model.IntId });
                }
            }

            model.ViewMode = RegistrationMode.RegistratorEdit;
            return View("Registration", model);
        }
    }
}