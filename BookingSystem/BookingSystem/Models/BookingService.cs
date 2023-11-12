using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using System.Windows.Input;

namespace BookingSystem.Models
{
    public class BookingService
    {
        #region Service
        //getService
        public Service getService(int id, out string errormsg)
        {
            Database db = new Database();
            string sqlstring = "SELECT * FROM Service WHERE id = @id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("id", System.Data.SqlDbType.Int).Value = id;

            Service service = null;

            errormsg = "";

            try
            {
                db.dbConnection.Open();
                SqlDataReader reader = dbCommand.ExecuteReader();

                if(reader.Read())
                {
                    service = new Service(Convert.ToInt16(reader["id"]), reader["name"].ToString(), reader["description"].ToString(), Convert.ToInt16(reader["booking_limit"]));
                } else
                {
                    errormsg = "No service found";
                    return null;
                }
                return service;
            } catch(Exception e)
            {
                errormsg = "Error fetching service: " + e.Message;
                return null;
            } finally
            {
                db.dbConnection.Close();
            }
        }

        //getServicesByUser
        public List<Service> getServicesByUser(int user_id, out string errormsg)
        {
            Database db = new Database();
            string sqlstring = "SELECT dbo.Service.id, dbo.Service.name, dbo.Service.description, dbo.Service.booking_limit FROM dbo.Service INNER JOIN dbo.UserHasService ON dbo.Service.id = dbo.UserHasService.service_id WHERE dbo.UserHasService.user_id = @id;\r\n";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("id", System.Data.SqlDbType.Int).Value = user_id;

            List<Service> services = new List<Service>();

            errormsg = "";

            try
            {
                db.dbConnection.Open();
                SqlDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Service service = new Service(Convert.ToInt16(reader["id"]), reader["name"].ToString(), reader["description"].ToString(), Convert.ToInt16(reader["booking_limit"]));
                    services.Add(service);
                }
                reader.Close();
                return services;
            }
            catch (Exception e)
            {
                errormsg = "Error fetching services: " + e.Message;
                return null;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }

        public List<User> getUsersByService(int service_id, out string errormsg)
        {
            Database db = new Database();
            string sqlstring = "SELECT [dbo].[User].[id], [dbo].[User].[name], [dbo].[User].[email] FROM [dbo].[User] INNER JOIN dbo.UserHasService ON [dbo].[User].[id] = dbo.UserHasService.user_id WHERE dbo.UserHasService.service_id = @id;\r\n";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("id", System.Data.SqlDbType.Int).Value = service_id;

            List<User> users = new List<User>();

            errormsg = "";

            try
            {
                db.dbConnection.Open();
                SqlDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    User user = new User(Convert.ToInt16(reader["id"]), reader["name"].ToString(), reader["email"].ToString());
                    users.Add(user);
                }
                reader.Close();
                return users;
            }
            catch (Exception e)
            {
                errormsg = "Error fetching users: " + e.Message;
                return null;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }


        //createService
        public bool createService(Service service, List<int> users)
        {
            Database db = new Database();
            string sqlstring = "INSERT INTO Service ([name], [description], [booking_limit]) OUTPUT INSERTED.id VALUES(@name, @description, @booking_limit)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("name", System.Data.SqlDbType.NVarChar).Value = service.Name;
            dbCommand.Parameters.Add("description", System.Data.SqlDbType.NVarChar).Value = service.Description;
            dbCommand.Parameters.Add("booking_limit", System.Data.SqlDbType.Int).Value = service.BookingLimit;

            try
            {
                db.dbConnection.Open();
                int service_id = (int)dbCommand.ExecuteScalar();

                foreach(int user_id in users)
                {
                    string sqlstring2 = "INSERT INTO UserHasService (user_id, service_id) VALUES (@user_id, @service_id)";
                    SqlCommand dbCommand2 = new SqlCommand(sqlstring2, db.dbConnection);
                    dbCommand2.Parameters.Add("user_id", System.Data.SqlDbType.Int).Value = user_id;
                    dbCommand2.Parameters.Add("service_id", System.Data.SqlDbType.Int).Value = service_id;
                    dbCommand2.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            } finally { db.dbConnection.Close(); }
        }

        //Update service, doesnt update users accociated with service
        public bool updateService(Service service)
        {
            Database db = new Database();
            string sqlstring = "UPDATE Service SET [name] = @name, [description] = @description, [booking_limit] = @booking_limit WHERE id = @id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("name", System.Data.SqlDbType.NVarChar).Value = service.Name;
            dbCommand.Parameters.Add("description", System.Data.SqlDbType.NVarChar).Value = service.Description;
            dbCommand.Parameters.Add("booking_limit", System.Data.SqlDbType.Int).Value = service.BookingLimit;
            dbCommand.Parameters.Add("id", System.Data.SqlDbType.Int).Value = service.ID;

            try
            {
                db.dbConnection.Open();
                int affectedRows = dbCommand.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }



        //addUsers
        public bool addUsers(int service_id, List<int> users)
        {
            Database db = new Database();

            try
            {
                db.dbConnection.Open();

                foreach (int user_id in users)
                {
                    string sqlstring2 = "INSERT INTO UserHasService (user_id, service_id) VALUES (@user_id, @service_id)";
                    SqlCommand dbCommand = new SqlCommand(sqlstring2, db.dbConnection);
                    dbCommand.Parameters.Add("user_id", System.Data.SqlDbType.Int).Value = user_id;
                    dbCommand.Parameters.Add("service_id", System.Data.SqlDbType.Int).Value = service_id;
                    dbCommand.ExecuteNonQuery();
                }

                return true;

            }
            catch (Exception e)
            {
                return false;
            } finally { db.dbConnection.Close(); }
        }

        //getUserCountOfService

        public int getUserCountOfService(int service_id) {
            Database db = new Database();
            string sqlstring = "SELECT COUNT(DISTINCT user_id) AS NumberOfUsers FROM UserHasService WHERE service_id = @service_id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("service_id", System.Data.SqlDbType.Int).Value = service_id;

            try
            {
                db.dbConnection.Open();
                SqlDataReader reader = dbCommand.ExecuteReader();

                if (reader.Read())
                {
                    int numberOfUsers = Convert.ToInt16(reader["NumberOfUsers"]);
                    return numberOfUsers;
                } else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                return -1;
            }
            finally
            {
                db.dbConnection.Close();
            }

        }

        //leaveService
        public bool leaveService(int service_id, int user_id) {
            Database db = new Database();
            string sqlstring = "DELETE FROM UserHasService WHERE user_id = @user_id AND service_id = @service_id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("user_id", System.Data.SqlDbType.Int).Value = user_id;
            dbCommand.Parameters.Add("service_id", System.Data.SqlDbType.Int).Value = service_id;

            try
            {
                db.dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();

                if(i == 1)
                {
                    if(getUserCountOfService(service_id) == 0)
                    {
                        deleteService(service_id);
                    }

                    return true;
                } else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }

        //deleteService
        public bool deleteService(int service_id)
        {
            Database db = new Database();
            string sqlstring = "DELETE FROM Service WHERE service_id = @service_id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("service_id", System.Data.SqlDbType.Int).Value = service_id;

            try
            {
                db.dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();

                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }
        #endregion

        #region Booking
        //getBookingsByUser
        public List<Booking> getBookingsByUser(int user_id, out string errormsg)
        {
            Database db = new Database();
            string sqlstring = "SELECT Booking.*, [User].name " +
                                "FROM Booking " +
                                "INNER JOIN [User] ON Booking.user_id = [User].id " +
                                "WHERE Booking.user_id = @id AND CONVERT(DATE, Booking.end_date) >= CONVERT(DATE, GETDATE())"; 
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("id", System.Data.SqlDbType.Int).Value = user_id;

            List<Booking> bookings = new List<Booking>();

            errormsg = "";

            try
            {
                db.dbConnection.Open();
                SqlDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    int service_id = Convert.ToInt16(reader["service_id"]);
                    Service service = getService(service_id, out errormsg);

                    Booking booking = new Booking(Convert.ToInt16(reader["id"]), Convert.ToDateTime(reader["start_date"]), Convert.ToDateTime(reader["end_date"]), Convert.ToInt16(reader["user_id"]), service, reader["name"].ToString());
                    bookings.Add(booking);
                 
                }
                reader.Close();
                return bookings;
            }
            catch (Exception e)
            {
                errormsg = "Error fetching bookings: " + e.Message;
                return null;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }

        //getBookingsByService

        public List<Booking> getBookingsByService(int service_id, out string errormsg)
        {
            Database db = new Database();
            string sqlstring = "SELECT Booking.*, [User].name " +
                               "FROM Booking " +
                               "INNER JOIN [User] ON Booking.user_id = [User].id " +
                               "WHERE Booking.service_id = @id AND CONVERT(DATE, Booking.end_date) >= CONVERT(DATE, GETDATE())";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("id", System.Data.SqlDbType.Int).Value = service_id;

            List<Booking> bookings = new List<Booking>();

            errormsg = "";

            try
            {
                db.dbConnection.Open();
                SqlDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Service service = getService(service_id, out errormsg);

                    Booking booking = new Booking(Convert.ToInt16(reader["id"]), Convert.ToDateTime(reader["start_date"]), Convert.ToDateTime(reader["end_date"]), Convert.ToInt16(reader["user_id"]), service, reader["name"].ToString());
                    bookings.Add(booking);

                }
                reader.Close();
                return bookings;
            }
            catch (Exception e)
            {
                errormsg = "Error fetching bookings: " + e.Message;
                return null;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }
        //getHoursBookedByUser
        public int getHoursBookedByUser(int user_id, int service_id)
        {
            Database db = new Database();

            string sqlstring = @"
                SELECT [User].id, [User].name, Booking.start_date, Booking.end_date, Service.description
                INTO #temp
                FROM [User]
                JOIN Booking ON [User].id = Booking.user_id
                JOIN Service ON Booking.service_id = Service.id
                WHERE [User].id = @user_id AND Service.id = @service_id
                AND MONTH(Booking.start_date) = MONTH(GETDATE())
                AND YEAR(Booking.start_date) = YEAR(GETDATE());

                SELECT SUM(DATEDIFF(HOUR, start_date, end_date)) AS total_hours FROM #temp;";

            int hours = 0;

            try
            {
                db.dbConnection.Open();

                using (SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection))
                {
                    dbCommand.Parameters.Add("service_id", System.Data.SqlDbType.Int).Value = service_id;
                    dbCommand.Parameters.Add("user_id", System.Data.SqlDbType.Int).Value = user_id;

                    using (SqlDataReader reader = dbCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hours = Convert.ToInt32(reader["total_hours"]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return 0;
            }
            finally
            {
                db.dbConnection.Close();
            }

            return hours;
        }


        //createBooking
        public bool createBooking(Booking booking, int service_id, out string errormsg)
        {
            Database db = new Database();
            string sqlstring = "INSERT INTO Booking ([start_date], [end_date], [user_id], [service_id]) VALUES(@start_date, @end_date, @user_id, @service_id)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("start_date", System.Data.SqlDbType.NVarChar).Value = booking.StartDate;
            dbCommand.Parameters.Add("end_date", System.Data.SqlDbType.NVarChar).Value = booking.EndDate;
            dbCommand.Parameters.Add("user_id", System.Data.SqlDbType.Int).Value = booking.UserID;
            dbCommand.Parameters.Add("service_id", System.Data.SqlDbType.Int).Value = service_id;

            errormsg = "";

            try
            {
                db.dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();

                if (i == 1)
                {
                    return true; // Successfully updated
                }
                else
                {
                    errormsg = "No movie found with the specified ID";
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally { db.dbConnection.Close(); }
        }

        //updateBooking
        public bool updateBooking(int booking_id,DateTime start_date, DateTime end_date, out string errormsg)
        {

            Database db = new Database();
            
            string sqlstring = "UPDATE Tbl_Movies SET start_date= @start_date end_date = @end_date WHERE booking_id = @booking_id";
            SqlCommand dbcommand = new SqlCommand(sqlstring, db.dbConnection);

            dbcommand.Parameters.Add("start_date", SqlDbType.DateTime).Value = start_date;
            dbcommand.Parameters.Add("end_date", SqlDbType.DateTime).Value = end_date;
            dbcommand.Parameters.Add("booking_id", SqlDbType.Int).Value = booking_id;

            errormsg = "";

            try
            {
                db.dbConnection.Open();
                int i = dbcommand.ExecuteNonQuery();

                if (i == 1)
                {
                    return true; // Successfully updated
                }
                else
                {
                    errormsg = "No movie found with the specified ID";
                    return false; // No rows affected means no movie with the specified ID
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return false;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }


        //deleteBooking
        public bool deleteBooking(int booking_id, out string errormsg)
        {
            Database db = new Database();
            string sqlstring = "DELETE FROM Booking WHERE id = @booking_id";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("booking_id", System.Data.SqlDbType.Int).Value = booking_id;

            errormsg = "";
            try
            {
                db.dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();

                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }

        #endregion
    }
}
