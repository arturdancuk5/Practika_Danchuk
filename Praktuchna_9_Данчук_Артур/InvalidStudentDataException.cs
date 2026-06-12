namespace Praktuchna_9_Данчук_Артур;

public class InvalidStudentDataException : Exception
{
    public InvalidStudentDataException(string message)
        : base(message)
    {
    }

    public InvalidStudentDataException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
