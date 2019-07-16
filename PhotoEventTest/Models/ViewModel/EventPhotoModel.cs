using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PhotoEventTest.Models;
using System.ComponentModel.DataAnnotations;

namespace PhotoEventTest.Models.ViewModel
{
    public class EventPhotoModel
    {
        public Events Events { get; set; }
        public IEnumerable<EventUserPhotos> EventUserPhotos { get; set; }

        public string UserIdToVote { get; set; }
        public string UserIdToVoteName { get; set; }
    }
}
