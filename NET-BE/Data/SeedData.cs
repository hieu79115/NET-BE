using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NET_BE.Data;
using NET_BE.Model;

public static class SeedData
{
    public static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new AttendanceDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AttendanceDbContext>>()))
        {
            if (context.Students.Any()) return;

            var random = new Random();

            var lastNames = new List<string> { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Đặng", "Bùi" };
            var middleNames = new List<string> { "Văn", "Thị", "Hữu", "Gia", "Ngọc", "Đức", "Minh", "Thanh", "Thành", "Anh" };
            var firstNames = new List<string> { "An", "Bình", "Chi", "Dũng", "Dung", "Hà", "Hùng", "Khoa", "Lan", "Linh", "Mai", "Nam", "Phúc", "Quân", "Thảo", "Trang", "Tuấn", "Việt", "Yến", "Hải" };

            // Seed students
            var students = new List<Student>();
            for (int i = 1; i <= 40; i++)
            {
                string codeSuffix = i.ToString("D3");
                string fullName = $"{lastNames[random.Next(lastNames.Count)]} {middleNames[random.Next(middleNames.Count)]} {firstNames[random.Next(firstNames.Count)]}";
                students.Add(new Student
                {
                    StudentId = $"48.01.104.{codeSuffix}",
                    Email = $"4801104{codeSuffix}@student.hcmue.edu.vn",
                    FullName = fullName,
                    Phone = $"09{i:D8}",
                    Address = $"Address of {fullName}",
                    Password = "123456",
                    DateOfBirth = new DateTime(2000 + random.Next(0, 5), random.Next(1, 13), random.Next(1, 29))
                });
            }
            context.Students.AddRange(students);

            // Seed subjects
            var subjects = new List<Subject>
            {
                new Subject { SubjectId = "Comp001", Name = "Nhập môn lập trình" },
                new Subject { SubjectId = "Comp002", Name = "Lập trình hướng đối tượng" },
                new Subject { SubjectId = "Comp003", Name = "Hệ quản trị cơ sở dữ liệu" },
                new Subject { SubjectId = "Comp004", Name = "Lập trình web" },
                new Subject { SubjectId = "Comp005", Name = "Phát triển ứng dụng di động" },
                new Subject { SubjectId = "Comp006", Name = "Cấu trúc dữ liệu và giải thuật" },
                new Subject { SubjectId = "Comp007", Name = "Kỹ nghệ phần mềm" },
                new Subject { SubjectId = "Comp008", Name = "Mạng máy tính" },
            };
            context.Subjects.AddRange(subjects);

            // Seed lecturers
            var academicTitles = new[] { "Thạc sĩ", "Tiến sĩ", "Phó Giáo sư", "Giáo sư" };
            var degrees = new[] { "Cử nhân", "Thạc sĩ", "Tiến sĩ" };
            var departments = new[] { "Công nghệ thông tin", "Kế toán", "Toán-Tin", "Giáo dục", "Điện tử" };
            var genders = new[] { "Nam", "Nữ" };

            var lecturers = new List<Lecturer>();
            for (int i = 0; i < 10; i++)
            {
                var fullName = $"{lastNames[random.Next(lastNames.Count)]} {middleNames[random.Next(middleNames.Count)]} {firstNames[random.Next(firstNames.Count)]}";
                var lecturerId = $"LC{(i + 1):D3}";
                var gender = genders[random.Next(genders.Length)];
                var birthYear = 1975 + random.Next(0, 15);
                var dob = new DateTime(birthYear, random.Next(1, 13), random.Next(1, 29));

                lecturers.Add(new Lecturer
                {
                    LecturerId = lecturerId,
                    FullName = fullName,
                    Email = $"{RemoveDiacritics(fullName.ToLower().Replace(" ", ""))}@hcmue.edu.vn",
                    Password = "123456",
                    PhoneNumber = $"09{random.Next(10000000, 99999999)}",
                    Gender = gender,
                    DateOfBirth = dob,
                    Department = departments[random.Next(departments.Length)],
                    AcademicTitle = academicTitles[random.Next(academicTitles.Length)],
                    Degree = degrees[random.Next(degrees.Length)]
                });
            }
            context.Lecturers.AddRange(lecturers);
            context.SaveChanges();

            // Seed class subjects (lớp học phần)
            var classSubjects = new List<ClassSubject>();
            var enrollments = new List<Enrollment>();
            foreach (var subject in subjects)
            {
                for (int i = 1; i <= 3; i++)
                {
                    var classSubjectId = $"{subject.SubjectId}{i:D2}";
                    var lecturer = lecturers[random.Next(lecturers.Count)];

                    var classSubject = new ClassSubject
                    {
                        ClassSubjectId = classSubjectId,
                        SubjectId = subject.SubjectId,
                        LecturerId = lecturer.LecturerId
                    };
                    classSubjects.Add(classSubject);

                    // Ghi danh ngẫu nhiên 8 sinh viên cho mỗi lớp học phần
                    var enrolledStudents = students.OrderBy(x => Guid.NewGuid()).Take(8).ToList();
                    foreach (var student in enrolledStudents)
                    {
                        enrollments.Add(new Enrollment
                        {
                            EnrollmentId = $"ENR{enrollments.Count + 1:D4}",
                            ClassSubjectId = classSubjectId,
                            StudentId = student.StudentId
                        });
                    }
                }
            }
            context.ClassSubjects.AddRange(classSubjects);
            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();

            // Seed schedules (lịch học) dựa trên ClassSubject
            var timeSlots = new[] { "7:30-9:00", "9:10-10:40", "10:50-12:20", "12:50-14:20", "14:30-16:00", "16:10-17:40" };
            var startDate = DateTime.Now.AddMonths(-2).Date;
            var schedules = new List<Schedule>();
            foreach (var classSubject in classSubjects)
            {
                var schedulesCount = random.Next(10, 16);
                for (int i = 0; i < schedulesCount; i++)
                {
                    var scheduleDate = startDate.AddDays(i * 7);
                    var timeSlot = timeSlots[random.Next(timeSlots.Length)];
                    schedules.Add(new Schedule
                    {
                        ScheduleId = $"SCH{schedules.Count + 1:D4}",
                        ClassSubjectId = classSubject.ClassSubjectId,
                        LecturerId = classSubject.LecturerId,
                        Date = scheduleDate,
                        TimeSlot = timeSlot
                    });
                }
            }
            context.Schedules.AddRange(schedules);
            context.SaveChanges();

            // Seed attendance (chấm điểm danh)
            var attendances = new List<Attendance>();
            foreach (var schedule in schedules)
            {
                var enrolledStudentIds = enrollments.Where(e => e.ClassSubjectId == schedule.ClassSubjectId).Select(e => e.StudentId).ToList();
                foreach (var studentId in enrolledStudentIds)
                {
                    bool isPresent = random.NextDouble() < 0.85;
                    if (isPresent)
                    {
                        var scheduledTime = DateTime.Parse(schedule.TimeSlot.Split('-')[0]);
                        var checkInTime = new DateTime(schedule.Date.Year, schedule.Date.Month, schedule.Date.Day, scheduledTime.Hour, scheduledTime.Minute, 0).AddMinutes(-random.Next(0, 30));

                        attendances.Add(new Attendance
                        {
                            AttendanceId = $"ATT{attendances.Count + 1:D5}",
                            StudentId = studentId,
                            ScheduleId = schedule.ScheduleId,
                            Status = AttendanceStatus.Present,
                            DateTime = checkInTime
                        });
                    }
                    else
                    {
                        var status = (AttendanceStatus)random.Next(1, 3);
                        attendances.Add(new Attendance
                        {
                            AttendanceId = $"ATT{attendances.Count + 1:D5}",
                            StudentId = studentId,
                            ScheduleId = schedule.ScheduleId,
                            Status = status,
                            DateTime = DateTime.MinValue
                        });
                    }
                }
            }
            context.Attendances.AddRange(attendances);
            context.SaveChanges();
        }
    }
}
