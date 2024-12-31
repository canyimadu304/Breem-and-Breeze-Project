using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeamAndBreeze.DataAccess
{
    internal class DBAccess
    {
        string connect = "Server = .; Initial Catalog = UserDB; Integrated Security = SSPI";

        public DataTable GetEmail()
        {
            DataTable table = new DataTable();
            string query = @"SELECT email FROM Users";

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connect);
                adapter.Fill(table);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return table;
        }

        public DataTable GetPassword(string email)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connect))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(null, connection);
                    command.CommandText = "SELECT password from Users WHERE email = @email";

                    SqlParameter emailParam = new SqlParameter("@email", SqlDbType.VarChar, 50);
                    emailParam.Value = email;
                    command.Parameters.Add(emailParam);
                    command.Prepare();

                    SqlDataReader reader = command.ExecuteReader();
                    table.Load(reader);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            return table;
        }

        public DataTable GetUserRole(string email)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connect))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(null, connection);
                    command.CommandText = "SELECT userRole from Users WHERE email = @email";

                    SqlParameter emailParam = new SqlParameter("@email", SqlDbType.VarChar, 50);
                    emailParam.Value = email;
                    command.Parameters.Add(emailParam);
                    command.Prepare();

                    SqlDataReader reader = command.ExecuteReader();
                    table.Load(reader);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            return table;
        }

        public DataTable GetUsername(string email)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(connect))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(null, connection);
                    command.CommandText = "SELECT username from Users WHERE email = @email";

                    SqlParameter emailParam = new SqlParameter("@email", SqlDbType.VarChar, 50);
                    emailParam.Value = email;
                    command.Parameters.Add(emailParam);
                    command.Prepare();

                    SqlDataReader reader = command.ExecuteReader();
                    table.Load(reader);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            return table;
        }

        public void AddNewUser(string username, string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(connect))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(null, connection);
                    command.CommandText = "INSERT INTO Users (username, email, password, userRole) VALUES (@username, @email, @password, @userRole)";

                    SqlParameter usernameParam = new SqlParameter("@username", SqlDbType.VarChar, 50);
                    SqlParameter emailParam = new SqlParameter("@email", SqlDbType.VarChar, 50);
                    SqlParameter passwordParam = new SqlParameter("@password", SqlDbType.VarChar, 50);
                    SqlParameter userRoleParam = new SqlParameter("@userRole", SqlDbType.VarChar, 50);

                    usernameParam.Value = username;
                    emailParam.Value = email;
                    passwordParam.Value = password;
                    userRoleParam.Value = "user";

                    command.Parameters.Add(usernameParam);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(passwordParam);
                    command.Parameters.Add(userRoleParam);

                    command.Prepare();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
    }
}
