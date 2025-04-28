using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using SqlInjectionDemo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace SqlInjectionDemo.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext context_;

        public RegisterModel(AppDbContext context)
        {
            context_ = context;
        }

        [BindProperty]
        public string Message { get; set; }

        public void OnPost()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            //
            var connection = context_.Database.GetDbConnection();
            connection.Open();
            var cmd = connection.CreateCommand();
            
            // Inserting a new user (purposefully vulnerable to SQL injection attack)
            cmd.CommandText = $"INSERT INTO Users (Username, PasswordHash) VALUES ('{username}', '{password}')";
            
            try
            {
                cmd.ExecuteNonQuery();
                Message = "Registered Successfully!";
            }
            catch (Exception)
            {
                Message = "Registration Failed!";
            }

            connection.Close();
        }
    }
}
