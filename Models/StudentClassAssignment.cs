﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStudentApi.Models
{
    [Table("StudentClassAssignments", Schema = "dbo")]
    public class StudentClassAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Student_ID { get; set; }
        public string Position { get; set; }
       public int WeeklyHours { get; set; }
        public string FultonFellow { get; set; }
        public string Email { get; set; }
        public string EducationLevel { get; set; }
        public string Subject { get; set; }
        public int CatalogNum { get; set; }
        public string ClassSession { get; set; }
        public string ClassNum { get; set; }
        public string Term { get; set; }
       // public string InstructorName { get; set; }
        public string InstructorFirstName { get; set; }
        public string InstructorLastName { get; set; }
        public float Compensation { get; set; }
        public DateTime? CreatedAt { get; set; }

        [MaxLength(50)]
        public string Location { get; set; }
    }
}
