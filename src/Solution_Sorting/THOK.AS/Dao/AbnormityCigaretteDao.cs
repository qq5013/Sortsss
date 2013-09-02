using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class AbnormityCigaretteDao: BaseDao
    {
        public DataTable FindLineAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT ORDERDATE, BATCHNO, LINECODE, CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY) QUANTITY FROM V_I_ABNORMALCIGARETTE " + where;
            sql += " GROUP BY ORDERDATE, BATCHNO, LINECODE, CIGARETTECODE, CIGARETTENAME ORDER BY ORDERDATE, BATCHNO, LINECODE";
            return ExecuteQuery(sql, "V_I_ABNORMALCIGARETTE", startRecord, pageSize).Tables[0];
        }

        public DataTable FindRouteAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT ORDERDATE, BATCHNO, ROUTECODE, ROUTENAME, CUSTOMERCODE, CUSTOMERNAME, CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY) QUANTITY FROM V_I_ABNORMALCIGARETTE " + where;
            sql += " GROUP BY ORDERDATE, BATCHNO, ROUTECODE, ROUTENAME, CUSTOMERCODE, CUSTOMERNAME, CIGARETTECODE, CIGARETTENAME";
            return ExecuteQuery(sql, "V_I_ABNORMALCIGARETTE", startRecord, pageSize).Tables[0];
        }

        public int FindLineCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM (SELECT ORDERDATE, BATCHNO, LINECODE, CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY) QUANTITY FROM V_I_ABNORMALCIGARETTE " + where;
            sql += " GROUP BY ORDERDATE, BATCHNO, LINECODE, CIGARETTECODE, CIGARETTENAME ) A";
            return (int)ExecuteScalar(sql);
        }

        public int FindRouteCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM (SELECT ORDERDATE, BATCHNO, ROUTECODE, ROUTENAME, CUSTOMERCODE, CUSTOMERNAME, CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY) QUANTITY FROM V_I_ABNORMALCIGARETTE " + where;
            sql += " GROUP BY ORDERDATE, BATCHNO, ROUTECODE, ROUTENAME, CUSTOMERCODE, CUSTOMERNAME, CIGARETTECODE, CIGARETTENAME ) A";
            return (int)ExecuteScalar(sql);
        }
    }
}
