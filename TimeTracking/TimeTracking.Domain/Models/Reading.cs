using TimeTracking.Domain.Enums;

namespace TimeTracking.Domain.Models
{
    public class Reading : BaseActivity
    {
        public TypeOfBook Genre { get; set; }
        public int NumberOfPages { get; set; }
        public Reading() { }

        public Reading(TypeOfBook genre, int numberOfPages)
        {
            Genre = genre;
            NumberOfPages = numberOfPages;
        }
    }
}
