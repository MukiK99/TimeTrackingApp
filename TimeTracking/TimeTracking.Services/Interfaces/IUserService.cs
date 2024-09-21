using TimeTracking.Domain.Models;

namespace TimeTracking.Services.Interfaces
{
    public interface IUserService : IServiceBase<User>
    {
        User CurrentUser { get; set; }
        bool Register(string firstName, string lastName,int age, string username, string password);
        void Login(string username, string password);
        bool ChangePassword(string oldPassword, string newPassword);
        bool ChangeFirstName(string oldFirstName, string newFirstName);
        bool ChangeLastName(string oldLastName, string newLastName);
        void DeactivateAccount();
    }
}
