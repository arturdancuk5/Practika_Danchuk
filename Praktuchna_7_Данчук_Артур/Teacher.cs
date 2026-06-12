namespace Praktuchna_7_Данчук_Артур;

public class Teacher : Person
{
    private string _department = string.Empty;
    private int _experienceYears;

    public Teacher(
        string fullName,
        DateTime dateOfBirth,
        string personalEmail,
        string department,
        int experienceYears)
        : base(fullName, dateOfBirth, personalEmail)
    {
        Department = department;
        ExperienceYears = experienceYears;
    }

    public string Department
    {
        get => _department;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Назва кафедри не може бути порожньою.");
            }

            _department = value.Trim();
        }
    }

    public int ExperienceYears
    {
        get => _experienceYears;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Стаж не може бути від'ємним.");
            }

            _experienceYears = value;
        }
    }

    public override void ShowDetailedInfo()
    {
        base.ShowDetailedInfo();
        Console.WriteLine($"Кафедра: {Department}");
        Console.WriteLine($"Стаж: {ExperienceYears} років");
    }
}
