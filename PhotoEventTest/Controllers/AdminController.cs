using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using PhotoEventTest.Models;
using PhotoEventTest.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace PhotoEventTest.Controllers
{

    [Authorize]
    public class AdminController : Controller
    {
        private PHOTOGRAPHYEVENTContext _context;
        public AdminController(PHOTOGRAPHYEVENTContext ctx)
        {
            _context = ctx;
        }
        public IActionResult Index()
        {
            var result = _context.Events.Include(e => e.EventUserPhotos).AsQueryable<Events>();
            return View(result);
        }

        public IActionResult Create()
        {
            return View(new Events() { IsClosed = false });
        }

        [HttpPost]
        public IActionResult Create(IFormFile newIntroPic, Events newEvent)
        {
            if (ModelState.IsValid)
            {
                if (newIntroPic != null && newIntroPic.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        newIntroPic.CopyTo(ms);
                        byte[] picBytes = ms.ToArray();
                        newEvent.IntroImage = picBytes;
                    }
                }

                _context.Events.Add(newEvent);
                _context.SaveChanges();
                TempData["actionresult"] = $"{newEvent.EventName} has been create.";
                return Redirect(nameof(Index));
            }
            else
            {
                return View(new Events { IsClosed = false });
            }
            
        }

        public IActionResult EventDetail(int eventId)
        {
            var displayingEvent = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            return View(displayingEvent);
        }

        public IActionResult EditEvent(int eventId)
        {
            Events selectedEvent = _context.Events.SingleOrDefault(e => e.EventId == eventId);
            IEnumerable<EventUserPhotos> selectedUserPhotos = _context.EventUserPhotos.Include(e => e.User).Where(e => e.EventId == eventId).AsQueryable<EventUserPhotos>();
            EventPhotoModel editingModel = new EventPhotoModel { Events = selectedEvent, EventUserPhotos = selectedUserPhotos };
            return View(editingModel);
        }

        [HttpPost]
        public IActionResult EditEvent(IFormFile introPicture, EventPhotoModel editingEvent)
        {
            if (ModelState.IsValid)
            {
                if (editingEvent.Events.EventId == 0)
                {
                    _context.Events.Add(editingEvent.Events);
                }
                else
                {
                    Events savingEvent = _context.Events.SingleOrDefault(e => e.EventId == editingEvent.Events.EventId);
                    if (savingEvent != null)
                    {
                        savingEvent.EventName = editingEvent.Events.EventName;
                        savingEvent.EventRule = editingEvent.Events.EventRule;
                        savingEvent.IsClosed = editingEvent.Events.IsClosed;
                        savingEvent.StartDate = editingEvent.Events.StartDate;
                        savingEvent.EndDate = editingEvent.Events.EndDate;
                        if (introPicture != null && introPicture.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                introPicture.CopyTo(ms);
                                byte[] imageBytes = ms.ToArray();
                                savingEvent.IntroImage = imageBytes;
                            }
                        }
                    }
                }
                _context.SaveChanges();
                TempData["actionresult"] = $"{editingEvent.Events.EventName} has been saved";
                return RedirectToAction("Index");
            }
            else
            {
                var returnData = _context.Events.FirstOrDefault(e => e.EventId == editingEvent.Events.EventId);
                return View(returnData);
            }
        }

        [HttpPost]
        public IActionResult DeleteEvent(int eventId)
        {
            Events deletingEvent = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            if (deletingEvent != null)
            {
                _context.Events.Remove(deletingEvent);
                _context.SaveChanges();
                TempData["actionresult"] = $"{deletingEvent.EventName} was deleted";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SetWinner(int eventId, string winnerId)
        {
            Events settingEvent = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            if (settingEvent != null)
            {
                settingEvent.Winner = winnerId;
                _context.SaveChanges();
                TempData["actionresult"] = $"{winnerId} was set to winner of {settingEvent.EventName}.";
            }
            return RedirectToAction("EditEvent", new { eventId });
        }

        public IActionResult UserList(string userId)
        {
            IEnumerable<Users> userList = _context.Users.AsQueryable<Users>();
            return View(userList);
        }

        public IActionResult UserEVentJoinHistory(string uid)
        {
            IEnumerable<UserEventJoinData> prizeWonEvents = from evt in _context.Events.Where(e => e.Winner == uid)
                                                            select new UserEventJoinData { EventName = evt.EventName, EventID = evt.EventId };
            IEnumerable<UserEventJoinData> participatedEvents = from evt in _context.EventUserPhotos.Where(e => e.UserId == uid).Include(e => e.Event)
                                                            select new UserEventJoinData { EventName = evt.Event.EventName, EventID = evt.EventId};

            List<IEnumerable<UserEventJoinData>> result = new List<IEnumerable<UserEventJoinData>>();
            result.Add(prizeWonEvents);
            result.Add(participatedEvents);
            ViewBag.UserId = uid;
            return View(result);
        }
    }
}