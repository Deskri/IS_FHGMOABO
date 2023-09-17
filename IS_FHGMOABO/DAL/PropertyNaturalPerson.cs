namespace IS_FHGMOABO.DAL
{
    public class PropertyNaturalPerson
    {
        public int PropertyId { get; set; }
        public Property Property { get; set; }
        public int NaturalPersonId { get; set; }
        public NaturalPerson NaturalPerson { get; set; }
    }
}
