using System.Text.RegularExpressions;

namespace Praktuchna_5_Данчук_Артур;

public enum StudentStatus
{
    Active,
    AcademicLeave,
    Expelled,
    Graduated
}

public class Student : Person, IEntity, IComparable<Student>, ICloneable
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
                throw new InvalidStudentDataException("Номер залікової книжки має містити рівно 8 цифр.");
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
                throw new InvalidStudentDataException("Середній бал має бути від 0 до 100.");
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

    public static bool operator ==(Student? left, Student? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.RecordBookNumber == right.RecordBookNumber;
    }

    public static bool operator !=(Student? left, Student? right)
    {
        return !(left == right);
    }

    public static bool operator >(Student? left, Student? right)
    {
        if (left is null || right is null)
        {
            throw new ArgumentNullException(left is null ? nameof(left) : nameof(right));
        }

        return left.AverageGrade > right.AverageGrade;
    }

    public static bool operator <(Student? left, Student? right)
    {
        if (left is null || right is null)
        {
            throw new ArgumentNullException(left is null ? nameof(left) : nameof(right));
        }

        return left.AverageGrade < right.AverageGrade;
    }

    public override bool Equals(object? obj)
    {
        return obj is Student student && this == student;
    }

    public override int GetHashCode()
    {
        return RecordBookNumber.GetHashCode(StringComparison.Ordinal);
    }

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

    public int CompareTo(Student? other)
    {
        if (other is null)
        {
            return 1;
        }

        var gradeComparison = other.AverageGrade.CompareTo(AverageGrade);

        if (gradeComparison != 0)
        {
            return gradeComparison;
        }

        return string.Compare(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public object Clone()
    {
        var clone = new Student(
            FullName,
            DateOfBirth,
            RecordBookNumber,
            AverageGrade,
            Status,
            EnrollmentDate,
            PersonalEmail,
            Notes);

        foreach (var grade in Journal)
        {
            clone.Journal.AddOrUpdateGrade(grade.Key, grade.Value);
        }

        return clone;
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
