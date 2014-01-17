using Hospital.Models.Enums;
using Raven.Client.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hospital.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        public ActionResult Index()
        {
            switch (CurrentUser.Role)
            {
                case Role.Patient: return RedirectToAction("PatientProfile");
                case Role.Doctor: return RedirectToAction("DoctorProfile");
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult PatientProfile()
        {
            return RedirectToAction("View", "Patient", new { id = CurrentUser.IntId });
        }

        public ActionResult DoctorProfile()
        {
            return RedirectToAction("View", "Doctors", new { id = CurrentUser.IntId });
        }
    }
}