using IS_FHGMOABO.DAL;
using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.RoomModels
{
    public class IndexRoomModel
    {
        /// <summary>
        /// Лист с помещениями для CRUD
        /// </summary>
        public List<Room> Rooms { get; set; }
        
        /// <summary>
        /// Дом, по которому работаем с помещениями
        /// </summary>
        public House House { get; set; }

        /// <summary>
        /// Модель добавления нового помещения
        /// </summary>
        public AddRoomModel AddRoom { get; set; }
    }
}
