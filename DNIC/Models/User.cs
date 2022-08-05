using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DNIC.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Relatonship

        public virtual List<UserCourseResult> UserCourseResults { get; set; }
    }
}
