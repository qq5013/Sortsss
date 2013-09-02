using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class RouteDao : BaseDao
    {
        public DataTable FindAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT * FROM V_AS_BI_ROUTE " + where + " ORDER BY AREACODE,ROUTECODE";
            return ExecuteQuery(sql, "V_AS_BI_ROUTE", startRecord, pageSize).Tables[0];
        }


        public int FindCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM V_AS_BI_ROUTE " + where;
            return (int)ExecuteScalar(sql);
        }

        public void BatchInsertRoute(DataTable dtData)
        {
            BatchInsert(dtData, "AS_BI_ROUTE");
        }

        /// <summary>
        /// ¸üÐÂ
        /// </summary>
        /// <param name="dataSet"></param>
        public void UpdateEntity(string sortID,string lineCode,string routeCode)
        {
            SqlCreate sqlCreate = new SqlCreate("AS_BI_ROUTE", SqlType.UPDATE);
            sqlCreate.Append("SORTID", sortID);
            sqlCreate.AppendQuote("LINECODE", lineCode);
            sqlCreate.AppendWhereQuote("ROUTECODE", routeCode);
            ExecuteNonQuery(sqlCreate.GetSQL());
        }

        public void Clear()
        {
            string sql = "TRUNCATE TABLE AS_BI_ROUTE";
            ExecuteNonQuery(sql);
        }

        internal void SynchronizeRoute(DataTable routeTable)
        {
            foreach (DataRow row in routeTable.Rows)
            {
                string sql = "IF '{0}' IN (SELECT ROUTECODE FROM AS_BI_ROUTE) " +
                                "BEGIN " +
                                    "UPDATE AS_BI_ROUTE SET ROUTENAME = '{1}',AREACODE = '{2}' WHERE ROUTECODE = '{0}' " +
                                "END " +
                             "ELSE " +
                                "BEGIN " +
                                    "INSERT AS_BI_ROUTE VALUES ('{0}','{1}','{2}','{3}','','') " +
                                "END";
                sql = string.Format(sql, row["ROUTECODE"], row["ROUTENAME"], row["AREACODE"], row["SORTID"]);
                ExecuteNonQuery(sql);
            }
        }
    }
}
