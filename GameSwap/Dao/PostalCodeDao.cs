using GameSwap.DaoInterface;
using GameSwap.Models;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;

namespace GameSwap.Dao
{
    public class PostalCodeDao : IPostalCode
    {

        public bool ValidatePostalCode(string postalCode, string city, string state, out string error)
        {
            error = string.Empty;
            string foundPostalCode = string.Empty;

            if (String.IsNullOrEmpty(postalCode) || String.IsNullOrEmpty(city) || String.IsNullOrEmpty(state))
            {
                error = "Not all values (postal code, city, and state) are supplied for a valid postal code. Could not create postal code.";
                return false;
            }

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT pc.PostalCode " +
                                           "FROM PostalCode pc " +
                                           $"WHERE pc.PostalCode = '{postalCode}' AND pc.City = LOWER('{city}') AND pc.State = LOWER('{state}');";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    int numberOfResults = dt.Rows.Count;
                    if (numberOfResults > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            foundPostalCode = row["PostalCode"].ToString();
                        }
                    }
                    else
                    {
                        error = "No address exists with that postal code, city and state";
                    }
                    
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to find address, couldn't validate postal code {e.Message}");
                    error = "No address exists with that postal code, city and state";
                }
            }
            return !String.IsNullOrEmpty(foundPostalCode);
        }



        public void Test()
        {
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string insert = "INSERT INTO PostalCode VALUES (12346,'atlanta','ga',45.246631,45.246631);";
                    MySqlCommand command = new MySqlCommand(insert, conn);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to insert new postal code {e.Message}");
                }
            }
        }
    }
}