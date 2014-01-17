using Hospital.Models.DatabaseEntities;
using Hospital.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public PatientInfo PatientInfo { get; set; }
        public DoctorInfo DoctorInfo { get; set; }
        public bool Editable { get; set; }
        public bool CanBeEdited { get; set; }
        public Role Role { get; set; }
    }
}