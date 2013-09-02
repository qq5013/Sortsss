using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class EmployeeDao : BaseDao
    {
        public DataTable FindAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT * FROM V_AS_BI_EMPLOYEE " + where;
            return ExecuteQuery(sql, "V_AS_BI_EMPLOYEE", startRecord, pageSize).Tables[0];
        }

        public int FindCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM V_AS_BI_EMPLOYEE " + where;
            return (int)ExecuteScalar(sql);
        }


        public void UpdateEntity(string employeeCode, string employeeName, string departmentID, string status, string remark)
        {
            SqlCreate sqlCreate = new SqlCreate("AS_BI_EMPLOYEE", SqlType.UPDATE);
            sqlCreate.AppendQuote("EMPLOYEECODE", employeeCode);
            sqlCreate.AppendQuote("EMPLOYEENAME", employeeName);
            sqlCreate.AppendQuote("DEPARTMENTID", departmentID);
            sqlCreate.AppendQuote("STATUS", status);
            sqlCreate.AppendQuote("REMARK", remark);
            sqlCreate.AppendWhereQuote("EMPLOYEECODE", employeeCode);
            ExecuteNonQuery(sqlCreate.GetSQL());
        }

        public void InsertEntity(string employeeCode, string employeeName, string departmentID, string status, string remark)
        {
            int maxID = Convert.ToInt32(ExecuteScalar("SELECT CASE WHEN MAX(EMPLOYEEID) IS NULL THEN 0 ELSE MAX(EMPLOYEEID) END FROM AS_BI_EMPLOYEE")) + 1;
            SqlCreate sqlCreate = new SqlCreate("AS_BI_EMPLOYEE", SqlType.INSERT);
            sqlCreate.AppendQuote("EMPLOYEEID", maxID.ToString().PadLeft(5, '0'));
            sqlCreate.AppendQuote("EMPLOYEECODE", employeeCode);
            sqlCreate.AppendQuote("EMPLOYEENAME", employeeName);
            sqlCreate.AppendQuote("DEPARTMENTID", departmentID);
            sqlCreate.AppendQuote("STATUS", status);
            sqlCreate.AppendQuote("REMARK", remark);
            ExecuteNonQuery(sqlCreate.GetSQL());
        }

        public void DeleteEntity(string employeeCode)
        {
            string sql = string.Format("DELETE FROM AS_BI_EMPLOYEE WHERE EMPLOYEECODE = '{0}'", employeeCode);
            ExecuteNonQuery(sql);
        }
    }
}
