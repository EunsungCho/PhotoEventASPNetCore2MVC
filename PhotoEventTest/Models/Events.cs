using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoEventTest.Models
{
    public partial class Events
    {
        public Events()
        {
            EventUserPhotos = new HashSet<EventUserPhotos>();
        }

        public int EventId { get; set; }

        [Required]
        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        public bool? IsClosed { get; set; }

        public string Winner { get; set; }

        public byte[] IntroImage { get; set; }

        [Display(Name = "Event Description")]
        public string EventRule { get; set; }

        public virtual ICollection<EventUserPhotos> EventUserPhotos { get; set; }
    }
}
