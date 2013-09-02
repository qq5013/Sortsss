using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class ScOrderDao : BaseDao
    {
        /// <summary>
        /// 2010-11-21
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="lineCode"></param>
        /// <returns></returns>
        public DataTable FindHandleSupplyOrder(string orderDate, int batchNo, string lineCode)
        {
            string sql = string.Format("SELECT * FROM AS_SC_ORDER A " +
                                        " LEFT JOIN AS_BI_CHANNEL B ON A.CHANNELCODE = B.CHANNELCODE AND A.LINECODE = B.LINECODE " +
                                        " WHERE B.CHANNELTYPE='5' AND A.ORDERDATE='{0}' " +
                                        " AND A.BATCHNO='{1}' AND A.LINECODE='{2}' " +
                                        " ORDER BY A.SORTNO ASC,A.QUANTITY DESC", orderDate, batchNo, lineCode);
            return ExecuteQuery(sql).Tables[0];
        }
        
        public void DeleteOldSupplyOrders(string orderDate, int batchNo,string lineCode)
        {
            string sql = string.Format("DELETE FROM AS_SC_ORDER " +
                                        " WHERE CHANNELCODE IN (SELECT CHANNELCODE FROM AS_BI_CHANNEL WHERE CHANNELTYPE = '5' AND LINECODE='{0}' ) "+
                                        " AND ORDERDATE='{1}' AND BATCHNO='{2}' AND LINECODE='{0}' ", lineCode,orderDate, batchNo);
            ExecuteNonQuery(sql);
        }
        public void InsertNewSupplyOrders(DataTable newSupplyOrders)
        {
            foreach (DataRow dataRow in newSupplyOrders.Rows)
            {
                SqlCreate sqlCreate = new SqlCreate("AS_SC_ORDER", SqlType.INSERT);               
                
                sqlCreate.AppendQuote("ORDERDATE", dataRow["ORDERDATE"]);
                sqlCreate.AppendQuote("BATCHNO", dataRow["BATCHNO"]);
                sqlCreate.AppendQuote("LINECODE", dataRow["LINECODE"]);
                sqlCreate.Append("SORTNO", dataRow["SORTNO"]);

                sqlCreate.AppendQuote("ORDERID", dataRow["ORDERID"]);
                sqlCreate.Append("ORDERNO", dataRow["ORDERNO"]);

                sqlCreate.AppendQuote("CIGARETTECODE", dataRow["CIGARETTECODE"]);
                sqlCreate.AppendQuote("CIGARETTENAME", dataRow["CIGARETTENAME"]);

                sqlCreate.AppendQuote("CHANNELCODE", dataRow["CHANNELCODE"]);
                sqlCreate.AppendQuote("CHANNELGROUP", dataRow["CHANNELGROUP"]);
                sqlCreate.AppendQuote("CHANNELORDER", dataRow["CHANNELORDER"]);

                sqlCreate.Append("QUANTITY", dataRow["QUANTITY"]);
                sqlCreate.Append("EXPORTNO", dataRow["EXPORTNO"]);
                sqlCreate.Append("PACKNO", dataRow["PACKNO"]);

                ExecuteNonQuery(sqlCreate.GetSQL());
            }
        }
        public void InsertHandSupplyOrders(DataTable newSupplyOrders)
        {
            foreach (DataRow dataRow in newSupplyOrders.Rows)
            {
                SqlCreate sqlCreate = new SqlCreate("AS_SC_HANDLESUPPLY", SqlType.INSERT);                
                
                sqlCreate.AppendQuote("ORDERDATE", dataRow["ORDERDATE"]);
                sqlCreate.AppendQuote("BATCHNO", dataRow["BATCHNO"]);
                sqlCreate.AppendQuote("LINECODE", dataRow["LINECODE"]);
                sqlCreate.Append("SORTNO", dataRow["SORTNO"]);

                sqlCreate.AppendQuote("SUPPLYBATCH", dataRow["SUPPLYBATCH"]);                
                sqlCreate.AppendQuote("ORDERID", dataRow["ORDERID"]);

                sqlCreate.AppendQuote("CIGARETTECODE", dataRow["CIGARETTECODE"]);
                sqlCreate.AppendQuote("CIGARETTENAME", dataRow["CIGARETTENAME"]);

                sqlCreate.AppendQuote("CHANNELCODE", dataRow["CHANNELCODE"]);

                sqlCreate.Append("QUANTITY", dataRow["QUANTITY"]);

                sqlCreate.AppendQuote("STATUS", "0");

                ExecuteNonQuery(sqlCreate.GetSQL());                
            }
        }
    }
}
