using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using SqlInjectionDemo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SQLitePCL;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore;


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
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            var connection = context_.Database.GetDbConnection();
            connection.Open();
            var cmd = connection.CreateCommand();

            //vulnerable query(does not parametize the query)
            cmd.CommandText = $"SELECT * FROM Users WHERE Username = '{username}' AND PasswordHash = '{password}'";
            var reader = cmd.ExecuteReader();

            Message = reader.HasRows ? "Login successful" : "Invalid login credentials.";
 
            connection.Close();
        }
    }
}

