using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.RoomModels
{
    public class EditRoomModel
    {
        /// <summary>
        /// Идентификатор собственности
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Внешний ключ для связи с House
        /// </summary>
        public int HouseId { get; set; }

        /// <summary>
        /// Значение для поиска совпадающих номеров комнат, за исключением этого значения входящего номера
        /// </summary>
        public string IncomingNumber { get; set; }

        /// <summary>
        /// Тип помещения
        /// </summary>
        [Required(ErrorMessage = "Требуется внести тип помещения")]
        [Display(Name = "Тип помещения")]
        public RoomType? Type { get; set; }

        /// <summary>
        /// Номер помещения
        /// </summary>
        [Required(ErrorMessage = "Требуется внести номер помещения")]
        [Display(Name = "Номер помещения")]
        public string Number { get; set; }

        /// <summary>
        /// Назначение
        /// </summary>
        [Required(ErrorMessage = "Требуется внести назначение")]
        [Display(Name = "Назначение")]
        public RoomPurpose? Purpose { get; set; }

        /// <summary>
        /// Общая площадь
        /// </summary>
        [Required(ErrorMessage = "Требуется внести общую площадь")]
        [Display(Name = "Общая площадь")]
        public decimal TotalArea { get; set; }

        /// <summary>
        /// Жилая площадь
        /// </summary>
        [Display(Name = "Жилая площадь")]
        public decimal? LivingArea { get; set; }

        /// <summary>
        /// Полезная площадь
        /// </summary>
        [Display(Name = "Полезная площадь")]
        public decimal? UsableArea { get; set; }

        /// <summary>
        /// Этаж
        /// </summary>
        [Display(Name = "Этаж")]
        public int? Floor { get; set; }

        /// <summary>
        /// Подъезд
        /// </summary>
        [Display(Name = "Подъезд")]
        public int? Entrance { get; set; }

        /// <summary>
        /// Кадастровый номер
        /// </summary>
        [Display(Name = "Кадастровый номер")]
        public string? CadastralNumber { get; set; }

        /// <summary>
        /// Приватизировано
        /// </summary>
        [Display(Name = "Приватизировано")]
        public bool IsPrivatized { get; set; }

        public enum RoomType
        {
            квартира,
            комната,
            офис,
            павильон,
            помещение,
        }

        public enum RoomPurpose
        {
            Жилое,
            Нежилое,
        }
    }
}
