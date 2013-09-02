using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace THOK.Optimize
{
    public class StockOptimize
    {
        public DataTable Optimize(bool isUseSynchronizeOptimize, DataTable channelTable, DataTable orderCTable, DataTable orderTTable, String orderDate, int batchNo)
        {
            
            foreach (DataRow row in orderCTable.Rows)
            {
                if (channelTable.Select(String.Format("CIGARETTECODE='{0}'", row["CIGARETTECODE"])).Length == 0)
                {
                    DataRow[] channelRows = channelTable.Select("(CHANNELTYPE = '1' OR CHANNELTYPE ='2')AND LEN(TRIM(CIGARETTECODE)) = 0", "ORDERNO");
                    if (channelRows.Length != 0)
                    {
                        channelRows[0]["CIGARETTECODE"] = row["CIGARETTECODE"];
                        channelRows[0]["CIGARETTENAME"] = row["CIGARETTENAME"];
                        channelRows[0]["QUANTITY"] = row["QUANTITY"];
                    }
                    else if(isUseSynchronizeOptimize)
                        throw new Exception("通道机分拣卷烟品牌数大于件烟缓存道数量，请减少在通道机上进行分拣的卷烟品牌。");
                    else
                        break ;  
                }
                else
                    continue;                

            }

            DataTable mixTable = GetMixTable();
            foreach (DataRow row in orderTTable.Rows)
            {
                if (channelTable.Select(String.Format("CIGARETTECODE='{0}'", row["CIGARETTECODE"])).Length == 0)
                {
                    DataRow[] channelRows = channelTable.Select("CHANNELTYPE = '3'", "QUANTITY ASC");
                    if (channelRows.Length != 0)
                    {
                        mixTable.Rows.Add(new object[] { orderDate, batchNo, channelRows[0]["CHANNELCODE"], row["CIGARETTECODE"], row["CIGARETTENAME"] });

                        channelRows[0]["QUANTITY"] = Convert.ToInt32(channelRows[0]["QUANTITY"]) + Convert.ToInt32(row["QUANTITY"]);
                    }
                }
                else
                    continue;
            }
            
            return mixTable;
        }

        public DataTable GetMixTable()
        {
            DataTable table = new DataTable("MIX");
            table.Columns.Add("ORDERDATE");
            table.Columns.Add("BATCHNO");
            table.Columns.Add("CHANNELCODE");
            table.Columns.Add("CIGARETTECODE");
            table.Columns.Add("CIGARETTENAME");

            return table;
        }
    }
}
