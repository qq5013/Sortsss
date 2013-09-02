using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace THOK.Optimize
{
    public class SupplyOptimize
    {
        public DataTable Optimize(DataTable channelTable)
        {
            DataTable supplyTable = GetEmptySupply();

            int serialNo = 1;

            foreach (DataRow row in channelTable.Rows)
            {
                //int quantity = Convert.ToInt32(row["QUANTITY"]) / 50;
                int supplyQuantity1 = Convert.ToInt32(row["SUPPLYQUANTITY"])/50;
                int supplyQuantity2 = Convert.ToInt32(row["PIECE"]);

                if (supplyQuantity1 >= 1 && row["CHANNELTYPE"].ToString() != "5")
                {
                    int count = 1;
                    if (row["CHANNELTYPE"].ToString() == "3")
                    {
                        count = supplyQuantity1 - (supplyQuantity2 > 0?supplyQuantity2:0) ;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        DataRow supplyRow = supplyTable.NewRow();
                        supplyRow["ORDERDATE"] = row["ORDERDATE"];
                        supplyRow["BATCHNO"] = row["BATCHNO"];
                        supplyRow["LINECODE"] = row["LINECODE"];
                        supplyRow["SERIALNO"] = serialNo++;

                        supplyRow["SORTNO"] = 0;

                        supplyRow["CIGARETTECODE"] = row["CIGARETTECODE"];
                        supplyRow["CIGARETTENAME"] = row["CIGARETTENAME"];

                        supplyRow["CHANNELCODE"] = row["CHANNELCODE"];
                        supplyRow["CHANNELGROUP"] = row["CHANNELGROUP"]; 
                        supplyRow["GROUPNO"] = row["GROUPNO"];
                        supplyRow["BATCH"] = i;

                        supplyTable.Rows.Add(supplyRow);
                    }
                }
            }
            return supplyTable;
        }

        public DataTable Optimize(DataTable channelTable, DataRow[] orderRows,ref int serialNo)
        {
            DataTable supplyTable = GetEmptySupply();
            DataRow channelRow = channelTable.Rows[0];
            foreach (DataRow row in orderRows)
            {
                int quantity = Convert.ToInt32(row["QUANTITY"]) / 50;

                if (quantity > 0)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        DataRow supplyRow = supplyTable.NewRow();
                        supplyRow["ORDERDATE"] = row["ORDERDATE"];
                        supplyRow["BATCHNO"] = channelRow["BATCHNO"];
                        supplyRow["LINECODE"] = channelRow["LINECODE"];

                        supplyRow["SERIALNO"] = serialNo;
                        supplyRow["SORTNO"] = serialNo;

                        supplyRow["CIGARETTECODE"] = row["CIGARETTECODE"];
                        supplyRow["CIGARETTENAME"] = row["CIGARETTENAME"];

                        supplyRow["CHANNELCODE"] = channelRow["CHANNELCODE"];
                        supplyRow["CHANNELGROUP"] = channelRow["CHANNELGROUP"];
                        supplyRow["GROUPNO"] = channelRow["GROUPNO"];
                        supplyRow["BATCH"] = 1;

                        supplyTable.Rows.Add(supplyRow);
                        serialNo++;
                    }                    
                }
            }
            return supplyTable;
        }

        private DataTable GetEmptySupply()
        {
            DataTable table = new DataTable("SUPPLY");

            table.Columns.Add("ORDERDATE");
            table.Columns.Add("BATCHNO");
            table.Columns.Add("LINECODE");
            table.Columns.Add("SERIALNO", typeof(Int32));
            
            table.Columns.Add("SORTNO");

            table.Columns.Add("CIGARETTECODE");
            table.Columns.Add("CIGARETTENAME");
            
            table.Columns.Add("CHANNELCODE");
            table.Columns.Add("CHANNELGROUP");
            table.Columns.Add("GROUPNO");
            table.Columns.Add("BATCH");

            return table;
        }
    }
}
