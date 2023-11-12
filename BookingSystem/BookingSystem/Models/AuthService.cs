using System;
using System.Data.SqlClient;

using System.Text;
using System.Security.Cryptography;

namespace BookingSystem.Models
{
	public class AuthService
	{

        private UserService userService;
		public AuthService()
		{
            userService = new UserService();
		}

       

        public string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public int Login(string email, string password)
		{
            Database db = new Database();

            db.dbConnection.Open();

           

            string sql = @"SELECT P.password_hash
               FROM [User] U
               JOIN [Password] P ON U.id = P.user_id
               WHERE U.email = @UserEmail;";

            using (SqlCommand command = new SqlCommand(sql, db.dbConnection))
            {
                
                command.Parameters.AddWithValue("@UserEmail", email);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPasswordHash = (string)reader["password_hash"];

                        // Hash the entered password
                 
                        string enteredPasswordHash = this.ComputeSha256Hash(password); 

                        if (enteredPasswordHash == storedPasswordHash)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                db.dbConnection.Close();

            }


        }

        public int Register(string name, string email, string password)
        {
            string hashed = ComputeSha256Hash(password);

            return userService.AddUser(name, email, hashed);
        }

		
    }
}

