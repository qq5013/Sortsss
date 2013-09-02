using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class SupplyDao: BaseDao
    {
        string lineCode = "";
        int serialNo = 0;

        public void DeleteSupply()
        {
            string sql = "TRUNCATE TABLE AS_SC_SUPPLY";
            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 2010-11-21
        /// </summary>
        /// <param name="supplyTable"></param>
        public void InsertSupply(DataTable supplyTable,bool isUseSerialNo)
        {
            DataRow[] rows = supplyTable.Select("", "BATCH,SERIALNO");
            int serialNo = 1;
            foreach (DataRow row in rows)
            {
                SqlCreate sqlCreate = new SqlCreate("AS_SC_SUPPLY", SqlType.INSERT);
                sqlCreate.AppendQuote("ORDERDATE", row["ORDERDATE"]);
                sqlCreate.Append("BATCHNO", row["BATCHNO"]);
                sqlCreate.AppendQuote("LINECODE", row["LINECODE"]);
                sqlCreate.Append("SERIALNO", isUseSerialNo ? row["SERIALNO"] : serialNo++);
                
                sqlCreate.Append("ORIGINALSORTNO", row["SORTNO"]);
                sqlCreate.Append("SORTNO", row["SORTNO"]);

                sqlCreate.AppendQuote("CIGARETTECODE", row["CIGARETTECODE"]);
                sqlCreate.AppendQuote("CIGARETTENAME", row["CIGARETTENAME"]);

                sqlCreate.AppendQuote("CHANNELCODE", row["CHANNELCODE"]);
                sqlCreate.Append("CHANNELGROUP", row["CHANNELGROUP"]);
                sqlCreate.Append("GROUPNO", row["GROUPNO"]);

                ExecuteNonQuery(sqlCreate.GetSQL());
            }
        }

        /// <summary>
        /// 2010-11-21
        /// </summary>
        /// <param name="supplyTable"></param>
        /// <param name="orderDate"></param>
        /// <param name="lineCode"></param>
        public void InsertSupply(DataTable supplyTable, string orderDate, string lineCode)
        {
            if (this.lineCode != lineCode)
            {
                this.lineCode = lineCode;
                string sql = string.Format("SELECT CASE WHEN MAX(SERIALNO) IS NULL THEN 1000 ELSE MAX(SERIALNO) END  FROM AS_SC_SUPPLY WHERE ORDERDATE='{0}' AND LINECODE='{1}'", orderDate, lineCode);
                serialNo = Convert.ToInt32(ExecuteScalar(sql));
            }

            foreach (DataRow row in supplyTable.Rows)
            {
                SqlCreate sqlCreate = new SqlCreate("AS_SC_SUPPLY", SqlType.INSERT);
                sqlCreate.AppendQuote("ORDERDATE", row["ORDERDATE"]);
                sqlCreate.Append("BATCHNO", row["BATCHNO"]);
                sqlCreate.AppendQuote("LINECODE", row["LINECODE"]);
                sqlCreate.Append("SORTNO", row["SORTNO"]);

                sqlCreate.Append("SERIALNO", serialNo++);                
                sqlCreate.Append("ORIGINALSORTNO", row["SORTNO"]);               
                
                sqlCreate.AppendQuote("CIGARETTECODE", row["CIGARETTECODE"]);
                sqlCreate.AppendQuote("CIGARETTENAME", row["CIGARETTENAME"]);

                sqlCreate.AppendQuote("CHANNELCODE", row["CHANNELCODE"]);
                sqlCreate.Append("CHANNELGROUP", row["CHANNELGROUP"]);
                sqlCreate.Append("GROUPNO", row["GROUPNO"]);

                ExecuteNonQuery(sqlCreate.GetSQL());
            }
        }

        public void InsertSupply(DataTable supplyTable, string lineCode, string orderDate, int batchNo)
        {
            if (this.lineCode != lineCode)
            {
                this.lineCode = lineCode;
                string sql = string.Format("SELECT CASE WHEN MAX(SERIALNO) IS NULL THEN 1000 ELSE MAX(SERIALNO) END  FROM AS_SC_SUPPLY WHERE LINECODE='{0}' AND ORDERDATE = '{1}' AND BATCHNO = {2} ", lineCode, orderDate, batchNo);
                serialNo = Convert.ToInt32(ExecuteScalar(sql));
            }

            foreach (DataRow row in supplyTable.Rows)
            {
                SqlCreate sqlCreate = new SqlCreate("AS_SC_SUPPLY", SqlType.INSERT);
                sqlCreate.AppendQuote("ORDERDATE", row["ORDERDATE"]);
                sqlCreate.Append("BATCHNO", row["BATCHNO"]);
                sqlCreate.Append("SERIALNO", serialNo++);
                sqlCreate.AppendQuote("LINECODE", row["LINECODE"]);
                sqlCreate.Append("ORIGINALSORTNO", row["SORTNO"]);
                sqlCreate.Append("SORTNO", row["SORTNO"]);
                sqlCreate.Append("GROUPNO", row["GROUPNO"]);
                sqlCreate.Append("CHANNELGROUP", row["CHANNELGROUP"]);
                sqlCreate.AppendQuote("CHANNELCODE", row["CHANNELCODE"]);
                sqlCreate.AppendQuote("CIGARETTECODE", row["CIGARETTECODE"]);
                sqlCreate.AppendQuote("CIGARETTENAME", row["CIGARETTENAME"]);
                ExecuteNonQuery(sqlCreate.GetSQL());
            }
        }

        public void AdjustSortNo(string lineCode, int aheadCount,string orderDate, int batchNo)
        {
            //aheadCount = aheadCount + 1000;
            //string sql = string.Format("UPDATE AS_SC_SUPPLY SET SORTNO = CASE WHEN SERIALNO <= {0} THEN 1 ELSE ORIGINALSORTNO - (SELECT MAX(ORIGINALSORTNO) FROM AS_SC_SUPPLY WHERE SERIALNO <= {0} AND LINECODE='{1}' AND ORDERDATE = '{2}' AND BATCHNO = {3}) + 1 END WHERE LINECODE='{1}' AND ORDERDATE = '{2}' AND BATCHNO = {3}", aheadCount, lineCode, orderDate, batchNo);
        }

        public void AdjustSortNo(string lineCode, int aheadCount)
        {
            string sql = string.Format("UPDATE AS_SC_SUPPLY SET SORTNO=CASE WHEN ORIGINALSORTNO<={0} THEN 1 ELSE ORIGINALSORTNO - {0} END WHERE LINECODE='{1}'", aheadCount, lineCode);
            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void DeleteSchedule(string orderDate, int batchNo)
        {
            string sql = string.Format("DELETE FROM AS_SC_SUPPLY WHERE ORDERDATE = '{0}' AND BATCHNO='{1}'", orderDate, batchNo);
            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <param name="orderDate"></param>
        public void DeleteHistory(string orderDate)
        {
            string sql = string.Format("DELETE FROM AS_SC_SUPPLY WHERE ORDERDATE < '{0}'", orderDate);
            ExecuteNonQuery(sql);
        }


        internal void AdjustSortNo1(string orderDate, int batchNo, string lineCode, int aheadCount1, int aheadCount2)
        {
            string sql = "UPDATE AS_SC_SUPPLY SET SORTNO = 1 " +
                            " WHERE ORDERDATE= '{0}' AND BATCHNO = '{1}' AND LINECODE = '{2}'" +
                            " AND SERIALNO IN " +
                            " (" +
                            " 	SELECT TOP {3} SERIALNO FROM AS_SC_SUPPLY A" +
                            " 	LEFT JOIN AS_BI_CHANNEL B " +
                            " 	ON A.LINECODE = B.LINECODE" +
                            " 	AND A.CHANNELCODE = B.CHANNELCODE" +
                            " 	WHERE A.ORDERDATE= '{0}' AND A.BATCHNO = '{1}' AND A.LINECODE = '{2}'" +
                            " 	AND A.ORIGINALSORTNO != 0	" +
                            " 	AND A.CHANNELGROUP = '{4}'	" +
                            " 	AND B.CHANNELTYPE = '{5}'" +
                            " 	ORDER BY SERIALNO" +
                            " )  " +

                            " UPDATE AS_SC_SUPPLY SET SORTNO = ORIGINALSORTNO + 1 -" +
                            " (" +
                            " 	SELECT MAX(ORIGINALSORTNO) FROM AS_SC_SUPPLY" +
                            " 	WHERE ORDERDATE= '{0}' AND BATCHNO = '{1}' AND LINECODE = '{2}' " +
                            " 	AND SERIALNO IN " +
                            " 	(" +
                            " 		SELECT TOP {3} SERIALNO FROM AS_SC_SUPPLY A" +
                            " 		LEFT JOIN AS_BI_CHANNEL B " +
                            " 		ON A.LINECODE = B.LINECODE" +
                            " 		AND A.CHANNELCODE = B.CHANNELCODE" +
                            " 		WHERE A.ORDERDATE= '{0}' AND A.BATCHNO = '{1}' AND A.LINECODE = '{2}'" +
                            " 		AND A.ORIGINALSORTNO != 0" +
                            " 		AND A.CHANNELGROUP = '{4}'" +
                            " 		AND B.CHANNELTYPE = '{5}'" +
                            " 		ORDER BY SERIALNO" +
                            " 	)" +
                            " )" +
                            " WHERE ORDERDATE= '{0}' AND BATCHNO = '{1}' AND LINECODE = '{2}'" +
                            " AND SERIALNO IN " +
                            " (" +
                            " 	SELECT SERIALNO " +
                            " 	FROM AS_SC_SUPPLY A " +
                            " 	LEFT JOIN AS_BI_CHANNEL B " +
                            " 	ON A.LINECODE = B.LINECODE " +
                            " 	AND A.CHANNELCODE = B.CHANNELCODE" +
                            " 	WHERE A.ORDERDATE= '{0}' AND A.BATCHNO = '{1}' AND A.LINECODE = '{2}'" +
                            " 	AND A.ORIGINALSORTNO != 0 " +
                            " 	AND A.SORTNO != 1 	" +
                            " 	AND A.CHANNELGROUP = '{4}'" +
                            " 	AND B.CHANNELTYPE = '{5}'" +
                            " )";

            ExecuteNonQuery(string.Format(sql, orderDate, batchNo, lineCode, aheadCount1, 1, 2));
            ExecuteNonQuery(string.Format(sql, orderDate, batchNo, lineCode, aheadCount2, 1, 3));
            ExecuteNonQuery(string.Format(sql, orderDate, batchNo, lineCode, aheadCount1, 2, 2));
            ExecuteNonQuery(string.Format(sql, orderDate, batchNo, lineCode, aheadCount2, 2, 3));
        }

        internal void AdjustSortNo(string orderDate, int batchNo, string lineCode, int channelGroup, int channelType, int aheadCount)
        {
            string sql = "UPDATE AS_SC_SUPPLY SET SORTNO = 0 " +
                " WHERE ORDERDATE= '{0}' AND BATCHNO = '{1}' AND LINECODE = '{2}'" +
                " AND SERIALNO IN " +
                " (" +
                " 	SELECT TOP {3} SERIALNO FROM AS_SC_SUPPLY A" +
                " 	LEFT JOIN AS_BI_CHANNEL B " +
                " 	ON A.LINECODE = B.LINECODE" +
                " 	AND A.CHANNELCODE = B.CHANNELCODE" +
                " 	WHERE A.ORDERDATE= '{0}' AND A.BATCHNO = '{1}' AND A.LINECODE = '{2}'" +
                " 	AND A.ORIGINALSORTNO != 0	" +
                " 	AND A.CHANNELGROUP = '{4}'	" +
                " 	AND B.CHANNELTYPE = '{5}'" +
                " 	ORDER BY SERIALNO" +
                " )  " +

                " UPDATE AS_SC_SUPPLY SET SORTNO = ORIGINALSORTNO + 1 -" +
                " (" +
                " 	SELECT MAX(ORIGINALSORTNO) FROM AS_SC_SUPPLY" +
                " 	WHERE ORDERDATE= '{0}' AND BATCHNO = '{1}' AND LINECODE = '{2}' " +
                " 	AND SERIALNO IN " +
                " 	(" +
                " 		SELECT TOP {3} SERIALNO FROM AS_SC_SUPPLY A" +
                " 		LEFT JOIN AS_BI_CHANNEL B " +
                " 		ON A.LINECODE = B.LINECODE" +
                " 		AND A.CHANNELCODE = B.CHANNELCODE" +
                " 		WHERE A.ORDERDATE= '{0}' AND A.BATCHNO = '{1}' AND A.LINECODE = '{2}'" +
                " 		AND A.ORIGINALSORTNO != 0" +
                " 		AND A.CHANNELGROUP = '{4}'" +
                " 		AND B.CHANNELTYPE = '{5}'" +
                " 		ORDER BY SERIALNO" +
                " 	)" +
                " )" +
                " WHERE ORDERDATE= '{0}' AND BATCHNO = '{1}' AND LINECODE = '{2}'" +
                " AND SERIALNO IN " +
                " (" +
                " 	SELECT SERIALNO " +
                " 	FROM AS_SC_SUPPLY A " +
                " 	LEFT JOIN AS_BI_CHANNEL B " +
                " 	ON A.LINECODE = B.LINECODE " +
                " 	AND A.CHANNELCODE = B.CHANNELCODE" +
                " 	WHERE A.ORDERDATE= '{0}' AND A.BATCHNO = '{1}' AND A.LINECODE = '{2}'" +
                " 	AND A.ORIGINALSORTNO != 0 " +
                " 	AND A.SORTNO != 0 " +
                " 	AND A.CHANNELGROUP = '{4}'" +
                " 	AND B.CHANNELTYPE = '{5}'" +
                " 	AND B.CHANNELTYPE = '2'" +
                " )";

            ExecuteNonQuery(string.Format(sql, orderDate, batchNo, lineCode, aheadCount, channelGroup, channelType));
        }
    }
}
