using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseMetadata.Entities
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
        Procedure,
        View
    }

    public enum Level2Types
    {
        NULL,
        Parameter,
        Column,
        Index,
        Constraint
    }
}
