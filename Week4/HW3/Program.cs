using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DelegatesLinQ.Homework
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Major { get; set; }
        public double GPA { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
        public DateTime EnrollmentDate { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
    }

    public class Course
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public double Grade { get; set; }
        public string Semester { get; set; }
        public string Instructor { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    public class StudentStatistics
    {
        public double MeanGPA { get; set; }
        public double MedianGPA { get; set; }
        public double StandardDeviation { get; set; }
        public double AgeGPACorrelation { get; set; }
        public List<int> OutlierIds { get; set; }
    }

    public static class StudentExtensions
    {
        public static IEnumerable<Student> FilterByAgeRange(this IEnumerable<Student> students, int minAge, int maxAge)
        {
            return students.Where(s => s.Age >= minAge && s.Age <= maxAge);
        }

        public static Dictionary<string, double> AverageGPAByMajor(this IEnumerable<Student> students)
        {
            return students.GroupBy(s => s.Major)
                          .ToDictionary(g => g.Key, g => g.Average(s => s.GPA));
        }

        public static string ToGradeReport(this Student student)
        {
            return $"Student: {student.Name} (ID: {student.Id})\n" +
                   $"Major: {student.Major}\n" +
                   $"GPA: {student.GPA:F2}\n" +
                   "Courses:\n" +
                   string.Join("\n", student.Courses.Select(c =>
                       $"  {c.Code} - {c.Name}: {c.Grade:F2} ({c.Semester})"));
        }

        public static StudentStatistics CalculateStatistics(this IEnumerable<Student> students)
        {
            var studentList = students.ToList();
            var gpas = studentList.Select(s => s.GPA).OrderBy(g => g).ToList();
            var n = gpas.Count;

            if (n == 0) return new StudentStatistics();

            var mean = gpas.Average();
            var median = n % 2 == 0 ? (gpas[n/2 - 1] + gpas[n/2]) / 2 : gpas[n/2];
            var variance = gpas.Average(g => Math.Pow(g - mean, 2));
            var stdDev = Math.Sqrt(variance);

            // Calculate Pearson correlation between age and GPA
            var ages = studentList.Select(s => (double)s.Age).ToList();
            var meanAge = ages.Average();
            var cov = ages.Zip(gpas, (a, g) => (a - meanAge) * (g - mean))
                         .Sum() / n;
            var stdDevAge = Math.Sqrt(ages.Average(a => Math.Pow(a - meanAge, 2)));
            var correlation = stdDevAge > 0 && stdDev > 0 ? cov / (stdDevAge * stdDev) : 0;

            // Identify outliers (using IQR method)
            var q1 = gpas[n/4];
            var q3 = gpas[3*n/4];
            var iqr = q3 - q1;
            var lowerBound = q1 - 1.5 * iqr;
            var upperBound = q3 + 1.5 * iqr;
            var outliers = studentList.Where(s => s.GPA < lowerBound || s.GPA > upperBound)
                                     .Select(s => s.Id).ToList();

            return new StudentStatistics
            {
                MeanGPA = mean,
                MedianGPA = median,
                StandardDeviation = stdDev,
                AgeGPACorrelation = correlation,
                OutlierIds = outliers
            };
        }
    }

    public class LinqDataProcessor
    {
        private List<Student> _students;
        private Dictionary<string, IQueryable<Student>> _queryCache;

        public LinqDataProcessor()
        {
            _students = GenerateSampleData();
            _queryCache = new Dictionary<string, IQueryable<Student>>();
        }

        public void BasicQueries()
        {
            Console.WriteLine("=== BASIC LINQ QUERIES ===");

            // 1. Students with GPA > 3.5
            var highPerformers = _students.Where(s => s.GPA > 3.5)
                                         .Select(s => new { s.Name, s.GPA });
            Console.WriteLine("\nStudents with GPA > 3.5:");
            foreach (var s in highPerformers)
                Console.WriteLine($"  {s.Name}: {s.GPA:F2}");

            // 2. Group by major
            var byMajor = _students.GroupBy(s => s.Major)
                                  .Select(g => new { Major = g.Key, Count = g.Count() });
            Console.WriteLine("\nStudents by Major:");
            foreach (var g in byMajor)
                Console.WriteLine($"  {g.Major}: {g.Count} students");

            // 3. Average GPA per major
            var avgGPA = _students.AverageGPAByMajor();
            Console.WriteLine("\nAverage GPA by Major:");
            foreach (var pair in avgGPA)
                Console.WriteLine($"  {pair.Key}: {pair.Value:F2}");

            // 4. Students in specific courses
            var csStudents = _students.Where(s => s.Courses.Any(c => c.Code.StartsWith("CS")))
                                     .Select(s => s.Name);
            Console.WriteLine("\nStudents in CS courses:");
            foreach (var name in csStudents)
                Console.WriteLine($"  {name}");

            // 5. Sort by enrollment date
            var sortedByDate = _students.OrderBy(s => s.EnrollmentDate)
                                       .Select(s => new { s.Name, s.EnrollmentDate });
            Console.WriteLine("\nStudents sorted by Enrollment Date:");
            foreach (var s in sortedByDate)
                Console.WriteLine($"  {s.Name}: {s.EnrollmentDate:yyyy-MM-dd}");
        }

        public void CustomExtensionMethods()
        {
            Console.WriteLine("\n=== CUSTOM EXTENSION METHODS ===");

            // Filter by age range
            var youngScholars = _students.FilterByAgeRange(19, 21)
                                        .Where(s => s.GPA > 3.5)
                                        .Select(s => s.Name);
            Console.WriteLine("\nHigh-performing students aged 19-21:");
            foreach (var name in youngScholars)
                Console.WriteLine($"  {name}");

            // Grade report for first student
            var firstStudent = _students.First();
            Console.WriteLine("\nGrade Report:");
            Console.WriteLine(firstStudent.ToGradeReport());

            // Statistics
            var stats = _students.CalculateStatistics();
            Console.WriteLine("\nStatistics:");
            Console.WriteLine($"  Mean GPA: {stats.MeanGPA:F2}");
            Console.WriteLine($"  Median GPA: {stats.MedianGPA:F2}");
            Console.WriteLine($"  Standard Deviation: {stats.StandardDeviation:F2}");
        }

        public void DynamicQueries()
        {
            Console.WriteLine("\n=== DYNAMIC QUERIES ===");

            // Dynamic filter: GPA > 3.5
            var gpaFilter = BuildDynamicFilter("GPA", ">", 3.5);
            var highGPAStudents = _students.AsQueryable().Where(gpaFilter)
                                          .Select(s => s.Name);
            Console.WriteLine("\nDynamic Filter (GPA > 3.5):");
            foreach (var name in highGPAStudents)
                Console.WriteLine($"  {name}");

            // Dynamic sort: by GPA descending
            var sortedByGPA = _students.AsQueryable()
                                      .OrderByDescending(s => s.GPA)
                                      .Select(s => new { s.Name, s.GPA });
            Console.WriteLine("\nDynamic Sort (by GPA descending):");
            foreach (var s in sortedByGPA)
                Console.WriteLine($"  {s.Name}: {s.GPA:F2}");

            // Dynamic grouping: by city
            var byCity = _students.GroupBy(s => s.Address.City)
                                 .Select(g => new { City = g.Key, Count = g.Count() });
            Console.WriteLine("\nDynamic Grouping (by City):");
            foreach (var g in byCity)
                Console.WriteLine($"  {g.City}: {g.Count} students");
        }

        public void StatisticalAnalysis()
        {
            Console.WriteLine("\n=== STATISTICAL ANALYSIS ===");
            var stats = _students.CalculateStatistics();

            Console.WriteLine($"Mean GPA: {stats.MeanGPA:F2}");
            Console.WriteLine($"Median GPA: {stats.MedianGPA:F2}");
            Console.WriteLine($"Standard Deviation: {stats.StandardDeviation:F2}");
            Console.WriteLine($"Age-GPA Correlation: {stats.AgeGPACorrelation:F2}");
            Console.WriteLine($"Outliers (Student IDs): {string.Join(", ", stats.OutlierIds)}");
        }

        public void PivotOperations()
        {
            Console.WriteLine("\n=== PIVOT OPERATIONS ===");

            // GPA ranges by Major
            var gpaRanges = _students.GroupBy(s => s.Major)
                .Select(g => new
                {
                    Major = g.Key,
                    Low = g.Count(s => s.GPA < 3.5),
                    Mid = g.Count(s => s.GPA >= 3.5 && s.GPA < 3.8),
                    High = g.Count(s => s.GPA >= 3.8)
                });
            Console.WriteLine("\nGPA Distribution by Major:");
            Console.WriteLine("Major\t\tLow(<3.5)\tMid(3.5-3.8)\tHigh(>=3.8)");
            foreach (var r in gpaRanges)
                Console.WriteLine($"{r.Major,-15}\t{r.Low}\t\t{r.Mid}\t\t{r.High}");

            // Course enrollment by semester and major
            var enrollment = _students.SelectMany(s => s.Courses.Select(c => new { s.Major, c.Semester }))
                .GroupBy(x => new { x.Major, x.Semester })
                .Select(g => new { g.Key.Major, g.Key.Semester, Count = g.Count() });
            Console.WriteLine("\nCourse Enrollment by Semester and Major:");
            foreach (var e in enrollment)
                Console.WriteLine($"  {e.Major} - {e.Semester}: {e.Count} courses");

            // Grade distribution by instructor
            var gradesByInstructor = _students.SelectMany(s => s.Courses)
                .GroupBy(c => c.Instructor)
                .Select(g => new
                {
                    Instructor = g.Key,
                    AvgGrade = g.Average(c => c.Grade),
                    Courses = g.Count()
                });
            Console.WriteLine("\nGrade Distribution by Instructor:");
            foreach (var g in gradesByInstructor)
                Console.WriteLine($"  {g.Instructor}: Avg Grade = {g.AvgGrade:F2}, Courses = {g.Courses}");
        }

        private Expression<Func<Student, bool>> BuildDynamicFilter(string property, string operation, double value)
        {
            var parameter = Expression.Parameter(typeof(Student), "s");
            var propertyExpression = Expression.Property(parameter, property);
            var constant = Expression.Constant(value);
            Expression comparison;

            switch (operation)
            {
                case ">": comparison = Expression.GreaterThan(propertyExpression, constant); break;
                case "<": comparison = Expression.LessThan(propertyExpression, constant); break;
                case "=": comparison = Expression.Equal(propertyExpression, constant); break;
                default: throw new ArgumentException("Invalid operation");
            }

            return Expression.Lambda<Func<Student, bool>>(comparison, parameter);
        }

        private List<Student> GenerateSampleData()
        {
            return new List<Student>
            {
                new Student
                {
                    Id = 1, Name = "Alice Johnson", Age = 20, Major = "Computer Science",
                    GPA = 3.8, EnrollmentDate = new DateTime(2022, 9, 1),
                    Email = "alice.j@university.edu",
                    Address = new Address { City = "Seattle", State = "WA", ZipCode = "98101" },
                    Courses = new List<Course>
                    {
                        new Course { Code = "CS101", Name = "Intro to Programming", Credits = 3, Grade = 3.7, Semester = "Fall 2022", Instructor = "Dr. Smith" },
                        new Course { Code = "MATH201", Name = "Calculus II", Credits = 4, Grade = 3.9, Semester = "Fall 2022", Instructor = "Prof. Johnson" }
                    }
                },
                new Student
                {
                    Id = 2, Name = "Bob Wilson", Age = 22, Major = "Mathematics",
                    GPA = 3.2, EnrollmentDate = new DateTime(2021, 9, 1),
                    Email = "bob.w@university.edu",
                    Address = new Address { City = "Portland", State = "OR", ZipCode = "97201" },
                    Courses = new List<Course>
                    {
                        new Course { Code = "MATH301", Name = "Linear Algebra", Credits = 3, Grade = 3.3, Semester = "Spring 2023", Instructor = "Dr. Brown" },
                        new Course { Code = "STAT101", Name = "Statistics", Credits = 3, Grade = 3.1, Semester = "Spring 2023", Instructor = "Prof. Davis" }
                    }
                },
                new Student
                {
                    Id = 3, Name = "Carol Davis", Age = 19, Major = "Computer Science",
                    GPA = 3.9, EnrollmentDate = new DateTime(2023, 9, 1),
                    Email = "carol.d@university.edu",
                    Address = new Address { City = "San Francisco", State = "CA", ZipCode = "94101" },
                    Courses = new List<Course>
                    {
                        new Course { Code = "CS102", Name = "Data Structures", Credits = 4, Grade = 4.0, Semester = "Fall 2023", Instructor = "Dr. Smith" },
                        new Course { Code = "CS201", Name = "Algorithms", Credits = 3, Grade = 3.8, Semester = "Fall 2023", Instructor = "Prof. Lee" }
                    }
                }
            };
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("=== HOMEWORK 3: LINQ DATA PROCESSOR ===");
            Console.WriteLine();

            LinqDataProcessor processor = new LinqDataProcessor();
            processor.BasicQueries();
            processor.CustomExtensionMethods();
            processor.DynamicQueries();
            processor.StatisticalAnalysis();
            processor.PivotOperations();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}