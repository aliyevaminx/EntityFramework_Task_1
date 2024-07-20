using EntityFramework_Task.Constant;
using EntityFramework_Task.Services;

public static class Program
{
    public static void Main()
    {
        while (true) 
        { 
            ShowMenu();
            
            string choiceInput = Console.ReadLine();
            int choice;
            bool isTrueFormat = int.TryParse(choiceInput, out choice);
            if (isTrueFormat )
            {
                switch ((Options)choice)
                {
                    case Options.Exit:
                        return;
                    case Options.AllTeachers:
                        TeacherService.GetAllTeachers();
                        break;
                    case Options.AddTeacher:
                        TeacherService.AddTeacher();
                        break;
                    case Options.UpdateTeacher:
                        TeacherService.UpdateTeacher();
                        break;
                    case Options.DeleteTeacher:
                        TeacherService.DeleteTeacher();
                        break;
                    case Options.DetailsOfTeacher:
                        TeacherService.GetDetailsOfTeacher();
                        break;
                    case Options.AllGroups:
                        GroupService.GetAllGroups();
                        break;
                    case Options.AddGroup:
                        GroupService.AddGroup();
                        break;
                    case Options.UpdateGroup:
                        GroupService.UpdateGroup();
                        break;
                    default:
                        Messages.InvalidInputMessage("Choice");
                        break;
                }
            }
        }
    }

    public static void ShowMenu() 
    {
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. All Teachers");
        Console.WriteLine("2. Add Teacher");
        Console.WriteLine("3. Update Teacher");
        Console.WriteLine("4. Delete Teacher");
        Console.WriteLine("5. Details of Teacher");
        Console.WriteLine("6. All Groups");
        Console.WriteLine("7. Add Group");
        Console.WriteLine("8. Update Group");
        Console.WriteLine("9. Delete Group");
        Console.WriteLine("10. Details of Group");
    }
}
