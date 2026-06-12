using System.Reflection;

namespace Praktuchna_9_Данчук_Артур;

public static class ReflectionAnalyzer
{
    public static void AnalyzeClassMetadata(Type type)
    {
        Console.WriteLine($"Аналіз класу: {type.Name}");
        Console.WriteLine("Публічні властивості:");

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            Console.WriteLine($"- {property.PropertyType.Name} {property.Name}");
        }

        Console.WriteLine("Публічні методи:");

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (method.IsSpecialName)
            {
                continue;
            }

            var parameters = string.Join(", ", method.GetParameters().Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}"));
            Console.WriteLine($"- {method.ReturnType.Name} {method.Name}({parameters})");
        }
    }

    public static void GetDeveloperInfo(Type type)
    {
        var attribute = type.GetCustomAttribute<DeveloperInfoAttribute>();

        if (attribute == null)
        {
            Console.WriteLine($"Атрибут DeveloperInfo для класу {type.Name} не знайдено.");
            return;
        }

        Console.WriteLine($"Розробник: {attribute.DeveloperName}");
        Console.WriteLine($"Дата створення: {attribute.DateCreated}");
    }
}
