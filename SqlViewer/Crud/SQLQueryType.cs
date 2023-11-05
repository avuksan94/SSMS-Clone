using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlViewer.Crud
{
    public enum SQLQueryType
    {
        Select,
        Insert,
        Update,
        Delete,
        Create,
        Alter,
        Drop,
        Truncate,
        Exec,
        Other
    }
}
