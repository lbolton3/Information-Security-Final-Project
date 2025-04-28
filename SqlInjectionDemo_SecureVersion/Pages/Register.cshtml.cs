using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using SqlInjectionDemo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


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

            //input validation

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Message = "Username and password cannot be empty.";
                return;
            }

            var hashPassword = HashedPassword(password);

            //
            var connection = context_.Database.GetDbConnection();
            connection.Open();
            var cmd = (SqliteCommand)connection.CreateCommand();
            
            
            cmd.CommandText = "INSERT INTO Users (Username, PasswordHash) VALUES (@username, @password)";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", hashPassword);

            
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
        private static string HashedPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

    }
}
