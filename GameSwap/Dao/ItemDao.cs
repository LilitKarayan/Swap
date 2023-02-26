using GameSwap.DaoInterface;
using GameSwap.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Dao
{
    public class ItemDao : IItem
    {
        public Item GetItemDetails(int itemNumber, out string error)
        {
            error = string.Empty;
            Item item = new Item();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemNumber, Description, NameTitle, ItemCondition, Email " +
                                           "FROM Item " +
                                            $"WHERE ItemNumber ='{itemNumber}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        item.ItemNumber = Int16.Parse(reader.GetString("ItemNumber"));
                        item.NameTitle = reader.GetString("NameTitle");
                        item.Description = reader.GetString("Description");
                        item.ItemCondition = reader.GetString("ItemCondition");
                        item.Email = reader.GetString("Email");

                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the item by ItemNumber {e.Message}");
                    error = "No Item with that ItemNumber";
                    item = null;
                }
            }

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemNumber " +
                                           "FROM CardGame " +
                                            $"WHERE ItemNumber ='{itemNumber}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    int numberOfResults = dt.Rows.Count;
                    if (numberOfResults == 1)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            item.Type = "CardGame";
                        }
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the item by ItemNumber {e.Message}");
                    error = "No Item Type with that ItemNumber";
                    item = null;
                }
            }

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemNumber " +
                                           "FROM BoardGame " +
                                            $"WHERE ItemNumber ='{itemNumber}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    int numberOfResults = dt.Rows.Count;
                    if (numberOfResults == 1)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            item.Type = "BoardGame";
                        }
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the item by ItemNumber {e.Message}");
                    error = "No Item Type with that ItemNumber";
                    item = null;
                }
            }

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemNumber, PieceCount " +
                                           "FROM JigsawPuzzle " +
                                            $"WHERE ItemNumber ='{itemNumber}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    int numberOfResults = dt.Rows.Count;
                    if (numberOfResults == 1)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            item.Type = "JigsawPuzzle";
                            item.PieceCount = Int16.Parse(row["PieceCount"].ToString());
                        }
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the item by ItemNumber {e.Message}");
                    error = "No Item Type with that ItemNumber";
                    item = null;
                }
            }

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemNumber, Platform " +
                                           "FROM ComputerGame " +
                                            $"WHERE ItemNumber ='{itemNumber}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    int numberOfResults = dt.Rows.Count;
                    if (numberOfResults == 1)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            item.Type = "ComputerGame";
                            item.Platform = row["Platform"].ToString();
                        }
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the item by ItemNumber {e.Message}");
                    error = "No Item Type with that ItemNumber";
                    item = null;
                }
            }

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemNumber, Media, VideoGamePlatformType " +
                                           "FROM VideoGame " +
                                            $"WHERE ItemNumber ='{itemNumber}';";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    int numberOfResults = dt.Rows.Count;
                    if (numberOfResults == 1)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            item.Type = "VideoGame";
                            item.Media = row["Media"].ToString();
                            item.VideoGamePlatform = row["VideoGamePlatformType"].ToString();
                        }
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the item by ItemNumber {e.Message}");
                    error = "No Item Type with that ItemNumber";
                    item = null;
                }
            }

            return item;
        }

        public bool CheckItemAvailibilityForSwap(int itemNumber, out string error)
        {
            error = string.Empty;
            bool availibilityForSwap = true;
            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ProposerEmail " +
                                           "FROM Swap " +
                                            $"WHERE (Status = 'swapped' OR Status = 'pending') AND (ProposerItem = '{itemNumber}' OR CounterpartyItem = '{itemNumber}');";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        availibilityForSwap = false;
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the availibility of item by ItemNumber {e.Message}");
                    error = "No Item with that ItemNumber";
                    availibilityForSwap = true;
                }
            }
            return availibilityForSwap;
        }

        public List<Item> GetItemsByEmail(string userEmail, int sellerItem, out string error)
        {
            error = string.Empty;
            List <Item> items = new List<Item>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT i.ItemNumber,i.NameTitle,i.ItemCondition, CASE " +
                                    "WHEN EXISTS(SELECT * FROM CardGame cg WHERE cg.ItemNumber = i.ItemNumber) THEN 'CardGame' " +
                                    "WHEN EXISTS(SELECT* FROM BoardGame bg WHERE bg.ItemNumber = i.ItemNumber) THEN 'BoardGame' " +
                                    "WHEN EXISTS(SELECT* FROM VideoGame vg WHERE vg.ItemNumber = i.ItemNumber) THEN 'VideoGame' " +
                                    "WHEN EXISTS(SELECT* FROM ComputerGame cmg WHERE cmg.ItemNumber = i.ItemNumber) THEN 'ComputerGame' " +
                                    $"WHEN EXISTS(SELECT* FROM JigsawPuzzle jp WHERE jp.ItemNumber = i.ItemNumber) THEN 'JigsawPuzzle' END AS Type FROM   Item i where i.Email = '{userEmail}'  AND i.ItemNumber NOT IN (SELECT ProposerItem FROM Swap WHERE ProposerEmail='{userEmail}' AND CounterPartyItem ='{sellerItem}' AND Status='rejected') and i.ItemNumber NOT IN (SELECT ProposerItem FROM Swap WHERE Status = 'swapped' OR 'pending' UNION ALL SELECT CounterpartyItem FROM cs6400_sp22_team032.Swap WHERE STATUS = 'swapped' OR 'pending') AND i.ItemNumber NOT IN (SELECT ProposerItem FROM Swap WHERE Status='pending' AND ProposerEmail = '{userEmail}') ORDER BY i.ItemNumber; ";

                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Item item = new Item();
                        item.ItemNumber = Int16.Parse(reader.GetString("ItemNumber"));
                        item.NameTitle = reader.GetString("NameTitle");
                        item.ItemCondition = reader.GetString("ItemCondition");
                        item.Type = reader.GetString("Type");
                        items.Add(item);
                    }


                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the items by user Email {e.Message}");
                    error = "No Item with that user Email";
                }
            }
            return items;
        }

        public List<Item> GetAllUnswappedItemsByEmail(string userEmail, out string error)
        {
            error = string.Empty;
            List<Item> items = new List<Item>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT i.ItemNumber,i.NameTitle,i.ItemCondition, i.Description, CASE " +
                                    "WHEN EXISTS(SELECT * FROM CardGame cg WHERE cg.ItemNumber = i.ItemNumber) THEN 'CardGame' " +
                                    "WHEN EXISTS(SELECT* FROM BoardGame bg WHERE bg.ItemNumber = i.ItemNumber) THEN 'BoardGame' " +
                                    "WHEN EXISTS(SELECT* FROM VideoGame vg WHERE vg.ItemNumber = i.ItemNumber) THEN 'VideoGame' " +
                                    "WHEN EXISTS(SELECT* FROM ComputerGame cmg WHERE cmg.ItemNumber = i.ItemNumber) THEN 'ComputerGame' " +
                                    $"WHEN EXISTS(SELECT* FROM JigsawPuzzle jp WHERE jp.ItemNumber = i.ItemNumber) THEN 'JigsawPuzzle' END AS Type FROM  Item i where i.Email = '{userEmail}'  AND i.ItemNumber NOT IN (SELECT ProposerItem FROM Swap WHERE Status = 'swapped' OR 'pending' UNION ALL SELECT CounterpartyItem FROM cs6400_sp22_team032.Swap WHERE STATUS = 'swapped' OR 'pending') ORDER BY i.ItemNumber;";

                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Item item = new Item();
                        item.ItemNumber = Int16.Parse(reader.GetString("ItemNumber"));
                        item.NameTitle = reader.GetString("NameTitle");
                        item.ItemCondition = reader.GetString("ItemCondition");
                        item.Description = reader.GetString("Description");
                        item.Type = reader.GetString("Type");
                        items.Add(item);
                    }


                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the items by user Email {e.Message}");
                    error = "No Item with that user Email";
                }
            }
            return items;
        }

        public List<(string Type, int Count)> GetItemCountsForUserByEmail(string userEmail, out string error)
        {
            error = string.Empty;
            List<(string Type, int Count)> itemCounts = new List<(string Type, int Count)>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) as Count, CASE " +
                                    "WHEN EXISTS(SELECT * FROM CardGame cg WHERE cg.ItemNumber = i.ItemNumber) THEN 'CardGame' " +
                                    "WHEN EXISTS(SELECT* FROM BoardGame bg WHERE bg.ItemNumber = i.ItemNumber) THEN 'BoardGame' " +
                                    "WHEN EXISTS(SELECT* FROM VideoGame vg WHERE vg.ItemNumber = i.ItemNumber) THEN 'VideoGame' " +
                                    "WHEN EXISTS(SELECT* FROM ComputerGame cmg WHERE cmg.ItemNumber = i.ItemNumber) THEN 'ComputerGame' " +
                                    $"WHEN EXISTS(SELECT* FROM JigsawPuzzle jp WHERE jp.ItemNumber = i.ItemNumber) THEN 'JigsawPuzzle' END AS Type FROM  Item i where i.Email = '{userEmail}'  AND i.ItemNumber NOT IN (SELECT ProposerItem FROM Swap WHERE Status = 'swapped' OR 'pending' UNION ALL SELECT CounterpartyItem FROM cs6400_sp22_team032.Swap WHERE STATUS = 'swapped' OR 'pending') GROUP BY Type;";

                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        itemCounts.Add((reader.GetString("Type"), Int16.Parse(reader.GetString("Count"))));
                    }


                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the items by user Email {e.Message}");
                    error = "No Item with that user Email";
                }
            }
            return itemCounts;
        }

        public List<SearchItem> GetItemsByKeyword(string keyword, string currentUserEmail, out string error)
        {
            error = string.Empty;
            List<SearchItem> items = new List<SearchItem>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemNumber, NameTitle, Description, ItemCondition, Type, Email, Distance, FoundIn FROM " +
                                   "(SELECT ItemNumber, NameTitle, Description, ItemCondition, Type, items.Email, (6371 * ACOS(COS(RADIANS(itemUserLatitude)) * COS(RADIANS(currentUserLatitude)) * COS(RADIANS(currentUserLongitude) - RADIANS(itemUserLongitude)) + SIN(RADIANS(itemUserLatitude)) * SIN(RADIANS(currentUserLatitude)))) Distance, " +
                                   $"CASE WHEN NameTitle LIKE '%{keyword}%' THEN 'Name' " +
                                   $"WHEN Description LIKE '%{keyword}%' Then 'Description' END FoundIn FROM " +
                                   "(SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'Jigsaw' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.jigsawpuzzle as b on i.ItemNumber = b.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'computer game' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.computergame as c on i.ItemNumber = c.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'video game' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.videogame as v on i.ItemNumber = v.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'board game' end as Type FROM cs6400_sp22_team032.item as i  JOIN cs6400_sp22_team032.boardgame as g on i.ItemNumber = g.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'card game' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.cardgame as a on i.ItemNumber = a.ItemNumber ) as items JOIN " +
                                   "(SELECT Email, pc.Latitude itemUserLatitude, pc.Longitude itemUserLongitude FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode) itemU on itemU.Email = items.Email JOIN " +
                                   $"(SELECT pc.Latitude currentUserLatitude, pc.Longitude currentUserLongitude FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode WHERE u.Email = '{currentUserEmail}') currentU ON 1 = 1) as i " +
                                   $"where (NameTitle LIKE '%{keyword}%' or Description LIKE '%{keyword}%') AND i.Email <> '{currentUserEmail}' AND ItemNumber NOT IN (SELECT ProposerItem as ItemNumber FROM cs6400_sp22_team032.Swap WHERE Status = 'swapped' OR STATUS = 'pending' UNION ALL SELECT CounterpartyItem as ItemNumber FROM cs6400_sp22_team032.Swap WHERE STATUS = 'swapped' OR STATUS = 'pending') ORDER BY Distance, ItemNumber;";

                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SearchItem searchItem = new SearchItem
                        {
                            ItemNumber = Int16.Parse(reader.GetString("ItemNumber")),
                            NameTitle = reader.GetString("NameTitle"),
                            Description = reader.GetString("Description"),
                            ItemCondition = reader.GetString("ItemCondition"),
                            Type = reader.GetString("Type"),
                            Distance = Double.Parse(reader.GetString("Distance")),
                            FoundIn = reader.GetString("FoundIn")
                        };

                        items.Add(searchItem);
                    }


                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the items by keyword {e.Message}");
                    error = "Error getting items by keyword";
                }
            }

            return items;
        }

        public List<SearchItem> GetItemsWithinMiles(double miles, string currentUserEmail, out string error)
        {
            error = string.Empty;
            List<SearchItem> items = new List<SearchItem>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemNumber, NameTitle, Description, ItemCondition, Type, Email, Distance FROM " +
                                   "(SELECT ItemNumber, NameTitle, Description, ItemCondition, Type, items.Email, (6371 * ACOS(COS(RADIANS(itemUserLatitude)) * COS(RADIANS(currentUserLatitude)) * COS(RADIANS(currentUserLongitude) - RADIANS(itemUserLongitude)) + SIN(RADIANS(itemUserLatitude)) * SIN(RADIANS(currentUserLatitude)))) as Distance FROM " +
                                   "(SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'Jigsaw' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.jigsawpuzzle as b on i.ItemNumber = b.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'computer game' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.computergame as c on i.ItemNumber = c.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'video game' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.videogame as v on i.ItemNumber = v.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'board game' end as Type FROM cs6400_sp22_team032.item as i  JOIN cs6400_sp22_team032.boardgame as g on i.ItemNumber = g.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'card game' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.cardgame as a on i.ItemNumber = a.ItemNumber ) as items JOIN " +
                                   "(SELECT Email, pc.Latitude itemUserLatitude, pc.Longitude itemUserLongitude FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode) itemU on itemU.Email = items.Email JOIN " +
                                   $"(SELECT pc.Latitude currentUserLatitude, pc.Longitude currentUserLongitude FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode WHERE u.Email = '{currentUserEmail}') currentU ON 1 = 1) as i " +
                                   $"where Distance <={miles} AND i.Email <> '{currentUserEmail}' AND ItemNumber NOT IN (SELECT ProposerItem as ItemNumber FROM cs6400_sp22_team032.Swap WHERE Status = 'swapped' OR STATUS = 'pending' UNION ALL SELECT CounterpartyItem as ItemNumber FROM cs6400_sp22_team032.Swap WHERE STATUS = 'swapped' OR STATUS = 'pending') ORDER BY Distance, ItemNumber;";

                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SearchItem searchItem = new SearchItem
                        {
                            ItemNumber = Int16.Parse(reader.GetString("ItemNumber")),
                            NameTitle = reader.GetString("NameTitle"),
                            Description = reader.GetString("Description"),
                            ItemCondition = reader.GetString("ItemCondition"),
                            Type = reader.GetString("Type"),
                            Distance = Double.Parse(reader.GetString("Distance")),
                            FoundIn = ""
                        };

                        items.Add(searchItem);
                    }


                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the items within miles {e.Message}");
                    error = "Error getting items within miles";
                }
            }

            return items;
        }

        public List<SearchItem> GetItemsByPostalCode(string postalCode, string currentUserEmail, out string error)
        {
            error = string.Empty;
            List<SearchItem> items = new List<SearchItem>();

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ItemNumber, NameTitle, Description, ItemCondition, Type, Email, Distance, PostalCode FROM " +
                                   "(SELECT ItemNumber, NameTitle, Description, ItemCondition, Type, PostalCode, items.Email, (6371 * ACOS(COS(RADIANS(itemUserLatitude)) * COS(RADIANS(currentUserLatitude)) * COS(RADIANS(currentUserLongitude) - RADIANS(itemUserLongitude)) + SIN(RADIANS(itemUserLatitude)) * SIN(RADIANS(currentUserLatitude)))) as Distance FROM " +
                                   "(SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'Jigsaw' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.jigsawpuzzle as b on i.ItemNumber = b.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'computer game' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.computergame as c on i.ItemNumber = c.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'video game' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.videogame as v on i.ItemNumber = v.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'board game' end as Type FROM cs6400_sp22_team032.item as i  JOIN cs6400_sp22_team032.boardgame as g on i.ItemNumber = g.ItemNumber " +
                                   "UNION SELECT i.ItemNumber, NameTitle, Description, ItemCondition, Email, CASE WHEN true then 'card game' end as Type FROM cs6400_sp22_team032.item as i JOIN cs6400_sp22_team032.cardgame as a on i.ItemNumber = a.ItemNumber ) as items JOIN " +
                                   "(SELECT Email, pc.Latitude itemUserLatitude, pc.Longitude itemUserLongitude, pc.PostalCode FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode) itemU on itemU.Email = items.Email JOIN " +
                                   $"(SELECT pc.Latitude currentUserLatitude, pc.Longitude currentUserLongitude FROM User u JOIN PostalCode pc ON u.PostalCode = pc.PostalCode WHERE u.Email = '{currentUserEmail}') currentU ON 1 = 1) as i " +
                                   $"where PostalCode ={postalCode} AND i.Email <> '{currentUserEmail}' AND ItemNumber NOT IN (SELECT ProposerItem as ItemNumber FROM cs6400_sp22_team032.Swap WHERE Status = 'swapped' OR STATUS = 'pending' UNION ALL SELECT CounterpartyItem as ItemNumber FROM cs6400_sp22_team032.Swap WHERE STATUS = 'swapped' OR STATUS = 'pending') ORDER BY Distance, ItemNumber";

                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SearchItem searchItem = new SearchItem
                        {
                            ItemNumber = Int16.Parse(reader.GetString("ItemNumber")),
                            NameTitle = reader.GetString("NameTitle"),
                            Description = reader.GetString("Description"),
                            ItemCondition = reader.GetString("ItemCondition"),
                            Type = reader.GetString("Type"),
                            Distance = Double.Parse(reader.GetString("Distance")),
                            FoundIn = ""
                        };

                        items.Add(searchItem);
                    }


                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the items by postal code {e.Message}");
                    error = "Error getting items by postal code";
                }
            }

            return items;
        }

        public Item GetItemDetails(string proposerEmail, string counterpartyEmail, string dateTime, string itemType, out string error)
        {
            Item detail = new Item();
            error = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT i.ItemNumber, i.NameTitle, i.ItemCondition, i.Description, j.ItemNumber as jigsawpuzzle, b.ItemNumber as boardgame, cg.ItemNumber as cardgame, " +
                        "c.ItemNumber as computergame, v.ItemNumber as videogame FROM Item i " +
                        $"JOIN swap s ON s.{itemType} = i.ItemNumber LEFT JOIN jigsawpuzzle j on j.ItemNumber = i.ItemNumber " +
                        "LEFT JOIN boardgame b on b.ItemNumber = i.ItemNumber LEFT JOIN cardgame cg on cg.ItemNumber = i.ItemNumber " +
                        "LEFT JOIN computergame c on c.ItemNumber = i.ItemNumber LEFT JOIN videogame v on v.ItemNumber = i.ItemNumber " +
                        $"WHERE s.ProposerEmail = '{proposerEmail}' AND s.CounterpartyEmail = '{counterpartyEmail}' AND s.ProposalDateTime = '{dateTime}';";

                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        detail.ItemNumber = reader.GetInt32("ItemNumber");
                        detail.NameTitle = reader.GetString("NameTitle");
                        detail.ItemCondition = reader.GetString("ItemCondition");
                        detail.Description = reader.GetString("Description");
                    }

                    if (reader["jigsawpuzzle"] != DBNull.Value)
                    {
                        detail.Type = "Jigsaw Puzzle";
                    }
                    else if (reader["videogame"] != DBNull.Value)
                    {
                        detail.Type = "Video Game";
                    }
                    else if (reader["computergame"] != DBNull.Value)
                    {
                        detail.Type = "Computer Game";
                    }
                    else if (reader["cardgame"] != DBNull.Value)
                    {
                        detail.Type = "Card Game";
                    }
                    else if (reader["boardgame"] != DBNull.Value)
                    {
                        detail.Type = "Board Game";
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to get the {itemType} Item {e.Message}");
                    error = $"Failed to get {itemType} item";
                }

                return detail;
            }
        }

        private int ListItem(string nameTitle, string description, string itemCondition, string email, out string error)
        {
            error = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO Item(NameTitle, Description, ItemCondition, Email)" +
                                   $"VALUES ('{nameTitle}', '{description}', '{itemCondition}', '{email}');";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    return (int)command.LastInsertedId;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Database Error (Item): Could not enter a new item in database {e.Message}");
                    error = "Could not enter a new item in database";
                    return -1;
                }

            }
        }

        public int ListGeneralItem(string nameTitle, string description, string itemCondition, string email, string type, out string error)
        {
            int itemNumber = ListItem(nameTitle, description, itemCondition, email, out error);
            if (itemNumber > -1)
            {
                switch (type)
                {
                    case "BoardGame":
                        using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
                        {
                            try
                            {
                                conn.Open();
                                string query = "INSERT INTO BoardGame(ItemNumber)" +
                                               $"VALUES ('{itemNumber}');";
                                MySqlCommand command = new MySqlCommand(query, conn);
                                command.ExecuteNonQuery();
                                return itemNumber;
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine($"Database Error (BoardGame): Could not enter a new board game in database {e.Message}");
                                error = "Could not enter a new board game in database";
                                return -1;
                            }

                        }

                    case "CardGame":
                        using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
                        {
                            try
                            {
                                conn.Open();
                                string query = "INSERT INTO CardGame(ItemNumber)" +
                                               $"VALUES ('{itemNumber}');";
                                MySqlCommand command = new MySqlCommand(query, conn);
                                command.ExecuteNonQuery();
                                return itemNumber;
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine($"Database Error (CardGame): Could not enter a new card game in database {e.Message}");
                                error = "Could not enter a new card game in database";
                                return -1;
                            }
                        }

                    default:
                        Debug.WriteLine($"Item type is unknown: {type}");
                        break;
                }
            }
            return -1;
        }

        public int ListJigsaw(string nameTitle, string description, string itemCondition, string email, int pieceCount, out string error)
        {
            int itemNumber = ListItem(nameTitle, description, itemCondition, email, out error);
            if (itemNumber > -1)
            {
                using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
                {
                    try
                    {
                        conn.Open();
                        string query = "INSERT INTO JigsawPuzzle(ItemNumber, PieceCount)" +
                                       $"VALUES ('{itemNumber}', '{pieceCount}');";
                        MySqlCommand command = new MySqlCommand(query, conn);
                        command.ExecuteNonQuery();
                        return itemNumber;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Database Error (JigsawPuzzle): Could not enter a new jigsaw puzzle in database {e.Message}");
                        error = "Could not enter a new jigsaw puzzle in database";
                        return -1;
                    }

                }
            }
            return -1;
        }

        public int ListComputerGame(string nameTitle, string description, string itemCondition, string email, string platform, out string error)
        {
            int itemNumber = ListItem(nameTitle, description, itemCondition, email, out error);
            if (itemNumber > -1)
            {
                using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
                {
                    try
                    {
                        conn.Open();
                        string query = "INSERT INTO ComputerGame(ItemNumber, Platform)" +
                                       $"VALUES ('{itemNumber}', '{platform}');";
                        MySqlCommand command = new MySqlCommand(query, conn);
                        command.ExecuteNonQuery();
                        return itemNumber;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Database Error (ComputerGame): Could not enter a new computer game in database {e.Message}");
                        error = "Could not enter a new computer game in database";
                        return -1;
                    }

                }
            }
            return -1;
        }

        public int ListVideoGame(string nameTitle, string description, string itemCondition, string email, string media, string videoGamePlatform, out string error)
        {
            int itemNumber = ListItem(nameTitle, description, itemCondition, email, out error);
            if (itemNumber > -1)
            {
                using (MySqlConnection conn = new MySqlConnection(Database.ConnectionStr))
                {
                    try
                    {
                        int numberOfUnacceptedSwaps = 0;

                        conn.Open();
                        // Check video game platform exists
                        string vgPlatformQuery = "SELECT COUNT(*) as typeCount FROM VideoGamePlatform " +
                                                 $"WHERE Type = '{videoGamePlatform}';";
                        MySqlCommand vgPlatformCommand = new MySqlCommand(vgPlatformQuery, conn);
                        MySqlDataReader reader = vgPlatformCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            numberOfUnacceptedSwaps = Int16.Parse(reader.GetString("typeCount"));

                        }
                        reader.Close();

                        // Add new video game platform if needed
                        if (numberOfUnacceptedSwaps == 0)
                        {
                            string vgPlatformInsertQuery = "INSERT INTO VideoGamePlatform(Type)" +
                                                           $"VALUES ('{videoGamePlatform}');";
                            MySqlCommand vgPlatFormInsertCommand = new MySqlCommand(vgPlatformInsertQuery, conn);
                            vgPlatFormInsertCommand.ExecuteNonQuery();
                        }

                        // Add new VideoGame listing
                        string vgQuery = "INSERT INTO VideoGame(ItemNumber, Media, VideoGamePlatformType)" +
                                         $"VALUES ('{itemNumber}', '{media}', '{videoGamePlatform}');";
                        MySqlCommand vgCommand = new MySqlCommand(vgQuery, conn);
                        vgCommand.ExecuteNonQuery();
                        return itemNumber;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Database Error (VideoGame): Could not enter a new video game in database {e.Message}");
                        error = "Could not enter a new video game in database";
                        return -1;
                    }
                }
            }
            return -1;
        }
    }
}
