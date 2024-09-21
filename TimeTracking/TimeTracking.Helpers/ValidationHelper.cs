namespace TimeTracking.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateStringInput(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        public static int ValidateNumberInput(string number, int range)
        {
            bool isNumber = int.TryParse(number, out int parsedNumber);
            if(!isNumber || parsedNumber > range || parsedNumber <= 0)
            {
                return -1;
            }
            return parsedNumber;
        }

        public static bool ValidInputLength(string input, int length)
        {
            if(input.Length < length)
            {
                ExtendedConsole.PrintError($"The input must be at least {length} characters long");
                return false;
            }
            return true;
        }

        public static bool ValidPasswordInput(string password)
        {
            bool hasCapitaLetter = false;
            bool hasNumber = false;
            bool validPassword = false;

            foreach(char letter in password)
            {
                if (char.IsUpper(letter))
                {
                    hasCapitaLetter = true;
                }
                else if(char.IsDigit(letter))
                {
                    hasNumber = true;
                }

                if(hasCapitaLetter && hasNumber)
                {
                    validPassword = true;
                    break;
                }
            }
            return validPassword;
        }

        public static bool ValidatePasswordEasier(string password)
        {
            return password.Length >= 6 && ValidPasswordInput(password);
        }

        public static bool ValidateFirstNameOnly(string firstName)
        {
            
            if(firstName.Length < 2)
            {
                return false;
            }
            foreach (char letter in firstName)
            {
                if (char.IsDigit(letter))
                {
                    return false;
                    
                }
            }
            return true;

        }
        public static bool ValidateLastNameOnly(string lastName)
        {
            
            if (lastName.Length < 2)
            {
                return false;
            }
            foreach (char letter in lastName)
            {
                if (char.IsDigit(letter))
                {
                    return false;
                }
            }
            return true;

        }
        public static bool ValidFirstAndLastName(string firstName, string lastName)
        {
            bool firstNameHasNoNumber = true;
            bool lastNameHasNoNumber = true;
            if(firstName.Length < 2 || lastName.Length < 2)
            {
                return false;
            }
            
            foreach(char letter in firstName)
            {
                if (char.IsDigit(letter))
                {
                    firstNameHasNoNumber = false;
                    break;
                }
            }
            foreach(char letter in lastName)
            {
                if (char.IsDigit(letter))
                {
                    lastNameHasNoNumber = false;
                    break;
                }
            }
            return firstNameHasNoNumber && lastNameHasNoNumber;
        }

        public static bool ValidAgeInput(int age, int bottomLimit, int upperLimit)
        {
            if(age < bottomLimit || age > upperLimit)
            {
                return false;
            }
            return true;
        }


        
    }
}
