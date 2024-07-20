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
                    Messages.InvalidInputMessage("end Date");
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

            if (choice == "y")
            {
            EnterNewGroupNameLine: Messages.InputMessage("new group name");
                string newGroupName = Console.ReadLine();

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
                Messages.InvalidInputMessage("choice");
                goto EnterChoiceForGroupLimit;
            }

            if (choice == "y")
            {
            EnterNewGroupLimitLine: Messages.InputMessage("new group limit");
                string newGroupLimitInpout = Console.ReadLine();
                int newGroupLimit;
                isTrueFormat = int.TryParse(newGroupLimitInpout, out newGroupLimit);
                

                if (!isTrueFormat)
                {
                    Messages.InvalidInputMessage("new group limit");
                    goto EnterNewGroupLimitLine;
                }
            }

        }
    }
}
