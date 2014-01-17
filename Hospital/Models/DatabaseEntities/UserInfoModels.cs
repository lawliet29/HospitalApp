using Hospital.Models.Enums;
using Raven.Imports.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Hospital.Util;

namespace Hospital.Models.DatabaseEntities
{
    public class PatientInfo
    {
        public Dictionary<string, string> IllnessHistory { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Birthday date")]
        public DateTime BirthdayDate { get; set; }
        public string Address { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }

        public PatientInfo()
        {
            IllnessHistory = new Dictionary<string, string>();
        }
    }

    public class DoctorInfo
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        [DisplayName("Qualification")]
        public DoctorType[] Types { get; set; }
        public DoctorTimesheet Timesheet { get; set; }

        public DoctorInfo() {
            Timesheet = new DoctorTimesheet();
        }

        [JsonIgnore]
        [DisplayName("Qualification")]
        public string Qualification
        {
            get
            {
                return new string(string
                    .Concat(Types.Select(t => t.ToString().SplitCamelCase() + ", "))
                    .Reverse()
                    .Skip(2)
                    .Reverse()
                    .ToArray());
            }
        }

        [JsonIgnore]
        [DisplayName("Free next time")]
        public string FreeNextTime
        {
            get
            {
                if (Timesheet.Registrations.Any(e => e.StartTime < DateTime.Now && e.EndTime > DateTime.Now))
                {
                    var futureEntries = Timesheet.Registrations.Where(e => e.StartTime > DateTime.Now);
                    if (!futureEntries.Any())
                    {
                        return Timesheet.Registrations.First(e => e.EndTime > DateTime.Now).EndTime.ToString();
                    }
                    else
                    {
                        return futureEntries.Max(e => e.EndTime).ToString();
                    }
                }
                else
                {
                    return "Now";
                }
            }
        }
    }
}