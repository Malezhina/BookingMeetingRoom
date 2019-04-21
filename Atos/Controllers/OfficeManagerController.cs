using Atos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atos.Controllers
{
    public class OfficeManagerController : Controller
    {
        // GET: OfficeManager
        //Переговорные комнаты.
        public ActionResult Index()
        {
            List<MeetingRoom> meetingRooms;

            List<Event> events;

            using (var _context = new ApplicationDbContext())
            {
                meetingRooms = _context.MeetingRooms.ToList();
                events = _context.Events.Where(e => e.IsConfirmed == null).ToList();

                foreach(var ev in events)
                {
                    ev.MeetingRoom = _context.MeetingRooms.Where(m => m.Id == ev.MeetingRoomId).SingleOrDefault();
                }
            }

            var tuple = new Tuple<List<MeetingRoom>, List<Event>>(meetingRooms, events);

            return View(tuple);
        }

        [HttpGet]
        //Управление ролями.
        public ActionResult RoleManagement()
        {
            return View();
        }

        [HttpGet]
        //Создание комнаты.
        public ActionResult CreateRoom()
        {
            return View();
        }

        [HttpPost]
        //Создание комнаты.
        public ActionResult CreateRoom(MeetingRoom meetingRoom)
        {
            if (meetingRoom.NumberOfSeats <= 0)
            {
                ModelState.AddModelError("NumberOfSeats", "Количество мест должно быть > 0");
            }
            if (ModelState.IsValid)
            {
                using (var _context = new ApplicationDbContext())
                {
                    _context.MeetingRooms.Add(meetingRoom);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(meetingRoom);
        }

        [HttpGet]
        //Редактирование комнаты.
        public ActionResult EditRoom(int id)
        {
            MeetingRoom meetingRoom;
            using (var _context = new ApplicationDbContext())
            {
                meetingRoom = _context.MeetingRooms.Where(m => m.Id == id).SingleOrDefault();
            }
            return View(meetingRoom);
        }

        [HttpPost]
        //Редактирование комнаты.
        public ActionResult EditRoom(MeetingRoom meetingRoom)
        {

            if (meetingRoom.NumberOfSeats <= 0)
            {
                ModelState.AddModelError("NumberOfSeats", "Количество мест должно быть > 0");
            }
            if (ModelState.IsValid)
            {
                using (var _context = new ApplicationDbContext())
                {
                    _context.Entry(meetingRoom).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(meetingRoom);
        }

        //Список заявок бронирований на подтверждение.
        public ActionResult BookingListEvents()
        {
            List<Event> events;

            using (var _context = new ApplicationDbContext())
            {
                events = _context.Events.Where(e => e.IsConfirmed == null).ToList();

                foreach (var ev in events)
                {
                    ev.MeetingRoom = _context.MeetingRooms.Where(m => m.Id == ev.MeetingRoomId).SingleOrDefault();
                }
            }

            return PartialView(events);
        }

        //Подтверждение или отклонение заявки.
        public void BookingConfirmation(int idEvent, string confirmation)
        {
            using (var _context = new ApplicationDbContext())
            {
                var eventRoom = _context.Events.Where(e => e.Id == idEvent).SingleOrDefault();
                if (confirmation == "Подтвердить")
                {
                    eventRoom.IsConfirmed = true;
                }
                else
                {
                    eventRoom.IsConfirmed = false;
                }
                _context.Entry(eventRoom).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
        }

        [HttpGet]
        //Удаление комнаты.
        public ActionResult DeleteRoom(int id)
        {
            MeetingRoom meetingRoom;
            using (var _context = new ApplicationDbContext())
            {
                meetingRoom = _context.MeetingRooms.Where(m => m.Id == id).SingleOrDefault();
            }
            return View(meetingRoom);
        }

        [HttpPost]
        //Удаление комнаты.
        public ActionResult DeleteRoom(MeetingRoom meetingRoom)
        {
            using (var _context = new ApplicationDbContext())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var events = _context.Events.Where(e => e.MeetingRoomId == meetingRoom.Id).ToList();

                        meetingRoom = _context.MeetingRooms.Where(m => m.Id == meetingRoom.Id).SingleOrDefault();

                        _context.Events.RemoveRange(events);
                        _context.MeetingRooms.Remove(meetingRoom);

                        _context.SaveChanges();
                        transaction.Commit();
                    }
                    catch(Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}