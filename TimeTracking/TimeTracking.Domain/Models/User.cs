namespace TimeTracking.Domain.Models
{
    public class User : BaseEntity
    {
       
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsDeactivated { get; set; } = false;
        public List<Reading> Reading { get; set; } = new List<Reading>();
        public List<Exercising> Exercising { get; set; } = new List<Exercising>();
        public List<Working> Working { get; set; } = new List<Working>();
        public List<OtherHobbies> OtherHobbies { get; set; } = new List<OtherHobbies>();
        public User()
        {

        }
        public User(string firstName, string lastName, int age, string username, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Username = username;
            Password = password;
        }
        public User(string firstName, string lastName, int age, string username, string password, bool isDeactivated)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Username = username;
            Password = password;
            IsDeactivated = isDeactivated;
        }



        public override string GetInfo()
        {
            return $"USER INFO: {FirstName} {LastName} {Age} years old with username {Username}";
        }
    }
}
