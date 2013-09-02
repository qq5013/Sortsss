using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class LineInfoDao : BaseDao
    {
        public DataTable FindAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT * FROM AS_BI_LINEINFO " + where;
            return ExecuteQuery(sql, "AS_BI_LINEINFO", startRecord, pageSize).Tables[0];
        }

        public int FindCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM AS_BI_LINEINFO " + where;
            return (int)ExecuteScalar(sql);
        }

        public void UpdateEntity(string lineCode, string status)
        {
            SqlCreate sqlCreate = new SqlCreate("AS_BI_LINEINFO", SqlType.UPDATE);
            sqlCreate.AppendQuote("STATUS", status);
            sqlCreate.AppendWhereQuote("LINECODE", lineCode);
            ExecuteNonQuery(sqlCreate.GetSQL());
        }

        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <returns></returns>
        public DataSet GetAvailabeLine(string lineType)
        {
            string sql = "SELECT * FROM AS_BI_LINEINFO WHERE LINETYPE = '{0}' AND STATUS = '1'";
            return ExecuteQuery(string.Format(sql,lineType));
        }
    }
}
