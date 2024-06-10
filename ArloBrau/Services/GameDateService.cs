using MySql.Data.MySqlClient;
using System;

namespace ArloBrau.Services
{
    public class GameDateService
    {
        private readonly string _connectionString;

        public GameDateService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertGameDate(DateTime startDate)
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
                    cmd.CommandText = "INSERT INTO game_date (`current_date`) VALUES (@current_date)";
                    cmd.Parameters.AddWithValue("@current_date", startDate.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Erro ao inserir a data de início no banco de dados", ex);
                }
            }
        }

        public void UpdateGameDateWithSaveId(int saveId)
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
                    cmd.CommandText = "UPDATE game_date SET save_id = @save_id WHERE save_id IS NULL";
                    cmd.Parameters.AddWithValue("@save_id", saveId);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Erro ao atualizar o save_id na tabela game_date", ex);
                }
            }
        }

        public DateTime GetBaseGameDate()
        {
            return new DateTime(2125, 1, 1);
        }
    }
}
