using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NET_BE.Data;
using NET_BE.Model;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new AttendanceDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AttendanceDbContext>>()))
        {
            // Return if DB has been seeded
            if (context.Students.Any())
            {
                return;
            }

            // Initialize random instance
            var random = new Random();

            // Seed Students (40 students)
            var students = new List<Student>();
            for (int i = 1; i <= 40; i++)
            {
                students.Add(new Student
                {
                    FullName = $"Student {i}",
                    Email = $"student{i}@example.com",
                    Phone = $"09{i.ToString("D8")}",
                    Address = $"Address of Student {i}",
                    DateOfBirth = new DateTime(
                        year: 2000 + random.Next(0, 5),
                        month: random.Next(1, 13),
                        day: random.Next(1, 29))
                });
            }
            context.Students.AddRange(students);

            // Seed Classes (5 classes)
            var classes = new List<Class>
            {
                new Class { ClassName = "SE1501" },
                new Class { ClassName = "SE1502" },
                new Class { ClassName = "SE1503" },
                new Class { ClassName = "SE1601" },
                new Class { ClassName = "SE1602" }
            };
            context.Classes.AddRange(classes);

            // Seed Subjects (8 subjects)
            var subjects = new List<Subject>
            {
                new Subject { Name = "Programming Fundamentals" },
                new Subject { Name = "Object-Oriented Programming" },
                new Subject { Name = "Database Systems" },
                new Subject { Name = "Web Development" },
                new Subject { Name = "Mobile Development" },
                new Subject { Name = "Data Structures and Algorithms" },
                new Subject { Name = "Software Engineering" },
                new Subject { Name = "Computer Networks" }
            };
            context.Subjects.AddRange(subjects);

            // Seed Lecturers (10 lecturers)
            var lecturers = new List<Lecturer>
            {
                new Lecturer { FullName = "Lecturer John Smith", Email = "john.smith@example.com" },
                new Lecturer { FullName = "Lecturer Sarah Johnson", Email = "sarah.johnson@example.com" },
                new Lecturer { FullName = "Lecturer David Brown", Email = "david.brown@example.com" },
                new Lecturer { FullName = "Lecturer Emily Davis", Email = "emily.davis@example.com" },
                new Lecturer { FullName = "Lecturer Michael Wilson", Email = "michael.wilson@example.com" },
                new Lecturer { FullName = "Lecturer Jennifer Taylor", Email = "jennifer.taylor@example.com" },
                new Lecturer { FullName = "Lecturer Robert Thomas", Email = "robert.thomas@example.com" },
                new Lecturer { FullName = "Lecturer Jessica White", Email = "jessica.white@example.com" },
                new Lecturer { FullName = "Lecturer Daniel Martin", Email = "daniel.martin@example.com" },
                new Lecturer { FullName = "Lecturer Lisa Anderson", Email = "lisa.anderson@example.com" }
            };
            context.Lecturers.AddRange(lecturers);

            // Save changes to get IDs
            context.SaveChanges();

            // Seed ClassSubjects (assign subjects to classes)
            var classSubjects = new List<ClassSubject>();
            foreach (var cls in classes)
            {
                // Each class has 4-5 subjects
                var subjectsForClass = subjects.OrderBy(x => Guid.NewGuid()).Take(random.Next(4, 6)).ToList();
                foreach (var subject in subjectsForClass)
                {
                    classSubjects.Add(new ClassSubject
                    {
                        ClassId = cls.Id,
                        SubjectId = subject.Id
                    });
                }
            }
            context.ClassSubjects.AddRange(classSubjects);
            context.SaveChanges();

            // Seed Schedules
            var schedules = new List<Schedule>();
            var timeSlots = new[] { "7:30-9:00", "9:10-10:40", "10:50-12:20", "12:50-14:20", "14:30-16:00", "16:10-17:40" };

            // Start date for schedules (2 months ago)
            var startDate = DateTime.Now.AddMonths(-2).Date;

            foreach (var classSubject in classSubjects)
            {
                // Create 10-15 schedules for each class-subject combination
                var schedulesCount = random.Next(10, 16);
                var lecturerForSubject = lecturers[random.Next(lecturers.Count)];

                for (int i = 0; i < schedulesCount; i++)
                {
                    var scheduleDate = startDate.AddDays(i * 7); // Weekly schedule
                    var timeSlot = timeSlots[random.Next(timeSlots.Length)];

                    schedules.Add(new Schedule
                    {
                        ClassSubjectId = classSubject.Id,
                        LecturerId = lecturerForSubject.Id,
                        Date = scheduleDate,
                        TimeSlot = timeSlot
                    });
                }
            }
            context.Schedules.AddRange(schedules);
            context.SaveChanges();

            // Seed Attendances
            var attendances = new List<Attendance>();

            // Assign students to classes (8 students per class)
            var studentsPerClass = 8;
            for (int i = 0; i < classes.Count; i++)
            {
                var classId = classes[i].Id;
                var classStudents = students.Skip(i * studentsPerClass).Take(studentsPerClass).ToList();

                // Find schedules for this class
                var classSchedules = schedules
                    .Where(s => classSubjects.Any(cs => cs.Id == s.ClassSubjectId && cs.ClassId == classId))
                    .ToList();

                foreach (var schedule in classSchedules)
                {
                    // For each student in this class
                    foreach (var student in classStudents)
                    {
                        // 85% chance of being present
                        bool isPresent = random.NextDouble() < 0.85;

                        // If present, create attendance record
                        if (isPresent)
                        {
                            var scheduledTime = DateTime.Parse(schedule.TimeSlot.Split('-')[0]);
                            var baseHour = scheduledTime.Hour;
                            var baseMinute = scheduledTime.Minute;

                            // Check-in time (random within 30 minutes before class start)
                            var checkInTime = new DateTime(
                                schedule.Date.Year,
                                schedule.Date.Month,
                                schedule.Date.Day,
                                baseHour,
                                baseMinute,
                                0
                            ).AddMinutes(-random.Next(0, 30));

                            attendances.Add(new Attendance
                            {
                                StudentId = student.StudentId,
                                ScheduleId = schedule.Id,
                                IsPresent = true,
                                CheckInTime = checkInTime
                            });
                        }
                        else
                        {
                            // Absent student record
                            attendances.Add(new Attendance
                            {
                                StudentId = student.StudentId,
                                ScheduleId = schedule.Id,
                                IsPresent = false,
                                CheckInTime = DateTime.MinValue // Default for absent students
                            });
                        }
                    }
                }
            }
            context.Attendances.AddRange(attendances);
            context.SaveChanges();
        }
    }
}
