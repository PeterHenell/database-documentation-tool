using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtendedPropertiesDocumentationTool
{
    public enum SaveModes
    {
        New,
        Update,
        Delete,
        NoAction
    }

    public enum Level1Types
    {
        Table,
        Procedure
    }

    public enum Level2Types
    {
        NULL,
        Parameter,
        Column
    }
}
