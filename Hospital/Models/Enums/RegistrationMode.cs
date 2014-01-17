using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital.Models.Enums
{
    public enum RegistrationMode
    {
        View,
        PatientCreate,
        RegistratorView,
        RegistratorEdit,
        DoctorView,
        DoctorEdit
    }
}