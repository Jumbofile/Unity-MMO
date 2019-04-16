using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Database
    {
        //database string
        private static string SqlSource = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\MMO\\server\\server\\Database.mdf;Integrated Security=True;";

        //returns the users encrypted password
        private static string getUserPassword(string username)
        {
            string pw = null;

            SqlConnection con = new SqlConnection(@SqlSource);
            con.Open();

            SqlCommand command = new SqlCommand("SELECT Password from Users WHERE Name = @user", con);
            command.Parameters.AddWithValue("@user", username);
            // int result = command.ExecuteNonQuery();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    pw = String.Format("{0}", reader["Password"]);
                }
            }

            con.Close();
            //con.Close();
            return pw;
        }



        //Checks if the user already exsists
        private static string userExists(string username)
        {
            string un = null;

            try
            {
                SqlConnection con = new SqlConnection(@SqlSource);
                con.Open();

                SqlCommand command = new SqlCommand("SELECT Name from Users WHERE Name = @user", con);
                command.Parameters.AddWithValue("@user", username);
                // int result = command.ExecuteNonQuery();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        un = String.Format("{0}", reader["Name"]);
                    }
                }

                con.Close();
            }
            catch (Exception e)
            {
                un = null;
            }
            //con.Close();
            return un;
        }

        //Checks if the user already exsists
        private static bool charNameTaken(string name)
        {
            string un = null;

            try
            {
                SqlConnection con = new SqlConnection(@SqlSource);
                con.Open();

                SqlCommand command = new SqlCommand("SELECT Name from Characters WHERE Name = @name", con);
                command.Parameters.AddWithValue("@name", name);
                // int result = command.ExecuteNonQuery();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        un = String.Format("{0}", reader["Name"]);
                    }
                }

                con.Close();
            }
            catch (Exception e)
            {
                un = null;
            }
            if(un != null)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        public static bool AuthenticateUser(string username, string password)
        {
            //int result = 0;
            if (userExists(username) != null)
            {
                return BCrypt.CheckPassword(password, getUserPassword(username));
            }
            else
            {
                return false;
            }
        }

        public static bool CreateCharacter(string name, int accountID, int classNum)
        {
            if(charNameTaken(name) == false)
            {
                SqlConnection con = new SqlConnection(@SqlSource);
                con.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Characters(ownerAcc, name, level, class) VALUES (@id, @name, 1, @class)", con);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@id", accountID);
                command.Parameters.AddWithValue("@class", classNum);
                // int result = command.ExecuteNonQuery();
                try
                {
                    command.ExecuteNonQuery();
                    //Console.WriteLine("Succ");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }

                con.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        //gets the id associated with the 
        public static int getAccountID(string username)
        {
            if(userExists(username) != null)
            {
                SqlConnection con = new SqlConnection(SqlSource);
                con.Open();

                SqlCommand command = new SqlCommand("Select ID from Users WHERE name = @user");
                command.Parameters.AddWithValue("@user", username);

                int id;
                try
                {
                    id = (Int32)command.ExecuteScalar();
                    return id;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    con.Close();
                    return -1;
                }
            }
            return -1;
        }
        //Creats a user in the database
        public static bool CreateUser(string username, string password, string email)
        {
            if (userExists(username) == null)
            {
                SqlConnection con = new SqlConnection(@SqlSource);
                con.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Users(name, password, email) VALUES (@user, @pass, @email)", con);
                command.Parameters.AddWithValue("@user", username);
                command.Parameters.AddWithValue("@pass", BCrypt.HashPassword(password, BCrypt.GenerateSalt()));
                command.Parameters.AddWithValue("@email", email);
                // int result = command.ExecuteNonQuery();
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Succ");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }

                con.Close();
                return true;
            }

            return true;
        }
    }
}
