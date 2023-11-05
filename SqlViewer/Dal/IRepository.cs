using SqlViewer.Models;
using System.Data;
using System.Data.SqlClient;

namespace SqlViewer.Dal
{
    internal interface IRepository
    {
        DataSet CreateDataSet(DBEntity dbEntity);
        IEnumerable<Column> GetColumns(DBEntity entity);
        IEnumerable<Database> GetDatabases();
        IEnumerable<DBEntity> GetDBEntities(Database database, DBEntityType entityType);
        IEnumerable<Parameter> GetParameters(DatabaseRoutine routine);
        IEnumerable<DatabaseRoutine> GetDatabaseRoutines(Database database, RoutineType routineType);
        //CRUD
        DataTable ExecuteSelectQuery(string query, Database database);
        int ExecuteNonQuery(string query, Database database);
        void ExecuteDDL(string ddlQuery, Database database);
        DataTable ExecuteStoredProcedure(string procName, List<SqlParameter> parameters, Database database);
        void LogIn(string server, string username, string password);
    }
}