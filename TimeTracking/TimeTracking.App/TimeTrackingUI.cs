using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Channels;
using TimeTracking.Domain.Enums;
using TimeTracking.Domain.Models;
using TimeTracking.Helpers;
using TimeTracking.Services;
using TimeTracking.Services.Enums;
using TimeTracking.Services.Interfaces;

namespace TimeTracking.App
{
    internal class TimeTrackingUI
    {
        private IUserService _userService;
        private IUIService _uiService;

        public TimeTrackingUI()
        {
            _userService = new UserService();
            _uiService = new UIService();
            InitializeStartingData();

        }


        public void InitApp()
        {
            int counter = 0;

            while (true)
            {
                if (counter > 3)
                {
                    Console.Clear();
                    ExtendedConsole.PrintInColor("Goodbye! Hope to meet you again!", ConsoleColor.Green);
                    Thread.Sleep(2000);
                    break;
                }

                Console.Clear();
                #region Login and Register
                if (_userService.CurrentUser is null)
                {
                    try
                    {
                        ExtendedConsole.PrintTitle("\n\t*** Time Tracking App ***\n");
                        int choice = _uiService.ChooseMenu(new List<string> { "Login", "Register", "Exit" });
                        if (choice == -1)
                        {
                            ExtendedConsole.PrintError("Invalid choice! Try again...");
                            continue;
                        }

                        if (choice == 1)
                        {
                            counter++;

                            User inputUser = _uiService.LoginMenu();

                            _userService.Login(inputUser.Username, inputUser.Password);
                            if (_userService.CurrentUser == null)
                            {
                                ExtendedConsole.PrintError($"You have {3 - counter} attempts left");
                                Thread.Sleep(1500);
                                continue;
                            }
                            if (_userService.CurrentUser != null && !_userService.CurrentUser.IsDeactivated)
                            {
                                ExtendedConsole.PrintSuccess($"\nWelcome {_userService.CurrentUser.FirstName} {_userService.CurrentUser.LastName}");
                                Thread.Sleep(3000);
                            }
                            if (_userService.CurrentUser != null && _userService.CurrentUser.IsDeactivated)
                            {

                                ExtendedConsole.PrintInColor("Do you want to activate your account?");
                                string answer = Console.ReadLine();
                                if (answer.ToLower() == "yes")
                                {
                                    _userService.CurrentUser.IsDeactivated = false;
                                    ExtendedConsole.PrintSuccess("You have successfully activated your account!");

                                }
                                else
                                {
                                    ExtendedConsole.PrintError("Your account is deactivated.Please activate it to login!");
                                    Thread.Sleep(1500);
                                    _userService.CurrentUser = null;
                                    continue;
                                }


                            }


                        }
                        if (choice == 2)
                        {
                            User newUser = _uiService.RegisterMenu();
                            _userService.Insert(newUser);
                            var db = _userService.GetAll();
                            db.ForEach(user => Console.WriteLine($"{user.FirstName} ({user.LastName}{user.Username})"));
                            Console.ReadLine();
                            continue;
                        }
                        if (choice == 3)
                        {
                            _uiService.EndMenu();
                            Thread.Sleep(2000);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExtendedConsole.PrintError(ex.Message);
                        continue;
                    }
                }
                #endregion

                #region Main Menu
                counter = 0;
                int mainMenuChoiceNumber = _uiService.MainMenu(_userService.CurrentUser);
                if (mainMenuChoiceNumber == -1)
                {
                    ExtendedConsole.PrintError("Invalid choice! Try again...");
                    continue;
                }

                MainMenuChoice mainMenuChoice = _uiService.MainMenuItems[mainMenuChoiceNumber - 1];

                switch (mainMenuChoice)
                {
                    case MainMenuChoice.Track:
                        ExtendedConsole.PrintInColor("TRACK MENU", ConsoleColor.Magenta);
                        int activityChoice = UserTrackChoice();
                        _userService.Update(_userService.CurrentUser);
                        break;
                    case MainMenuChoice.UserStatistics:
                        ExtendedConsole.PrintInColor("USER STATISTICS", ConsoleColor.Magenta);
                        int userStatsChoice = UserStatisticsChoice(_userService.CurrentUser);
                        _userService.Update(_userService.CurrentUser);
                        Thread.Sleep(2000);
                        break;
                    case MainMenuChoice.AccountManagement:
                        ManageAccount(_userService.CurrentUser);
                        break;
                    case MainMenuChoice.Logout:
                        Console.Clear();
                        ExtendedConsole.PrintInColor($"\n\n         Goodbye {_userService.CurrentUser.Username}. Hope to see you again!", ConsoleColor.Yellow);
                        Thread.Sleep(1500);
                        _userService.CurrentUser = null;
                        continue;

                }
                #endregion

            }
        }

        private int UserTrackChoice()
        {
            int userChoice = 0;
            while (true)
            {
                Console.Clear();
                ExtendedConsole.PrintTitle("\n                         ***Track Menu**\n\n");
                userChoice = _uiService.ChooseMenu(new List<string> { "Reading", "Exercising", "Working", "Other Hobbies", "Back to Main Menu" });
                Console.Clear();
                if (userChoice == -1)
                {
                    ExtendedConsole.PrintError("Invalid choice! Try again...");
                    Thread.Sleep(1500);
                    continue;
                }
                if (userChoice == 1)
                {
                    ExtendedConsole.PrintTitle("\n                         ***Reading Menu**\n\n");
                    Reading readingObject = ReadingMenu();
                    _userService.CurrentUser.Reading.Add(readingObject);
                    Thread.Sleep(1500);
                    break;
                }
                if (userChoice == 2)
                {
                    ExtendedConsole.PrintTitle("\n                         ***Exercising Menu**\n\n");

                    Exercising exercisingObject = ExercisingMenu();
                    _userService.CurrentUser.Exercising.Add(exercisingObject);
                    break;
                }
                if (userChoice == 3)
                {
                    ExtendedConsole.PrintTitle("\n                         ***Working Menu**\n\n");

                    Working workingObject = WorkingMenu();
                    _userService.CurrentUser.Working.Add(workingObject);
                    break;

                }
                if (userChoice == 4)
                {
                    ExtendedConsole.PrintTitle("\n                         ***Hobby Menu**\n\n");
                    OtherHobbies hobby = OtherHobbiesMenu();
                    _userService.CurrentUser.OtherHobbies.Add(hobby);
                    break;
                }
                if (userChoice == 5)
                {

                    break;
                }


            }
            return userChoice;

        }

        private Reading ReadingMenu()
        {


            int readingChoice = _uiService.ChooseMenu(new List<TypeOfBook> { TypeOfBook.BellesLettres, TypeOfBook.Fiction, TypeOfBook.ProfessionalLiterature });
            switch (readingChoice)
            {
                case 1:
                    Reading bellesLettresReading = new Reading();
                    bellesLettresReading.Title = ExtendedConsole.GetInput("Title: ");
                    bellesLettresReading.ExtraInfoForActivity += ExtraInfo();
                    bellesLettresReading.Genre = TypeOfBook.BellesLettres;
                    double blElapsedTime = TimeElapsed();
                    bellesLettresReading.TimeSpentOnActivity += blElapsedTime;
                    bellesLettresReading.NumberOfPages += PageRead();
                    return bellesLettresReading;

                case 2:
                    Reading fictionReading = new Reading();
                    fictionReading.Title = ExtendedConsole.GetInput("Title: ");
                    fictionReading.ExtraInfoForActivity += ExtraInfo();
                    fictionReading.Genre = TypeOfBook.Fiction;
                    double fictionElapsedTime = TimeElapsed();
                    fictionReading.TimeSpentOnActivity += fictionElapsedTime;
                    fictionReading.NumberOfPages += PageRead();
                    return fictionReading;

                case 3:
                    Reading professionalLiteratureReading = new Reading();
                    professionalLiteratureReading.Title = ExtendedConsole.GetInput("Title: ");
                    professionalLiteratureReading.ExtraInfoForActivity += ExtraInfo();
                    professionalLiteratureReading.Genre = TypeOfBook.ProfessionalLiterature;
                    double professionalElapsedTime = TimeElapsed();
                    professionalLiteratureReading.TimeSpentOnActivity += professionalElapsedTime;
                    professionalLiteratureReading.NumberOfPages += PageRead();
                    return professionalLiteratureReading;

                default:
                    return new Reading();

            }

        }

        private Exercising ExercisingMenu()
        {
            int exercisingChoice = _uiService.ChooseMenu(new List<TypeOfExercise> { TypeOfExercise.General, TypeOfExercise.Running, TypeOfExercise.Sport });
            switch (exercisingChoice)
            {
                case 1:
                    Exercising generalExercising = new Exercising();
                    generalExercising.Title = "General";
                    generalExercising.ExtraInfoForActivity += ExtraInfo();
                    generalExercising.ExerciseType = TypeOfExercise.General;
                    double generalExercisingElapsedTime = TimeElapsed();
                    generalExercising.TimeSpentOnActivity += generalExercisingElapsedTime;
                    return generalExercising;
                case 2:
                    Exercising runningExercising = new Exercising();
                    runningExercising.Title = "Running";
                    runningExercising.ExtraInfoForActivity += ExtraInfo();
                    runningExercising.ExerciseType = TypeOfExercise.Running;
                    double runningExercisingElapsedTime = TimeElapsed();
                    runningExercising.TimeSpentOnActivity += runningExercisingElapsedTime;
                    return runningExercising;
                case 3:
                    Exercising sportExercising = new Exercising();
                    sportExercising.Title = ExtendedConsole.GetInput("Name of the sport: ");
                    sportExercising.ExtraInfoForActivity += ExtraInfo();
                    sportExercising.ExerciseType = TypeOfExercise.Sport;
                    double sportExercisingElapsedTime = TimeElapsed();
                    sportExercising.TimeSpentOnActivity += sportExercisingElapsedTime;
                    return sportExercising;
                default:
                    return new Exercising();

            }

        }
        private Working WorkingMenu()
        {
            int workingChoice = _uiService.ChooseMenu(new List<WorkPlace> { WorkPlace.WorkingAtHome, WorkPlace.WorkingAtTheOffice });
            switch (workingChoice)
            {
                case 1:
                    Working workAtHome = new Working();
                    workAtHome.Title = "Work At Home";
                    workAtHome.ExtraInfoForActivity += ExtraInfo();
                    workAtHome.WorkPlace = WorkPlace.WorkingAtHome;
                    double workingElapsedTime = TimeElapsed();
                    workAtHome.TimeSpentOnActivity += workingElapsedTime;
                    return workAtHome;
                case 2:
                    Working workAtTheOffice = new Working();
                    workAtTheOffice.Title = "Work At The Office";
                    workAtTheOffice.ExtraInfoForActivity += ExtraInfo();
                    workAtTheOffice.WorkPlace = WorkPlace.WorkingAtTheOffice;
                    double workAtTheOfficeElapsedTime = TimeElapsed();
                    workAtTheOffice.TimeSpentOnActivity += workAtTheOfficeElapsedTime;
                    return workAtTheOffice;
                default:
                    return new Working();
            }
        }

        private OtherHobbies OtherHobbiesMenu()
        {
            OtherHobbies someHobby = new OtherHobbies();
            someHobby.Title = ExtendedConsole.GetInput("Name of the hobby: ");
            someHobby.ExtraInfoForActivity += ExtraInfo();
            someHobby.Hobby = OtherHobby.OtherHobbies;
            double hobbyTime = TimeElapsed();
            someHobby.TimeSpentOnActivity += hobbyTime;
            return someHobby;
        }
        private int PageRead()
        {
            string numberOfPages = ExtendedConsole.GetInput("Number of pages read today: ");
            return int.Parse(numberOfPages);


        }



        private int UserStatisticsChoice(User currentUser)
        {
            int userStatsChoice = 0;
            while (true)
            {
                Console.Clear();
                ExtendedConsole.PrintTitle("\n                         ***User Statistics Menu***\n\n");
                userStatsChoice = _uiService.ChooseMenu(new List<string> { "Reading Stats", "Exercising Stats", "Working Stats", "Other Hobbies Stats", "Global stats", "Back to Main Menu" });
                Console.Clear();
                if (userStatsChoice == -1)
                {
                    ExtendedConsole.PrintError("Invalid choice! Try again...");
                    Thread.Sleep(1500);
                    continue;
                }
                if (userStatsChoice == 1)
                {
                    List<Reading> readingUserList = currentUser.Reading;
                    double totalTimeReading = readingUserList.Sum(x => x.TimeSpentOnActivity);
                    double averageOfAllRecords = Math.Round(totalTimeReading / readingUserList.Count / 60);
                    int totalNumberOfPages = readingUserList.Sum(x => x.NumberOfPages);
                    TypeOfBook favouriteType = readingUserList.
                        GroupBy(x => x.Genre)
                        .OrderByDescending(g => g.Count())
                        .Select(r => r.Key)
                        .FirstOrDefault();
                    ExtendedConsole.PrintInColor($"Total time spent on reading for user:{currentUser.Username} is:\n{Math.Round((totalTimeReading / 3600), 3)} hours", ConsoleColor.Green);
                    ExtendedConsole.PrintInColor($"Average of all activity records:\n{Math.Round(averageOfAllRecords, 3)} minutes", ConsoleColor.Green);
                    ExtendedConsole.PrintInColor($"Total number of pages:\n{totalNumberOfPages}");
                    ExtendedConsole.PrintInColor($"Favourite type of books:\n{favouriteType}");
                    Console.ReadLine();
                }
                if (userStatsChoice == 2)
                {
                    List<Exercising> exercisingUserList = currentUser.Exercising;
                    double totalTimeExercising = exercisingUserList.Sum(x => x.TimeSpentOnActivity);
                    double averageOfAllExercisingRecords = Math.Floor(totalTimeExercising / exercisingUserList.Count / 60);
                    TypeOfExercise favouriteTypeOfExercise = exercisingUserList.
                        GroupBy(x => x.ExerciseType)
                        .OrderByDescending(g => g.Count())
                        .Select(r => r.Key)
                        .FirstOrDefault();
                    ExtendedConsole.PrintInColor($"Total time spent on exercising for user:{currentUser.Username} is:\n{Math.Round((totalTimeExercising / 3600), 3)} hours", ConsoleColor.Green);
                    ExtendedConsole.PrintInColor($"Average of all exercising activity records:\n{Math.Round(averageOfAllExercisingRecords, 3)} minutes", ConsoleColor.Green);
                    ExtendedConsole.PrintInColor($"Favourite type of exercise:\n{favouriteTypeOfExercise}");
                    Console.ReadLine();
                }
                if (userStatsChoice == 3)
                {
                    List<Working> workingUserList = currentUser.Working;
                    double totalTimeWorking = workingUserList.Sum(x => x.TimeSpentOnActivity);
                    double totalTimeWorkingRounded = Math.Round(workingUserList.Sum(x => x.TimeSpentOnActivity) / 3600, 3);
                    double averageOfAllWorkingRecords = Math.Round(totalTimeWorking / workingUserList.Count / 60, 3);
                    List<Working> homeWorking = workingUserList.Where(x => x.WorkPlace == WorkPlace.WorkingAtHome).ToList();
                    double homeWorkingHours = Math.Round(homeWorking.Sum(x => x.TimeSpentOnActivity) / 3600, 3);
                    List<Working> officeWorking = workingUserList.Where(x => x.WorkPlace == WorkPlace.WorkingAtTheOffice).ToList();

                    double officeWorkingHours = Math.Round(officeWorking.Sum(x => x.TimeSpentOnActivity) / 3600, 3);
                    ;
                    ExtendedConsole.PrintInColor($"Total time spent on working for user:{currentUser.Username} is:\n{totalTimeWorkingRounded} hours", ConsoleColor.Green);
                    ExtendedConsole.PrintInColor($"Average of all working activity records:\n{averageOfAllWorkingRecords} minutes", ConsoleColor.Green);
                    ExtendedConsole.PrintInColor($"Working from home hours: \n{homeWorkingHours}");
                    ExtendedConsole.PrintInColor($"Working at the office hours: \n{officeWorkingHours}");
                    Console.ReadLine();

                }
                if (userStatsChoice == 4)
                {
                    List<OtherHobbies> otherHobbiesList = currentUser.OtherHobbies;
                    double totalTimeSpentOnHobbies = otherHobbiesList.Sum(x => x.TimeSpentOnActivity);
                    ExtendedConsole.PrintInColor($"Total time spent on hobbies for user:{currentUser.Username} is:\n{Math.Round((totalTimeSpentOnHobbies / 3600), 3)} hours", ConsoleColor.Green);
                    var hobbiesNames = otherHobbiesList.DistinctBy(x => x.Title).ToList();
                    ExtendedConsole.PrintInColor("Hobbies names: ", ConsoleColor.Green);
                    hobbiesNames.ForEach(x => ExtendedConsole.PrintInColor(x.Title, ConsoleColor.Green));
                    Console.ReadLine();

                }
                if (userStatsChoice == 5)
                {
                    List<Reading> readingUserList = currentUser.Reading;
                    List<Exercising> exercisingUserList = currentUser.Exercising;
                    List<Working> workingUserList = currentUser.Working;
                    List<OtherHobbies> otherHobbiesList = currentUser.OtherHobbies;

                    double totalTimeReading = readingUserList.Sum(x => x.TimeSpentOnActivity);
                    double totalTimeExercising = exercisingUserList.Sum(x => x.TimeSpentOnActivity);
                    double totalTimeWorking = workingUserList.Sum(x => x.TimeSpentOnActivity);
                    double totalTimeSpentOnHobbies = otherHobbiesList.Sum(x => x.TimeSpentOnActivity);
                    double totalTimeSpent = Math.Round((totalTimeReading + totalTimeExercising + totalTimeWorking + totalTimeSpentOnHobbies) / 3600, 3);

                    Dictionary<string, int> listLength = new Dictionary<string, int>
                    {
                        {"Reading", readingUserList.Count },
                        {"Exercising", exercisingUserList.Count },
                        {"Working", workingUserList.Count },
                        {"OtherHobbies", otherHobbiesList.Count }

                    };

                    var longestList = listLength.OrderByDescending(x => x.Value).FirstOrDefault();
                    if (listLength.Any(x => x.Value == 0))
                    {
                        longestList = new KeyValuePair<string, int>("", 0);
                    }

                    ExtendedConsole.PrintInColor($"Global total time spent:\n{totalTimeSpent} hours", ConsoleColor.Green);
                    ExtendedConsole.PrintInColor($"User favourite activity is:\n{longestList.Key}", ConsoleColor.Green);
                    Console.ReadLine();

                }
                if (userStatsChoice == 6)
                {

                    break;
                }
            }
            return userStatsChoice;

        }

        private void ManageAccount(User currentUser)
        {
            int userAccountManagementChoice = 0;
            while (true)
            {
                Console.Clear();
                ExtendedConsole.PrintTitle("\n                         ***Account Management Menu***\n\n");
                userAccountManagementChoice = _uiService.ChooseMenu(new List<string> { "Change Password", "Change FirstName", "Change LastName", "Deactivate Account", "Back to Main Menu" });
                Console.Clear();
                if (userAccountManagementChoice == -1)
                {
                    ExtendedConsole.PrintError("Invalid choice! Try again...");
                    Thread.Sleep(1500);
                    continue;
                }
                if (userAccountManagementChoice == 1)
                {
                    string oldPassword = ExtendedConsole.GetInput("Enter old Password:");
                    string newPassword = ExtendedConsole.GetInput("Enter new Password:");
                    if (!ValidationHelper.ValidateStringInput(oldPassword) || !ValidationHelper.ValidateStringInput(newPassword))
                    {
                        ExtendedConsole.PrintError("Please enter proper values!");
                        Thread.Sleep(2000);
                        continue;
                    }
                    bool isChangedPassword = _userService.ChangePassword(oldPassword, newPassword);
                    if (isChangedPassword)
                    {
                        ExtendedConsole.PrintSuccess("Successfully changed a password!");
                        Thread.Sleep(2000);
                        continue;
                    }
                    else
                    {

                        ExtendedConsole.PrintError("Password changed failed! Please try again.");
                        Thread.Sleep(2000);
                        continue;
                    }
                }

                if (userAccountManagementChoice == 2)
                {
                    string oldFirstName = ExtendedConsole.GetInput("Enter old FirstName:");
                    string newFirstName = ExtendedConsole.GetInput("Enter new FirstName:");
                    if (!ValidationHelper.ValidateStringInput(oldFirstName) || !ValidationHelper.ValidateStringInput(newFirstName))
                    {
                        ExtendedConsole.PrintError("Please enter proper values!");
                        Thread.Sleep(2000);
                        continue;
                    }
                    bool isChangedFirstName = _userService.ChangeFirstName(oldFirstName, newFirstName);
                    if (isChangedFirstName)
                    {
                        ExtendedConsole.PrintSuccess("Successfully changed firstname!");
                        Thread.Sleep(2000);
                        continue;
                    }
                    else
                    {

                        ExtendedConsole.PrintError("Firstname changed failed! Please try again.");
                        Thread.Sleep(2000);
                        continue;
                    }
                }

                if (userAccountManagementChoice == 3)
                {
                    string oldLastName = ExtendedConsole.GetInput("Enter old LastName:");
                    string newLastName = ExtendedConsole.GetInput("Enter new LastName:");
                    if (!ValidationHelper.ValidateStringInput(oldLastName) || !ValidationHelper.ValidateStringInput(newLastName))
                    {
                        ExtendedConsole.PrintError("Please enter proper values!");
                        Thread.Sleep(2000);
                        continue;
                    }
                    bool isChangedLastName = _userService.ChangeLastName(oldLastName, newLastName);
                    if (isChangedLastName)
                    {
                        ExtendedConsole.PrintSuccess("Successfully changed Lastname!");
                        Thread.Sleep(2000);
                        continue;
                    }
                    else
                    {

                        ExtendedConsole.PrintError("Lastname changed failed! Please try again.");
                        Thread.Sleep(2000);
                        continue;
                    }
                }
                if (userAccountManagementChoice == 4)
                {

                    ExtendedConsole.PrintSuccess($"\n\n\n\n\n\n                            User ***{currentUser.Username}*** has successfully deactivated its account!");
                    Thread.Sleep(2000);
                    _userService.DeactivateAccount();
                    Thread.Sleep(1000);
                    _userService.CurrentUser = null;
                    break;
                }

                if (userAccountManagementChoice == 5)
                {
                    break;
                }

            }
        }


        private double TimeElapsed()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.WriteLine("Countdown started...");
            Thread.Sleep(1000);
            Console.Write("Press the ENTER button to stop the timer after you finish your activity!");
            string stoppingPoint = Console.ReadLine();
            stopWatch.Stop();
            double elapsedTimeInSeconds = stopWatch.Elapsed.TotalSeconds;
            double elapsedTimeInMinutes = elapsedTimeInSeconds / 60;
            double elapsedTimeInHours = elapsedTimeInSeconds / 3600;
            Console.WriteLine($"Elapsed Time in Minutes: {Math.Floor(elapsedTimeInMinutes)} minutes");
            Console.WriteLine($"Elapsed Time in Seconds: {Math.Round(elapsedTimeInSeconds)} seconds");
            Console.WriteLine($"Elapsed Time in Hours: {Math.Floor(elapsedTimeInHours)} hours");
            Console.WriteLine("Press ENTER to proceed!");
            Console.ReadLine();


            return Math.Round(elapsedTimeInSeconds);

        }
        private string ExtraInfo()
        {
            Console.WriteLine("Enter some extra info for the activity!");
            string info = Console.ReadLine();
            Thread.Sleep(1500);
            return info;
        }


        private void InitializeStartingData()
        {
            User bob = new User("Bob", "Bobsky", 23, "bob123", "Bob123", false);
            User rob = new User("Rob", "Robsky", 30, "rob123", "Rob123", false);
            User dan = new User("Dan", "Danielson", 28, "dan222", "Dan123", false);
            User jerry = new User("Jerry", "Jenson", 30, "jerry789", "Jerry789", false);
            User jessica = new User("Jessica", "Parker", 50, "jessica555", "Jessica555", false);
            User amy = new User("Amy", "White", 47, "amy456", "Amy456", false);
            User robin = new User("Robin", "Roben", 22, "robin777", "Robin777", false);
            User sarah = new User("Sarah", "Johnson", 35, "sarah456", "Sarah456", false);
            User david = new User("David", "Davis", 29, "david789", "David789", false);
            User emily = new User("Emily", "Emerson", 27, "emily111", "Emily111", false);
            User alex = new User("Alex", "Alexander", 40, "alex222", "Alex222", false);
            User samantha = new User("Samantha", "Smith", 33, "samantha777", "Samantha777", false);
            User jason = new User("Jason", "Jackson", 26, "jason333", "Jason333", false);
            User lisa = new User("Lisa", "Lee", 31, "lisa999", "Lisa999", false);
            User chris = new User("Chris", "Christensen", 45, "chris123", "Chris123", false);
            User michael = new User("Michael", "Miller", 38, "michael555", "Michael555", false);
            User linda = new User("Linda", "Lindberg", 50, "linda666", "Linda666", false);
            List<User> seedUsers = new List<User>() { bob, rob, dan, jerry, jessica, amy, robin, sarah, david, emily, alex, samantha, jason, lisa, chris, michael, linda };
            _userService.Seed(seedUsers);


        }
    }
}
