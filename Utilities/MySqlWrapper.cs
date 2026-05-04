
using MySqlConnector;

using System;
using System.Linq;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all database functions for sql.
    /// </summary>
    internal class MySqlWrapper
    {
        /// <summary>
        /// Contains the mysql database connection data: adress, port, user and password.
        /// </summary>
        private static string mysql_connection = Configurations.SqlConnection;



        #region Sql statement builder
        /// <summary>
        /// Building the MySqlCommand for all database functions.
        /// </summary>
        private static MySqlCommand BuildMySqlCommand(MySqlConnection sql_connection, string sqlStatment, Dictionary<string, object> arguments = null)
        {
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = sql_connection,
                CommandText = sqlStatment
            };

            if (arguments != null)
            {
                foreach (var obj in arguments)
                {
                    cmd.Parameters.AddWithValue(obj.Key, obj.Value);
                }
            }

            return cmd;
        }



        /// <summary>
        /// This functon returns the first column of the first row in the result set, or a null reference if the result set is empty.<br/>
        /// Returns a maximum of 2033 characters.
        /// </summary>
        internal static async Task<object> SQLExecuteScalar(string sqlStatment, Dictionary<string, object> arguments = null)
        {
            try
            {
                MySqlConnection sql_connection = new MySqlConnection(mysql_connection);

                await sql_connection.OpenAsync();
                MySqlCommand cmd = BuildMySqlCommand(sql_connection, sqlStatment, arguments);
                object result = await cmd.ExecuteScalarAsync();
                await sql_connection.CloseAsync();

                return result is DBNull ? null : result;
            }
            catch (Exception exception)
            {
                await Utilities.SendDevLogMessage(1, exception.Message);
                return null;
            }
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.<para/>
        /// Returning:<br/>
        /// int - number of rows that was updated/inserted/deleted.
        /// </summary>
        internal static async Task<int> SQLExecuteNonQuery(string sqlStatment, Dictionary<string, object> arguments = null)
        {
            try
            {
                MySqlConnection sql_connection = new MySqlConnection(mysql_connection);
                await sql_connection.OpenAsync();
                MySqlCommand cmd = BuildMySqlCommand(sql_connection, sqlStatment, arguments);

                int updateCount = await cmd.ExecuteNonQueryAsync();
                await sql_connection.CloseAsync();

                return updateCount;
            }
            catch (Exception exception)
            {
                await Utilities.SendDevLogMessage(1, exception.Message);
                return 0;
            }
        }

        /// <summary>
        /// Sends the CommandText to the Connection and builds a SqlDataReader. Gives back a dynamic List of objects.<para/>
        /// Values can be get by the column name of the data.
        /// </summary>
        internal static async Task<List<dynamic>> SQLExecuteReader(string sqlStatment, Dictionary<string, object> arguments = null)
        {
            try
            {
                List<dynamic> result = new List<dynamic>();
                MySqlConnection sql_connection = new MySqlConnection(mysql_connection);

                await sql_connection.OpenAsync();
                MySqlCommand cmd = BuildMySqlCommand(sql_connection, sqlStatment, arguments);

                using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var columns = await reader.GetColumnSchemaAsync();

                    while (await reader.ReadAsync())
                    {
                        dynamic resultSet = new ExpandoObject();
                        IDictionary<string, object> row = resultSet;

                        for (int i = 0; i < columns.Count; i++)
                        {
                            row.Add(columns[i].ColumnName, reader.IsDBNull(i) ? null : reader.GetValue(i));
                        }
                        result.Add(row);
                    }
                }

                await sql_connection.CloseAsync();
                return result;
            }
            catch (Exception exception)
            {
                await Utilities.SendDevLogMessage(1, exception.Message);
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// Gives back a List of all objects from a specific column without dynamic.<para/>
        /// If no column is called (*) it will give back the content from the first column in the table.<para/>
        /// Returns an empty list, if nothing is found.
        /// </summary>
        internal static async Task<List<T>> SQLExecuteColumnReader<T>(string sqlStatment, Dictionary<string, object> arguments = null)
        {
            try
            {
                List<T> result = new List<T>();
                MySqlConnection sql_connection = new MySqlConnection(mysql_connection);

                await sql_connection.OpenAsync();
                MySqlCommand cmd = BuildMySqlCommand(sql_connection, sqlStatment, arguments);

                using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    var columns = await reader.GetColumnSchemaAsync();

                    while (await reader.ReadAsync())
                    {
                        if (!reader.IsDBNull(0))
                            result.Add((T)reader.GetValue(0));
                    }
                }

                await sql_connection.CloseAsync();
                return result;
            }
            catch (Exception exception)
            {
                await Utilities.SendDevLogMessage(1, exception.Message);
                return new List<T> { };
            }
        }
        #endregion



        #region Sql functions
        /// <summary>
        /// Setting, adding or removing integer for an existing table and column in database that is related to the given identifier.<para/>
        /// Returns false if updated could not be saved.<para/>
        /// 0 = To set amount.<br/>
        /// 1 = To add amount.<br/>
        /// 2 = To remove amount.<para/>
        /// </summary>
        internal static async Task<bool> SetIntegerForIdentifier(string table, string targetColumn, Dictionary<string, object> whereConditions, int integer, int setting, bool canBeNegative)
        {
            string whereClause = string.Join(" AND ", whereConditions.Keys.Select(key => $"`{key}` = @{key}"));

            object currentAmount = await SQLExecuteScalar( $"SELECT `{targetColumn}` FROM `{table}` WHERE {whereClause}", whereConditions);

            if (currentAmount == null) return false;

            int newAmount;

            if (setting == 1)
                newAmount = Convert.ToInt32(currentAmount) + integer;
            else if (setting == 2)
            {
                newAmount = Convert.ToInt32(currentAmount) - integer;
                if (newAmount < 0 && !canBeNegative)
                    newAmount = 0;
            }
            else
                newAmount = integer;

            var updateParameters = whereConditions;
            updateParameters.Add("column", newAmount);

            int updateCount = await SQLExecuteNonQuery( $"UPDATE `{table}` SET `{targetColumn}` = @column WHERE {whereClause}", updateParameters);

            return updateCount > 0;
        }
        #endregion
    }
}
