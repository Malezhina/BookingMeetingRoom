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
            List<MeetingRoom> roomList;

            using (var _context = new ApplicationDbContext())
            {
                roomList = _context.MeetingRooms.ToList();
            }
            return View(roomList);
        }

        [HttpGet]
        //Открытие экрана с описанием комнаты.
        public ActionResult OpenRoom(int id)
        {
            MeetingRoom meetingRoom;

            using (var _context = new ApplicationDbContext())
            {
                meetingRoom = _context.MeetingRooms.Where(m => m.Id == id).SingleOrDefault();

            }
            return View(meetingRoom);
        }

        [HttpPost]
        //Отображение списка дат и времени уже зарезервированных комнат.
        public ActionResult DisplayListEvent(int id, DateTime datetime)
        {
            List<Event> events;

            using (var _context = new ApplicationDbContext())
            {
                events = _context.Events.Where(e => e.MeetingRoomId == id && e.StartEvent > datetime).ToList();
            }

            return PartialView(events);
        }
    }
}