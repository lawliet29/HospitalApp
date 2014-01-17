using Hospital.Models.DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Linq;
using System.Web.Mvc.Async;

namespace Hospital.Controllers
{
    public class HomeController : BaseController
    {
        private IDocumentSession _syncDatabaseSession;

        public HomeController(IDocumentSession syncDatabaseSession)
        {
            _syncDatabaseSession = syncDatabaseSession;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UserActions()
        {
            if (CurrentUser == null)
                return new EmptyResult();

            switch (CurrentUser.Role)
            {
                case Models.Enums.Role.Patient:
                {
                    int count = _syncDatabaseSession
                        .Query<Registration>()
                        .Count(r => r.UserId == CurrentUser.Id && 
                            r.Status == Models.Enums.RegistrationStatus.Assigned);


                    return PartialView("_PatientActions", model: count);
                }

                case Models.Enums.Role.Registrator:
                {
                    int count = _syncDatabaseSession
                        .Query<Registration>()
                        .Count(r => r.Status == Models.Enums.RegistrationStatus.SentToRegistrator
                        || r.Status == Models.Enums.RegistrationStatus.RejectedByDoctor);


                    return PartialView("_RegistratorActions", model: count);
                }

                case Models.Enums.Role.Doctor:
                {
                    int count = _syncDatabaseSession
                        .Query<Registration>()
                        .Count(r => r.Status == Models.Enums.RegistrationStatus.SentToDoctor);

                    return PartialView("_DoctorActions", count);
                }

                default:
                {
                    return new EmptyResult();
                }
            }

        }

        protected override void HandleUnknownAction(string actionName)
        {
            if (actionName.StartsWith("_"))
            {
                var asyncActionName = actionName.Substring(1, actionName.Length - 1);
                RouteData.Values["action"] = asyncActionName;

                var controllerDescriptor = new ReflectedAsyncControllerDescriptor(this.GetType());
                var actionDescriptor = controllerDescriptor.FindAction(ControllerContext, asyncActionName)
                    as AsyncActionDescriptor;

                if (actionDescriptor != null)
                {
                    AsyncCallback endDelegate = delegate(IAsyncResult asyncResult)
                    {

                    };

                    IAsyncResult ar = actionDescriptor.BeginExecute(ControllerContext, RouteData.Values, endDelegate, null);
                    var actionResult = actionDescriptor.EndExecute(ar) as ActionResult;

                    if (actionResult != null)
                    {
                        actionResult.ExecuteResult(ControllerContext);
                    }
                }
            }
            else
            {
                base.HandleUnknownAction(actionName);
            }
        }
    }
}