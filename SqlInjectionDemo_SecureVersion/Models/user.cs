using System.ComponentModel.DataAnnotations;

namespace SqlInjectionDemo.Models
{
    public class User
    {
        public int ID{get;set;}
        public string Username{get;set;}
        public string PasswordHash{get;set;}
    }
}