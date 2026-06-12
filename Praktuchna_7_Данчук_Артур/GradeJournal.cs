namespace Praktuchna_7_Данчук_Артур;

public class GradeJournal : Dictionary<string, double>
{
    public void AddOrUpdateGrade(string subject, double grade)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Назва предмета не може бути порожньою.");
        }

        if (grade < 0 || grade > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(grade), "Оцінка має бути від 0 до 100.");
        }

        this[subject.Trim()] = grade;
    }

    public double RecalculateAverage()
    {
        if (Count == 0)
        {
            return 0;
        }

        return Values.Average();
    }
}
