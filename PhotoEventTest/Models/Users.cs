using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhotoEventTest.Models
{
    public partial class Users
    {
        public Users()
        {
            EventUserPhotos = new HashSet<EventUserPhotos>();
        }

        [Display(Name = "User ID")]
        public string UserId { get; set; }
        public string Password { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        
        public bool? IsAdmin { get; set; }

        [Display(Name = "Member Join Date")]
        public DateTime? EntryDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<EventUserPhotos> EventUserPhotos { get; set; }
    }
}
