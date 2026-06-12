namespace Praktuchna_5_Данчук_Артур;

public static class StudentExtensions
{
    public static bool IsEligibleForScholarship(this Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        return student.AverageGrade >= 75 && student.Status == StudentStatus.Active;
    }
}
