namespace Praktuchna_4_Данчук_Артур;

public static class Program
{
    private static StudentGroup _group = CreateDefaultGroup();
    private const string DataFileName = "student_group.json";

    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        var running = true;

        while (running)
        {
            ShowMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddStudent();
                    break;
                case "2":
                    RemoveStudent();
                    break;
                case "3":
                    ShowAllStudents();
                    break;
                case "4":
                    SearchStudent();
                    break;
                case "5":
                    EditStudent();
                    break;
                case "6":
                    ShowExcellentStudents();
                    break;
                case "7":
                    ShowStatistics();
                    break;
                case "8":
                    SaveOrLoad();
                    break;
                case "9":
                    AssignCurator();
                    break;
                case "10":
                    ShowCuratorInfo();
                    break;
                case "11":
                    SortStudentsByGrade();
                    break;
                case "12":
                    CloneStudent();
                    break;
                case "13":
                    CompareStudentsByGrade();
                    break;
                case "14":
                    GetStudentByIndexer();
                    break;
                case "15":
                    CheckScholarshipEligibility();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Невірний пункт меню.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private static StudentGroup CreateDefaultGroup()
    {
        return new StudentGroup
        {
            GroupName = "КН-21",
            Specialty = "Комп'ютерні науки",
            Course = 2
        };
    }

    private static void ShowMenu()
    {
        Console.WriteLine("=== Управління групою студентів ===");
        Console.WriteLine($"Група: {_group.GroupName} | Спеціальність: {_group.Specialty} | Курс: {_group.Course}");
        Console.WriteLine("1. Додати студента");
        Console.WriteLine("2. Видалити студента");
        Console.WriteLine("3. Вивести всіх студентів");
        Console.WriteLine("4. Пошук студента");
        Console.WriteLine("5. Редагування студента");
        Console.WriteLine("6. Відмінники");
        Console.WriteLine("7. Статистика");
        Console.WriteLine("8. Зберегти / завантажити");
        Console.WriteLine("9. Призначити куратора групи");
        Console.WriteLine("10. Вивести інформацію про куратора");
        Console.WriteLine("11. Відсортувати студентів за середнім балом");
        Console.WriteLine("12. Клонувати студента");
        Console.WriteLine("13. Порівняти двох студентів за балами");
        Console.WriteLine("14. Отримати студента за індексом або заліковкою");
        Console.WriteLine("15. Перевірити право на стипендію");
        Console.WriteLine("0. Вихід");
        Console.Write("Оберіть пункт: ");
    }

    private static void AddStudent()
    {
        try
        {
            Console.Write("ПІБ: ");
            var fullName = Console.ReadLine() ?? string.Empty;

            Console.Write("Дата народження (dd.MM.yyyy): ");
            var dateOfBirth = ReadDate();

            Console.Write("Номер залікової (8 цифр): ");
            var recordBookNumber = Console.ReadLine() ?? string.Empty;

            Console.Write("Середній бал (0-100): ");
            var averageGrade = ReadDouble();

            Console.WriteLine("Статус: 1-Active, 2-AcademicLeave, 3-Expelled, 4-Graduated");
            Console.Write("Оберіть статус: ");
            var status = ReadStatus();

            Console.Write("Дата зарахування (dd.MM.yyyy): ");
            var enrollmentDate = ReadDate();

            Console.Write("Email: ");
            var email = Console.ReadLine() ?? string.Empty;

            Console.Write("Примітки: ");
            var notes = Console.ReadLine() ?? string.Empty;

            var student = new Student(
                fullName,
                dateOfBirth,
                recordBookNumber,
                averageGrade,
                status,
                enrollmentDate,
                email,
                notes);

            _group.AddStudent(student);
            Console.WriteLine("Студента успішно додано.");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Помилка: {exception.Message}");
        }
    }

    private static void RemoveStudent()
    {
        Console.Write("Введіть номер залікової або ПІБ: ");
        var query = Console.ReadLine() ?? string.Empty;

        if (_group.RemoveStudent(query))
        {
            Console.WriteLine("Студента видалено.");
        }
        else
        {
            Console.WriteLine("Студента не знайдено.");
        }
    }

    private static void ShowAllStudents()
    {
        if (_group.GroupSize == 0)
        {
            Console.WriteLine("Група порожня.");
            return;
        }

        var index = 1;

        foreach (var student in _group.Students)
        {
            Console.WriteLine($"--- Студент {index} ---");
            student.ShowDetailedInfo();
            index++;
        }
    }

    private static void SearchStudent()
    {
        Console.Write("Введіть номер залікової або ПІБ: ");
        var query = Console.ReadLine() ?? string.Empty;
        var student = _group.FindStudent(query);

        if (student == null)
        {
            Console.WriteLine("Студента не знайдено.");
            return;
        }

        student.ShowDetailedInfo();
    }

    private static void EditStudent()
    {
        Console.Write("Введіть номер залікової або ПІБ: ");
        var query = Console.ReadLine() ?? string.Empty;
        var student = _group.FindStudent(query);

        if (student == null)
        {
            Console.WriteLine("Студента не знайдено.");
            return;
        }

        try
        {
            Console.WriteLine("1. Змінити ПІБ");
            Console.WriteLine("2. Змінити середній бал");
            Console.WriteLine("3. Змінити статус");
            Console.WriteLine("4. Змінити email");
            Console.WriteLine("5. Змінити примітки");
            Console.Write("Оберіть поле: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Нове ПІБ: ");
                    student.FullName = Console.ReadLine() ?? string.Empty;
                    break;
                case "2":
                    Console.Write("Новий середній бал: ");
                    student.UpdateAverageGrade(ReadDouble());
                    break;
                case "3":
                    Console.WriteLine("1-Active, 2-AcademicLeave, 3-Expelled, 4-Graduated");
                    student.Status = ReadStatus();
                    break;
                case "4":
                    Console.Write("Новий email: ");
                    student.PersonalEmail = Console.ReadLine() ?? string.Empty;
                    break;
                case "5":
                    Console.Write("Нові примітки: ");
                    student.SetNotes(Console.ReadLine() ?? string.Empty);
                    break;
                default:
                    Console.WriteLine("Невірний пункт.");
                    return;
            }

            Console.WriteLine("Дані оновлено.");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Помилка: {exception.Message}");
        }
    }

    private static void ShowExcellentStudents()
    {
        var excellentStudents = _group.GetExcellentStudents();

        if (excellentStudents.Count == 0)
        {
            Console.WriteLine("Відмінників немає.");
            return;
        }

        foreach (var student in excellentStudents)
        {
            Console.WriteLine($"{student.FullName} | {student.AverageGrade:F2} | {student.RecordBookNumber}");
        }
    }

    private static void ShowStatistics()
    {
        Console.WriteLine($"Розмір групи: {_group.GroupSize}");
        Console.WriteLine($"Середній бал групи: {_group.AverageGroupGrade:F2}");
        Console.WriteLine($"Активних: {_group.GetStudentsByStatus(StudentStatus.Active).Count}");
        Console.WriteLine($"На академічній відпустці: {_group.GetStudentsByStatus(StudentStatus.AcademicLeave).Count}");
        Console.WriteLine($"Відрахованих: {_group.GetStudentsByStatus(StudentStatus.Expelled).Count}");
        Console.WriteLine($"Випускників: {_group.GetStudentsByStatus(StudentStatus.Graduated).Count}");
        Console.WriteLine($"Відмінників: {_group.GetExcellentStudents().Count}");
        Console.WriteLine($"Студентів з низьким балом: {_group.Students.Count(student => student.IsFailing())}");
        Console.WriteLine($"Куратор: {(_group.Curator == null ? "не призначено" : _group.Curator.FullName)}");
    }

    private static void SaveOrLoad()
    {
        Console.WriteLine("1. Зберегти у файл");
        Console.WriteLine("2. Завантажити з файлу");
        Console.Write("Оберіть дію: ");
        var choice = Console.ReadLine();

        try
        {
            if (choice == "1")
            {
                _group.SaveToFile(DataFileName);
                Console.WriteLine($"Дані збережено у файл {DataFileName}.");
            }
            else if (choice == "2")
            {
                _group = StudentGroup.LoadFromFile(DataFileName);
                Console.WriteLine("Дані успішно завантажено.");
            }
            else
            {
                Console.WriteLine("Невірний пункт.");
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Помилка: {exception.Message}");
        }
    }

    private static void AssignCurator()
    {
        try
        {
            Console.Write("ПІБ куратора: ");
            var fullName = Console.ReadLine() ?? string.Empty;

            Console.Write("Дата народження (dd.MM.yyyy): ");
            var dateOfBirth = ReadDate();

            Console.Write("Email: ");
            var email = Console.ReadLine() ?? string.Empty;

            Console.Write("Кафедра: ");
            var department = Console.ReadLine() ?? string.Empty;

            Console.Write("Стаж (років): ");
            var experienceYears = ReadInt();

            _group.Curator = new Teacher(fullName, dateOfBirth, email, department, experienceYears);
            Console.WriteLine("Куратора групи успішно призначено.");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Помилка: {exception.Message}");
        }
    }

    private static void ShowCuratorInfo()
    {
        if (_group.Curator == null)
        {
            Console.WriteLine("Куратор групи не призначений.");
            return;
        }

        Person person = _group.Curator;
        person.ShowDetailedInfo();
    }

    private static void SortStudentsByGrade()
    {
        if (_group.GroupSize == 0)
        {
            Console.WriteLine("Група порожня.");
            return;
        }

        _group.SortStudentsByGrade();
        Console.WriteLine("Студентів відсортовано за середнім балом (за спаданням):");

        foreach (var student in _group.Students)
        {
            Console.WriteLine($"{student.FullName} | {student.AverageGrade:F2}");
        }
    }

    private static void CloneStudent()
    {
        Console.Write("Введіть номер залікової для клонування: ");
        var recordBookNumber = Console.ReadLine() ?? string.Empty;

        try
        {
            var clone = _group.CloneStudent(recordBookNumber);

            if (clone == null)
            {
                Console.WriteLine("Студента не знайдено.");
                return;
            }

            Console.WriteLine("Копію студента створено:");
            clone.ShowDetailedInfo();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Помилка: {exception.Message}");
        }
    }

    private static void CompareStudentsByGrade()
    {
        if (_group.GroupSize < 2)
        {
            Console.WriteLine("Для порівняння потрібно щонайменше двох студентів.");
            return;
        }

        Console.Write("Номер залікової першого студента: ");
        var firstQuery = Console.ReadLine() ?? string.Empty;
        Console.Write("Номер залікової другого студента: ");
        var secondQuery = Console.ReadLine() ?? string.Empty;

        var firstStudent = _group.FindStudent(firstQuery);
        var secondStudent = _group.FindStudent(secondQuery);

        if (firstStudent == null || secondStudent == null)
        {
            Console.WriteLine("Один або обидва студенти не знайдені.");
            return;
        }

        if (firstStudent > secondStudent)
        {
            Console.WriteLine($"{firstStudent.FullName} має вищий бал ({firstStudent.AverageGrade:F2}) за {secondStudent.FullName} ({secondStudent.AverageGrade:F2}).");
        }
        else if (firstStudent < secondStudent)
        {
            Console.WriteLine($"{secondStudent.FullName} має вищий бал ({secondStudent.AverageGrade:F2}) за {firstStudent.FullName} ({firstStudent.AverageGrade:F2}).");
        }
        else
        {
            Console.WriteLine("Студенти мають однаковий середній бал.");
        }
    }

    private static void GetStudentByIndexer()
    {
        if (_group.GroupSize == 0)
        {
            Console.WriteLine("Група порожня.");
            return;
        }

        Console.WriteLine("1. За індексом у списку");
        Console.WriteLine("2. За номером залікової");
        Console.Write("Оберіть спосіб: ");
        var choice = Console.ReadLine();

        try
        {
            Student student;

            if (choice == "1")
            {
                Console.Write($"Введіть індекс (0-{_group.GroupSize - 1}): ");
                var index = ReadInt();
                student = _group[index];
            }
            else if (choice == "2")
            {
                Console.Write("Введіть номер залікової: ");
                var recordBookNumber = Console.ReadLine() ?? string.Empty;
                student = _group[recordBookNumber];
            }
            else
            {
                Console.WriteLine("Невірний пункт.");
                return;
            }

            student.ShowDetailedInfo();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Помилка: {exception.Message}");
        }
    }

    private static void CheckScholarshipEligibility()
    {
        Console.Write("Введіть номер залікової або ПІБ: ");
        var query = Console.ReadLine() ?? string.Empty;
        var student = _group.FindStudent(query);

        if (student == null)
        {
            Console.WriteLine("Студента не знайдено.");
            return;
        }

        if (student.IsEligibleForScholarship())
        {
            Console.WriteLine($"{student.FullName} має право на стипендію.");
        }
        else
        {
            Console.WriteLine($"{student.FullName} не має права на стипендію.");
        }
    }

    private static DateTime ReadDate()
    {
        var input = Console.ReadLine() ?? string.Empty;

        if (!DateTime.TryParseExact(input, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
        {
            throw new FormatException("Некоректний формат дати.");
        }

        return date;
    }

    private static double ReadDouble()
    {
        var input = Console.ReadLine() ?? string.Empty;

        if (!double.TryParse(input, out var value))
        {
            throw new FormatException("Некоректне числове значення.");
        }

        return value;
    }

    private static int ReadInt()
    {
        var input = Console.ReadLine() ?? string.Empty;

        if (!int.TryParse(input, out var value))
        {
            throw new FormatException("Некоректне ціле число.");
        }

        return value;
    }

    private static StudentStatus ReadStatus()
    {
        return Console.ReadLine() switch
        {
            "1" => StudentStatus.Active,
            "2" => StudentStatus.AcademicLeave,
            "3" => StudentStatus.Expelled,
            "4" => StudentStatus.Graduated,
            _ => throw new FormatException("Невірний статус.")
        };
    }
}
