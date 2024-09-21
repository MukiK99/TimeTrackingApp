namespace TimeTracking.Domain.Models
{
    public abstract class BaseActivity
    {
        public string Title {  get; set; }
        public double TimeSpentOnActivity { get; set; }
        public string ExtraInfoForActivity {  get; set; } 
    }
}
