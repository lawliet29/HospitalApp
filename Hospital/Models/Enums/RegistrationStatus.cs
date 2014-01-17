using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital.Models.Enums
{
    public enum RegistrationStatus
    {
        SentToRegistrator,
        Rejected,
        SentToDoctor,
        RejectedByDoctor,
        Assigned,
        Resolved
    }
}