namespace Praktuchna_6_Данчук_Артур;

public class StudentNotFoundException : Exception
{
    public StudentNotFoundException(string message)
        : base(message)
    {
    }

    public StudentNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
