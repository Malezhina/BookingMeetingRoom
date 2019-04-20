using Atos.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atos.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        //Отображение списка комнат переговоров.
        [HttpGet]
        public ActionResult Index()
        {
            List<MeetingRoomHelper> roomHelperList = new List<MeetingRoomHelper>();

            using (var _context = new ApplicationDbContext())
            {
                var roomList = _context.MeetingRooms.ToList();

                foreach (var room in roomList)
                {
                    var meetingRoomHelper = new MeetingRoomHelper
                    {
                        Id = room.Id,
                        NameRoom = room.NameRoom,
                        Description = room.Description,
                        NumberOfSeats = room.NumberOfSeats,
                        MarkerBoard = room.MarkerBoard,
                        Projector = room.Projector
                    };
                    //Первое мероприятие для комнаты.
                    try
                    {
                        var firstEvent = _context.Events.Where(e => e.MeetingRoomId == room.Id && e.StartEvent > DateTime.Now)
                       .OrderBy(e => e.StartEvent).First();

                        meetingRoomHelper.StartFirstEvent = firstEvent.StartEvent;
                        meetingRoomHelper.StopFirstEvent = firstEvent.StopEvent;
                    }
                    catch(InvalidOperationException)
                    {
                        meetingRoomHelper.StartFirstEvent = new DateTime();
                        meetingRoomHelper.StopFirstEvent = new DateTime();
                    }
                   

                    roomHelperList.Add(meetingRoomHelper);
                }
            }
            return View(roomHelperList);
        }

        [HttpGet]
        //Открытие экрана с описанием комнаты.
        public ActionResult OpenRoom(int? id)
        {
            MeetingRoom meetingRoom;
            List<Event> events;
            Tuple<MeetingRoom, List<Event>> tuple;
            using (var _context = new ApplicationDbContext())
            {
                meetingRoom = _context.MeetingRooms.Where(m => m.Id == id).SingleOrDefault();
                events = _context.Events.Where(e => e.MeetingRoomId == id && e.StartEvent >= DateTime.Now).ToList();
            }
            tuple = new Tuple<MeetingRoom, List<Event>>(meetingRoom, events);

            return View(tuple);
        }

        [HttpPost]
        //Отображение списка дат и времени уже зарезервированных комнат.
        public ActionResult DisplayListEvent(int id, DateTime date)
        { 
            List<Event> events;

            if (date == null || date.Date < DateTime.Now.Date)
            {
                date = DateTime.Now;
            }

            using (var _context = new ApplicationDbContext())
            {
                events = _context.Events.Where(e => e.MeetingRoomId == id && e.StartEvent >= date).ToList();
            }

            return PartialView(events);
        }

        [HttpGet]
        //История бронирований.
        public ActionResult BookingHistory()
        {
            return View();
        }

        [HttpGet]
        //Бронирование комнаты.
        public ActionResult BookingRoom(int id)
        {
            using (var _context = new ApplicationDbContext())
            {
                var room = _context.MeetingRooms.Where(m => m.Id == id).SingleOrDefault();
                ViewBag.Room = room.NameRoom;
                ViewBag.RoomId = room.Id;
            }
            return View();
        }

        [HttpPost]
        //Бронирование комнаты.
        public ActionResult BookingRoom(Event eventRoom, int RoomId, DateTime StartTime, DateTime StopTime)
        {
            eventRoom.StartEvent = eventRoom.StartEvent.AddHours(StartTime.Hour).AddMinutes(StartTime.Minute);
            eventRoom.StopEvent = eventRoom.StopEvent.AddHours(StopTime.Hour).AddMinutes(StopTime.Minute);

            ViewBag.RoomId = RoomId;

            if (eventRoom.StartEvent >= eventRoom.StopEvent)
            {
                ModelState.AddModelError("StartEvent", "Начало мероприятия должно быть > окончания");
                ModelState.AddModelError("StopEvent", "Начало мероприятия должно быть > окончания");
            }

            if (ModelState.IsValid)
            {
                using (var _context = new ApplicationDbContext())
                {
                    eventRoom.MeetingRoomId = RoomId;

                    var user = _context.Users.Where(u => u.UserName == User.Identity.Name).SingleOrDefault();

                    eventRoom.NameUser = String.Concat(user.Surname, " ", user.Name);
                    eventRoom.ApplicationUserId = user.Id;

                    _context.Events.Add(eventRoom);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(eventRoom);
        }
    }
}