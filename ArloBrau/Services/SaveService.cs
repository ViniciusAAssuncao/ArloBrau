using ArloBrau.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ArloBrau.Services
{
    public class SaveService
    {
        private readonly string _connectionString;

        public SaveService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CreateSave(string saveName)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                MySqlCommand cmd = connection.CreateCommand();
                MySqlTransaction transaction = connection.BeginTransaction();
                cmd.Connection = connection;
                cmd.Transaction = transaction;

                try
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM saves WHERE save_name = @saveName";
                    cmd.Parameters.AddWithValue("@saveName", saveName);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        throw new DuplicateNameException("Um save com esse nome já existe.");
                    }

                    cmd.CommandText = "INSERT INTO saves (save_name) VALUES (@saveName)";
                    cmd.ExecuteNonQuery();

                    int newSaveId = (int)cmd.LastInsertedId;

                    transaction.Commit();
                    return newSaveId;
                }
                catch (DuplicateNameException)
                {
                    transaction.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Erro ao criar o save no banco de dados", ex);
                }
            }
        }

        public void AssignSaveToPlayer(string playerId, int saveId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                MySqlCommand cmd = connection.CreateCommand();
                MySqlTransaction transaction = connection.BeginTransaction();
                cmd.Connection = connection;
                cmd.Transaction = transaction;

                try
                {
                    cmd.CommandText = "UPDATE player_manager SET save_id = @save_id WHERE playerid = @playerid";
                    cmd.Parameters.AddWithValue("@save_id", saveId);
                    cmd.Parameters.AddWithValue("@playerid", playerId);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Erro ao atualizar o jogador com o save_id", ex);
                }
            }
        }

        public List<Save> GetAllSaves()
        {
            List<Save> saves = new List<Save>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT save_id, save_name, created_at FROM saves", connection);

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            saves.Add(new Save
                            {
                                SaveId = reader.GetInt32("save_id"),
                                SaveName = reader.GetString("save_name"),
                                SaveDate = reader.GetDateTime("created_at")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao acessar o banco de dados", ex);
                }
            }

            return saves;
        }

        public async Task<List<Save>> GetAllSavesAsync()
        {
            List<Save> saves = new List<Save>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                MySqlCommand cmd = new MySqlCommand("SELECT save_id, save_name, created_at FROM saves", connection);

                try
                {
                    await connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            saves.Add(new Save
                            {
                                SaveId = reader.GetInt32("save_id"),
                                SaveName = reader.GetString("save_name"),
                                SaveDate = reader.GetDateTime("created_at")
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao acessar o banco de dados", ex);
                }
            }

            return saves;
        }

        public async Task DeleteSaveAsync(int saveId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand cmd = connection.CreateCommand();
                MySqlTransaction transaction = connection.BeginTransaction();
                cmd.Connection = connection;
                cmd.Transaction = transaction;

                try
                {
                    cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 0";
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText = "DELETE FROM saves WHERE save_id = @saveId";
                    cmd.Parameters.AddWithValue("@saveId", saveId);
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1";
                    await cmd.ExecuteNonQueryAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Erro ao deletar o save no banco de dados", ex);
                }
            }
        }
    }
}