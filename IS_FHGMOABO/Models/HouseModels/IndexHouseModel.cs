using IS_FHGMOABO.DAL;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IS_FHGMOABO.Models.HouseModels
{
    public class IndexHouseModel
    {
        [BindNever]
        public List<House> Houses { get; set; }

        public AddHouseModel AddHouse { get; set; }
    }
}
