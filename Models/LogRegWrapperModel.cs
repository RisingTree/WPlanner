using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models
{
    public class LogRegWrapperModel
    {
        public User RegUser{get;set;}
        public LoginUser LogUser{get;set;}

    }
}