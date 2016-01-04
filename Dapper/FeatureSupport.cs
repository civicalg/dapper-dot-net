using System;
using System.Data;
using System.Data.Common;

#if COREFX
using IDbConnection = System.Data.Common.DbConnection;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace Dapper
{
    /// <summary>
    /// Handles variances in features per DBMS
    /// </summary>
    class FeatureSupport
    {
        private static readonly FeatureSupport
            Default = new FeatureSupport(false),
            Postgres = new FeatureSupport(true),
            Db2Informix = new FeatureSupport(false, "(select {0} from systables where 1 = 0)");

        /// <summary>
        /// Gets the feature set based on the passed connection
        /// </summary>
//        public static FeatureSupport Get(IDbConnection connection)
//        {
//            string name = connection?.GetType().Name;
//            if (string.Equals(name, "npgsqlconnection", StringComparison.OrdinalIgnoreCase)) return Postgres;            
//            return Default;
//        }

        //it is safer to use a parameter than connection because of potential wrappers (e.g. MiniProfiler)
        public static FeatureSupport Get(IDbDataParameter parameter)
        {
            string name = parameter?.GetType().Name;
            if (string.Equals(name, "npgsqlparameter", StringComparison.OrdinalIgnoreCase)) return Postgres;
            if (string.Equals(name, "db2parameter", StringComparison.OrdinalIgnoreCase)) return Db2Informix;
            return Default;
        }

        private FeatureSupport(bool arrays, string emptyListSelectTemplate = null)
        {
            Arrays = arrays;
            EmptyListSelectTemplate = emptyListSelectTemplate ?? "(SELECT {0} WHERE 1 = 0)";
        }
        /// <summary>
        /// True if the db supports array columns e.g. Postgresql
        /// </summary>
        public bool Arrays { get; }

        public string EmptyListSelectTemplate { get; }
    }
}
