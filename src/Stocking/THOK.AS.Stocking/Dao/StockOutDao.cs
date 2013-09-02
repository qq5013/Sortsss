using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Stocking.Dao
{
    public class StockOutDao: BaseDao
    {
        string sql;
        //~ ��ѯ�Ѳ�����������
        public int FindOutQuantity()
        {
            return Convert.ToInt32(ExecuteScalar( "SELECT COUNT(*) FROM AS_STOCK_OUT WHERE STATE='1' "));
        }

        //~ ���¾���������Ϣ��
        public void UpdateCigarette(string barcode, string CIGARETTECODE)
        {
            sql = "UPDATE AS_SC_SUPPLY SET BARCODE = '{0}' WHERE CIGARETTECODE = '{1}'";
            ExecuteNonQuery(string.Format(sql, barcode, CIGARETTECODE));

            sql = "UPDATE AS_STOCK_IN SET BARCODE = '{0}' WHERE CIGARETTECODE = '{1}'";
            ExecuteNonQuery(string.Format(sql, barcode, CIGARETTECODE));

            sql = "UPDATE AS_STOCK_OUT SET BARCODE = '{0}' WHERE CIGARETTECODE = '{1}'";
            ExecuteNonQuery(string.Format(sql, barcode, CIGARETTECODE));
        }

        //~ ��ѯ�������ID
        internal int FindMaxOutID()
        {
            return Convert.ToInt32(ExecuteScalar("SELECT ISNULL(MAX(STOCKOUTID),0) FROM AS_STOCK_OUT"));
        }

        //~ ���ɲ����ƻ���
        internal void Insert(int outID, DataTable supplyTable)
        {
            foreach (DataRow row in supplyTable.Rows)
            {
                SqlCreate sqlCreate = new SqlCreate("AS_STOCK_OUT", SqlType.INSERT);
                sqlCreate.Append("STOCKOUTID", ++outID);
                sqlCreate.AppendQuote("ORDERDATE", row["ORDERDATE"]);
                sqlCreate.Append("BATCHNO", row["BATCHNO"]);
                sqlCreate.AppendQuote("LINECODE", row["LINECODE"]);
                sqlCreate.Append("SORTNO", row["SORTNO"]);
                sqlCreate.Append("SERIALNO", row["SERIALNO"]);
                sqlCreate.AppendQuote("CIGARETTECODE", row["CIGARETTECODE"]);
                sqlCreate.AppendQuote("CIGARETTENAME", row["CIGARETTENAME"]);
                sqlCreate.AppendQuote("BARCODE", row["BARCODE"]);
                sqlCreate.AppendQuote("CHANNELCODE", row["CHANNELCODE"]);
                ExecuteNonQuery(sqlCreate.GetSQL());
            }
        }



        //~ ��ѯδ�żƻ�����Ĳ����ƻ���
        public DataTable FindNoSupplyOrder(bool b1,bool b2)
        {
            string sql = @"SELECT * FROM AS_STOCK_OUT C
                            LEFT JOIN AS_SC_CHANNELUSED D
		                            ON C.ORDERDATE = D.ORDERDATE
		                            AND C.BATCHNO = D.BATCHNO
		                            AND C.LINECODE = D.LINECODE
		                            AND C.CHANNELCODE = D.CHANNELCODE
	                        WHERE D.CHANNELTYPE != '{0}' AND D.CHANNELTYPE != '{1}'
                            AND C.STATE='0'  
                            ORDER BY STOCKOUTID";
            return ExecuteQuery(string.Format(sql, b1 ? 5 : 0, b2 ? 2 : 0)).Tables[0];
        }

        //~ ���²�������ƻ�Ϊ���żƻ����⣻
        public void UpdateStatus(DataTable table)
        {
            DataRow[] stockOutRows = table.Select(string.Format("STATE = '1'"), "STOCKOUTID");
            foreach (DataRow row in stockOutRows)
            {
                SqlCreate sqlCreate = new SqlCreate("AS_STOCK_OUT", SqlType.UPDATE);
                sqlCreate.AppendQuote("STATE", "1");
                sqlCreate.AppendWhere("STOCKOUTID", row["STOCKOUTID"]);
                ExecuteNonQuery(sqlCreate.GetSQL());
            }
        }
    }
}
