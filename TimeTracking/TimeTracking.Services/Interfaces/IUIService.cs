using TimeTracking.Domain.Models;
using TimeTracking.Services.Enums;

namespace TimeTracking.Services.Interfaces
{
    public interface IUIService
    {
        public List<MainMenuChoice> MainMenuItems { get; set; }

        User RegisterMenu();
        User LoginMenu();
        int MainMenu(User user);
        int ChooseMenu<T>(List<T> items);
        void EndMenu();
    }
}
