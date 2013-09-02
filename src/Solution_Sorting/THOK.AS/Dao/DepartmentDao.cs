using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using System.Data.SqlClient;
using System.Data.Sql;

namespace THOK.AS.Dao
{
    public class DepartmentDao : BaseDao
    {
        public DataTable FindAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT * FROM AS_BI_DEPARTMENT " + where;
            return ExecuteQuery(sql, "AS_BI_DEPARTMENT", startRecord, pageSize).Tables[0];
        }

        public int FindCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM AS_BI_DEPARTMENT " + where;
            return (int)ExecuteScalar(sql);
        }


        public void UpdateEntity(string departmentID, string departmentName, string remark)
        {
            SqlCreate sqlCreate = new SqlCreate("AS_BI_DEPARTMENT", SqlType.UPDATE);
            sqlCreate.AppendQuote("DEPARTMENTNAME", departmentName);
            sqlCreate.AppendQuote("REMARK", remark);
            sqlCreate.AppendWhereQuote("DEPARTMENTID", departmentID);
            ExecuteNonQuery(sqlCreate.GetSQL());
        }

        public void InsertEntity(string departmentName, string remark)
        {
            int maxID = Convert.ToInt32(ExecuteScalar("SELECT CASE WHEN MAX(DEPARTMENTID) IS NULL THEN 0 ELSE MAX(DEPARTMENTID) END FROM AS_BI_DEPARTMENT")) + 1;
            SqlCreate sqlCreate = new SqlCreate("AS_BI_DEPARTMENT", SqlType.INSERT);
            sqlCreate.AppendQuote("DEPARTMENTID", maxID.ToString().PadLeft(5, '0'));
            sqlCreate.AppendQuote("DEPARTMENTNAME", departmentName);
            sqlCreate.AppendQuote("REMARK", remark);
            ExecuteNonQuery(sqlCreate.GetSQL());
        }

        public void DeleteEntity(string departmentID)
        {
            string sql = string.Format("DELETE FROM AS_BI_DEPARTMENT WHERE DEPARTMENTID = '{0}'", departmentID);
            ExecuteNonQuery(sql);
        }
    }
}

