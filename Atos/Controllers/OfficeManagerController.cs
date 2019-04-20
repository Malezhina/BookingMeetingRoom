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

            using (var _context = new ApplicationDbContext())
            {
                meetingRooms = _context.MeetingRooms.ToList();
            }
            return View(meetingRooms);
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
    }
}