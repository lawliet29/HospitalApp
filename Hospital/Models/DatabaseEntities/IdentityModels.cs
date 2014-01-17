using AspNet.Identity.RavenDB.Entities;
using Hospital.Models.Enums;
using Raven.Imports.Newtonsoft.Json;
namespace Hospital.Models
{
    public class ApplicationUser : RavenUser
    {
        public string FullName { get; set; }
        public Role Role { get; set; }

        [JsonIgnore]
        public int IntId
        {
            get
            {
                return int.Parse(Id.Substring(Id.IndexOf("/") + 1));
            }
        }
    }
}