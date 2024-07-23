using EntityFramework_Task.Constant;
using EntityFramework_Task.Contexts;
using EntityFramework_Task.Entities;
using EntityFramework_Task.Extensions;
using Microsoft.EntityFrameworkCore;
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

            if (DateTime.Now.Year - birthDate.Year < 6)
            {
                Messages.MustBeGivenYearsOld("6");
                return;
            }

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
            var studentsCount = _contexts.Students.Count(s => s.GroupId == groupId);

            if (studentsCount > existGroup.Limit)
            {
                Messages.HasAlreadyMessage("Group", "filled");
                return;
            }

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

        public static void UpdateStudent()
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

            DateTime newBirthDate = student.BirthDate;
            if (choice == "y")
            {
            NewBirthDateInputLine: Messages.InputMessage("new birth date");
                string newBirthDateInput = Console.ReadLine();
                isTrueFormat = DateTime.TryParseExact(newBirthDateInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out newBirthDate);

                if (DateTime.Now.Year - newBirthDate.Year < 6)
                {
                    Messages.MustBeGivenYearsOld("6");
                    return;
                }

                if (!isTrueFormat)
                {
                    Messages.InvalidInputMessage("new birth date");
                    goto NewBirthDateInputLine;
                }
            }



        WantToChangeGroupIdLine: Messages.WantToChangeMessage("student group");

            choice = Console.ReadLine();
            if (!choice.IsValidChoice())
            {
                Messages.InvalidInputMessage("Choice");
                goto WantToChangeGroupIdLine;
            }

            int newGroupId = student.GroupId;
            if (choice == "y")
            {
                GroupService.GetAllGroups();
            EnterNewGroupIdLine: Messages.InputMessage("new group id"); 
                string newGroupIdInput = Console.ReadLine();
                isTrueFormat = int.TryParse(newGroupIdInput, out newGroupId);

                var studentsCount = _contexts.Students.Count(s => s.GroupId == newGroupId);
                var group = _contexts.Groups.FirstOrDefault(g => g.Id == newGroupId);

                if (studentsCount >= group.Limit)
                {
                    Messages.HasAlreadyMessage("Group", "filled");
                    return;
                }

                if (!isTrueFormat || group is null)
                {
                    Messages.InvalidInputMessage("group id");
                    goto WantToChangeGroupIdLine;
                }
            }

            if (!string.IsNullOrEmpty(newName)) { student.Name = newName; }
            if (!string.IsNullOrEmpty(newSurname)) { student.Surname = newSurname; }
            if (!string.IsNullOrEmpty(newEmail)) { student.Email = newEmail; }
            if (newBirthDate != student.BirthDate) { student.BirthDate = newBirthDate; }
            if (newGroupId != student.GroupId) { student.GroupId = newGroupId; }

            _contexts.Students.Update(student);

            try
            {
                _contexts.SaveChanges();
            }
            catch (Exception)
            {
                Messages.ErrorHasOccured();
            }

            Messages.SuccessMessage("Student", "updated");
        }

        public static void DeleteStudent()
        {
            GetAllStudents();
        InputIdLine: Messages.InputMessage("student Id");
            var inputId = Console.ReadLine();
            int Id;
            bool isTrueIdFormat = int.TryParse(inputId, out Id);
            if (!isTrueIdFormat)
            {
                Messages.InvalidInputMessage("Student ID");
                goto InputIdLine;
            }

            var student = _contexts.Students.Find(Id);
            if (student is null)
            {
                Messages.NotFoundMessage("Student");
                return;
            }

            _contexts.Students.Remove(student);

            try
            {
                _contexts.SaveChanges();
            }
            catch (Exception)
            {
                Messages.ErrorHasOccured();
            }

            Messages.SuccessMessage("Student", "deleted");
        }

        public static void GetDetailsOfStudent()
        {
            GetAllStudents();

        InputIdLine: Messages.InputMessage("student Id");
            var inputId = Console.ReadLine();
            int input;
            bool isTrueIdFormat = int.TryParse(inputId, out input);
            if (!isTrueIdFormat)
            {
                Messages.InvalidInputMessage("student Id");
                goto InputIdLine;
            }

            var student = _contexts.Students.Find(input);
            if (student is null)
            {
                Messages.NotFoundMessage("student");
                return;
            }

            var group = _contexts.Groups.FirstOrDefault(g => g.Id == student.GroupId);
            Console.WriteLine($"Id: {student.Id} Name: {student.Name} Surname: {student.Surname} " +
                $"Email: {student.Email} Birth Date: {student.BirthDate} Group Name: {group.Name} ");


        }
    }
}