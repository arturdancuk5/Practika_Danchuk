using System.Text.RegularExpressions;

namespace Praktuchna_2_Данчук_Артур;

public enum StudentStatus
{
    Active,
    AcademicLeave,
    Expelled,
    Graduated
}

public class Student : Person
{
    private string _recordBookNumber = string.Empty;
    private double _averageGrade;
    private string _notes = string.Empty;

    public Student(
        string fullName,
        DateTime dateOfBirth,
        string recordBookNumber,
        double averageGrade,
        StudentStatus status,
        DateTime enrollmentDate,
        string personalEmail,
        string notes = "")
        : base(fullName, dateOfBirth, personalEmail)
    {
        RecordBookNumber = recordBookNumber;
        AverageGrade = averageGrade;
        Status = status;
        EnrollmentDate = enrollmentDate;
        Notes = notes;
    }

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

    public string Notes
    {
        get => _notes;
        private set => _notes = value ?? string.Empty;
    }

    public GradeJournal Journal { get; } = new();

    public override void ShowDetailedInfo()
    {
        base.ShowDetailedInfo();
        Console.WriteLine($"Номер залікової: {RecordBookNumber}");
        Console.WriteLine($"Середній бал: {AverageGrade:F2}");
        Console.WriteLine($"Статус: {Status}");
        Console.WriteLine($"Дата зарахування: {EnrollmentDate:dd.MM.yyyy}");
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
