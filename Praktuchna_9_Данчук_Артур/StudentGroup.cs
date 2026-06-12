using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Praktuchna_9_Данчук_Артур;

public class StudentGroup
{
    public delegate void GroupNotificationHandler(string message);

    private readonly Repository<Student> _students = new();

    public event GroupNotificationHandler? OnStudentAdded;

    public string GroupName { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public int Course { get; set; }

    public Teacher? Curator { get; set; }

    public int GroupSize => _students.GetAll().Count;

    public double AverageGroupGrade
    {
        get
        {
            var students = _students.GetAll();
            return students.Count == 0 ? 0 : students.Average(student => student.AverageGrade);
        }
    }

    public IReadOnlyList<Student> Students => _students.GetAll().AsReadOnly();

    public Student this[int index]
    {
        get
        {
            var students = _students.GetAll();

            if (index < 0 || index >= students.Count)
            {
                throw new StudentNotFoundException("Студента за вказаним індексом не знайдено.");
            }

            return students[index];
        }
    }

    public Student this[string recordBookNumber]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(recordBookNumber))
            {
                throw new StudentNotFoundException("Номер залікової не може бути порожнім.");
            }

            var student = _students.GetAll().FirstOrDefault(item =>
                item.RecordBookNumber == recordBookNumber.Trim());

            if (student == null)
            {
                throw new StudentNotFoundException($"Студента з номером залікової {recordBookNumber.Trim()} не знайдено.");
            }

            return student;
        }
    }

    public void AddStudent(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        if (_students.GetAll().Any(existing => existing.RecordBookNumber == student.RecordBookNumber))
        {
            throw new InvalidOperationException("Студент з таким номером залікової вже існує.");
        }

        _students.Add(student);
        OnStudentAdded?.Invoke($"Студента {student.FullName} успішно додано до групи {GroupName}.");
    }

    public void RemoveStudent(string query)
    {
        var student = FindStudent(query);
        _students.Remove(student);
    }

    public Student FindStudent(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new StudentNotFoundException("Запит для пошуку студента не може бути порожнім.");
        }

        var normalizedQuery = query.Trim();

        var student = _students.GetAll().FirstOrDefault(item =>
            item.RecordBookNumber == normalizedQuery ||
            item.FullName.Equals(normalizedQuery, StringComparison.OrdinalIgnoreCase));

        if (student == null)
        {
            throw new StudentNotFoundException($"Студента за запитом \"{normalizedQuery}\" не знайдено.");
        }

        return student;
    }

    public List<Student> GetExcellentStudents()
    {
        return _students.GetAll().Where(student => student.IsExcellent()).ToList();
    }

    public List<Student> GetStudentsByStatus(StudentStatus status)
    {
        return _students.GetAll().Where(student => student.Status == status).ToList();
    }

    public List<Student> GetTopStudents(int count)
    {
        return _students.GetAll()
            .OrderByDescending(student => student.AverageGrade)
            .Take(count)
            .ToList();
    }

    public double GetAverageGradeOfActiveStudents()
    {
        return _students.GetAll()
            .Where(student => student.Status == StudentStatus.Active)
            .Select(student => student.AverageGrade)
            .DefaultIfEmpty(0)
            .Average();
    }

    public void SortStudentsByGrade()
    {
        var sorted = _students.GetAll();
        sorted.Sort();

        foreach (var student in _students.GetAll())
        {
            _students.Remove(student);
        }

        foreach (var student in sorted)
        {
            _students.Add(student);
        }
    }

    public Student CloneStudent(string recordBookNumber)
    {
        var student = FindStudent(recordBookNumber);
        var clone = (Student)student.Clone();
        clone.FullName = $"{student.FullName} (Copy)";
        clone.RecordBookNumber = GenerateUniqueRecordBookNumber(student.RecordBookNumber);
        _students.Add(clone);

        return clone;
    }

    public async Task SaveToFileAsync(string filePath)
    {
        var data = new StudentGroupData
        {
            GroupName = GroupName,
            Specialty = Specialty,
            Course = Course,
            Curator = Curator == null ? null : TeacherData.FromTeacher(Curator),
            Students = _students.GetAll().Select(StudentData.FromStudent).ToList()
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        await using var stream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);

        await JsonSerializer.SerializeAsync(stream, data, options);
    }

    public static async Task<StudentGroup> LoadFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл не знайдено.", filePath);
        }

        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        var data = await JsonSerializer.DeserializeAsync<StudentGroupData>(stream, options)
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

    public async Task<string> GenerateHeavyReportAsync()
    {
        await Task.Delay(3000);

        return $"Аналітичний звіт групи {GroupName}: студентів — {GroupSize}, середній бал — {AverageGroupGrade:F2}.";
    }

    private string GenerateUniqueRecordBookNumber(string baseNumber)
    {
        if (!int.TryParse(baseNumber, out var number))
        {
            number = 10000000;
        }

        for (var attempt = 0; attempt < 1000; attempt++)
        {
            number = (number + 1) % 100000000;
            var candidate = number.ToString("D8");

            if (!_students.GetAll().Any(student => student.RecordBookNumber == candidate))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Не вдалося згенерувати унікальний номер залікової.");
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
