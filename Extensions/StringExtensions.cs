using EntityFramework_Task.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityFramework_Task.Extensions
{
    internal static class StringExtensions
    {
        public static bool IsValidChoice(this string choice)
        {
            if (choice == "y" || choice == "n")
                 return true;
            return false;
        }

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                return regex.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
