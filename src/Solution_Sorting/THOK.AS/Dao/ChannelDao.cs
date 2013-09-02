using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
namespace THOK.AS.Dao
{
    public class ChannelDao : BaseDao
    {
        public DataTable FindAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT * FROM AS_BI_CHANNEL " + where;
            return ExecuteQuery(sql, "AS_BI_CHANNEL", startRecord, pageSize).Tables[0];
        }

        public int FindCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM AS_BI_CHANNEL " + where;
            return (int)ExecuteScalar(sql);
        }

        /// <summary>
        /// 2010-11-19 todo
        /// </summary>
        /// <param name="lineCode"></param>
        /// <returns></returns>
        public DataSet FindAvailableChannel(string lineCode)
        {
            string sql = string.Format("SELECT * ,ROW_NUMBER() OVER (ORDER BY CASE WHEN GROUPNO > 8 THEN ABS(GROUPNO - 17) ELSE GROUPNO END,GROUPNO) AS CHANNELINDEX FROM AS_BI_CHANNEL WHERE LINECODE = '{0}' ", lineCode);
            return ExecuteQuery(sql);
        }

        public DataSet FindChannelSchedule(string orderDate, int batchNo, string lineCode, bool IsAdvancedSupply)
        {
            string sql = "SELECT *,CASE WHEN CHANNELTYPE='3' THEN 50 ELSE 40 END REMAINQUANTITY,CASE WHEN CHANNELTYPE='3' THEN QUANTITY / 50 - 3 ELSE QUANTITY / 50 -1 END PIECE " +
                "FROM AS_SC_CHANNELUSED WHERE LINECODE = '{0}' AND BATCHNO = '{1}' AND ORDERDATE = '{2}'";
            if (IsAdvancedSupply)
            {
                sql = "SELECT *, CASE WHEN CHANNELTYPE='3' THEN 50 ELSE 50 END REMAINQUANTITY,CASE WHEN CHANNELTYPE='3' THEN QUANTITY / 50 ELSE QUANTITY / 50 -1 END PIECE " +
                "FROM AS_SC_CHANNELUSED WHERE LINECODE = '{0}' AND BATCHNO = '{1}' AND ORDERDATE = '{2}'";
            }
            return ExecuteQuery(string.Format(sql, lineCode, batchNo, orderDate));
        }

        /// <summary>
        /// 2010-11-21
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="lineCode"></param>
        /// <returns></returns>22
        public DataSet FindChannelSchedule(string orderDate, int batchNo, string lineCode,int remainCount)
        {
            string sql = "SELECT *, 50 REMAINQUANTITY,CASE WHEN CHANNELTYPE='3' THEN QUANTITY / 50 - 3 ELSE QUANTITY / 50 - 1 END PIECE " +
                "FROM AS_SC_CHANNELUSED WHERE LINECODE = '{0}' AND BATCHNO = '{1}' AND ORDERDATE = '{2}' ORDER BY CHANNELORDER";

            sql = "SELECT A.*,D.CIGARETTECODE  AS D_CIGARETTECODE,"+
                    " 	CASE WHEN (A.QUANTITY + ISNULL(B.QUANTITY,0))%50 > 16 " +
                    " 		    THEN (A.QUANTITY + ISNULL(B.QUANTITY,0))%50 - 16" +
                    "       WHEN (ISNULL(C.QUANTITY,0) - ISNULL(B.QUANTITY,0)) > 16 " +
                    " 		    THEN (ISNULL(C.QUANTITY,0) - ISNULL(B.QUANTITY,0)) - 16" +
                    " 		ELSE 50 + (A.QUANTITY + ISNULL(B.QUANTITY,0))%50 + (ISNULL(C.QUANTITY,0) - ISNULL(B.QUANTITY,0)) - 16" +
                    " 	END REMAINQUANTITY, " +
                    " 	CASE WHEN A.CHANNELTYPE='3' " +
                    " 		THEN " +
                    " 	        CASE WHEN (A.QUANTITY + ISNULL(B.QUANTITY,0))%50 > 16 " +
                    " 		            THEN (A.QUANTITY + ISNULL(B.QUANTITY,0))/ 50 - {3} " +
                    "               WHEN (ISNULL(C.QUANTITY,0) - ISNULL(B.QUANTITY,0)) > 16 " +
                    " 		            THEN (A.QUANTITY + ISNULL(B.QUANTITY,0))/ 50 - {3} " +
                    " 		        ELSE (A.QUANTITY + ISNULL(B.QUANTITY,0))/ 50 - ({3} + 1 ) " +
                    " 	        END " +
                    " 		ELSE (A.QUANTITY + ISNULL(B.QUANTITY,0))/ 50 - 1 " +
                    " 	END PIECE, " +
                    "   ISNULL(C.QUANTITY,0) AS BALANCE, "+
                    "   (A.QUANTITY + ISNULL(B.QUANTITY,0)) AS SUPPLYQUANTITY" +
                    " FROM AS_SC_CHANNELUSED A" +
                    " LEFT JOIN (SELECT CHANNELID,LINECODE,CHANNELCODE,CHANNELNAME," +
                    " 	CIGARETTECODE,CIGARETTENAME,SUM(QUANTITY) AS QUANTITY " +
                    " 	FROM AS_SC_BALANCE" +
                    " 	WHERE LINECODE = '{0}' " +
                    " 	AND BATCHNO = '{1}' " +
                    " 	AND ORDERDATE = '{2}' " +
                    " 	GROUP BY CHANNELID,LINECODE,CHANNELCODE,CHANNELNAME," +
                    " 	CIGARETTECODE,CIGARETTENAME) B " +
                    " ON A.CHANNELID = B.CHANNELID" +
                    " LEFT JOIN (SELECT CHANNELID,LINECODE,CHANNELCODE,CHANNELNAME," +
                    " 	CIGARETTECODE,CIGARETTENAME,SUM(QUANTITY) AS QUANTITY " +
                    " 	FROM AS_SC_BALANCE " +
                    " 	GROUP BY CHANNELID,LINECODE,CHANNELCODE,CHANNELNAME," +
                    " 	CIGARETTECODE,CIGARETTENAME) C" +
                    " ON A.CHANNELID = C.CHANNELID" +
                    " LEFT JOIN AS_BI_CHANNEL D ON A.CHANNELID = D.CHANNELID " +
                    " WHERE A.LINECODE = '{0}' " +
                    " AND A.BATCHNO = '{1}' " +
                    " AND A.ORDERDATE = '{2}' " +
                    " ORDER BY A.CHANNELORDER";
            return ExecuteQuery(string.Format(sql, lineCode, batchNo, orderDate,remainCount));
        }

        /// <summary>
        /// 2010-11-19 todo
        /// </summary>
        /// <param name="channelTable"></param>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void SaveChannelSchedule(DataTable channelTable, string orderDate, int batchNo)
        {
            foreach (DataRow channelRow in channelTable.Rows)
            {
                SqlCreate sql = new SqlCreate("AS_SC_CHANNELUSED", SqlType.INSERT);
                sql.AppendQuote("ORDERDATE", orderDate);
                sql.AppendQuote("BATCHNO", batchNo);
                sql.AppendQuote("CHANNELID", channelRow["CHANNELID"]);
                sql.AppendQuote("LINECODE", channelRow["LINECODE"]);

                sql.AppendQuote("CHANNELCODE", channelRow["CHANNELCODE"]);
                sql.AppendQuote("CHANNELNAME", channelRow["CHANNELNAME"]);
                sql.AppendQuote("CHANNELTYPE", channelRow["CHANNELTYPE"]);
                
                sql.AppendQuote("CIGARETTECODE", channelRow["CIGARETTECODE"]);
                sql.AppendQuote("CIGARETTENAME", channelRow["CIGARETTENAME"]);
                sql.Append("QUANTITY", channelRow["QUANTITY"]);

                sql.AppendQuote("STATUS", channelRow["STATUS"]);

                sql.AppendQuote("LEDGROUP", channelRow["LEDGROUP"]);
                sql.AppendQuote("LEDNO", channelRow["LEDNO"]);
                sql.AppendQuote("LED_X", channelRow["LED_X"]);
                sql.AppendQuote("LED_Y", channelRow["LED_Y"]);
                sql.AppendQuote("LED_WIDTH", channelRow["LED_WIDTH"]);
                sql.AppendQuote("LED_HEIGHT", channelRow["LED_HEIGHT"]);
                
                sql.AppendQuote("GROUPNO", channelRow["GROUPNO"]);
                sql.AppendQuote("CHANNELGROUP", channelRow["CHANNELGROUP"]);
                sql.AppendQuote("CHANNELORDER", channelRow["CHANNELORDER"]);
                sql.AppendQuote("CHANNELADDRESS", channelRow["CHANNELADDRESS"]);
                sql.AppendQuote("SUPPLYADDRESS", channelRow["SUPPLYADDRESS"]);

                ExecuteNonQuery(sql.GetSQL());
            }
        }

        /// <summary>
        /// 2010-11-21
        /// </summary>
        /// <param name="channelTable"></param>
        public void Update(DataTable channelTable)
        {
            foreach (DataRow channelRow in channelTable.Rows)
            {
                string sql = string.Format("UPDATE AS_SC_CHANNELUSED SET SORTNO={0} WHERE ORDERDATE='{1}' AND BATCHNO='{2}' AND LINECODE='{3}' AND CHANNELCODE='{4}'",
                    channelRow["SORTNO"], channelRow["ORDERDATE"], channelRow["BATCHNO"].ToString().Trim(), channelRow["LINECODE"], channelRow["CHANNELCODE"]);
                ExecuteNonQuery(sql);
            }
        }

        public void Update(DataTable channelTable, string orderDate, int batchNo)
        {
            foreach (DataRow channelRow in channelTable.Rows)
            {
                string sql = string.Format("UPDATE AS_SC_CHANNELUSED SET SORTNO={0} ,QUANTITY = '{1}' WHERE ORDERDATE='{2}' AND BATCHNO='{3}' AND LINECODE='{4}' AND CHANNELCODE='{5}'",
                    channelRow["SORTNO"],channelRow["QUANTITY"], orderDate, batchNo, channelRow["LINECODE"], channelRow["CHANNELCODE"]);
                ExecuteNonQuery(sql);
            }
        }

        public string GetEnableChannel()
        {
            string strSql = "SELECT COUNT(CHANNELID) FROM AS_BI_CHANNEL WHERE CHANNELTYPE ='2' AND STATUS='1'";
            return ExecuteQuery(strSql).Tables[0].Rows[0][0].ToString();
        }

        public DataTable FindMultiBrandChannel(string lineCode)
        {
            string strSql = string.Format("SELECT * ,0 SORTNO FROM AS_BI_CHANNEL WHERE CHANNELTYPE ='5' AND STATUS='1' AND LINECODE='{0}' ", lineCode);
            return ExecuteQuery(strSql).Tables[0];
        }

        /// <summary>
        /// 2010-11-21
        /// </summary>
        /// <param name="channelTable"></param>
        public void UpdateQuantity(DataTable channelTable, bool isUseBalance)
        {
            foreach (DataRow channelRow in channelTable.Rows)
            {
                string sql = "UPDATE AS_SC_CHANNELUSED SET CIGARETTECODE = '{0}',CIGARETTENAME = '{1}',QUANTITY={2} " +
                                " WHERE ORDERDATE='{3}' AND BATCHNO='{4}' AND LINECODE='{5}' AND CHANNELCODE='{6}'";
                ExecuteNonQuery(string.Format(sql,channelRow["CIGARETTECODE"],channelRow["CIGARETTENAME"],channelRow["QUANTITY"],
                    channelRow["ORDERDATE"], channelRow["BATCHNO"].ToString().Trim(), channelRow["LINECODE"], channelRow["CHANNELCODE"]));

                if (isUseBalance && channelRow["CHANNELTYPE"].ToString() == "3" && channelRow["CIGARETTECODE"].ToString() == channelRow["D_CIGARETTECODE"].ToString())
                {
                    SqlCreate sqlCreate = new SqlCreate("AS_SC_BALANCE", SqlType.INSERT);

                    sqlCreate.AppendQuote("ORDERDATE", channelRow["ORDERDATE"]);
                    sqlCreate.Append("BATCHNO", channelRow["BATCHNO"]);
                    sqlCreate.AppendQuote("CHANNELID", channelRow["CHANNELID"]);
                    sqlCreate.AppendQuote("LINECODE", channelRow["LINECODE"]);
                    sqlCreate.AppendQuote("CHANNELCODE", channelRow["CHANNELCODE"]);
                    sqlCreate.AppendQuote("CHANNELNAME", channelRow["CHANNELNAME"]);
                    sqlCreate.AppendQuote("CIGARETTECODE", channelRow["CIGARETTECODE"]);
                    sqlCreate.AppendQuote("CIGARETTENAME", channelRow["CIGARETTENAME"]);

                    int quantity = Convert.ToInt32(channelRow["QUANTITY"]) % 50;
                    int balance = Convert.ToInt32(channelRow["BALANCE"]);

                    sqlCreate.Append("QUANTITY", balance >= quantity ? 0 - quantity : 50 - quantity);

                    ExecuteNonQuery(sqlCreate.GetSQL());
                }
                else
                {
                    ExecuteNonQuery(string.Format ("DELETE FROM AS_SC_BALANCE WHERE CHANNELID = '{0}'",channelRow["CHANNELID"]));
                }
            }
        }

        internal void UpdateEntity(string channelID, string cigaretteCode, string cigaretteName, string status)
        {
            SqlCreate sqlCreate = new SqlCreate("AS_BI_CHANNEL", SqlType.UPDATE);
            sqlCreate.AppendQuote("CIGARETTECODE", cigaretteCode.Trim());
            sqlCreate.AppendQuote("CIGARETTENAME", cigaretteName.Trim());
            sqlCreate.AppendQuote("STATUS", status);
            sqlCreate.AppendWhereQuote("CHANNELID", channelID);
            ExecuteNonQuery(sqlCreate.GetSQL());

            string sql = "INSERT INTO AS_SC_BALANCE_OUT"+
                            " SELECT GETDATE(),LINECODE,CHANNELCODE,CHANNELNAME,CIGARETTECODE, CIGARETTENAME, "+
                            " SUM(QUANTITY) AS QUANTITY,0"+
                            " FROM AS_SC_BALANCE"+
                            " WHERE CHANNELID = '{0}'" +
                            " GROUP BY LINECODE,CHANNELCODE,CHANNELNAME,CIGARETTECODE, CIGARETTENAME"+
                            " HAVING ISNULL(SUM(QUANTITY),0) > 0 ";
            ExecuteNonQuery(string.Format(sql, channelID));

            sql = "DELETE FROM AS_SC_BALANCE WHERE CHANNELID = '{0}'";
            ExecuteNonQuery(string.Format(sql, channelID));
        }
    }
}
