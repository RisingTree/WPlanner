using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using System.Collections.Generic;
namespace WeddingPlanner.Models
{
    public class Guest
    {
        [Key]
        public int GuestId{get;set;}
        public int UserId{get;set;}
        public User User {get;set;}
        public int WeddingId{get;set;}
        public Wedding Wedding {get;set;}
        public DateTime CreatedAt {get;set;}= DateTime.Now;
        public DateTime UpdatedAt {get;set;}=DateTime.Now;
    }
}
