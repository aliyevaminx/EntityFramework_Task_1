using EntityFramework_Task.Constant;
using EntityFramework_Task.Contexts;
using EntityFramework_Task.Entities;
using EntityFramework_Task.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EntityFramework_Task.Services
{
    internal static class StudentService
    {
        private static readonly AppDbContexts _contexts;

        static StudentService()
        {
            _contexts = new AppDbContexts();
        }

        public static void GetAllStudents()
        {
            foreach (var student in _contexts.Students.ToList())
            {
                Console.WriteLine($"Id: {student.Id} Name: {student.Name} Surname: {student.Surname}");
            }
        }

        public static void AddStudent()
        {
        EnterStudentNameLine: Messages.InputMessage("student name");
            string studentName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(studentName))
            {
                Messages.InvalidInputMessage("Student name");
                goto EnterStudentNameLine;
            }

        EnterStudentSurnameLine: Messages.InputMessage("student surname");
            string studentSurname = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(studentSurname))
            {
                Messages.InvalidInputMessage("student surname");
                goto EnterStudentSurnameLine;
            }

        EnterStudentEmailLine: Messages.InputMessage("student email");
            string studentEmail = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(studentEmail) || !studentEmail.IsValidEmail())
            {
                Messages.InvalidInputMessage("Email");
                goto EnterStudentEmailLine;
            }

        EnterStudentBirthDate: Messages.InputMessage("birth date(dd.MM.yyyy)");
            string birthDateInput = Console.ReadLine();
            DateTime birthDate;
            bool isTrueFormat = DateTime.TryParseExact(birthDateInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate);

            if (!isTrueFormat)
            {
                Messages.InvalidInputMessage("Birth date");
                goto EnterStudentBirthDate;
            }

            GroupService.GetAllGroups();
        EnterGroupIdAddStudent: Messages.InputMessage("group id");
            string groupIdInput = Console.ReadLine();
            int groupId;
            isTrueFormat = int.TryParse(groupIdInput, out groupId);
            var existGroup = _contexts.Groups.FirstOrDefault(g => g.Id == groupId);
            if (!isTrueFormat || existGroup is null)
            {
                Messages.InvalidInputMessage("group id");
                goto EnterGroupIdAddStudent;
            }


            Student student = new Student
            {
                Name = studentName,
                Surname = studentSurname,
                Email = studentEmail,
                BirthDate = birthDate,
                GroupId = groupId
            };

            _contexts.Add(student);
            try
            {
                _contexts.SaveChanges();
            }
            catch (Exception)
            {
                Messages.ErrorHasOccured();
            }

            Messages.SuccessMessage("Student", "added");
        }

        public static void UpdateTeacher()
        {
            GetAllStudents();

        EnterStudentIdLine: Messages.InputMessage("student id");
            string studentIdInput = Console.ReadLine();
            int studentId;
            bool isTrueFormat = int.TryParse(studentIdInput, out studentId);

            if (!isTrueFormat)
            {
                Messages.InvalidInputMessage("Student Id");
                goto EnterStudentIdLine;
            }

            var student = _contexts.Students.Find(studentId);
            if (student is null)
            {
                Messages.NotFoundMessage("Student");
                return;
            }

        WantToChangeNameLine: Messages.WantToChangeMessage("student name");
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

        WantToChangeSurnameLine: Messages.WantToChangeMessage("student surname");
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

        WantToChangeEmailLine: Messages.WantToChangeMessage("student email");
            choice = Console.ReadLine();
            if (!choice.IsValidChoice())
            {
                Messages.InvalidInputMessage("Choice");
                goto WantToChangeEmailLine;
            }

            string newEmail = string.Empty;
            if (choice == "y")
            {
            NewEmailInputLine: Messages.InputMessage("new surname");
                newEmail = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newSurname) || !newEmail.IsValidEmail())
                    goto NewEmailInputLine;
            }

        WantToChangeBirthDateLine: Messages.WantToChangeMessage("student birth date");
            choice = Console.ReadLine();
            if (!choice.IsValidChoice())
            {
                Messages.InvalidInputMessage("Choice");
                goto WantToChangeBirthDateLine;
            }

            string newBirthDate = string.Empty;
            if (choice == "y")
            {
            NewBirthDateInputLine: Messages.InputMessage("new birth date");
                newBirthDate = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newBirthDate))
                    goto NewBirthDateInputLine;
            }

            if (!string.IsNullOrEmpty(newName)) { student.Name = newName; }
            if (!string.IsNullOrEmpty(newSurname)) { student.Surname = newSurname; }
            if (!string.IsNullOrEmpty(newEmail)) { student.Email = newEmail; }

            _contexts.Students.Update(student);

            try
            {
                _contexts.SaveChanges();
            }
            catch (Exception)
            {
                Messages.ErrorHasOccured();
            }

            Messages.SuccessMessage("student", "updated");
        }

        public static void DeleteTeacher()
        {
            GetAllStudents();
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