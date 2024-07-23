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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityFramework_Task.Services
{
    internal static class GroupService
    {
        private static readonly AppDbContexts _contexts;

        static GroupService()
        {
            _contexts = new AppDbContexts();
        }

        public static void GetAllGroups()
        {
            if (_contexts.Groups.Count() <= 0)
                Messages.NotFoundMessage("Groups");

            foreach (var group in _contexts.Groups.ToList())
            {
                var teacher = _contexts.Teachers.FirstOrDefault(t => t.Id == group.TeacherId);

                Console.WriteLine($"Id: {group.Id} Name: {group.Name} Limit: {group.Limit} Begin Date: {group.BeginDate} End Date: {group.EndDate} " +
                    $"Teacher: {teacher.Name} {teacher.Surname}");
            }
        }
        public static void AddGroup()
        {
            if (_contexts.Teachers.Count() > 0)
            {
            EnterGroupNameLine: Messages.InputMessage("group name");
                string groupName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(groupName))
                {
                    Messages.InvalidInputMessage("Group name");
                    goto EnterGroupNameLine;
                }

                var existedGroup = _contexts.Groups.FirstOrDefault(g => g.Name == groupName);
                if (existedGroup is not null)
                {
                    Messages.AlreadyExistMessage(groupName);
                    goto EnterGroupNameLine;
                }

            EnterGroupLimitLine: Messages.InputMessage("group limit");
                string limitInput = Console.ReadLine();
                int limit;
                bool isTrueFormat = int.TryParse(limitInput, out limit);
                if (!isTrueFormat || limit <= 0)
                {
                    Messages.InvalidInputMessage("Group Limit");
                    goto EnterGroupLimitLine;
                }


            EnterGroupBeginDate: Messages.InputMessage("begin date (dd.MM.yyyy)");
                string beginDateInput = Console.ReadLine();
                DateTime beginDate;
                isTrueFormat = DateTime.TryParseExact(beginDateInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out beginDate);
                if (!isTrueFormat)
                {
                    Messages.InvalidInputMessage("Begin Date");
                    goto EnterGroupBeginDate;
                }

            EnterGroupEndDate: Messages.InputMessage("end date (dd.MM.yyyy)");
                string endDateInput = Console.ReadLine();
                DateTime endDate;
                isTrueFormat = DateTime.TryParseExact(endDateInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                if (!isTrueFormat || beginDate.Date.AddMonths(6).Date > endDate.Date)
                {
                    Messages.InvalidInputMessage("End Date");
                    goto EnterGroupEndDate;
                }


                TeacherService.GetAllTeachers();
            EnterTeacherIdLine: Messages.InputMessage("teacher Id");
                string teacherIdInput = Console.ReadLine();
                int teacherId;
                isTrueFormat = int.TryParse(teacherIdInput, out teacherId);
                if (!isTrueFormat)
                {
                    Messages.InvalidInputMessage("teacher Id");
                    goto EnterTeacherIdLine;
                }

                var teacher = _contexts.Teachers.FirstOrDefault(t => t.Id == teacherId);
                if (teacher is null)
                {
                    Messages.NotFoundMessage("Teacher");
                    goto EnterTeacherIdLine;
                }

                var groupCountOfTeacher = _contexts.Groups.Count(g => g.TeacherId == teacherId);
                if (groupCountOfTeacher >= 2)
                {
                    Messages.HasAlreadyMessage("Teacher", "2 groups");
                    goto EnterTeacherIdLine;
                }

                Entities.Group group = new Entities.Group
                {
                    Name = groupName,
                    Limit = limit,
                    BeginDate = beginDate,
                    EndDate = endDate,
                    TeacherId = teacherId
                };

                _contexts.Groups.Add(group);

                try
                {
                    _contexts.SaveChanges();
                }
                catch (Exception)
                {
                    Messages.ErrorHasOccured();
                }

                Messages.SuccessMessage("Group", "added");

            }
            else
                Messages.HasNotMessage("teacher", "to add group");
        }
        public static void UpdateGroup()
        {
            if (_contexts.Groups.Count() <= 0)
            {
                Messages.HasNotMessage("any group", "to show");
                return;
            }

            GetAllGroups();
        EnterGroupIdLine: Messages.InputMessage("group Id to update");
            string groupIdInput = Console.ReadLine();
            int groupId;
            bool isTrueFormat = int.TryParse(groupIdInput, out groupId);
            var existGroup = _contexts.Groups.FirstOrDefault(g => g.Id == groupId);
            if (!isTrueFormat || existGroup is null)
            {
                Messages.InvalidInputMessage("Group id");
                goto EnterGroupIdLine;
            }

        EnterChoiceForGroupName: Messages.WantToChangeMessage("group name");
            var choice = Console.ReadLine();
            if (!choice.IsValidChoice())
            {
                Messages.InvalidInputMessage("choice");
                goto EnterChoiceForGroupName;
            }

            string newGroupName = string.Empty;
            if (choice == "y")
            {
            EnterNewGroupNameLine: Messages.InputMessage("new group name");
                newGroupName = Console.ReadLine();

                var existGroupName = _contexts.Groups.FirstOrDefault(n => n.Name == newGroupName);
                if (string.IsNullOrWhiteSpace(newGroupName) || existGroupName is not null)
                {
                    Messages.InvalidInputMessage("new group name");
                    goto EnterNewGroupNameLine;
                }
            }

        EnterChoiceForGroupLimit: Messages.WantToChangeMessage("group limit");
            choice = Console.ReadLine();
            if (!choice.IsValidChoice())
            {
                Messages.InvalidInputMessage("Choice");
                goto EnterChoiceForGroupLimit;
            }
            int newGroupLimit = existGroup.Limit;
            if (choice == "y")
            {
            EnterNewGroupLimitLine: Messages.InputMessage("new group limit");
                string newGroupLimitInput = Console.ReadLine();
                isTrueFormat = int.TryParse(newGroupLimitInput, out newGroupLimit);

                int countOfStudents = _contexts.Students.Count(s => s.GroupId == groupId);

                if (countOfStudents > newGroupLimit)
                {
                    Messages.InputMessage("correct new limit or remove some students from group.");
                    return;
                }

                if (!isTrueFormat)
                {
                    Messages.InvalidInputMessage("new group limit");
                    goto EnterNewGroupLimitLine;
                }
            }

        EnterChoiceForGroupBeginDate: Messages.WantToChangeMessage("group begin date");
            choice = Console.ReadLine();
            if (!choice.IsValidChoice())
            {
                Messages.InvalidInputMessage("Choice");
                goto EnterChoiceForGroupBeginDate;
            }

            bool endDateChanged = false;
            DateTime newGroupBeginDate = existGroup.BeginDate;
            DateTime newGroupEndDate = existGroup.EndDate;
            if (choice == "y")
            {
            EnterNewGroupBeginDate: Messages.InputMessage("new group begin date");
                string newGroupBeginDateInput = Console.ReadLine();

                isTrueFormat = DateTime.TryParseExact(newGroupBeginDateInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out newGroupBeginDate);
                if (!isTrueFormat)
                {
                    Messages.InvalidInputMessage("new group begin date");
                    goto EnterNewGroupBeginDate;
                }

                if (newGroupBeginDate.Date.AddMonths(6) > newGroupEndDate.Date)
                {
                    Console.WriteLine("End Date must be at least 6 months later. Change end date");
                EnterNewGroupEndDate: Messages.InputMessage("new group end date");
                    string newGroupEndDateInput = Console.ReadLine();
     
                    isTrueFormat = DateTime.TryParseExact(newGroupEndDateInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out newGroupEndDate);
                    if (!isTrueFormat)
                    {
                        Messages.InvalidInputMessage("new group end date");
                        goto EnterNewGroupEndDate;
                    }

                    if (newGroupBeginDate.Date.AddMonths(6) > newGroupEndDate.Date)
                    {
                        Messages.InvalidInputMessage("End Date");
                        goto EnterNewGroupEndDate;
                    }
                    endDateChanged = true;
                }
            }
            if (!endDateChanged)
            {
            EnterChoiceForGroupEndDate: Messages.WantToChangeMessage("group end date");
                choice = Console.ReadLine();
                if (!choice.IsValidChoice())
                {
                    Messages.InvalidInputMessage("Choice");
                    goto EnterChoiceForGroupEndDate;
                }

                if (choice == "y")
                {
                EnterNewGroupEndDate: Messages.InputMessage("new group end date");
                    string newGroupEndDateInput = Console.ReadLine();


                    isTrueFormat = DateTime.TryParseExact(newGroupEndDateInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out newGroupEndDate);
                    if (!isTrueFormat)
                    {
                        Messages.InvalidInputMessage("new group end date");
                        goto EnterNewGroupEndDate;
                    }

                    if (existGroup.BeginDate.Date.AddMonths(6).Date > newGroupEndDate.Date )
                    {
                        Messages.InvalidInputMessage("End Date");
                        goto EnterNewGroupEndDate;
                    }
                }
            }

        EnterChoiceForTeacher: Messages.WantToChangeMessage("teacher");
            choice = Console.ReadLine();
            if (!choice.IsValidChoice())
            {
                Messages.InvalidInputMessage("choice");
                goto EnterChoiceForTeacher;
            }

            int teacherId = existGroup.TeacherId;

            if (choice == "y")
            {
                TeacherService.GetAllTeachers();
            EnterTeacherIdLine: Messages.InputMessage("teacher id");
                string teacherIdInput = Console.ReadLine();
                isTrueFormat = int.TryParse(teacherIdInput, out teacherId);
                var teacher = _contexts.Teachers.FirstOrDefault(t => t.Id == teacherId);

                if (!isTrueFormat || teacher is null)
                {
                    Messages.InvalidInputMessage("teacher id");
                    goto EnterTeacherIdLine;
                }

                var groupCountOfTeacher = _contexts.Groups.Count(g => g.TeacherId == teacherId);
                if (groupCountOfTeacher >= 2)
                {
                    Messages.HasAlreadyMessage("Teacher", "2 groups");
                    goto EnterTeacherIdLine;
                }
            }

            if (!string.IsNullOrEmpty(newGroupName)) { existGroup.Name = newGroupName; }
            if (newGroupLimit != existGroup.Limit) { existGroup.Limit = existGroup.Limit; }
            if (newGroupBeginDate != existGroup.BeginDate) { existGroup.BeginDate = newGroupBeginDate; }
            if (newGroupEndDate != existGroup.EndDate) { existGroup.EndDate = newGroupEndDate;  }
            if (teacherId != existGroup.TeacherId) { existGroup.TeacherId = teacherId; }

            try
            {
                _contexts.SaveChanges();
            }
            catch (Exception)
            {
                Messages.ErrorHasOccured();
            }


            Messages.SuccessMessage("Group", "updated");
        }
        public static void DeleteGroup()
        {
            GetAllGroups();
        InputIdLine: Messages.InputMessage("group Id");
            var inputId = Console.ReadLine();
            int Id;
            bool isTrueIdFormat = int.TryParse(inputId, out Id);
            if (!isTrueIdFormat)
            {
                Messages.InvalidInputMessage("Group ID");
                goto InputIdLine;
            }

            var group = _contexts.Groups.Find(Id);
            if (group is null)
            {
                Messages.NotFoundMessage("Group");
                return;
            }

            _contexts.Groups.Remove(group);

            try
            {
                _contexts.SaveChanges();
            }
            catch (Exception)
            {
                Messages.ErrorHasOccured();
            }

            Messages.SuccessMessage("Group", "deleted");
        }
        public static void GetDetailsOfGroup()
        {
            GetAllGroups();

        InputIdLine: Messages.InputMessage("group Id");
            var inputId = Console.ReadLine();
            int input;
            bool isTrueIdFormat = int.TryParse(inputId, out input);
            if (!isTrueIdFormat)
            {
                Messages.InvalidInputMessage("group Id");
                goto InputIdLine;
            }

            var group = _contexts.Groups.FirstOrDefault(g => g.Id == input);
            if (group is null)
            {
                Messages.NotFoundMessage("student");
                return;
            }

            var teacher = _contexts.Teachers.FirstOrDefault(t => t.Id == group.TeacherId);
            Console.WriteLine($"Id: {group.Id} Name: {group.Name} Limit: {group.Limit} " +
                $"Begin Date: {group.BeginDate} End Date: {group.EndDate} Teacher: {teacher.Name} {teacher.Surname} ");


        }
    }
}
