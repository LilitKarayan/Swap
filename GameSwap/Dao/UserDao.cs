using GameSwap.Controllers;
using GameSwap.DaoInterface;
using GameSwap.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Dao
{
    public class UserDao : IUser
    {
        public bool UserExists(string email)
        {
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT COUNT(Email) FROM User WHERE Email='{email}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        count = reader.GetInt32("Count(Email)");
                     }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Couldn't check if email already exists {e.Message}");
                }
            }
            return count != 0;
        }

        public bool RegisterUser(string email, string password, string nickname, string firstName, string lastName, string postalCode, out string error)
        {
            error = string.Empty;

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(nickname) || String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName) || String.IsNullOrEmpty(postalCode))
            {
                error = "Not all values supplied. Could not create user account.";
                return false;
            }
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string insert = "INSERT INTO User(Email, Password, Nickname, FirstName, LastName, PostalCode)" +
                                    $"VALUES ('{email}','{password}','{nickname}','{firstName}','{lastName}','{postalCode}');";
                    MySqlCommand command = new MySqlCommand(insert, conn);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Database Error (Users): Cannot enter a new user in database {e.Message}");
                    error = "Cannot enter a new user in database";
                    return false;
                }
            }
        }

        public bool ValidateUser(string emailOrPhoneNumber, out string error)
        {
            error = string.Empty;
            string email = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT User.Email " +
                                           "FROM User LEFT JOIN Phone ON User.Email=Phone.Email " +
                                           $"WHERE User.Email ='{emailOrPhoneNumber}' " +
                                           $"OR PhoneNumber ='{emailOrPhoneNumber}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    int numberOfResults = dt.Rows.Count;
                    if (numberOfResults == 1)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            email = row["Email"].ToString();
                        }
                    }
                    else
                    {
                        error = "No user account exists with that email or phone number";
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to validate user, couldn't find email {e.Message}");
                    error = "No user account exists with that email or phone number";
                }
            }
            return !String.IsNullOrEmpty(email);
        }


        public User ValidatePassword(string emailOrPhoneNumber, string password, out string error)
        {
            error = string.Empty;
            User user = new User();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT User.Email, FirstName, LastName " +
                                           "FROM User LEFT JOIN Phone ON User.Email=Phone.Email " +
                                            $"WHERE (User.Email ='{emailOrPhoneNumber}' OR PhoneNumber ='{emailOrPhoneNumber}') AND Password ='{password}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    int numberOfResults = dt.Rows.Count;
                    if (numberOfResults == 1)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            user.Email = row["Email"].ToString();
                            user.FirstName = row["FirstName"].ToString();
                            user.LastName = row["LastName"].ToString();
                        }
                    }
                    else
                    {
                        user = null;
                        error = "Didn’t enter correct password for the user";

                    }
                    reader.Close();

                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to validate user's password {e.Message}");
                    error = "Didn’t enter correct password";
                    user = null;
                }
            }

            return user;
        }

        public User GetUserFullInfoByEmail(string userEmail, out string error)
        {
            error = string.Empty;
            User user = new User();
            PostalCode postalCode = new PostalCode();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT u.Email, Nickname, u.PostalCode, City, State, Longitude, Latitude " +
                                           "FROM User u JOIN PostalCode p ON u.PostalCode = p.PostalCode " +
                                            $"WHERE u.Email ='{userEmail}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        user.Email = reader.GetString("Email");
                        user.Nickname = reader.GetString("Nickname");
                        postalCode.Code = reader.GetString("PostalCode");
                        postalCode.City = reader.GetString("City");
                        postalCode.State = reader.GetString("State");
                        postalCode.Longitude = Double.Parse(reader.GetString("Longitude"));
                        postalCode.Latitude = Double.Parse(reader.GetString("Latitude"));
                        user.PostalCode = postalCode;
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the item by ItemNumber {e.Message}");
                    error = "No Item with that ItemNumber";
                    user = null;
                }
            }
            return user;
        }

        public double GetUserAverageRating(string userEmail, out string error)
        {
            error = string.Empty;
            double averageRating = -1.0;
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT AVG(Rating) as AverageRating FROM " +
                                            $"(SELECT ProposerRating as Rating FROM Swap WHERE ProposerEmail = '{userEmail}' UNION ALL " +
                                            $"SELECT CounterpartyRating as Rating FROM Swap WHERE CounterpartyEmail = '{userEmail}') as r; ";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        averageRating = Double.Parse(reader.GetString("AverageRating"));
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the average rating of user by user email {e.Message}");
                    error = "No rating with that email";
                    averageRating = -1.0;
                }
            }
            return averageRating;
        }

        public double GetDistance(string proposerEmail, string counterpartyEmail, out string error)
        {
            error = string.Empty;
            double distance = -1.0;
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ( 6371 * ACOS( COS( RADIANS(itemUserLatitude) ) * COS( RADIANS( currentUserLatitude ) ) * COS( RADIANS( currentUserLongitude ) - RADIANS(itemUserLongitude) ) + SIN( RADIANS(itemUserLatitude) ) * SIN(RADIANS(currentUserLatitude)) ) ) distance FROM " +
                                            $"(SELECT pc.Latitude itemUserLatitude, pc.Longitude itemUserLongitude FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode WHERE u.Email = '{counterpartyEmail}') itemU JOIN " +
                                            $"(SELECT pc.Latitude currentUserLatitude, pc.Longitude currentUserLongitude FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode WHERE u.Email = '{proposerEmail}') currentU ON 1 = 1; ";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        distance = Double.Parse(reader.GetString("distance"));
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the distance of users by user emails {e.Message}");
                    error = "No distance with these emails";
                    distance = -1.0;
                }
            }
            return distance;
        }

        public int GetNumberOfUnacceptedSwaps(string userEmail, out string error)
        {
            error = string.Empty;
            int numberOfUnacceptedSwaps = -1;
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT Count(*) as unacceptedSwaps FROM Swap WHERE (counterpartyEmail = '{userEmail}' AND status = 'pending');";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        numberOfUnacceptedSwaps = Int16.Parse(reader.GetString("unacceptedSwaps"));

                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the number of unaccepted swaps of users by user emails {e.Message}");
                    error = "Could not find unaccepted swaps for the user's emails";
                    numberOfUnacceptedSwaps = -1;
                }
            }
            return numberOfUnacceptedSwaps;
        }

        public int GetNumberOfUnratedSwaps(string userEmail, out string error)
        {
            error = string.Empty;
            int numberOfUnratedSwaps = 0;
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT COUNT(*) as unratedSwaps FROM Swap WHERE ((ProposerEmail = '{userEmail}' AND CounterpartyRating IS NULL) OR (CounterpartyEmail = '{userEmail}' AND ProposerRating IS NULL )) AND Status = 'swapped';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        numberOfUnratedSwaps = Int16.Parse(reader.GetString("unratedSwaps"));

                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the number of unrated swaps of users by user emails {e.Message}");
                    error = "Could not find unrated swaps for the user's emails";
                    numberOfUnratedSwaps = -1;
                }
            }
            return numberOfUnratedSwaps;
        }
        public int GetMaxDaysPassedForPendingSwap(string userEmail, out string error)
        {
            error = string.Empty;
            int daysPassedPedndingProposal = 0;
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT MAX(DATEDIFF(DATE(NOW()),DATE(ProposalDateTime))) as maxDays FROM Swap WHERE CounterpartyEmail = '{userEmail}' AND Status = 'pending';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        daysPassedPedndingProposal = Int16.Parse(reader.GetString("maxDays"));

                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the  days passed from oldest proposal which has still pending status by user emails {e.Message}");
                    error = "Could not find the days passed from the oldest pening proposal for the user's emails";
                    daysPassedPedndingProposal = -1;
                }
            }
            return daysPassedPedndingProposal;
        }
        public string GetNickname(string proposerEmail, string counterpartyEmail, string dateTime, out string error)
        {
            error = string.Empty;
            string nickname = null;
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT CASE WHEN ProposerEmail = '{LoginController.user.Email}' THEN cu.Nickname ELSE pu.Nickname END AS Nickname " +
                    "FROM Swap s JOIN User pu ON s.ProposerEmail = pu.Email JOIN User cu ON s.CounterpartyEmail = cu.Email " +
                    $"WHERE s.ProposerEmail = '{proposerEmail}' AND s.CounterpartyEmail = '{counterpartyEmail}' AND s.ProposalDateTime = '{dateTime}'; ";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        nickname = reader.GetString("Nickname");
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get user's nickname {e.Message}");
                    error = "Could not get user's nickanme";
                }
            }
            return nickname;
        }

        public void GetFirstNameAndEmailOfSwap(string proposerEmail, string counterpartyEmail, string dateTime, out string error, out string firstname, out string email)
        {
            error = string.Empty;
            firstname = string.Empty;
            email = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = $"SELECT CASE WHEN s.ProposerEmail='{LoginController.user.Email}' THEN cu.Firstname ELSE pu.Firstname END AS Firstname,  CASE WHEN s.ProposerEmail='{LoginController.user.Email}' THEN s.CounterpartyEmail ELSE s.ProposerEmail END AS Email " +
                        "FROM Swap s JOIN User pu ON s.ProposerEmail = pu.Email JOIN User cu ON s.CounterpartyEmail = cu.Email " +
                        $"WHERE s.ProposerEmail = '{proposerEmail}' AND s.CounterpartyEmail = '{counterpartyEmail}' AND s.ProposalDateTime = '{dateTime}' AND s.Status = 'swapped'; ";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        email = reader.GetString("Email");
                        firstname = reader.GetString("Firstname");
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get counterparty's name and email {e.Message}");
                    error = "Could not get counterparty's name and email";
                }
            }
        }
        public User GetRegisteredInfoByEmail(string userEmail, out string error)
        {
            error = string.Empty;
            User user = new User();
            PostalCode postalCode = new PostalCode();
            Phone phone = new Phone();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT u.Email, Nickname, Password, FirstName, LastName, u.PostalCode, City, State, PhoneNumber, Type, CAST(IfShare as CHAR) as IfShare " +
                                           "FROM User u JOIN PostalCode p ON u.PostalCode = p.PostalCode LEFT JOIN Phone ph ON u.Email = ph.Email " +
                                            $"WHERE u.Email ='{userEmail}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        user.Email = reader.GetString("Email");
                        user.Nickname = reader.GetString("Nickname");
                        user.Password = reader.GetString("Password");
                        user.FirstName = reader.GetString("FirstName");
                        user.LastName = reader.GetString("LastName");
                        postalCode.Code = reader.GetString("PostalCode");
                        postalCode.City = reader.GetString("City");
                        postalCode.State = reader.GetString("State");
                        try
                        {
                            phone.PhoneNumber = reader.GetString("PhoneNumber");
                        }
                        catch (Exception e)
                        {
                            phone.PhoneNumber = "";
                        }
                        try
                        {
                            phone.Type = reader.GetString("Type");
                        }
                        catch (Exception e)
                        {
                            phone.Type = "";
                        }
                        try
                        {
                            phone.IfShare = reader.GetString("IfShare");
                        }
                        catch (Exception e)
                        {
                            phone.IfShare = "-1.0";
                        }
                        user.PostalCode = postalCode;

                        user.Phone = phone;

                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the User Information {e.Message}");
                    error = "No User with that email";
                    user = null;
                }
            }
            return user;
        }

        public bool UpdateUserInfoByEmail(string email, string password, string nickname, string firstName, string lastName, string postalCode, out string error)
        {
            error = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE User " +
                                    $"SET Nickname = '{nickname}',Password = '{password}',FirstName = '{firstName}',LastName = '{lastName}',PostalCode = '{postalCode}' " +
                                    $"WHERE Email = '{email}'; ";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Database Error (Users): Cannot update the user in the database {e.Message}");
                    error = "Cannot update the user in database";
                    return false;
                }
            }
        }
    }
}
