using SqlViewer.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlViewer.Models
{
    internal class Database
    {
		private readonly Lazy<IEnumerable<DBEntity>> tables; //non - nullable -> initialized in constructor
		private readonly Lazy<IEnumerable<DBEntity>> views;
		private readonly Lazy<IEnumerable<DatabaseRoutine>> procedures;
		private readonly Lazy<IEnumerable<DatabaseRoutine>> functions;

		public Database()
		{
			tables = new Lazy<IEnumerable<DBEntity>>(() => RepositoryFactory.GetRepository().GetDBEntities(this, DBEntityType.Table)); // ref this -> must be in constructor!
			views = new Lazy<IEnumerable<DBEntity>>(() => RepositoryFactory.GetRepository().GetDBEntities(this, DBEntityType.View));
            procedures = new Lazy<IEnumerable<DatabaseRoutine>>(() => RepositoryFactory.GetRepository().GetDatabaseRoutines(this, RoutineType.Procedure));
			functions = new Lazy<IEnumerable<DatabaseRoutine>>(() => RepositoryFactory.GetRepository().GetDatabaseRoutines(this, RoutineType.Function));

        }
        public IList<DBEntity> Tables
		{
			get => new List<DBEntity>(tables.Value);
		}
		public IList<DBEntity> Views
		{
			get => new List<DBEntity>(views.Value);
		}

		public IList<DatabaseRoutine> Procedures
		{
			get => new List<DatabaseRoutine>(procedures.Value);
		}

        public IList<DatabaseRoutine> Functions
        {
            get => new List<DatabaseRoutine>(functions.Value);
        }

        public string? Name { get; set; } // nullable
        public override string ToString() => Name!; // ToString() for ComboBox!, null forgiving (!)
    }
}
