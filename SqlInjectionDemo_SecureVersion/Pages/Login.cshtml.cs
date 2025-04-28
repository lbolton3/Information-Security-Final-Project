using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using SqlInjectionDemo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SQLitePCL;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;



namespace SqlInjectionDemo.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext context_;


        public LoginModel(AppDbContext context)
        {
            context_ = context;
        }


        [BindProperty]

        public string Message { get; set; }

        public void OnPost()
        {
            var username = Request.Form["username"].ToString();;
            var password = Request.Form["password"].ToString();;

            // validating inputs made by user
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Message = "Username and password are required.";
                return;
            }

            // here we hash the password
            var hashPassword = HashedPassword(password);
            var connection = context_.Database.GetDbConnection();
            connection.Open();
            var cmd = (SqliteCommand)connection.CreateCommand();
            //secure query using parameterized values/inputs

            cmd.CommandText = "SELECT * FROM Users WHERE Username = @username AND PasswordHash = @password";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", hashPassword);

            var reader = cmd.ExecuteReader();

            Message = reader.HasRows ? "Login successful" : "Invalid login credentials.";

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

