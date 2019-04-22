using Atos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        [Authorize(Roles = "officeManager")]
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

        //Управление ролями.
        [Authorize(Roles = "officeManager")]
        public ActionResult RoleManagement()
        {
            List<UserHelper> userList = new List<UserHelper>();

            using (var _context = new ApplicationDbContext())
            {
                var users = _context.Users.Where(u => u.UserName != User.Identity.Name)
                    .OrderBy(u => u.Surname).ThenBy(u => u.Name).ToList();

                foreach (var user in users)
                {
                    UserHelper userHelper = new UserHelper();

                    var roleId = user.Roles.Where(r => r.UserId == user.Id).Select(r => r.RoleId).ToList();

                    if (roleId.Count() == 2)
                    {
                        userHelper.Role = "officeManager";
                    }
                    else
                    {
                        userHelper.Role = "employee";
                    }
                    userHelper.Id = user.Id;
                    userHelper.Surname = user.Surname;
                    userHelper.Name = user.Name;

                    userList.Add(userHelper);
                }
            }

            return View(userList);
        }

        //Частичное представление для формы управления ролями.
        [Authorize(Roles = "officeManager")]
        public ActionResult RoleManagementList()
        {
            List<UserHelper> userList = new List<UserHelper>();

            using (var _context = new ApplicationDbContext())
            {
                var users = _context.Users.Where(u => u.UserName != User.Identity.Name)
                    .OrderBy(u => u.Surname).ThenBy(u => u.Name).ToList();

                foreach (var user in users)
                {
                    UserHelper userHelper = new UserHelper();

                    var roleId = user.Roles.Where(r => r.UserId == user.Id).Select(r => r.RoleId).ToList();
                    
                    if(roleId.Count() == 2)
                    {
                        userHelper.Role = "officeManager";
                    }
                    else
                    {
                        userHelper.Role = "employee";
                    }
                    userHelper.Id = user.Id;
                    userHelper.Surname = user.Surname;
                    userHelper.Name = user.Name;

                    userList.Add(userHelper);
                }
            }

            return PartialView(userList);
        }

        //Назначение роли "Офис менеджер".
        public void AssignmentRole(string id)
        {
            using (var _context = new ApplicationDbContext())
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));

                userManager.AddToRole(id, "officeManager");

                _context.SaveChanges();
            }
        }

        //Создание комнаты.
        [HttpGet]
        [Authorize(Roles = "officeManager")]
        public ActionResult CreateRoom()
        {
            return View();
        }

        //Создание комнаты.
        [HttpPost]
        [Authorize(Roles = "officeManager")]
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

        //Редактирование комнаты.
        [HttpGet]
        [Authorize(Roles = "officeManager")]
        public ActionResult EditRoom(int id)
        {
            MeetingRoom meetingRoom;
            using (var _context = new ApplicationDbContext())
            {
                meetingRoom = _context.MeetingRooms.Where(m => m.Id == id).SingleOrDefault();
            }
            return View(meetingRoom);
        }

        //Редактирование комнаты.
        [HttpPost]
        [Authorize(Roles = "officeManager")]
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
        [Authorize(Roles = "officeManager")]
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
        [HttpGet]
        [Authorize(Roles = "officeManager")]
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

        //Удаление комнаты.
        [HttpGet]
        [Authorize(Roles = "officeManager")]
        public ActionResult DeleteRoom(int id)
        {
            MeetingRoom meetingRoom;
            using (var _context = new ApplicationDbContext())
            {
                meetingRoom = _context.MeetingRooms.Where(m => m.Id == id).SingleOrDefault();
            }
            return View(meetingRoom);
        }

        //Удаление комнаты.
        [HttpPost]
        [Authorize(Roles = "officeManager")]
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