using System.Data.SqlClient;

namespace BookingSystem.Models
{
    public class MetaService
    {

        public Meta GetMetaData(string URL, out string errormsg)
        {

            Database db = new Database();
            string sqlstring = "SELECT * FROM PageMetadata WHERE PageUrl = @url";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db.dbConnection);
            dbCommand.Parameters.Add("url", System.Data.SqlDbType.NVarChar).Value = URL;

            Meta meta = null;

            errormsg = "";

            try
            {
                db.dbConnection.Open();
                SqlDataReader reader = dbCommand.ExecuteReader();

                if (reader.Read())
                {
                    meta = new Meta(reader["PageUrl"].ToString(), reader["keywords"].ToString(), reader["description"].ToString(), Convert.ToDateTime( reader["publishedDate"]), reader["Locale"].ToString(), reader["SiteName"].ToString(), reader["ContentType"].ToString());
                }
                else
                {
                    errormsg = "No meta found";
                    return null;
                }
                return meta;
            }
            catch (Exception e)
            {
                errormsg = "Error fetching semetarvice: " + e.Message;
                return null;
            }
            finally
            {
                db.dbConnection.Close();
            }
        }

    }
}
