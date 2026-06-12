using System.Text.RegularExpressions;

namespace Praktuchna_1_Данчук_Артур;

public enum StudentStatus
{
    Active,
    AcademicLeave,
    Expelled,
    Graduated
}

public class Student
{
    private string _fullName = string.Empty;
    private string _recordBookNumber = string.Empty;
    private double _averageGrade;
    private string _personalEmail = string.Empty;
    private string _notes = string.Empty;

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Повне ім'я не може бути порожнім.");
            }

            if (value.Trim().Length < 5)
            {
                throw new ArgumentException("Повне ім'я має містити щонайменше 5 символів.");
            }

            _fullName = value.Trim();
        }
    }

    public DateTime DateOfBirth { get; set; }

    public int Age => CalculateAge();

    public string RecordBookNumber
    {
        get => _recordBookNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^\d{8}$"))
            {
                throw new ArgumentException("Номер залікової книжки має містити рівно 8 цифр.");
            }

            _recordBookNumber = value;
        }
    }

    public double AverageGrade
    {
        get => _averageGrade;
        private set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Середній бал має бути від 0 до 100.");
            }

            _averageGrade = value;
        }
    }

    public StudentStatus Status { get; set; }

    public DateTime EnrollmentDate { get; init; }

    public string PersonalEmail
    {
        get => _personalEmail;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("Некоректний формат електронної пошти.");
            }

            _personalEmail = value.Trim();
        }
    }

    public string Notes
    {
        get => _notes;
        private set => _notes = value ?? string.Empty;
    }

    public GradeJournal Journal { get; } = new();

    public Student(
        string fullName,
        DateTime dateOfBirth,
        string recordBookNumber,
        double averageGrade,
        StudentStatus status,
        DateTime enrollmentDate,
        string personalEmail,
        string notes = "")
    {
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        RecordBookNumber = recordBookNumber;
        AverageGrade = averageGrade;
        Status = status;
        EnrollmentDate = enrollmentDate;
        PersonalEmail = personalEmail;
        Notes = notes;
    }

    public void ShowDetailedInfo()
    {
        Console.WriteLine($"ПІБ: {FullName}");
        Console.WriteLine($"Дата народження: {DateOfBirth:dd.MM.yyyy}");
        Console.WriteLine($"Вік: {Age}");
        Console.WriteLine($"Номер залікової: {RecordBookNumber}");
        Console.WriteLine($"Середній бал: {AverageGrade:F2}");
        Console.WriteLine($"Статус: {Status}");
        Console.WriteLine($"Дата зарахування: {EnrollmentDate:dd.MM.yyyy}");
        Console.WriteLine($"Email: {PersonalEmail}");
        Console.WriteLine($"Примітки: {(string.IsNullOrWhiteSpace(Notes) ? "немає" : Notes)}");
        Console.WriteLine($"Років до випуску: {GetYearsToGraduation()}");
    }

    public void UpdateAverageGrade(double newGrade)
    {
        AverageGrade = newGrade;
    }

    public void UpdateAverageFromJournal()
    {
        AverageGrade = Journal.RecalculateAverage();
    }

    public bool IsExcellent()
    {
        return AverageGrade >= 90;
    }

    public bool IsFailing()
    {
        return AverageGrade < 60;
    }

    public int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;

        if (DateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    public int GetYearsToGraduation()
    {
        const int programDurationYears = 4;
        var yearsStudied = Math.Max(0, DateTime.Today.Year - EnrollmentDate.Year);

        if (EnrollmentDate.Date > DateTime.Today.AddYears(-yearsStudied))
        {
            yearsStudied--;
        }

        return Math.Max(0, programDurationYears - yearsStudied);
    }

    public void SetNotes(string notes)
    {
        Notes = notes;
    }
}
