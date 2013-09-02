using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
namespace THOK.AS.Dao
{
    public class BatchDao : BaseDao
    {
        public DataTable FindAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            String sql = "SELECT *,ISNULL(BATCHNO_ONEPRO,BATCHNO) AS NO1BATCH FROM AS_BI_BATCH" + where + "ORDER BY ORDERDATE DESC,BATCHNO DESC";
            return ExecuteQuery(sql, "AS_BI_BATCH", startRecord, pageSize).Tables[0];
        }

        public int FindCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM AS_BI_BATCH " + where;
            return (int)ExecuteScalar(sql);
        }

        public DataTable FindBatch(string orderDate)
        {
            string sql = string.Format("SELECT * FROM AS_BI_BATCH WHERE ORDERDATE='{0}' ORDER BY BATCHNO", orderDate);
            return ExecuteQuery(sql).Tables[0];
        }

        public DataTable FindBatch(string orderDate, int batchNo)
        {
            string sql = string.Format("SELECT * FROM AS_BI_BATCH WHERE ORDERDATE='{0}' AND BATCHNO={1}", orderDate, batchNo);
            return ExecuteQuery(sql).Tables[0];
        }

        public bool BatchNoExists(string orderDate, int batchNo)
        {
            string sql = string.Format("SELECT COUNT(*) FROM AS_BI_BATCH WHERE ORDERDATE='{0}' AND BATCHNO={1}", orderDate, batchNo);
            return Convert.ToBoolean(ExecuteScalar(sql));
        }

        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <param name="orderDate"></param>
        public void DeleteHistory(string orderDate)
        {
            string sql = string.Format("DELETE FROM AS_BI_BATCH WHERE ORDERDATE < '{0}'", orderDate);            
            ExecuteNonQuery(sql);
        }

        public void InsertEntity(string orderDate, int batchNo)
        {
            DateTime SCDATE = DateTime.Parse(orderDate);

            SqlCreate sqlCreate = new SqlCreate("AS_BI_BATCH", SqlType.INSERT);
            sqlCreate.Append("BATCHNO", batchNo);
            sqlCreate.AppendQuote("BATCHNAME", string.Format("{0}第{1}批次", orderDate, batchNo));
            sqlCreate.AppendQuote("ORDERDATE", orderDate);
            sqlCreate.AppendQuote("ISVALID", 0);
            sqlCreate.AppendQuote("EXECUTEUSER", 0);
            sqlCreate.AppendQuote("EXECUTEIP", 0);
            sqlCreate.AppendQuote("ISUPTONOONEPRO", 0);
            sqlCreate.AppendQuote("SCDATE", DateTime.Now.ToShortDateString());
            ExecuteNonQuery(sqlCreate.GetSQL());
        }

        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ip"></param>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void UpdateExecuter(string user, string ip, string orderDate, int batchNo)
        {
            string sql = string.Format("UPDATE AS_BI_BATCH SET EXECUTEUSER='{0}',EXECUTEIP='{1}' " +
                "WHERE ORDERDATE='{2}' AND BATCHNO={3}", user, ip, orderDate, batchNo);
            ExecuteNonQuery(sql);
        }

        public void UpdateNoOnePro(string orderDate, int batchNo, string user)
        {
            string sql = string.Format("UPDATE AS_BI_BATCH SET ISUPTONOONEPRO='1',SENDNOONEUSER='{0}' " +
                "WHERE ORDERDATE='{1}' AND BATCHNO={2}", user, orderDate, batchNo);
            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="status"></param>
        public void UpdateIsValid(string orderDate, int batchNo, string status)
        {
            string sql = "UPDATE AS_BI_BATCH SET ISVALID='{0}' WHERE ORDERDATE='{1}' AND BATCHNO={2}";
            ExecuteNonQuery(string.Format(sql,status, orderDate, batchNo));
        }


        internal void UpdateEntity(string orderDate, string sortBatch, string no1Batch,string no1UpLoadState)
        {
            string sql = "UPDATE AS_BI_BATCH SET BATCHNO_ONEPRO = {0},ISUPTONOONEPRO='{3}' WHERE ORDERDATE='{1}' AND BATCHNO='{2}'";
            ExecuteNonQuery(string.Format(sql, no1Batch, orderDate, sortBatch, no1UpLoadState));
        }

        internal void SelectBalanceIntoHistory(string orderDate, int batchNo)
        {
            string sql = "DELETE AS_SC_BALANCE_HISTORY WHERE ORDERDATE = '{0}' AND BATCHNO = {1} ";
            ExecuteNonQuery(string.Format(sql, orderDate, batchNo));

            sql = "INSERT INTO AS_SC_BALANCE_HISTORY " +
                    " SELECT '{0}',{1},LINECODE,CHANNELCODE,CHANNELNAME,CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY) AS QUANTITY " +
                    " FROM AS_SC_BALANCE " +
                    " GROUP BY LINECODE,CHANNELCODE,CHANNELNAME,CIGARETTECODE, CIGARETTENAME";
            ExecuteNonQuery(string.Format(sql, orderDate, batchNo));
        }

        internal bool CheckOrder(string orderDate, int batchNo)
        {
            string sql = @"SELECT A.ORDERID,A.ROUTECODE,A.CUSTOMERCODE,B.CIGARETTENAME,
	                            ISNULL(SUM(B.QUANTITY),0),
	                            (SELECT ISNULL(SUM(QUANTITY),0) FROM AS_SC_ORDER   
		                            WHERE ORDERID = A.ORDERID AND CIGARETTECODE = B.CIGARETTECODE),   
	                            ISNULL(SUM(B.QUANTITY),0)-
	                            (SELECT ISNULL(SUM(QUANTITY),0) FROM AS_SC_ORDER   
		                            WHERE ORDERID = A.ORDERID AND CIGARETTECODE = B.CIGARETTECODE ),  
	                            D.CUSTOMERNAME  
	                            FROM AS_I_ORDERMASTER A  
	                            LEFT JOIN AS_I_ORDERDETAIL B ON A.ORDERID = B.ORDERID  
	                            LEFT JOIN AS_BI_CUSTOMER D ON A.CUSTOMERCODE = D.CUSTOMERCODE  
	                            LEFT JOIN AS_BI_CIGARETTE E ON B.CIGARETTECODE = E.CIGARETTECODE   
	                            WHERE  A.ORDERDATE ='{0}' AND A.BATCHNO = {1}  AND E.ISABNORMITY = 0 
	                            AND A.ORDERID NOT IN (SELECT ORDERID FROM AS_HANDLE_SORT_ORDER WHERE ORDERDATE = '{0}')   
	                            GROUP BY A.ORDERID,A.ROUTECODE,A.CUSTOMERCODE,B.CIGARETTECODE,B.CIGARETTENAME,D.CUSTOMERNAME   
	                            HAVING ISNULL(SUM(B.QUANTITY),0) != 
	                            (SELECT ISNULL(SUM(QUANTITY),0) FROM AS_SC_ORDER   
		                            WHERE ORDERID = A.ORDERID AND CIGARETTECODE = B.CIGARETTECODE)  
	                            ORDER BY A.ROUTECODE ";
            return (ExecuteQuery(string.Format(sql, orderDate, batchNo)).Tables[0].Rows.Count == 0);
        }

        /// <summary>
        /// 查询本次优化的总数量
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public int BatchCount(string orderDate, int batchNo)
        {
            string sql = @"SELECT SUM(ORDER_QUANTITY) FROM DBO.AS_I_ORDERDETAIL WHERE ORDERDATE='{0}' AND BATCHNO={1}";
            return Convert.ToInt32(ExecuteQuery(string.Format( sql,orderDate,batchNo)).Tables[0].Rows[0][0]);
        }
    }
}
