using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace GK.WebScraping.DB
{
    [Keyless]
    public class tvp_IntTable
    {
        public int Value { get; set; }

        private static readonly SqlMetaData[] recordSchema = {
            new SqlMetaData("value", SqlDbType.Int),
        };

        public static SqlDataRecord ToSqlDataRecord(object value)
        {
            var record = new SqlDataRecord(recordSchema);
            record.SetInt32(0, Convert.ToInt32(value));
            return record;
        }

        public static SqlParameter GenerateSqlParameter(String parameterName, params object[] values)
        {
            SqlParameter retval = new SqlParameter(parameterName, System.Data.SqlDbType.Structured);
            retval.Value = Array.ConvertAll<object, SqlDataRecord>(values, tvp_IntTable.ToSqlDataRecord);
            retval.TypeName = "tvp_IntTable";
            return retval;
        }
    }
}
