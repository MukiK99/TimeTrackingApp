using TimeTracking.Domain.Enums;

namespace TimeTracking.Domain.Models
{
    public class Working : BaseActivity
    {
       
        public WorkPlace WorkPlace { get; set; }
        public Working() { }
        
    }
}
