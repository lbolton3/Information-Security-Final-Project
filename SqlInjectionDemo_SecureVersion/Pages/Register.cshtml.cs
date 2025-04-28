using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using SqlInjectionDemo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


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

        public async Task OnPost()
        {
            var username = Request.Form["username"].ToString();;
            var password = Request.Form["password"].ToString();;

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
            var encryptedUsername = Encrypt(username);
            cmd.Parameters.AddWithValue("@username", encryptedUsername);
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

        // AES-256 Encryption/Decryption functions

        private static string Encrypt(string plainText)
        {
            byte[] key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
            byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using(var cs = new CryptoStream(ms,encryptor,CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        private static string Decrypt(string cipherText)
        {
            byte[] key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
            byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var CipherBits = Convert.FromBase64String(cipherText);

            using var ms = new MemoryStream(CipherBits);
            using var cs  = new CryptoStream(ms,decryptor,CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }



    }
}
