using System;
using GameSwap.DaoInterface;
using GameSwap.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using GameSwap.Controllers;

namespace GameSwap.Dao
{
    public class PhoneDao : IPhone
    {

        public bool AddPhone(string phoneNumber, string type, bool ifShare, string email, out string error)
        {
            int shareIf = 0;
            if (ifShare)
            {
                shareIf = 1;
            }
            error = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string insert = "INSERT INTO Phone(PhoneNumber, Type, IfShare, Email) " +
                                    $"VALUES ('{phoneNumber}','{type}','{shareIf}','{email}');";
                    MySqlCommand command = new MySqlCommand(insert, conn);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Database Error: Cannot add a phone number in database {e.Message}");
                    error = "The phone number belongs to another user!";
                    return false;
                }
            }
        }

        internal bool AddPhone(object phoneNumber, object type, object ifShare, string email, out string error)
        {
            throw new NotImplementedException();
        }

        public void GetPhoneNumberAndType(string proposerEmail, string counterpartyEmail, string dateTime, out string phoneNumber, out string type, out string error)
        {
            error = string.Empty;
            phoneNumber = string.Empty;
            type = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT p.PhoneNumber, p.Type FROM swap s JOIN phone p on p.Email = CASE WHEN s.ProposerEmail = '{LoginController.user.Email}' THEN s.CounterpartyEmail WHEN s.CounterpartyEmail = '{LoginController.user.Email}' THEN s.ProposerEmail END " +
                        $"WHERE s.ProposerEmail = '{proposerEmail}' AND s.CounterpartyEmail = '{counterpartyEmail}' AND s.ProposalDateTime = '{dateTime}' AND s.Status = 'swapped' AND p.IfShare = true;";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        phoneNumber = reader.GetString("PhoneNumber");
                        type = reader.GetString("Type");
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get counterparty's phone and type {e.Message}");
                    error = "Could not get counterparty's phone and type";
                }
            }
        }

        public bool UpdatePhone(string phoneNumber, string type, bool ifShare, string email, out string error)
        {
            int shareIf = 0;
            if (ifShare)
            {
                shareIf = 1;
            }
            error = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Phone " +
                                    $"SET PhoneNumber = '{phoneNumber}',Type = '{type}',IfShare = '{shareIf}' " +
                                    $"WHERE Email = '{email}'; ";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Database Error: Cannot update phone number in database {e.Message}");
                    error = "Cannot update the phone number in database: The phone number belongs to another user!";
                    return false;
                }
            }
        }

        public bool ValidatePhone(string phoneNumber, string email, out string error)
        {
            error = string.Empty;
            string phoneNumb = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT PhoneNumber " +
                                           "FROM Phone  " +
                                           $"WHERE PhoneNumber ='{phoneNumber}' AND Email <> '{email}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    int numberOfResults = dt.Rows.Count;
                    if (numberOfResults > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            phoneNumb = row["PhoneNumber"].ToString();
                        }
                        error = "Other user exists with this  phone number";
                    }
                    else
                    {
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to validate phone number {e.Message}");
                    error = "No other user account exists with that phone number";
                }
            }
            return String.IsNullOrEmpty(phoneNumb);
        }
    }
}