using Hospital.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Raven.Client;
using Microsoft.Owin.Security;

namespace Hospital.Controllers
{
    public abstract class BaseController : Controller
    {
        private ApplicationUser _currentUser;
        private object _userLock = new object();

        protected ApplicationUser CurrentUser { 
            get 
            {
                if (AuthenticationManager == null || 
                    AuthenticationManager.User == null || 
                    AuthenticationManager.User.Identity == null ||
                    AuthenticationManager.User.Identity.IsAuthenticated == false) {
                    return null;
                }

                if (_currentUser == null)
                {
                    lock (_userLock)
                    {
                        if (_currentUser == null)
                        {
                            _currentUser = _userManager.FindById(AuthenticationManager.User.Identity.GetUserId());
                        }
                    }
                }

                return _currentUser;
            }
        }
        protected IAsyncDocumentSession DatabaseSession { get; private set; }


        private UserManager<ApplicationUser> _userManager;
        public BaseController()
        {
            _userManager = DependencyResolver.Current.GetService<UserManager<ApplicationUser>>();
            DatabaseSession = DependencyResolver.Current.GetService<IAsyncDocumentSession>();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}