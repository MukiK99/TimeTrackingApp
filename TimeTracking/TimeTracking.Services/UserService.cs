using TimeTracking.Domain.Models;
using TimeTracking.Helpers;
using TimeTracking.Services.Interfaces;

namespace TimeTracking.Services
{
    public class UserService : ServiceBase<User>, IUserService
    {
        public User CurrentUser { get; set; }
        public bool Register(string firstName, string lastName, int age, string username, string password)
        {
            User existingUser = GetAll().FirstOrDefault(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));

            if (existingUser != null)
            {
               
                return false;
            }

            User newUser = new User(firstName, lastName, age, username, password, false);
           

            Console.WriteLine("User registered successfully.");
            return true;



        }

        public void Login(string username, string password)
        {
            User userDb = GetAll().SingleOrDefault(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase) && u.Password == password);
            if (userDb is null)
            {
                ExtendedConsole.PrintError("Login failed! Invalid credentials entered! Try again...");


            }
            CurrentUser = userDb;

        }

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            if (CurrentUser.Password != oldPassword || !ValidationHelper.ValidatePasswordEasier(newPassword) || oldPassword == newPassword)
            {
                return false;
            }
            CurrentUser.Password = newPassword;
            return Update(CurrentUser);
        }

        public bool ChangeFirstName(string oldFirstName, string newFirstName)
        {
            if(CurrentUser.FirstName != oldFirstName || !ValidationHelper.ValidateFirstNameOnly(newFirstName) || oldFirstName == newFirstName)
            {
                return false;
            }
            CurrentUser.FirstName = newFirstName;
            return Update(CurrentUser);
        }
        public bool ChangeLastName(string oldLastName, string newLastName)
        {
            if(CurrentUser.LastName != oldLastName || !ValidationHelper.ValidateLastNameOnly(newLastName) || oldLastName == newLastName)
            {
                return false;
            }
            CurrentUser.LastName = newLastName;
            return Update(CurrentUser);
        }

        public void DeactivateAccount()
        {
            if(CurrentUser != null)
            {
                CurrentUser.IsDeactivated = true;
                Update(CurrentUser);
            }


        }
        

    }
}
