using System.Text.Json;
using System.Text.Json.Serialization;

namespace Praktuchna_2_Данчук_Артур;

public class StudentGroup
{
    private readonly List<Student> _students = new();

    public string GroupName { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public int Course { get; set; }

    public Teacher? Curator { get; set; }

    public int GroupSize => _students.Count;

    public double AverageGroupGrade =>
        _students.Count == 0 ? 0 : _students.Average(student => student.AverageGrade);

    public IReadOnlyList<Student> Students => _students.AsReadOnly();

    public void AddStudent(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        if (_students.Any(existing => existing.RecordBookNumber == student.RecordBookNumber))
        {
            throw new InvalidOperationException("Студент з таким номером залікової вже існує.");
        }

        _students.Add(student);
    }

    public bool RemoveStudent(string recordBookNumber)
    {
        var student = FindStudent(recordBookNumber);

        if (student == null)
        {
            return false;
        }

        return _students.Remove(student);
    }

    public Student? FindStudent(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return null;
        }

        var normalizedQuery = query.Trim();

        return _students.FirstOrDefault(student =>
            student.RecordBookNumber == normalizedQuery ||
            student.FullName.Equals(normalizedQuery, StringComparison.OrdinalIgnoreCase));
    }

    public List<Student> GetExcellentStudents()
    {
        return _students.Where(student => student.IsExcellent()).ToList();
    }

    public List<Student> GetStudentsByStatus(StudentStatus status)
    {
        return _students.Where(student => student.Status == status).ToList();
    }

    public void SaveToFile(string filePath)
    {
        var data = new StudentGroupData
        {
            GroupName = GroupName,
            Specialty = Specialty,
            Course = Course,
            Curator = Curator == null ? null : TeacherData.FromTeacher(Curator),
            Students = _students.Select(StudentData.FromStudent).ToList()
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }

    public static StudentGroup LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл не знайдено.", filePath);
        }

        var json = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        var data = JsonSerializer.Deserialize<StudentGroupData>(json, options)
            ?? throw new InvalidOperationException("Не вдалося прочитати дані з файлу.");

        var group = new StudentGroup
        {
            GroupName = data.GroupName,
            Specialty = data.Specialty,
            Course = data.Course,
            Curator = data.Curator?.ToTeacher()
        };

        foreach (var studentData in data.Students)
        {
            group.AddStudent(studentData.ToStudent());
        }

        return group;
    }

    private sealed class StudentGroupData
    {
        public string GroupName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int Course { get; set; }
        public TeacherData? Curator { get; set; }
        public List<StudentData> Students { get; set; } = new();
    }

    private sealed class TeacherData
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string PersonalEmail { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }

        public static TeacherData FromTeacher(Teacher teacher)
        {
            return new TeacherData
            {
                FullName = teacher.FullName,
                DateOfBirth = teacher.DateOfBirth,
                PersonalEmail = teacher.PersonalEmail,
                Department = teacher.Department,
                ExperienceYears = teacher.ExperienceYears
            };
        }

        public Teacher ToTeacher()
        {
            return new Teacher(FullName, DateOfBirth, PersonalEmail, Department, ExperienceYears);
        }
    }

    private sealed class StudentData
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string RecordBookNumber { get; set; } = string.Empty;
        public double AverageGrade { get; set; }
        public StudentStatus Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string PersonalEmail { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public Dictionary<string, double> Journal { get; set; } = new();

        public static StudentData FromStudent(Student student)
        {
            return new StudentData
            {
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                RecordBookNumber = student.RecordBookNumber,
                AverageGrade = student.AverageGrade,
                Status = student.Status,
                EnrollmentDate = student.EnrollmentDate,
                PersonalEmail = student.PersonalEmail,
                Notes = student.Notes,
                Journal = student.Journal.ToDictionary(pair => pair.Key, pair => pair.Value)
            };
        }

        public Student ToStudent()
        {
            var student = new Student(
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
                student.Journal.AddOrUpdateGrade(grade.Key, grade.Value);
            }

            return student;
        }
    }
}
