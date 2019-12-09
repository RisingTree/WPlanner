using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using System.Collections.Generic;
namespace WeddingPlanner.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if((DateTime)value < DateTime.Now)
                {
                    return new ValidationResult("Date must be in the future");
                }
                return ValidationResult.Success;
            }
    }



    public class Wedding
    {
        [Key]
        public int WeddingId{get;set;}
        [Required]
        public string Bride {get;set;}
        [Required]
        public string Groom{get;set;}
        [Required]
        [FutureDate]
        public DateTime Date {get;set;}
        [Required]
        public string Address{get;set;}
        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdatedAt{get;set;} = DateTime.Now;
        public int UserId{get;set;}
        public User User{get;set;}
        public List<Guest> InAttendance{get;set;}

    }
}
