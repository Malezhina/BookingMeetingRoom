using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atos.Models
{
    public class MeetingRoomHelper
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

        [Required]
        [Display(Name = "Время начала первого бронирования комнаты")]
        public DateTime StartFirstEvent { get; set; }

        [Required]
        [Display(Name = "Время окончания первого бронирования комнаты")]
        public DateTime StopFirstEvent { get; set; }
    }

    public class UserHelper
    {
        public string Id { get; set; }

        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Роль")]
        public string Role { get; set; }
    } 
}