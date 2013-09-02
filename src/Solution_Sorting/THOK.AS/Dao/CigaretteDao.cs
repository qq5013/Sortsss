using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class CigaretteDao : BaseDao
    {
        public DataTable FindAll(int startRecord, int pageSize, string filter)
        {
             string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT * FROM AS_BI_CIGARETTE" + where;
            return ExecuteQuery(sql, "AS_BI_CIGARETTE", startRecord, pageSize).Tables[0];
        }

        public int FindCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM AS_BI_CIGARETTE " + where;
            return (int)ExecuteScalar(sql);
        }

        public void UpdateEntity(string cigaretteCode,string cigaretteName,string isAbnormity, string barcode)
        {
            SqlCreate sqlCreate = new SqlCreate("AS_BI_CIGARETTE", SqlType.UPDATE);
            sqlCreate.AppendQuote("CIGARETTENAME", cigaretteName);
            sqlCreate.AppendQuote("ISABNORMITY", isAbnormity);
            sqlCreate.AppendQuote("BARCODE", barcode);
            sqlCreate.AppendWhereQuote("CIGARETTECODE", cigaretteCode);
            ExecuteNonQuery(sqlCreate.GetSQL());
        }

        public void BatchInsertCigarette(DataTable dtData)
        {
            BatchInsert(dtData, "AS_BI_CIGARETTE");
        }

        public void Clear()
        {
            string sql = "TRUNCATE TABLE AS_BI_CIGARETTE";
            ExecuteNonQuery(sql);
        }

        internal void SynchronizeCigarette(DataTable cigaretteTable)
        {
            foreach (DataRow row in cigaretteTable.Rows)
            {
                string sql = "IF '{0}' IN (SELECT CIGARETTECODE FROM AS_BI_CIGARETTE) " +
                                "BEGIN " +
                                    "IF '{1}' =  '1' " +
                                    "BEGIN " +
                                        "UPDATE AS_BI_CIGARETTE SET ISABNORMITY = '{1}',CIGARETTENAME = '{2}' WHERE CIGARETTECODE = '{0}' " +
                                    "END " +
                                "END " +
                             "ELSE " +
                                "BEGIN " +
                                    "INSERT AS_BI_CIGARETTE VALUES ('{0}','{2}','{1}','{3}') " +
                                "END";
                sql = string.Format(sql, row["CIGARETTECODE"], row["ISABNORMITY"].ToString() == "1" ? "1" : "0", row["CIGARETTENAME"], row["BARCODE"]);
                ExecuteNonQuery(sql);
            }
        }
    }
}
