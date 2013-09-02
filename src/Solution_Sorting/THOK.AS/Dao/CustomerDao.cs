using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class CustomerDao : BaseDao
    {
        public DataTable FindAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT * FROM V_AS_BI_CUSTOMER " + where + " ORDER BY ROUTECODE, SORTID";
            return ExecuteQuery(sql, "V_AS_BI_CUSTOMER", startRecord, pageSize).Tables[0];
        }


        public int FindCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM V_AS_BI_CUSTOMER " + where;
            return (int)ExecuteScalar(sql);
        }

        public void BatchInsertCustomer(DataTable dtData)
        {
            BatchInsert(dtData, "AS_BI_CUSTOMER");
        }

        public void Clear()
        {
            string sql = "TRUNCATE TABLE AS_BI_CUSTOMER";
            ExecuteNonQuery(sql);
        }

        internal void SynchronizeCustomer(DataTable customerTable)
        {
            foreach (DataRow row in customerTable.Rows)
            {
                string sql = "IF '{0}' IN (SELECT CUSTOMERCODE FROM AS_BI_CUSTOMER) " +
                                " BEGIN " +
                                    " UPDATE AS_BI_CUSTOMER SET CUSTOMERNAME = '{1}',ROUTECODE = '{2}',AREACODE = '{3}',LICENSENO = '{4}', " +
                                    " SORTID = '{5}' ,TELNO = '{6}',ADDRESS = '{7}',N_CUST_CODE = '{8}' " +
                                    " WHERE CUSTOMERCODE = '{0}' " +
                                " END " +
                             "ELSE " +
                                " BEGIN " +
                                    " INSERT AS_BI_CUSTOMER VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}') " +
                                " END";
                sql = string.Format(sql, row["CUSTOMERCODE"], row["CUSTOMERNAME"], row["ROUTECODE"], row["AREACODE"], row["LICENSENO"], row["SORTID"], row["TELNO"], row["ADDRESS"], row["N_CUST_CODE"]);
                ExecuteNonQuery(sql);
            }
        }
    }
}
