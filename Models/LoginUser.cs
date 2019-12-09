using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        [NotMapped]
        public string LoginEmail {get;set;}
        [Required]
        [NotMapped]
        public string LoginPassword{get;set;}

    }
}