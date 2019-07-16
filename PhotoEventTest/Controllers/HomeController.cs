using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhotoEventTest.Models;
using PhotoEventTest.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace PhotoEventTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly PHOTOGRAPHYEVENTContext _context; 

        public HomeController(PHOTOGRAPHYEVENTContext ctx)
        {
            _context = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

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

            int width = tmpImg.Width;
            int height = tmpImg.Height;
            string calWidth = "not available";
            try
            {
                calWidth = ((int)((float)width / (float)height * 50)).ToString();
            }
            catch (Exception ex)
            {
                calWidth = ex.Message;
            }

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
