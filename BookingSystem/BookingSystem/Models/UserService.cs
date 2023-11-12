using System;
using System.Data.SqlClient;

using System.Text;
using System.Security.Cryptography;
using System.Data;

namespace BookingSystem.Models
{
    
    public class UserService
    {
        private Database db;


        public UserService()
        {
            db = new Database();
        }


        public int AddUser(string name, string email, string hashedPassword)
        {


            User user = GetUserByEmail(email);

            if (user != null)
            {
                return -2;
            }


            db.dbConnection.Open();

            string sql = "EXEC InsertUserWithPassword @Name, @Email, @PasswordHash";

            using (SqlCommand command = new SqlCommand(sql, db.dbConnection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                SqlParameter resultParameter = new SqlParameter("@Result", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(resultParameter);

                try
                {
                    command.ExecuteNonQuery();

                   

                    
                        db.dbConnection.Close();
                        return 1;
                    
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur during execution
                    // You can log the exception details for debugging
                    db.dbConnection.Close();
                    return -1;
                }
            }

        }

        public User GetUserByEmail(string email)
        {
            db.dbConnection.Open();



            string sql = @"SELECT * FROM [User] WHERE [User].email = @UserEmail";

            using (SqlCommand command = new SqlCommand(sql, db.dbConnection))
            {

                command.Parameters.AddWithValue("@UserEmail", email);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    
                    if (reader.Read())
                    {

                        User user = new User((int)reader["id"], (string)reader["name"], (string)reader["email"]);
                        db.dbConnection.Close();
                        return user;
                    }
                    else
                    {
                        db.dbConnection.Close();
                        return null;
                    }
                }
            }
        }


        public User GetUserByID(int id)
        {
            db.dbConnection.Open();



            string sql = @"SELECT * FROM [User] WHERE [User].id = @UserID";

            using (SqlCommand command = new SqlCommand(sql, db.dbConnection))
            {

                command.Parameters.AddWithValue("@UserID", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        User user = new User((int)reader["id"], (string)reader["name"], (string)reader["email"]);
                        db.dbConnection.Close();
                        return user;
                    }
                    else
                    {
                        db.dbConnection.Close();
                        return null;
                    }
                }
            }
        }

        public List<User> GetUsers(out string errormsg)
        {
            List<User> users = new List<User>();

            errormsg = "";

            try
            {
                db.dbConnection.Open();

                string sql = @"SELECT * FROM [User]";

                using (SqlCommand command = new SqlCommand(sql, db.dbConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User((int)reader["id"], (string)reader["name"], (string)reader["email"]);
                            users.Add(user);
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                errormsg = e.Message;
            }
            finally
            {
                if (db.dbConnection.State == System.Data.ConnectionState.Open)
                {
                    db.dbConnection.Close();
                }
            }

            return users;
        }

    }
}

