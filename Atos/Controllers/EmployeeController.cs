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
        [Authorize]
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
                        var firstEvent = _context.Events.Where(e => e.MeetingRoomId == room.Id && e.StartEvent > DateTime.Now && e.IsConfirmed != false)
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

        //Открытие экрана с описанием комнаты.
        [HttpGet]
        [Authorize]
        public ActionResult OpenRoom(int? id)
        {
            MeetingRoom meetingRoom;
            List<Event> events;
            Tuple<MeetingRoom, List<Event>> tuple;
            using (var _context = new ApplicationDbContext())
            {
                meetingRoom = _context.MeetingRooms.Where(m => m.Id == id).SingleOrDefault();
                events = _context.Events.Where(e => e.MeetingRoomId == id && e.StartEvent >= DateTime.Now && e.IsConfirmed != false)
                    .OrderBy(e => e.StartEvent).ThenBy(e => e.StopEvent).ToList();
            }
            tuple = new Tuple<MeetingRoom, List<Event>>(meetingRoom, events);

            return View(tuple);
        }

        //Отображение списка дат и времени уже зарезервированных комнат.
        [HttpPost]
        [Authorize]
        public ActionResult DisplayListEvent(int id, DateTime date)
        { 
            List<Event> events;

            if (date == null || date < DateTime.Now)
            {
                date = DateTime.Now;
            }

            using (var _context = new ApplicationDbContext())
            {
                events = _context.Events.Where(e => e.MeetingRoomId == id && e.StartEvent >= date && e.IsConfirmed != false)
                    .OrderBy(e => e.StartEvent).ThenBy(e => e.StopEvent).ToList();
            }

            return PartialView(events);
        }

        //История бронирований.
        [HttpGet]
        [Authorize]
        public ActionResult BookingHistory()
        {
            List<Event> events;

            using (var _context = new ApplicationDbContext())
            {
                var userId = _context.Users.Where(u => u.UserName == User.Identity.Name).SingleOrDefault().Id;

                events = _context.Events.Where(e => e.ApplicationUserId == userId)
                    .OrderBy(e => e.StartEvent).ThenBy(e => e.StopEvent).ToList();

                foreach (var eventBooking in events)
                {
                    var meetingRoom = _context.MeetingRooms.Where(m => m.Id == eventBooking.MeetingRoomId).SingleOrDefault();
                    eventBooking.MeetingRoom = meetingRoom;
                }
            }

            return View(events);
        }

        //Частичное представление списка истории бронирований для сотрудника.
        [Authorize]
        public ActionResult BookingHistoryList()
        {
            List<Event> events;

            using (var _context = new ApplicationDbContext())
            {
                var userId = _context.Users.Where(u => u.UserName == User.Identity.Name).SingleOrDefault().Id;

                events = _context.Events.Where(e => e.ApplicationUserId == userId)
                    .OrderBy(e => e.StartEvent).ThenBy(e => e.StopEvent).ToList();

                foreach(var eventBooking in events)
                {
                    var meetingRoom = _context.MeetingRooms.Where(m => m.Id == eventBooking.MeetingRoomId).SingleOrDefault();
                    eventBooking.MeetingRoom = meetingRoom;
                }
            }

            return PartialView(events);
        }

        //Отмена бронирования мероприятия.
        public void CancelBookinRoom(int id)
        {
            using (var _context = new ApplicationDbContext())
            {
                var eventBooking = _context.Events.Where(e => e.Id == id).SingleOrDefault();

                eventBooking.IsConfirmed = false;

                _context.Entry(eventBooking).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
        }

        //Бронирование комнаты.
        [HttpGet]
        [Authorize]
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

        //Бронирование комнаты.
        [HttpPost]
        [Authorize]
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

            if (eventRoom.StartEvent < DateTime.Now)
            {
                ModelState.AddModelError("StartEvent", "Начало мероприятия должно быть > текущего времени");
            }

            if (eventRoom.StopEvent < DateTime.Now)
            {
                ModelState.AddModelError("StopEvent", "Окончание мероприятия должно быть > текущего времени");
            }
    
            if (ModelState.IsValid)
            {
                using (var _context = new ApplicationDbContext())
                {
                    eventRoom.MeetingRoomId = RoomId;

                    var user = _context.Users.Where(u => u.UserName == User.Identity.Name).SingleOrDefault();

                    eventRoom.NameUser = String.Concat(user.Surname, " ", user.Name);
                    eventRoom.ApplicationUserId = user.Id;

                    //Проверка - свободно ли время
                    var events = _context.Events.Where(e=> e.IsConfirmed != false).OrderBy(e => e.StartEvent).ThenBy(e => e.StopEvent).ToArray();
                    if (eventRoom.StopEvent <= events[0].StartEvent)
                    {
                        _context.Events.Add(eventRoom);
                        _context.SaveChanges();
                        return RedirectToAction("OpenRoom", new { id = RoomId });
                    }
                    
                    for (int i = 1; i < events.Count() - 2; i++)
                    {
                        if (eventRoom.StartEvent >= events[i-1].StopEvent &&
                            eventRoom.StopEvent <= events[i+1].StartEvent)
                        {
                            _context.Events.Add(eventRoom);
                            _context.SaveChanges();
                            return RedirectToAction("OpenRoom", new {id = RoomId });
                        }
                    }

                    if (eventRoom.StartEvent >= events[events.Count()-1].StopEvent)
                    {
                        _context.Events.Add(eventRoom);
                        _context.SaveChanges();
                        return RedirectToAction("OpenRoom", new { id = RoomId });
                    }
                    else
                    {
                        ModelState.AddModelError("StartEvent", "Время занято");
                        ModelState.AddModelError("StopEvent", "Время занято");
                    }
                }
            }
            return View(eventRoom);
        }
    }
}