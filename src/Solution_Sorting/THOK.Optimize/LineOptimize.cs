using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace THOK.Optimize
{
    public class LineOptimize
    {
        public DataTable Optimize(DataTable routeTable, DataTable lineTable, string orderDate, int batchNo)
        {
            DataTable scLineTable = GetEmptyTable();

            if (lineTable.Rows.Count > 1)
            {
                DataColumn column = new DataColumn("TOTALQUANTITY", typeof(Int32));
                column.DefaultValue = 0;
                lineTable.Columns.Add(column);

                foreach (DataRow routeRow in routeTable.Rows)
                {
                    DataRow lineRow = null;
                    DataRow [] lineRows = lineTable.Select(string.Format("LINECODE = '{0}'", routeRow["LINECODE"]), "TOTALQUANTITY");
                    if (lineRows.Length > 0)
                    {
                        lineRow = lineRows[0];
                    }
                    else
                    {
                        lineRow = lineTable.Select("", "TOTALQUANTITY")[0];
                    }

                    AddRow(scLineTable, routeRow, lineRow["LINECODE"], orderDate, batchNo);
                    lineRow["TOTALQUANTITY"] = Convert.ToInt32(lineRow["TOTALQUANTITY"]) + Convert.ToInt32(routeRow["QUANTITY"]);
                }

                lineTable.Columns.Remove("TOTALQUANTITY");
            }
            else if (lineTable.Rows.Count == 1)
            {
                object lineCode = lineTable.Rows[0]["LINECODE"];
                foreach (DataRow routeRow in routeTable.Rows)
                {
                    AddRow(scLineTable, routeRow, lineCode, orderDate, batchNo);
                }
            }
            else
            {
                throw new Exception("可用的生产线数为0，请检查生产线基础表。");
            }

            return scLineTable;
        }

        private DataTable GetEmptyTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("LINECODE");
            table.Columns.Add("ROUTECODE");
            table.Columns.Add("QUANTITY", typeof(Int32));
            table.Columns.Add("BATCHNO");
            table.Columns.Add("ORDERDATE");
            
            return table;
        }

        private void AddRow(DataTable lineTable, DataRow routeRow, object lineCode, string orderDate, int batchNo)
        {
            DataRow row = lineTable.NewRow();
            row["LINECODE"] = lineCode;
            row["ROUTECODE"] = routeRow["ROUTECODE"];
            row["QUANTITY"] = routeRow["QUANTITY"];
            row["BATCHNO"] = batchNo;
            row["ORDERDATE"] = orderDate;
            lineTable.Rows.Add(row);
        }
    }
}
