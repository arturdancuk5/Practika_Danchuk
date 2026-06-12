namespace Praktuchna_9_Данчук_Артур;

[AttributeUsage(AttributeTargets.Class)]
public class DeveloperInfoAttribute : Attribute
{
    public DeveloperInfoAttribute(string developerName, string dateCreated)
    {
        DeveloperName = developerName;
        DateCreated = dateCreated;
    }

    public string DeveloperName { get; }

    public string DateCreated { get; }
}
