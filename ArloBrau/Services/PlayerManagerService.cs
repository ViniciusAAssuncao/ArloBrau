using ArloBrau.Models;
using MySql.Data.MySqlClient;
using System;

namespace ArloBrau.Services
{
    public class PlayerManagerService
    {
        private readonly string _connectionString;

        public PlayerManagerService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<PlayerManager> GetAllPlayerManagers()
        {
            List<PlayerManager> players = new List<PlayerManager>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM player_manager", connection);

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            players.Add(new PlayerManager
                            {
                                Name = reader.GetString("name"),
                                BirthDate = reader.GetDateTime("birth_date"),
                                Nationality = reader.GetString("nationality"),
                                TechnicalFormation = reader.IsDBNull(reader.GetOrdinal("technical_formation")) ? null : reader.GetString("technical_formation"),
                                PlayerHistory = reader.GetString("player_history"),
                                Height = reader.GetInt32("height"),
                                SkinColor = reader.GetString("skin_color"),
                                VitaCertified = reader.GetBoolean("vita_certified"),
                                CcslCertified = reader.GetBoolean("ccsl_certified")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao acessar o banco de dados", ex);
                }
            }

            return players;
        }

        public string InsertPlayerManager(PlayerManager playerManager)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string uuid = Guid.NewGuid().ToString();

                MySqlCommand cmd = new MySqlCommand("INSERT INTO player_manager (playerid, name, birth_date, nationality, technical_formation, player_history, height, skin_color, vita_certified, ccsl_certified) VALUES (@playerid, @name, @birth_date, @nationality, @technical_formation, @player_history, @height, @skin_color, @vita_certified, @ccsl_certified)", connection);

                cmd.Parameters.AddWithValue("@playerid", uuid);
                cmd.Parameters.AddWithValue("@name", playerManager.Name);
                cmd.Parameters.AddWithValue("@birth_date", playerManager.BirthDate);
                cmd.Parameters.AddWithValue("@nationality", playerManager.Nationality);
                cmd.Parameters.AddWithValue("@technical_formation", playerManager.TechnicalFormation);
                cmd.Parameters.AddWithValue("@player_history", playerManager.PlayerHistory);
                cmd.Parameters.AddWithValue("@height", playerManager.Height);
                cmd.Parameters.AddWithValue("@skin_color", playerManager.SkinColor);
                cmd.Parameters.AddWithValue("@vita_certified", playerManager.VitaCertified);
                cmd.Parameters.AddWithValue("@ccsl_certified", playerManager.CcslCertified);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    return uuid;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao inserir o jogador no banco de dados", ex);
                }
            }
        }
    }
}
