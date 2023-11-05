using SqlViewer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region myComments

//https://learn.microsoft.com/en-us/dotnet/api/system.data.dataset?view=net-7.0&viewFallbackFrom=dotnet-plat-ext-7.0
//https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/nameof
//https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader?view=dotnet-plat-ext-7.0
//https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand?view=dotnet-plat-ext-7.0
//https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldataadapter?view=dotnet-plat-ext-7.0


/*
 Yield Return: The methods that return collections (IEnumerable<T>) use the yield return keyword, 
which allows them to produce a sequence of values lazily (on-demand). This means they will only compute 
and return the next value in the sequence when it is requested, rather than computing all values upfront.

Using Statements: The using statement ensures that the database connections, commands, and readers 
are disposed of correctly after usage, freeing up resources(Football manager app problem with closing connections)

Nullable connection string: The comment suggests that this code might be compared with
Kotlin where nullable handling is strict, but in C# (especially in C# 8.0 onwards),
nullable reference types allow similar strict null checks.

SqlCommand: This code uses ADO.NET's SqlCommand for executing SQL against a database. 
 */

#endregion

namespace SqlViewer.Dal
{
    internal class SqlRepository : IRepository
    {
        #region constants
        private const string ConnectionString = "Server={0};Uid={1};Pwd={2}";
        private const string SelectDatabases = "SELECT name As Name FROM sys.databases";
        private const string SelectEntities = "SELECT TABLE_SCHEMA AS [Schema], TABLE_NAME AS Name FROM {0}.INFORMATION_SCHEMA.{1}S";
        private const string SelectProcedures = "SELECT SPECIFIC_NAME as Name, ROUTINE_DEFINITION as Definition FROM {0}.INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE'";
        private const string SelectColumns = "SELECT COLUMN_NAME as Name, DATA_TYPE as DataType FROM {0}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{1}'";
        private const string SelectProcFuncParameters = "SELECT PARAMETER_NAME as Name, PARAMETER_MODE as Mode, DATA_TYPE as DataType FROM {0}.INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME='{1}'";
        private const string SelectQuery = "SELECT * FROM {0}.{1}.{2}";
        //My ADD 
        private const string SelectViews = "SELECT TABLE_NAME as Name, VIEW_DEFINITION as Definition FROM {0}.INFORMATION_SCHEMA.VIEWS";
        private const string SelectFunctions = "SELECT SPECIFIC_NAME as Name, ROUTINE_DEFINITION as Definition FROM {0}.INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'FUNCTION'";

        #endregion

        public string? cs; // nullable -> here we can use cs, with only warning, in kotlin cannot compile!

        public void LogIn(string server, string username, string password)
        {
            using (SqlConnection con = new SqlConnection(string.Format(ConnectionString, server, username, password)))
            {
                cs = con.ConnectionString; // paziti na redoslijed - nakon, nestaje password!!!
                con.Open(); // let us open the connection so we can see whether we are logged in
                //cs = con.ConnectionString;
            }
        }
        public IEnumerable<Database> GetDatabases()
        {
            using SqlConnection con = new(cs);
            con.Open();
            using SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = SelectDatabases;
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                yield return new Database()
                {
                    Name = dr[nameof(Database.Name)].ToString()
                };
            }
        }
        public IEnumerable<DBEntity> GetDBEntities(Database database, DBEntityType entityType)
        {
            using SqlConnection con = new(cs);
            con.Open();
            using SqlCommand cmd = con.CreateCommand();

            cmd.CommandText = string.Format(SelectEntities, database.Name, entityType.ToString());
            cmd.CommandType = System.Data.CommandType.Text;
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                yield return new DBEntity()
                {
                    Name = dr[nameof(DBEntity.Name)].ToString(),
                    Schema = dr[nameof(DBEntity.Schema)].ToString(),
                    Database = database
                };
            }
        }
        public IEnumerable<DatabaseRoutine> GetDatabaseRoutines(Database database, RoutineType routineType)
        {
            using SqlConnection con = new SqlConnection(cs);
            con.Open();
            using SqlCommand cmd = con.CreateCommand();

            string selectQuery = routineType == RoutineType.Procedure ? SelectProcedures : SelectFunctions;

            cmd.CommandText = string.Format(selectQuery, database.Name);
            cmd.CommandType = System.Data.CommandType.Text;
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                yield return new DatabaseRoutine()
                {
                    Name = dr[nameof(DatabaseRoutine.Name)].ToString(),
                    Definition = dr[nameof(DatabaseRoutine.Definition)].ToString(),
                    Database = database,
                    Type = routineType
                };
            }
        }

        public IEnumerable<Column> GetColumns(DBEntity entity)
        {
            using SqlConnection con = new(cs);
            con.Open();
            using SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = string.Format(SelectColumns, entity.Database?.Name, entity.Name);
            cmd.CommandType = System.Data.CommandType.Text;
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                yield return new Column()
                {
                    Name = dr[nameof(Column.Name)].ToString(),
                    DataType = dr[nameof(Column.DataType)].ToString()
                };
            }
        }
        public IEnumerable<Parameter> GetParameters(DatabaseRoutine routine)
        {
            using SqlConnection con = new SqlConnection(cs);
            con.Open();
            using SqlCommand cmd = con.CreateCommand();

            cmd.CommandText = string.Format(SelectProcFuncParameters, routine.Database?.Name, routine.Name);
            cmd.CommandType = System.Data.CommandType.Text;
            //Console.WriteLine(cmd.ToString());
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                yield return new Parameter
                {
                    Name = dr[nameof(Parameter.Name)].ToString(),
                    Mode = dr[nameof(Parameter.Mode)].ToString(),
                    DataType = dr[nameof(Parameter.DataType)].ToString()
                };
            }
        }

        public DataSet CreateDataSet(DBEntity dbEntity)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlDataAdapter da = new SqlDataAdapter(string.Format(SelectQuery, dbEntity.Database, dbEntity.Schema, dbEntity.Name), con);
                DataSet ds = new DataSet(dbEntity.Name!);
                da.Fill(ds);
                ds.Tables[0].TableName = dbEntity.Name;
                return ds;
            }
        }

        //SELECT
        public DataTable ExecuteSelectQuery(string query, Database database)
        {
            string? databaseName = database.Name;
            //validate inputs
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query should not be empty.", nameof(query));
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Database name should not be empty.", nameof(databaseName));

            //using a DataSet to hold the result
            DataTable dt = new();

            using (SqlConnection con = new(cs))
            {
                con.Open();

                con.ChangeDatabase(databaseName);

                SqlDataAdapter da = new SqlDataAdapter(query, con);

                //try to fill the DataSet
                //this will only work for SELECT statements. 
                da.Fill(dt);
            }

            return dt;
        }

        //INSERT,UPDATE,DELETE
        public int ExecuteNonQuery(string query, Database database)
        {
            string? databaseName = database.Name;
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Query should not be empty.", nameof(query));

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Database name should not be empty.", nameof(databaseName));

            int affectedRows = 0;
            using (SqlConnection con = new(cs))
            {
                con.Open();

                // Ensure the connection is switched to the correct database
                con.ChangeDatabase(databaseName);

                using SqlCommand cmd = new(query, con);
                affectedRows = cmd.ExecuteNonQuery();  //returns number of affected rows
            }

            return affectedRows;
        }

        //DDL(i could use ExecuteNonQuery but i dont need to return anything for DDL)
        public void ExecuteDDL(string ddlQuery, Database database)
        {
            string? databaseName = database.Name;
            // Validate inputs
            if (string.IsNullOrWhiteSpace(ddlQuery))
                throw new ArgumentException("DDL query should not be empty!", nameof(ddlQuery));
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Database name should not be empty!", nameof(databaseName));

            using SqlConnection con = new(cs);
            con.Open();

            con.ChangeDatabase(databaseName);

            using SqlCommand cmd = new SqlCommand(ddlQuery, con);
            cmd.ExecuteNonQuery(); // Execute the DDL command
        }

        public DataTable ExecuteStoredProcedure(string procName, List<SqlParameter> parameters, Database database)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(cs))
            {
                using (SqlCommand cmd = new SqlCommand(procName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }

                    connection.Open();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }


    }
}
