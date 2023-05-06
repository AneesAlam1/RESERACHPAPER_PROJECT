using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace RPaperUploadProject.DATABASE
{
    public class DBManager
    {
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-O6T7UC6\\SQLEXPRESS;Initial Catalog=RPAPERDB;Integrated Security=True");
        public bool InsertUpdateDelete(String command)
        {
            SqlCommand cmd = new SqlCommand(command, con);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            int j = cmd.ExecuteNonQuery();
            if (j > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public DataTable ReadBulkData(string command)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter sa = new SqlDataAdapter(command, con);
            sa.Fill(dt);
            return dt;

        }

        public string Randomcode()
        {
            string result="";
            Random rnd = new Random();
            int a = rnd.Next(0, 9);
            char b = (char)rnd.Next(65, 90);
            int c = rnd.Next(1, 5);
            int d = rnd.Next(6, 9);
            char e = (char)rnd.Next(80, 90);
            char f = (char)rnd.Next(65, 90);
            int g = rnd.Next(1, 9);
            result = result + a + b + c + d + e + f + g;

            return result;
        }
    }
}