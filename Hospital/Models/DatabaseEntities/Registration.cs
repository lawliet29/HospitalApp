using Hospital.Models.Enums;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Hospital.Util;

namespace Hospital.Models.DatabaseEntities
{
    public class Registration
    {
        public Registration()
        {
            DesiredTypes = new DoctorType[0];
        }
        public string Id { get; set; }
        public DateTime Submitted { get; set; }
        [DisplayName("Date of appointment")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }

        public string DoctorId { get; set; }
        public string UserId { get; set; }

        [DisplayName("Require services of")]
        public DoctorType[] DesiredTypes { get; set; }
        [DisplayName("Symptoms")]
        [Required]
        public string Symptoms { get; set; }

        public string Diagnosis { get; set; }

        public string Resolution { get; set; }

        public RegistrationStatus Status { get; set; }

        public async Task LoadEntitiesAsync(IAsyncDocumentSession databaseSession, bool eagerly = true)
        {
            if (DoctorId != null)
            {
                var doctor = await databaseSession.LoadAsync<ApplicationUser>(DoctorId);
                if (doctor != null) {
                    Doctor = new UserViewModel
                    {
                        Id = doctor.IntId,
                        Email = doctor.UserName,
                        Name = doctor.FullName,
                        DoctorInfo = eagerly 
                            ? await databaseSession.Query<DoctorInfo>().FirstOrDefaultAsync(d => d.UserId == doctor.Id)
                            : null
                    };
                }
            }

            if (UserId != null)
            {
                var user = await databaseSession.LoadAsync<ApplicationUser>(UserId);
                if (user != null)
                {
                    Patient = new UserViewModel
                    {
                        Id = user.IntId,
                        Email = user.UserName,
                        Name = user.FullName,
                        PatientInfo = eagerly
                            ? await databaseSession.Query<PatientInfo>().FirstOrDefaultAsync(d => d.UserId == user.Id)
                            : null
                    };
                }
            }
        }


        [JsonIgnore]
        public UserViewModel Doctor { get; private set; }

        [JsonIgnore]
        public UserViewModel Patient { get; private set; }

        [JsonIgnore]
        public RegistrationMode ViewMode { get; set; }

        [JsonIgnore]
        public int? IntId { get { return Id == null ? null : (int?)int.Parse(Id.Substring(Id.IndexOf("/") + 1)); } }

        [JsonIgnore]
        [DisplayName("Services requested")]
        public string DesiredTypesString 
        {        
            get
            {
                return new string(string
                    .Concat(DesiredTypes.Select(t => t.ToString().SplitCamelCase() + ", "))
                    .Reverse()
                    .Skip(2)
                    .Reverse()
                    .ToArray());
            }
        }

        [JsonIgnore]
        public DateTime? EndTime { get { return StartTime.HasValue ? null : (DateTime?)StartTime.Value.AddHours(2); } }
    }
}