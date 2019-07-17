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
using Microsoft.AspNetCore.Http;
using System.IO;

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

        // shows previous events data with related EventUserPhotos data to represent the number of participants in each events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.Where(e => e.IsClosed == true).Include(e => e.EventUserPhotos)
                .ToListAsync());
        }

        // retrieves event and related EventUserPhotos, UserIdToVote, UserIdToVoterName to represent to whom logged in user voted.
        public async Task<IActionResult> Details(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            var prevEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            IEnumerable<EventUserPhotos> evtUserPhoto = _context.EventUserPhotos.Include(e => e.User).Where(e => e.EventId == eventId);

            var userIdToVote = evtUserPhoto.FirstOrDefault(ev => ev.UserId == User.Identity.Name)?.UserIdToVote;

            // just gets user's first name
            string userIdToVoteName = _context.Users.SingleOrDefault(u => u.UserId == userIdToVote)?.FirstName;
            if (prevEvent == null)
            {
                return NotFound();
            }
            EventPhotoModel epModel = new EventPhotoModel { Events = prevEvent, EventUserPhotos = evtUserPhoto, UserIdToVote = userIdToVote, UserIdToVoteName = userIdToVoteName };
            return View(epModel);
        }

        // shows current event data
        [Authorize]
        public ViewResult CurrentEvent()
        {
            Events curEvent = _context.Events.SingleOrDefault(e => e.IsClosed == false);
            if (curEvent == null)
            {
                return View(new EventPhotoModel());
            }
            IEnumerable<EventUserPhotos> evtUserPhoto = _context.EventUserPhotos.Include(e => e.User).Where(e => e.EventId == curEvent.EventId);
            var userIdToVote = evtUserPhoto?.FirstOrDefault(ev => ev.UserId == User.Identity.Name)?.UserIdToVote;
            string userIdToVoteName = _context.Users.SingleOrDefault(u => u.UserId == userIdToVote)?.FirstName;

            EventPhotoModel epModel = new EventPhotoModel { Events = curEvent, EventUserPhotos = evtUserPhoto, UserIdToVote = userIdToVote, UserIdToVoteName = userIdToVoteName };
            return View(epModel);
        }

        // when vote button clicked, increase vote score 1 and sets the UserIdToVote field to user id to be voted
        [Authorize]
        public IActionResult UpdateVote(string UserIdToVote, int EventId)
        {
            if (ModelState.IsValid)
            {
                EventUserPhotos tobeVoted = _context.EventUserPhotos.FirstOrDefault(e => e.EventId == EventId && e.UserId == UserIdToVote);
                if (tobeVoted != null)
                {
                    if (tobeVoted.VoteScore == null)
                        tobeVoted.VoteScore = 0;
                    tobeVoted.VoteScore += 1;
                    EventUserPhotos voter = _context.EventUserPhotos.FirstOrDefault(e => e.EventId == EventId && e.UserId == User.Identity.Name);
                    if (voter != null)
                    {
                        voter.UserIdToVote = UserIdToVote;
                        _context.SaveChanges();
                        return RedirectToAction("CurrentEvent");
                    }
                    else
                    {
                        return RedirectToAction("CurrentEvent");
                    }
                }
                else
                {
                    return RedirectToAction("CurrentEvent");
                }
            }
            else
            {
                return RedirectToAction("CurrentEvent");
            }
        }

        // when user participates to current event, user uploads his or her own photo with photo title
        // when saving user's photo, it is also saved with thumbnail size to make a fast showing in detail page.
        [HttpPost]
        public IActionResult ImageUpload(IFormFile files, EventUserPhotos euPhotos)
        {
            if (files == null || files.Length == 0)
            {
                ModelState.AddModelError("", "No file was selected.");
                return RedirectToAction("CurrentEvent");
            }

            Byte[] bytes = null;
            System.Drawing.Image tmpImg = null;

            using (MemoryStream fs = new MemoryStream())
            {
                files.CopyTo(fs);
                bytes = fs.ToArray();
                tmpImg = System.Drawing.Image.FromStream(fs);
            }

            // makes thumbnail photo set to height of 50
            int width = tmpImg.Width;
            int height = tmpImg.Height;
            string calWidth = ((int)((float)width / (float)height * 50)).ToString();

            System.Drawing.Image thumImg = tmpImg.GetThumbnailImage((int)((float)width / (float)height * 50), 50, null, IntPtr.Zero);
            Byte[] thumBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                thumImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                thumBytes = ms.ToArray();
            }
            var eventUserPhoto = _context.EventUserPhotos.SingleOrDefault(e => e.EventId == euPhotos.EventId && e.UserId == User.Identity.Name);
            if (eventUserPhoto != null)
            {
                eventUserPhoto.Photo = bytes;
                eventUserPhoto.ThumbnailPhoto = thumBytes;
            }
            else
            {
                euPhotos.Photo = bytes;
                euPhotos.ThumbnailPhoto = thumBytes;
                _context.EventUserPhotos.Add(euPhotos);
            }
            _context.SaveChanges();
            return RedirectToAction("CurrentEvent");
        }

        public ViewResult JoinEvent(int curEventId)
        {
            return View(new EventUserPhotos() { EventId = curEventId, UserId = User.Identity.Name });
        }
    }
}
