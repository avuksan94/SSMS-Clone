using SqlViewer.Crud;
using SqlViewer.Dal;
using SqlViewer.Models;
using System.Data;
using System.Data.SqlClient;

namespace SqlViewer.View
{
    public partial class MainForm : Form
    {
        private const string FileFilter = "XML files(*.xml)|*.xml|All files(*.*)|*.*";
        private const string FileName = "{0}.xml";

        private List<Database>? databases;
        private Database? database;
        private DBEntity? dbEntity;
        private enum TagType
        {
            Databases, Tables, Views, Procedures, Functions
        }
        public MainForm()
        {
            InitializeComponent();
            LoadDatabases();
            InitTreeView();
            ClearForm();
        }

        private void LoadDatabases() => databases = new List<Database>(RepositoryFactory.GetRepository().GetDatabases());

        private void InitTreeView()
        {
            // we need empty so it is expandable
            var databaseNode = new TreeNode(TagType.Databases.ToString(), new[] { new TreeNode() }) { Tag = TagType.Databases };
            _ = twServer?.Nodes.Add(databaseNode);
        }

        private void ClearForm()
        {
            tbContent.Text = string.Empty;
            dbEntity = null;
        }

        private void TsbSelect_Click(object sender, EventArgs e)
        {
           
        }

        private void TsbXML_Click(object sender, EventArgs e)
        {
            if (dbEntity == null)
            {
                return;
            }
            var dialog = new SaveFileDialog
            {
                FileName = string.Format(FileName, dbEntity.Name),
                Filter = FileFilter
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DataSet ds = RepositoryFactory.GetRepository().CreateDataSet(dbEntity);
                ds.WriteXml(dialog.FileName, XmlWriteMode.WriteSchema);
            }
        }

        private void TwServer_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is null || databases == null)
            {
                return;
            }
            ClearForm();
            twServer.BeginUpdate();

            switch (e.Node)
            {
                case { Tag: TagType.Databases }:
                    e.Node.Nodes.Clear();
                    databases.ForEach(db => e.Node.Nodes.Add(new TreeNode(db.ToString(), new[] { new TreeNode() }) { Tag = db }));
                    break;
                case { Tag: Database db }:
                    e.Node.Nodes.Clear();
                    e.Node.Nodes.Add(new TreeNode(TagType.Tables.ToString(), new[] { new TreeNode() }) { Tag = TagType.Tables });
                    e.Node.Nodes.Add(new TreeNode(TagType.Views.ToString(), new[] { new TreeNode() }) { Tag = TagType.Views });
                    e.Node.Nodes.Add(new TreeNode(TagType.Procedures.ToString(), new[] { new TreeNode() }) { Tag = TagType.Procedures });
                    e.Node.Nodes.Add(new TreeNode(TagType.Functions.ToString(), new[] { new TreeNode() }) { Tag = TagType.Functions });
                    break;
                case { Tag: TagType.Tables }:
                    e.Node.Nodes.Clear();
                    database = e.Node.Parent.Tag as Database;
                    database?.Tables.ToList().ForEach(table => e.Node.Nodes.Add(new TreeNode(table.ToString(), new[] { new TreeNode() }) { Tag = table }));
                    break;

                case { Tag: TagType.Views }:
                    e.Node.Nodes.Clear();
                    database = e.Node.Parent.Tag as Database;
                    database?.Views.ToList().ForEach(view => e.Node.Nodes.Add(new TreeNode(view.ToString(), new[] { new TreeNode() }) { Tag = view }));
                    break;
                case { Tag: TagType.Procedures }:
                    e.Node.Nodes.Clear();
                    database = e.Node.Parent.Tag as Database;
                    database?.Procedures.ToList().ForEach(proc => e.Node.Nodes.Add(new TreeNode(proc.ToString(), new[] { new TreeNode() }) { Tag = proc }));
                    break;
                case { Tag: TagType.Functions }:
                    e.Node.Nodes.Clear();
                    database = e.Node.Parent.Tag as Database;
                    database?.Functions.ToList().ForEach(func => e.Node.Nodes.Add(new TreeNode(func.ToString(), new[] { new TreeNode() }) { Tag = func }));
                    break;
                case { Tag: DBEntity entity }:
                    e.Node.Nodes.Clear();
                    dbEntity = entity;
                    entity.Columns.ToList().ForEach(column => e.Node.Nodes.Add(new TreeNode(column.ToString())));
                    break;
                case { Tag: DatabaseRoutine routine }:
                    if (routine.Type == RoutineType.Procedure)
                    {
                        e.Node.Nodes.Clear();
                        tbContent.Text = routine.Definition;
                        routine.Parameters.ToList().ForEach(param => e.Node.Nodes.Add(new TreeNode(param.ToString())));
                    }
                    if (routine.Type == RoutineType.Function)
                    {
                        e.Node.Nodes.Clear();
                        tbContent.Text = routine.Definition;
                        routine.Parameters.ToList().ForEach(param => e.Node.Nodes.Add(new TreeNode(param.ToString())));
                    }
                    break;
            }

            twServer.EndUpdate();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) => Application.Exit();

        private void TwServer_AfterCollapse(object sender, TreeViewEventArgs e) => ClearForm();

        private void btnExecuteCRUD_Click(object sender, EventArgs e)
        {
            tbMessages.Clear();
            tbMessages.ForeColor = Color.Black;
            if (database == null)
            {
                MessageBox.Show("Please select a database before executing the query.");
                return;
            }

            string query = tbContent.Text;

            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Please enter a SQL query.");
                return;
            }

            SQLQueryType queryType = DetermineQueryType(query);

            try
            {
                switch (queryType)
                {
                    case SQLQueryType.Select:
                        DataTable result = RepositoryFactory.GetRepository().ExecuteSelectQuery(query, database);
                        // Display the result in a DataGridView.
                        dgResults.DataSource = result;
                        tbMessages.Text = $"{result.Rows.Count} rows returned.";
                        break;
                    case SQLQueryType.Insert:
                    case SQLQueryType.Update:
                    case SQLQueryType.Delete:
                        int affectedRows = RepositoryFactory.GetRepository().ExecuteNonQuery(query, database);
                        tbMessages.Text = ($"{affectedRows} rows affected.");
                        break;
                    case SQLQueryType.Create:
                        RepositoryFactory.GetRepository().ExecuteDDL(query, database);
                        tbMessages.Text = "Create command executed successfully.";
                        break;
                    case SQLQueryType.Alter:
                        RepositoryFactory.GetRepository().ExecuteDDL(query, database);
                        tbMessages.Text = "Alter command executed successfully.";
                        break;
                    case SQLQueryType.Drop:
                        RepositoryFactory.GetRepository().ExecuteDDL(query, database);
                        tbMessages.Text = "Drop command executed successfully.";
                        break;
                    case SQLQueryType.Truncate:
                        RepositoryFactory.GetRepository().ExecuteDDL(query, database);
                        tbMessages.Text = "Truncate command executed successfully.";
                        break;
                    case SQLQueryType.Exec:
                        // Split the query into individual words
                        string[] words = query.Trim().Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                        // Check if there are enough words to contain the procedure name
                        if (words.Length < 2)
                        {
                            tbMessages.Text = "Invalid EXEC format.";
                            break;
                        }

                        string procName = words[1];

                        List<SqlParameter> parameters = new List<SqlParameter>();

                        if (words.Length > 2)
                        {
                            for (int i = 2; i < words.Length; i += 3)
                            {
                                if (i + 2 < words.Length && words[i + 1] == "=")
                                {
                                    string paramName = words[i];
                                    string paramValue = words[i + 2];

                                    parameters.Add(new SqlParameter(paramName, paramValue));
                                }
                                else
                                {
                                    tbMessages.Text = $"Invalid parameter format starting at: {words[i]}";
                                    return;
                                }
                            }
                        }

                        DataTable procResult = RepositoryFactory.GetRepository().ExecuteStoredProcedure(procName, parameters, database);
                        dgResults.DataSource = procResult;
                        tbMessages.Text = $"{procResult.Rows.Count} rows returned.";

                        break;

                    default:
                        tbMessages.ForeColor = Color.Red;
                        tbMessages.Text = "Unsupported or unrecognized SQL command.";
                        break;
                }
            }
            catch (Exception ex)
            {
                tbMessages.ForeColor = Color.Red;
                tbMessages.Text = ex.Message;
            }
        }

        public SQLQueryType DetermineQueryType(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return SQLQueryType.Other;

            string[] words = query.Trim().Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return SQLQueryType.Other;

            switch (words[0].ToUpper())
            {
                //DML
                case "SELECT":
                    return SQLQueryType.Select;
                case "INSERT":
                    return SQLQueryType.Insert;
                case "UPDATE":
                    return SQLQueryType.Update;
                case "DELETE":
                    return SQLQueryType.Delete;
                //DDL ones
                case "CREATE":
                    return SQLQueryType.Create;
                case "ALTER":
                    return SQLQueryType.Alter;
                case "DROP":
                    return SQLQueryType.Drop;
                case "TRUNCATE":
                    return SQLQueryType.Truncate;
                case "EXEC":
                    return SQLQueryType.Exec;
                default:
                    return SQLQueryType.Other;
            }
        }

        private void lbInfo_Click(object sender, EventArgs e)
        {

        }
    }
}