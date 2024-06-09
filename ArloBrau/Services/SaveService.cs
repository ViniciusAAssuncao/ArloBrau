using MySql.Data.MySqlClient;
using System;
using System.Data;

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
    }
}
