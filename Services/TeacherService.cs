using EntityFramework_Task.Constant;
using EntityFramework_Task.Contexts;
using EntityFramework_Task.Entities;
using EntityFramework_Task.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EntityFramework_Task.Services
{
    internal static class TeacherService
    {
        private static readonly AppDbContexts _contexts;

        static TeacherService()
        {
            _contexts = new AppDbContexts();
        }

        public static void GetAllTeachers()
        {
            foreach (var teacher in _contexts.Teachers.ToList())
            {
                Console.WriteLine($"Id: {teacher.Id} Name: {teacher.Name} Surname: {teacher.Surname}");
            }
        }

        public static void AddTeacher()
        {
        EnterTeacherNameLine: Messages.InputMessage("teacher name");
            string teacherName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(teacherName))
            {
                Messages.InvalidInputMessage("Teacher name");
                goto EnterTeacherNameLine;
            }

        EnterTeacherSurnameLine: Messages.InputMessage("teacher surname");
            string teacherSurname = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(teacherSurname))
            {
                Messages.InvalidInputMessage("Teacher surname");
                goto EnterTeacherSurnameLine;
            }

            Teacher teacher = new Teacher
            {
                Name = teacherName,
                Surname = teacherSurname
            };

            _contexts.Add(teacher);
            try
            {
                _contexts.SaveChanges();
            }
            catch (Exception)
            {
                Messages.ErrorHasOccured();
            }

            Messages.SuccessMessage("Teacher", "added");
        }

        public static void UpdateTeacher()
        {
            GetAllTeachers();

        EnterTeacherIdLine: Messages.InputMessage("teacher id");
            string teacherIdInput = Console.ReadLine();
            int teacherId;
            bool isTrueFormat = int.TryParse(teacherIdInput, out teacherId);

            if (!isTrueFormat)
            {
                Messages.InvalidInputMessage("Teacher Id");
                goto EnterTeacherIdLine;
            }

            var teacher = _contexts.Teachers.Find(teacherId);
            if (teacher is null)
            {
                Messages.NotFoundMessage("Teacher");
                return;
            }

        WantToChangeNameLine: Messages.WantToChangeMessage("teacher name");
            var choice = Console.ReadLine();
            if (!choice.IsValidChoice())
            {
                Messages.InvalidInputMessage("Choice");
                goto WantToChangeNameLine;
            }

            string newName = string.Empty;
            if (choice == "y")
            {
            NewNameInputLine: Messages.InputMessage("new name");
                newName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newName))
                    goto NewNameInputLine;
            }

        WantToChangeSurnameLine: Messages.WantToChangeMessage("teacher surname");
            choice = Console.ReadLine();
            if (!choice.IsValidChoice())
            {
                Messages.InvalidInputMessage("Choice");
                goto WantToChangeSurnameLine;
            }

            string newSurname = string.Empty;
            if (choice == "y")
            {
            NewSurnameInputLine: Messages.InputMessage("new surname");
                newSurname = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newSurname))
                    goto NewSurnameInputLine;
            }

            if (!string.IsNullOrEmpty(newName)) { teacher.Name = newName; }
            if (!string.IsNullOrEmpty(newSurname)) { teacher.Surname = newSurname; }

            _contexts.Teachers.Update(teacher);

            try
            {
                _contexts.SaveChanges();
            }
            catch (Exception)
            {
                Messages.ErrorHasOccured();
            }

            Messages.SuccessMessage("Teacher", "updated");
        }

        public static void DeleteTeacher()
        {
            GetAllTeachers();
        InputIdLine: Messages.InputMessage("teacher Id");
            var inputId = Console.ReadLine();
            int Id;
            bool isTrueIdFormat = int.TryParse(inputId, out Id);
            if (!isTrueIdFormat)
            {
                Messages.InvalidInputMessage("Teacher ID");
                goto InputIdLine;
            }

            var teacher = _contexts.Teachers.Find(Id);
            if (teacher is null)
            {
                Messages.NotFoundMessage("Teacher");
                return;
            }

            _contexts.Teachers.Remove(teacher);

            try
            {
                _contexts.SaveChanges();
            }
            catch (Exception)
            {
                Messages.ErrorHasOccured();
            }

            Messages.SuccessMessage("Teacher", "deleted");
        }

        public static void GetDetailsOfTeacher()
        {
            GetAllTeachers();
        InputIdLine: Messages.InputMessage("teacher Id");
            var inputId = Console.ReadLine();
            int input;
            bool isTrueIdFormat = int.TryParse(inputId, out input);
            if (!isTrueIdFormat)
            {
                Messages.InvalidInputMessage("teacher Id");
                goto InputIdLine;
            }

            var teacher = _contexts.Teachers.Find(input);
            if (teacher is null) 
            {
                Messages.NotFoundMessage("teacher");
                return;
            }

            Console.WriteLine($"Id: {teacher.Id} Name: {teacher.Name} Surname: {teacher.Surname}");


        }
    }
}
