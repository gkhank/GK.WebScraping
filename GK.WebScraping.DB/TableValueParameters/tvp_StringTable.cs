using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace GK.WebScraping.DB
{
    [Keyless]
    public class tvp_StringTable
    {
        public string Value { get; set; }

        private static readonly SqlMetaData[] recordSchema = {
            new SqlMetaData("value", SqlDbType.VarChar, 900),
        };

        public static SqlDataRecord ToSqlDataRecord(object value)
        {
            var record = new SqlDataRecord(recordSchema);
            record.SetString(0, Convert.ToString(value));
            return record;
        }

        public static SqlParameter GenerateSqlParameter(String parameterName, params object[] values)
        {
            SqlParameter retval = new SqlParameter(parameterName, System.Data.SqlDbType.Structured);
            retval.Value = Array.ConvertAll<object, SqlDataRecord>(values, tvp_StringTable.ToSqlDataRecord);
            retval.TypeName = "tvp_StringTable";
            return retval;
        }
    }
}
