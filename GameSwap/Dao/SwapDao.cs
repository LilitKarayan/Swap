using GameSwap.Controllers;
using GameSwap.DaoInterface;
using GameSwap.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Dao
{
    public class SwapDao : ISwap
    {
        public bool ProposeSwap(string proposerEmail, string counterpartyEmail, int proposerItem, int counterpartyItem, out string error)
        {
            error = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO Swap(ProposerEmail, CounterpartyEmail, ProposalDateTime,  Status, ProposerItem, CounterpartyItem) " +
                                            $"VALUES('{proposerEmail}','{counterpartyEmail}',NOW(),'pending',{proposerItem},{counterpartyItem});";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to insert swap. {e.Message}");
                    error = "Could not do swap proposal.";
                    return false;
                }
            }
        }

        public string RateSwap(string proposerEmail, string counterPartyEmail, int rating, string time)
        {
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string update = $"UPDATE Swap SET ProposerRating = IF(CounterpartyEmail = '{LoginController.user.Email}', {rating}, ProposerRating), " +
                                             $"CounterpartyRating = IF(ProposerEmail = '{LoginController.user.Email}', {rating}, CounterpartyRating) " +
                                             $"WHERE ProposerEmail = '{proposerEmail}' AND CounterpartyEmail = '{counterPartyEmail}' AND ProposalDateTime = '{time}';";
                    MySqlCommand command = new MySqlCommand(update, conn);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to rate swap {e.Message}");
                    return $"Failed to rate swap {e.Message}";
                }
            }
            return null;

        }

        public List<Swap> FindAssociatedSwaps()
        {
            List<Swap> swaps = new List<Swap>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ProposerEmail, CounterpartyEmail, ProposalDateTime, AcceptanceDate, Status, ProposerRating, CounterpartyRating, ProposerItem, CounterpartyItem " +
                        $"FROM Swap WHERE ProposerEmail = '{LoginController.user.Email}' OR CounterpartyEmail = '{LoginController.user.Email}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Swap swap = new Swap()
                        {
                            ProposerEmail = reader.GetString("ProposerEmail"),
                            CounterpartyEmail = reader.GetString("CounterpartyEmail"),
                            ProposalDateTime = reader.GetDateTime("ProposalDateTime"),
                            AcceptanceDate = reader.GetDateTime("AcceptanceDate"),
                            Status = reader.GetString("Status"),
                            ProposerRating = reader.GetInt32("ProposerRating"),
                            CounterpartyRating = reader.GetInt32("CounterpartyRating"),
                            ProposerItem = reader.GetInt32("ProposerItem"),
                            CounterpartyItem = reader.GetInt32("CounterpartyItem")
                        };
                        swaps.Add(swap);
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get swaps for user {e.Message}");
                }
            }
            return swaps;
        }

        public List<SwapHistorySummary> GetSwapHistorySummary()
        {
            List<SwapHistorySummary> histories = new List<SwapHistorySummary>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT 'Proposer' AS 'My Role', ar.Total, ar.Accepted, ar.Rejected, (Rejected/(Rejected+Accepted))*100 AS 'Rejected %' " + 
                                           "FROM (SELECT COUNT(*) AS Total, COUNT(case when s.Status = 'swapped' then s.Status else NULL end) AS Accepted, " +
                                           $"COUNT( case when s.Status = 'Rejected' then s.Status else NULL end) AS Rejected FROM Swap s WHERE ProposerEmail = '{LoginController.user.Email}')" +
                                           " ar UNION " +
                                           "SELECT 'Counterparty' AS 'My Role', ar.Total, ar.Accepted, ar.Rejected, (Rejected / (Rejected + Accepted)) * 100 AS 'Rejected %' " + 
                                           "FROM (SELECT COUNT(*) AS Total, COUNT(case when s.Status = 'swapped' then s.Status else NULL end) AS Accepted, " +
                                           $"COUNT( case when s.Status = 'Rejected' then s.Status else NULL end) AS Rejected FROM Swap s WHERE CounterpartyEmail = '{LoginController.user.Email}') ar;";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SwapHistorySummary history = new SwapHistorySummary()
                        {
                            MyRole = reader.GetString("My Role"),
                            Total = reader.GetInt32("Total"),
                            Accepted = reader.GetInt32("Accepted"),
                            Rejected = reader.GetInt32("Rejected"),
                            RejectedPercentage = reader["Rejected %"] != DBNull.Value ? reader.GetDouble("Rejected %") : 0
                        };
                        histories.Add(history);
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get swap history summary for user {e.Message}");
                }
            }
            return histories;
        }

        public List<SwapHistory> GetSwapHistory()
        {
            List<SwapHistory> histories = new List<SwapHistory>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT tmp.ProposalDateTime, tmp.AcceptanceDate, tmp.Status, tmp.Role, tmp.Pname, tmp.Cpname, tmp.Nickname, CASE WHEN status = 'rejected' THEN NULL ELSE tmp.Rating END AS Rating, tmp.ProposerEmail, tmp.CounterpartyEmail FROM " +
                                           "((SELECT s.ProposalDateTime, s.AcceptanceDate, s.Status,  'Proposer' AS Role, pi.NameTitle as Pname, ci.NameTitle as Cpname, u.Nickname, s.CounterpartyRating as Rating, s.ProposerEmail, s.CounterpartyEmail " +
                                           "FROM Swap s JOIN Item pi ON s.ProposerItem = pi.ItemNumber JOIN Item ci ON s.CounterpartyItem = ci.ItemNumber JOIN User u ON s.CounterpartyEmail = u.Email " +
                                           $"WHERE s.ProposerEmail = '{LoginController.user.Email}') UNION " +
                                           "(SELECT s.ProposalDateTime, s.AcceptanceDate, s.Status, 'Counterparty' AS Role, pi.NameTitle as Pname, ci.NameTitle as Cpname, u.Nickname, s.ProposerRating as Rating, s.ProposerEmail, s.CounterpartyEmail " +
                                           "FROM Swap s JOIN Item pi ON s.ProposerItem = pi.ItemNumber JOIN Item ci ON s.CounterpartyItem = ci.ItemNumber JOIN User u ON s.ProposerEmail = u.Email " +
                                           $"WHERE CounterpartyEmail = '{LoginController.user.Email}')) AS tmp WHERE tmp.Status = 'rejected' or tmp.Status = 'swapped' ORDER BY tmp.AcceptanceDate DESC, tmp.ProposalDateTime ASC;";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SwapHistory history = new SwapHistory()
                        {
                            ProposalDateTime = reader.GetDateTime("ProposalDateTime"),
                            Status = reader.GetString("Status"),
                            Role = reader.GetString("Role"),
                            ProposerItemName = reader.GetString("Pname"),
                            CounterpartyItemName = reader.GetString("Cpname"),
                            Nickname = reader.GetString("Nickname"),
                            ProposerEmail = reader.GetString("ProposerEmail"),
                            CounterpartyEmail = reader.GetString("CounterpartyEmail"),
                        };

                        history.AcceptanceDate = null;
                        if (reader["AcceptanceDate"] != DBNull.Value)
                        {
                            history.AcceptanceDate = reader.GetDateTime("AcceptanceDate");
                        }

                        history.Rating = null;
                        if (reader["Rating"] != DBNull.Value)
                        {
                            history.Rating = reader.GetInt32("Rating");
                        }

                        histories.Add(history);
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get swap history for user {e.Message}");
                }
            }
            return histories;
        }

        public SwapDetail GetSwapDetails(string proposerEmail, string counterpartyEmail, string dateTime)
        {
            SwapDetail detail = new SwapDetail();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT s.ProposalDateTime, s.AcceptanceDate, s.Status, " +
                        $"CASE WHEN ProposerEmail='{LoginController.user.Email}' THEN 'Proposer' ELSE 'Counterparty' END AS 'My Role', " +
                        $"CASE WHEN Status='rejected' THEN NULL WHEN ProposerEmail='{LoginController.user.Email}' THEN CounterpartyRating ELSE ProposerRating END AS 'Rating Left' " +
                        $"FROM Swap s WHERE s.ProposerEmail = '{proposerEmail}' AND s.CounterpartyEmail = '{counterpartyEmail}' AND s.ProposalDateTime = '{dateTime}'; ";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        detail = new SwapDetail()
                        {
                            Proposed = reader.GetDateTime("ProposalDateTime"),
                            Status = reader.GetString("Status"),
                            Role = reader.GetString("My Role")
                        };

                        detail.AcceptedRejected = null;
                        if (reader["AcceptanceDate"] != DBNull.Value)
                        {
                            detail.AcceptedRejected = reader.GetDateTime("AcceptanceDate");
                        }

                        detail.RatingLeft = null;
                        if (reader["Rating Left"] != DBNull.Value)
                        {
                            detail.RatingLeft = reader.GetInt32("Rating Left");
                        }
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get swap details {e.Message}");
                }
            }
            return detail;
        }

        public List<AcceptRejectSwap> GetUnacceptedSwapsByEmail(string userEmail, out string error)
        {
            error = string.Empty;
            List<AcceptRejectSwap> acceptRejectSwaps = new List<AcceptRejectSwap>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT p.PhoneNumber, p.Type, CAST(p.IfShare as DOUBLE) as IfShare,  u.FirstName, s.ProposerEmail, s.CounterpartyEmail, ProposalDateTime, di.NameTitle as DesiredItem, di.ItemNumber as DesiredItemNumber, u.Nickname as Proposer, pi.NameTitle as ProposedItem, pi.ItemNumber as ProposedItemNumber, " +
                                    "(SELECT AVG(Rating) as AverageRating " +
                                    "FROM " +
                                    "(SELECT ProposerRating as Rating FROM Swap WHERE ProposerEmail = s.ProposerEmail " +
                                    "UNION ALL " +
                                    "SELECT CounterpartyRating as Rating FROM Swap WHERE CounterpartyEmail = s.ProposerEmail) as r) as Rating, " +
                                    "(SELECT(6371 * ACOS(COS(RADIANS(itemUserLatitude)) * COS(RADIANS(currentUserLatitude)) * COS(RADIANS(currentUserLongitude) - RADIANS(itemUserLongitude)) + SIN(RADIANS(itemUserLatitude)) * SIN(RADIANS(currentUserLatitude)))) distance " +
                                    "FROM " +
                                    "(SELECT pc.Latitude itemUserLatitude, pc.Longitude itemUserLongitude FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode WHERE u.Email = s.CounterpartyEmail) itemU JOIN " +
                                    "(SELECT pc.Latitude currentUserLatitude, pc.Longitude currentUserLongitude FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode WHERE u.Email = s.ProposerEmail) currentU ON 1 = 1) as Distance " +
                                    "FROM Swap s JOIN Item di ON s.CounterpartyItem = di.ItemNumber JOIN Item pi ON s.ProposerItem = pi.ItemNumber JOIN User u ON s.ProposerEmail = u.Email LEFT JOIN Phone p ON s.ProposerEmail = p.Email " +
                                    $"WHERE Status = 'pending' AND CounterpartyEmail = '{userEmail}' ORDER BY ProposalDateTime; ";

                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        AcceptRejectSwap acceptRejectSwap = new AcceptRejectSwap();
                        acceptRejectSwap.ProposalDateTime = DateTime.Parse(reader.GetString("ProposalDateTime"));
                        acceptRejectSwap.DesiredItemNameTitle = reader.GetString("DesiredItem");
                        acceptRejectSwap.DesiredItemNumber = Int16.Parse(reader.GetString("DesiredItemNumber"));
                        acceptRejectSwap.ProposerNickname = reader.GetString("Proposer");
                        try
                        {
                            if (Double.Parse(reader.GetString("Rating")) >= 0)
                            {
                                acceptRejectSwap.ProposerRating = Double.Parse(reader.GetString("Rating"));
                            }
                        }
                        catch (Exception e)
                        {
                            acceptRejectSwap.ProposerRating = -1.0;

                        }
                        acceptRejectSwap.Distance = Double.Parse(reader.GetString("Distance"));
                        acceptRejectSwap.ProposedItemNameTitle = reader.GetString("ProposedItem");
                        acceptRejectSwap.ProposedItemNumber = Int16.Parse(reader.GetString("ProposedItemNumber"));
                        acceptRejectSwap.ProposerEmail = reader.GetString("ProposerEmail");
                        acceptRejectSwap.CounterpartyEmail = reader.GetString("CounterpartyEmail");
                        acceptRejectSwap.ProposerFirstName = reader.GetString("FirstName");
                        try
                        {
                            acceptRejectSwap.ProposerPhoneNumber = reader.GetString("PhoneNumber");
                        }
                        catch (Exception e)
                        {
                            acceptRejectSwap.ProposerPhoneNumber = "";
                        }
                        try
                        {
                            acceptRejectSwap.ProposerPhoneNumberType = reader.GetString("Type");
                        }
                        catch (Exception e)
                        {
                            acceptRejectSwap.ProposerPhoneNumberType = "";
                        }
                        try
                        {
                            acceptRejectSwap.ProposerPhoneNumberIfShare = Double.Parse(reader.GetString("IfShare"));
                        }
                        catch (Exception e)
                        {
                            acceptRejectSwap.ProposerPhoneNumberIfShare = -1.0;
                        }

                        acceptRejectSwaps.Add(acceptRejectSwap);


                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the Unaccepted Swaps by user Email {e.Message}");
                    error = "No Unaccepted Swaps with that user Email";
                    acceptRejectSwaps = null;
                }
            }
            return acceptRejectSwaps;
        }

        public bool acceptOrRejectSwap(string proposalDateTime, string proposerEmail, string counterpartyEmail,string action, out string error)
        {
            error = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string update = "UPDATE Swap " +
                                    $"SET AcceptanceDate = NOW(), Status = '{action}' " +
                                    $"WHERE ProposerEmail = '{proposerEmail}' AND CounterpartyEmail = '{counterpartyEmail}' AND ProposalDateTime = '{proposalDateTime}';";
                    MySqlCommand command = new MySqlCommand(update, conn);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Database Error (Swap): Cannot update a swap in database {e.Message}");
                    error = "Cannot update the swap in the database";
                    return false;
                }
            }
        }

        public List<SwapInfo> GetSwaps()
        {
            List<SwapInfo> swaps = new List<SwapInfo>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM ((SELECT s.AcceptanceDate, s.ProposalDateTime, 'Proposer' AS Role, pi.NameTitle as ProposerItem, ci.NameTitle as DesiredItem, u.Nickname, s.CounterpartyRating as Rating, s.CounterpartyEmail, s.ProposerEmail " +
                                           "FROM Swap s JOIN Item pi ON s.ProposerItem = pi.ItemNumber JOIN Item ci ON s.CounterpartyItem = ci.ItemNumber JOIN User u ON s.CounterpartyEmail = u.Email " +
                                           $"WHERE s.ProposerEmail = '{LoginController.user.Email}' AND s.CounterpartyRating is NULL AND s.Status = 'swapped') UNION " +
                                           "(SELECT s.AcceptanceDate, s.ProposalDateTime, 'CounterParty' AS Role, pi.NameTitle as ProposerItem, ci.NameTitle as DesiredItem, u.Nickname, s.ProposerRating as Rating, s.CounterpartyEmail, s.ProposerEmail " +
                                           "FROM Swap s JOIN Item pi ON s.ProposerItem = pi.ItemNumber JOIN Item ci ON s.CounterpartyItem = ci.ItemNumber JOIN User u ON s.ProposerEmail = u.Email " +
                                           $"WHERE CounterpartyEmail = '{LoginController.user.Email}' AND s.ProposerRating is NULL AND s.Status = 'swapped')) AS tmp ORDER BY tmp.AcceptanceDate DESC; ";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SwapInfo info = new SwapInfo()
                        {
                            AcceptanceDate = reader.GetDateTime("AcceptanceDate"),
                            ProposalDateTime = reader.GetDateTime("ProposalDateTime"),
                            Role = reader.GetString("Role"),
                            ProposerItemName = reader.GetString("ProposerItem"),
                            DesiredItemName = reader.GetString("DesiredItem"),
                            Nickname = reader.GetString("Nickname"),
                            ProposerEmail = reader.GetString("ProposerEmail"),
                            CounterpartyEmail = reader.GetString("CounterpartyEmail"),
                        };

                        info.Rating = null;
                        if (reader["Rating"] != DBNull.Value)
                        {
                            info.Rating = reader.GetInt32("Rating");
                        }

                        swaps.Add(info);
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get rate swaps for user {e.Message}");
                }
            }
            return swaps;
        }
    }
}
