﻿using System.ComponentModel.DataAnnotations;

namespace NET_BE.Model
{
    public class Subject
    {
        public string SubjectId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public double FinalWeight { get; set; } = 0.6;

        [Range(1, 10)]
        public int Credits { get; set; } = 3;

        public ICollection<ClassSubject> ClassSubjects { get; set; }
    }
}