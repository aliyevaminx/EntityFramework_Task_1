using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EntityFramework_Task.Constant
{
    internal class Messages
    {
        public static void InputMessage(string title) => Console.WriteLine($"Enter {title}");
        public static void InvalidInputMessage(string title) => Console.WriteLine($"{title} is invalid");
        public static void ErrorHasOccured() => Console.WriteLine("Error has occured");
        public static void SuccessMessage(string title, string process) => Console.WriteLine($"{title} successfully {process}");
        public static void NotFoundMessage(string title) => Console.WriteLine($"{title} not found");
        public static void WantToChangeMessage(string title) => Console.WriteLine($"Do you want to change {title}? y or n");
        public static void HasNotMessage(string title, string content) => Console.WriteLine($"There is not {title} {content}");
        public static void AlreadyExistMessage(string title) => Console.WriteLine($"{title} already exists");
        public static void HasAlreadyMessage(string title, string content) => Console.WriteLine($"{title} has already {content}");
    }
}
