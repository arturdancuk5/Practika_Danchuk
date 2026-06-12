using System.Text.RegularExpressions;

namespace Praktuchna_9_Данчук_Артур;

public abstract class Person
{
    private string _fullName = string.Empty;
    private string _personalEmail = string.Empty;

    protected Person(string fullName, DateTime dateOfBirth, string personalEmail)
    {
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        PersonalEmail = personalEmail;
    }

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidStudentDataException("Повне ім'я не може бути порожнім.");
            }

            if (value.Trim().Length < 5)
            {
                throw new InvalidStudentDataException("Повне ім'я має містити щонайменше 5 символів.");
            }

            _fullName = value.Trim();
        }
    }

    public DateTime DateOfBirth { get; set; }

    public int Age => CalculateAge();

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

    public virtual void ShowDetailedInfo()
    {
        Console.WriteLine($"ПІБ: {FullName}");
        Console.WriteLine($"Дата народження: {DateOfBirth:dd.MM.yyyy}");
        Console.WriteLine($"Вік: {Age}");
        Console.WriteLine($"Email: {PersonalEmail}");
    }

    protected int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;

        if (DateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
