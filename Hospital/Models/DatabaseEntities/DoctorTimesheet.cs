using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Hospital.Models.DatabaseEntities
{
    public class DoctorTimesheet
    {
        public IList<string> EntryIds { get; set; }

        public DoctorTimesheet()
        {
            EntryIds = new List<string>();
            Registrations = new List<Registration>();
        }

        public async void LoadRegistrationsAsync(IAsyncDocumentSession databaseSession)
        {
            Registrations = await databaseSession.LoadAsync<Registration>(EntryIds);
        }

        [JsonIgnore]
        public IList<Registration> Registrations { get; private set; }
    }
}