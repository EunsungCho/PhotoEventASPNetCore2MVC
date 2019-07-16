using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PhotoEventTest.Models;
using System.ComponentModel.DataAnnotations;

namespace PhotoEventTest.Models.ViewModel
{
    public class ApiEventPhotoModel
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
    }
}
