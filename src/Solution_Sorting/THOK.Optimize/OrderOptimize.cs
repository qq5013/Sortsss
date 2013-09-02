using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace THOK.Optimize
{
    /// <summary>
    /// 垂直式半自动分拣系统订单优化
    /// </summary>
    public class OrderOptimize
    {
        public DataSet Optimize(DataRow masterRow, DataRow[] orderRows, DataTable channelTable, ref int sortNo)
        {           
            DataSet ds = new DataSet();
            DataTable masterTable = GetEmptyOrderMaster();
            DataTable detailTable = GetEmptyOrderDetail();
            DataTable supplyTable = GetEmptySupply();

            int sort1 = sortNo;
            int sort2 = sortNo;

            foreach (DataRow detailRow in orderRows)
            {
                DataRow[] channelRows = channelTable.Select(string.Format("CHANNELCODE='{0}'", detailRow["CHANNELCODE"]));
                if (channelRows[0]["CHANNELTYPE"].ToString() == "5")
                {
                    channelRows[0]["CIGARETTECODE"] = detailRow["CIGARETTECODE"];
                    channelRows[0]["CIGARETTENAME"] = detailRow["CIGARETTENAME"];
                }
                if (detailRow["CHANNELGROUP"].ToString() == "1")
                    Optimize(masterRow, channelRows[0], detailTable, supplyTable, Convert.ToInt32(detailRow["QUANTITY"]), ref sort1, Convert.ToInt32(detailRow["ORDERNO"]), Convert.ToInt32(detailRow["EXPORTNO"]), Convert.ToInt32(detailRow["PACKNO"]));
                else
                    Optimize(masterRow, channelRows[0], detailTable, supplyTable, Convert.ToInt32(detailRow["QUANTITY"]), ref sort2, Convert.ToInt32(detailRow["ORDERNO"]), Convert.ToInt32(detailRow["EXPORTNO"]), Convert.ToInt32(detailRow["PACKNO"]));
            }

            int sort = sort1 > sort2 ? sort1 - sortNo : sort2 - sortNo;
            for (int i = 0; i < sort + 1; i++)
            {
                int[] quantity = new int[2];
                for (int j = 0; j < 2; j++)
                {
                    object o = detailTable.Compute("SUM(QUANTITY)", String.Format("SORTNO={0} AND CHANNELGROUP={1}", sortNo, j + 1));
                    if (o == DBNull.Value)
                        quantity[j] = 0;
                    else
                        quantity[j] = Convert.ToInt32(o);
                }

                int tmpExportNo = 0;
                int tmpExportNo1 = 0;

                tmpExportNo = Convert.ToInt32(masterRow["EXPORTNO"]);
                tmpExportNo1 = Convert.ToInt32(masterRow["EXPORTNO1"]);

                if (sortNo  != sort1)
                {
                    masterRow["EXPORTNO"] = 0;
                }
                if (sortNo != sort2)
                {
                    masterRow["EXPORTNO1"] = 0;
                }

                AddMaster(masterRow, masterTable, sortNo++, quantity[0], quantity[1]);

                masterRow["EXPORTNO"] = tmpExportNo;
                masterRow["EXPORTNO1"] = tmpExportNo1;
            }

            ds.Tables.Add(masterTable);
            ds.Tables.Add(detailTable);
            ds.Tables.Add(supplyTable);
            return ds;
        }

        public DataSet OptimizeUseWholePiecesSortLine(DataRow masterRow, DataRow[] orderRows, DataTable channelTable, ref int sortNo)
        {
            DataSet ds = new DataSet();
            DataTable masterTable = GetEmptyOrderMaster();
            DataTable detailTable = GetEmptyOrderDetail();

            int orderNo = 1;
            int quantity = 0;
            foreach (DataRow detailRow in orderRows)
            {
                if (Convert.ToInt32(detailRow["QUANTITY"]) / 50 > 0)
                {
                    DataRow[] channelRows = channelTable.Select(string.Format("CHANNELCODE='{0}'", "1001"));
                    channelRows[0]["CIGARETTECODE"] = detailRow["CIGARETTECODE"];
                    channelRows[0]["CIGARETTENAME"] = detailRow["CIGARETTENAME"];

                    for (int i = 0; i < Convert.ToInt32(detailRow["QUANTITY"]) / 50; i++)
                    {
                        AddDetail(masterRow, channelRows[0], detailTable, sortNo,50, orderNo++, 1,1);
                        AddMaster(masterRow, masterTable, sortNo++, quantity, 0);
                    }                    

                    quantity = quantity + (Convert.ToInt32(detailRow["QUANTITY"]) / 50)*50;
                    channelRows[0]["QUANTITY"] = Convert.ToInt32(channelRows[0]["QUANTITY"]) + (Convert.ToInt32(detailRow["QUANTITY"]) / 50) * 50;
                }

            }        

            ds.Tables.Add(masterTable);
            ds.Tables.Add(detailTable);
            return ds;
        }

        private void Optimize(DataRow masterRow, DataRow channelRow, DataTable detailTable, DataTable supplyTable, int quantity, ref int sortNo,int orderNo,int exportNo,int packNo)
        {
            /**
             * 
             * 需要拆单的情况
             * 1、通道机只剩最后5条时需要拆单，每一条为一个订单。7条拆成3,1,1,1,1；6条拆成2,1,1,1,1；5条拆成1,1,1,1,1
             * 2、立式机只剩最后3条时需要拆单，第一条为一个订单。5条拆成3,1,1；4条需要拆成2,1,1；3条拆成1,1,1
             * 3、一个立式机单次出烟大于22条时需要拆单。23条拆成22,1；24条拆成22,2； 
             * 
             * */

            //烟道类型
            int channelType = Convert.ToInt32(channelRow["CHANNELTYPE"]);
            //烟道总数量
            int channelQuantity = Convert.ToInt32(channelRow["QUANTITY"]);
            //当前余留数量
            int remainQuantity = Convert.ToInt32(channelRow["REMAINQUANTITY"]);
            //件数量（待补货）
            int piece = Convert.ToInt32(channelRow["PIECE"]);

            #region 拆单数量处理            
             
            int[] orderQuantity = null;

            int breakQuantity = 15;

            if (channelType == 2 || channelType == 3)
            {
                if ((channelType == 2 || channelType == 2 )&& quantity > breakQuantity)
                {
                    //如果符合拆单条件拆单
                    int count = quantity / breakQuantity;
                    
                    if (channelQuantity - quantity <= 3)
                    {
                        //如果需要拆单并且此次出烟包括最后3条
                        int tmp = quantity % breakQuantity;
                        if (tmp == 0)
                        {
                            orderQuantity = new int[count + 2];
                            for (int i = 0; i < count; i++)
                                orderQuantity[i] = breakQuantity;
                            orderQuantity[count - 1] = breakQuantity - 2;
                            orderQuantity[count] = 1;
                            orderQuantity[count + 1] = 1;
                        }
                        else if (tmp > 0 && tmp < 3)
                        {
                            orderQuantity = new int[count + 2];
                            for (int i = 0; i < count; i++)
                                orderQuantity[i] = breakQuantity;
                            if (tmp == 1)
                                orderQuantity[count - 1] = breakQuantity - 1;
                            orderQuantity[count] = 1;
                            orderQuantity[count + 1] = 1;
                        }
                        else if (tmp >= 3)
                        {
                            orderQuantity = new int[count + 3];
                            for (int i = 0; i < count; i++)
                                orderQuantity[i] = breakQuantity;
                            orderQuantity[count] = tmp - 2;
                            orderQuantity[count + 1] = 1;
                            orderQuantity[count + 2] = 1;
                        }
                    }
                    else
                    {
                        //如果需要拆单但此次出烟不包括最后3条
                        int tmp = quantity % breakQuantity;
                        if (tmp != 0)
                        {
                            orderQuantity = new int[count + 1];
                            for (int i = 0; i < count; i++)
                                orderQuantity[i] = breakQuantity;
                            orderQuantity[orderQuantity.Length - 1] = tmp;
                        }
                        else
                        {
                            orderQuantity = new int[count];
                            for (int i = 0; i < count; i++)
                                orderQuantity[i] = breakQuantity;
                        }
                    }
                }
                else if (quantity > 1 && channelQuantity - quantity <= 3)
                {
                    //如果只剩最后三条需要拆单
                    if (quantity >= 3)
                    {
                        orderQuantity = new int[3];
                        orderQuantity[0] = quantity - 2;
                    }
                    else
                    {   
                        orderQuantity = new int[quantity];
                        orderQuantity[0] = 1;
                    }
                    for (int i = 1; i < orderQuantity.Length; i++)
                        orderQuantity[i] = 1;
                }
                else
                {
                    //不拆单
                    orderQuantity = new int[1];
                    orderQuantity[0] = quantity;
                }
            }
            else if (channelType == 5)
            {
                //不拆单
                orderQuantity = new int[1];
                orderQuantity[0] = quantity;
            }
            else
            {
                //不拆单
                orderQuantity = new int[1];
                orderQuantity[0] = quantity;
            }
            
            int tmpChannelQuantity = channelQuantity;
            List<int> orderQuantityList = new List<int>();
            
            for(int i = 0;i < orderQuantity.Length;i++)
            {
                if (tmpChannelQuantity > 7 && tmpChannelQuantity - orderQuantity[i] < 7)
                {
                    int[] tempOrderQuantity = new int[2];
                    tempOrderQuantity[0] = orderQuantity[i] - (7 - (tmpChannelQuantity - orderQuantity[i]));
                    tempOrderQuantity[1] = 7 - (tmpChannelQuantity - orderQuantity[i]);
                    orderQuantityList.Add(tempOrderQuantity[0]);
                    orderQuantityList.Add(tempOrderQuantity[1]);
                }
                else
                {
                    orderQuantityList.Add(orderQuantity[i]);
                }
                tmpChannelQuantity -= orderQuantity[i];
            }
            orderQuantity = new int[orderQuantityList.Count];
            orderQuantityList.CopyTo(orderQuantity);
            
            #endregion 

            #region 拆单结果处理            
            
            int tmpQuantity = channelQuantity;           
            for (int i = 0; i < orderQuantity.Length; i++)
            {
                if (i != 0)
                    sortNo++;
                AddDetail(masterRow, channelRow, detailTable, sortNo, orderQuantity[i],orderNo,exportNo,packNo);

                //记录分拣倒数第3条的订单号 todo;
                if (tmpQuantity >= 7 && tmpQuantity - orderQuantity[i] < 7)
                    channelRow["SORTNO"] = sortNo;
                tmpQuantity -= orderQuantity[i];

                //带补货的分拣系统需要计算补货点
                if (channelType !=5 )
                {
                    for (int j = 0; j < orderQuantity[i] / 50; j++)
                    {
                        if (piece > 0)
                        {
                            AddSupply(supplyTable, sortNo, channelRow);
                            piece--;
                        }
                    }

                    remainQuantity -= orderQuantity[i] % 50;
                    if (piece > 0 && remainQuantity <= 0)
                    {
                        remainQuantity += 50;
                        AddSupply(supplyTable, sortNo, channelRow);
                        piece--;
                    }
                }
            }

            //记录当前烟道未分拣剩余数量
            channelRow["QUANTITY"] = channelQuantity - quantity;
            channelRow["REMAINQUANTITY"] = remainQuantity;
            channelRow["PIECE"] = piece;

            #endregion
        }

        private void AddMaster(DataRow masterRow, DataTable masterTable, int sortNo, int quantity1, int quantity2)
        {
            DataRow newRow = masterTable.NewRow();
            newRow["ORDERDATE"] = masterRow["ORDERDATE"];
            newRow["BATCHNO"] = masterRow["BATCHNO"];
            newRow["LINECODE"] = masterRow["LINECODE"];
            newRow["SORTNO"] = sortNo;

            newRow["ORDERID"] = masterRow["ORDERID"];
            newRow["AREACODE"] = masterRow["AREACODE"];
            newRow["AREANAME"] = masterRow["AREANAME"];
            newRow["ROUTECODE"] = masterRow["ROUTECODE"];
            newRow["ROUTENAME"] = masterRow["ROUTENAME"];
            newRow["CUSTOMERCODE"] = masterRow["CUSTOMERCODE"];
            newRow["CUSTOMERNAME"] = masterRow["CUSTOMERNAME"];

            newRow["LICENSENO"] = masterRow["LICENSENO"];
            newRow["ADDRESS"] = masterRow["ADDRESS"];
            newRow["CUSTOMERSORTNO"] = masterRow["CUSTOMERSORTNO"];
            newRow["ORDERNO"] = masterRow["ORDERNO"];

            newRow["QUANTITY"] = quantity1;
            newRow["QUANTITY1"] = quantity2;

            newRow["ABNORMITY_QUANTITY"] = masterRow["ABNORMITY_QUANTITY"];

            newRow["EXPORTNO"] = masterRow["EXPORTNO"];
            newRow["EXPORTNO1"] = masterRow["EXPORTNO1"];
            newRow["PACKNO"] = masterRow["PACKNO"];
            newRow["PACKNO1"] = masterRow["PACKNO1"];

            masterTable.Rows.Add(newRow);
        }

        private void AddDetail(DataRow masterRow, DataRow channelRow, DataTable detailTable, int sortNo, int quantity,int orderNo,int exportNo,int packNo)
        {
            DataRow newRow = detailTable.NewRow();
            newRow["ORDERDATE"] = masterRow["ORDERDATE"];
            newRow["BATCHNO"] = masterRow["BATCHNO"];
            newRow["LINECODE"] = masterRow["LINECODE"];
            newRow["SORTNO"] = sortNo;
            newRow["ORDERID"] = masterRow["ORDERID"];
            newRow["ORDERNO"] = orderNo;
            newRow["CIGARETTECODE"] = channelRow["CIGARETTECODE"];
            newRow["CIGARETTENAME"] = channelRow["CIGARETTENAME"];
            newRow["CHANNELCODE"] = channelRow["CHANNELCODE"];
            newRow["QUANTITY"] = quantity;
            newRow["CHANNELGROUP"] = channelRow["CHANNELGROUP"];
            newRow["CHANNELORDER"] = channelRow["CHANNELORDER"];
            newRow["EXPORTNO"] = exportNo;
            newRow["PACKNO"] = packNo;

            detailTable.Rows.Add(newRow);
        }

        private void AddSupply(DataTable supplyTable, int sortNo, DataRow channelRow)
        {
            DataRow newRow = supplyTable.NewRow();
            newRow["ORDERDATE"] = channelRow["ORDERDATE"];
            newRow["BATCHNO"] = channelRow["BATCHNO"];
            newRow["LINECODE"] = channelRow["LINECODE"];
            newRow["SORTNO"] = sortNo;

            newRow["CIGARETTECODE"] = channelRow["CIGARETTECODE"];
            newRow["CIGARETTENAME"] = channelRow["CIGARETTENAME"];
            
            newRow["CHANNELCODE"] = channelRow["CHANNELCODE"];
            newRow["CHANNELGROUP"] = channelRow["CHANNELGROUP"];
            newRow["GROUPNO"] = channelRow["GROUPNO"];

            supplyTable.Rows.Add(newRow);
        }

        private DataTable GetEmptyOrderMaster()
        {
            DataTable table = new DataTable("MASTER");

            table.Columns.Add("ORDERDATE");
            table.Columns.Add("BATCHNO", typeof(Int32));
            table.Columns.Add("LINECODE");
            table.Columns.Add("SORTNO", typeof(Int32));
            
            table.Columns.Add("ORDERID");
            table.Columns.Add("AREACODE");
            table.Columns.Add("AREANAME");
            table.Columns.Add("ROUTECODE");
            table.Columns.Add("ROUTENAME");
            table.Columns.Add("CUSTOMERCODE");
            table.Columns.Add("CUSTOMERNAME");

            table.Columns.Add("LICENSENO");
            table.Columns.Add("ADDRESS");
            table.Columns.Add("CUSTOMERSORTNO", typeof(Int32)); 
            table.Columns.Add("ORDERNO", typeof(Int32)); 

            table.Columns.Add("QUANTITY");
            table.Columns.Add("QUANTITY1");

            table.Columns.Add("ABNORMITY_QUANTITY", typeof(Int32));

            table.Columns.Add("EXPORTNO", typeof(Int32));
            table.Columns.Add("EXPORTNO1", typeof(Int32));
            table.Columns.Add("PACKNO", typeof(Int32));
            table.Columns.Add("PACKNO1", typeof(Int32));
            return table;
        }

        private DataTable GetEmptyOrderDetail()
        {
            DataTable table = new DataTable("DETAIL");
            table.Columns.Add("ORDERDATE");
            table.Columns.Add("BATCHNO");
            table.Columns.Add("LINECODE");
            table.Columns.Add("SORTNO", typeof(Int32));  
          
            table.Columns.Add("ORDERID");
            table.Columns.Add("ORDERNO", typeof(Int32));
            table.Columns.Add("CIGARETTECODE");
            table.Columns.Add("CIGARETTENAME");            
            table.Columns.Add("QUANTITY", typeof(Int32));

            table.Columns.Add("CHANNELCODE");
            table.Columns.Add("CHANNELGROUP");
            table.Columns.Add("CHANNELORDER");
            table.Columns.Add("EXPORTNO", typeof(Int32));
            table.Columns.Add("PACKNO", typeof(Int32));
            return table;
        }

        private DataTable GetEmptySupply()
        {
            DataTable table = new DataTable("SUPPLY");
            table.Columns.Add("ORDERDATE");
            table.Columns.Add("BATCHNO");
            table.Columns.Add("LINECODE");
            table.Columns.Add("SORTNO");

            table.Columns.Add("CIGARETTECODE");
            table.Columns.Add("CIGARETTENAME");
            
            table.Columns.Add("CHANNELCODE");
            table.Columns.Add("CHANNELGROUP");
            table.Columns.Add("GROUPNO");

            return table;
        }        
    }
}
