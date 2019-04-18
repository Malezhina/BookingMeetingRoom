using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atos.Models
{
    public class MeetingRoom
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название комнаты")]
        public string NameRoom { get; set; }

        [Display(Name = "Описание комнаты")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Количество кресел")]
        public int NumberOfSeats { get; set; }

        [Required]
        [Display(Name = "Наличие проектора")]
        public bool Projector { get; set; }

        [Required]
        [Display(Name = "Наличие маркерной доски")]
        public bool MarkerBoard { get; set; }
    }

    public class Event
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название мероприятия")]
        public string NameEvent { get; set; }

        [Required]
        [Display(Name = "Начало мероприятия")]
        public DateTime StartEvent { get; set; }

        [Required]
        [Display(Name = "Конец мероприятия")]
        public DateTime StopEvent { get; set; }

        public bool IsConfirmed { get; set; }

        public int MeetingRoomId { get; set; }

        public MeetingRoom MeetingRoom { get; set;}

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}