using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace TCAProcess
{
    internal class TCAQueryHelper
    {
        #region Fields

        private const int COMMAND_TIMEOUT_MS = 8000; // Default for command sqlcmd
        private readonly string SqlConnectionString;

        #endregion

        #region Constructor

        public TCAQueryHelper(string serverName, string dbName)
        {
            SqlConnectionString =
                $"Data Source={serverName};Initial Catalog={dbName};Integrated Security=true;";
        }

        #endregion

        #region Public methods

        public int GetRowCount(string query)
        {
            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = COMMAND_TIMEOUT_MS;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if(reader.Read())
                        return reader.GetInt32(0);

                    throw new Exception("SqlDataReader read fail");
                }
            }
        }

        public TCAResultObj GetErrors(string query)
        {

            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = COMMAND_TIMEOUT_MS;

                var rows =
                    new List<TCAResultRow>();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<string> columnTitles =
                        Enumerable
                            .Range(0, reader.FieldCount)
                            .Select(reader.GetName)
                            .ToList();

                    while(reader.Read())
                    {
                        List<string> values = 
                            Enumerable
                                .Range(0, reader.FieldCount)
                                .Select((i)=>reader.GetValue(i).ToString())
                                .ToList();

                        rows.Add(
                            new TCAResultRow(values));
                    }

                    return
                        new TCAResultObj(
                            new TCAResultRow(columnTitles),
                            rows);
                }
            }
        }

        #endregion


    }
}
