using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhotoEventTest.Models;
using PhotoEventTest.Models.ViewModel;

using Microsoft.AspNetCore.Authorization;

namespace PhotoEventTest.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly PHOTOGRAPHYEVENTContext _context;

        public EventsController(PHOTOGRAPHYEVENTContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.Where(e => e.IsClosed == true).Include(e => e.EventUserPhotos)
                .ToListAsync());
        }

        public async Task<IActionResult> Details(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var prevEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            IEnumerable<EventUserPhotos> evtUserPhoto = _context.EventUserPhotos.Include(e => e.User).Where(e => e.EventId == eventId);

            var userIdToVote = evtUserPhoto.FirstOrDefault(ev => ev.UserId == User.Identity.Name)?.UserIdToVote;
            string userIdToVoteName = _context.Users.SingleOrDefault(u => u.UserId == userIdToVote)?.FirstName;
            if (prevEvent == null)
            {
                return NotFound();
            }
            EventPhotoModel epModel = new EventPhotoModel { Events = prevEvent, EventUserPhotos = evtUserPhoto, UserIdToVote = userIdToVote, UserIdToVoteName = userIdToVoteName };
            return View(epModel);
        }
    }
}
