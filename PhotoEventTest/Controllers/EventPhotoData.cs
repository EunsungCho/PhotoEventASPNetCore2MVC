using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using PhotoEventTest.Models;
using PhotoEventTest.Models.ViewModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PhotoEventTest.Controllers
{
    [Route("api/[controller]")]
    public class EventPhotoData : Controller
    {
        private PHOTOGRAPHYEVENTContext _context;
        public EventPhotoData(PHOTOGRAPHYEVENTContext ctx) => _context = ctx;

        [HttpPost]
        public byte[] Post(int eventId, string userId)
        {
            EventUserPhotos evtPhoto = _context.EventUserPhotos.SingleOrDefault(e => (e.EventId == eventId) && (e.UserId == userId));
            
            if (evtPhoto == null)
                return null;
            return evtPhoto.Photo;
        }

    }
}
