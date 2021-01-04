using System;
using System.ComponentModel.DataAnnotations;

namespace LogInWeb.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        [Required]
        [Display(Name="Full Name")]
        public string FullName{ get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}