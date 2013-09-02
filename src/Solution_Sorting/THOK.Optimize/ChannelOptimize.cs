using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace THOK.Optimize
{
    public class ChannelOptimize
    {     
        public void Optimize(DataTable orderTable, DataTable lineOrderTable, DataTable channelTable, DataTable deviceTable, Dictionary<string, string> param)
        {
            //允许占用2个通道机的品牌数量
            int ocupyCount = Convert.ToInt32(param["OcupyCount"]);
            int ocupyCount1 = Convert.ToInt32(param["OcupyCount1"]);
            List<string> splitProduct = new List<string>();
            DataTable tmpTable = GenerateTmpTable();

            //查找占用两个通道机的品牌
            if (ocupyCount != 0)
            {
                foreach (DataRow channelRow in channelTable.Rows)
                {
                    string productCode = channelRow["CIGARETTECODE"].ToString();
                    if (productCode != "")
                    {
                        string filter = string.Format("STATUS = '1' AND CHANNELTYPE = '3' AND CIGARETTECODE = '{0}'", productCode);
                        int count = Convert.ToInt32(channelTable.Compute("COUNT(CIGARETTECODE)", filter));

                        if (count >= 2 && !splitProduct.Contains(productCode))
                        {
                            ocupyCount--;
                            splitProduct.Add(productCode);
                        }
                    }
                }
            }

            //固定烟道分配
            foreach (DataRow deviceRow in deviceTable.Rows)
            {
                string channelType = deviceRow["CHANNELTYPE"].ToString();
                SetFixedChannel(lineOrderTable, channelTable, channelType);
            }

            //非固定烟道分配
            foreach (DataRow deviceRow in deviceTable.Rows)
            {
                string channelType = deviceRow["CHANNELTYPE"].ToString();

                switch (channelType)
                {
                    case "2": //立式机 
                        SetTowerChannel(lineOrderTable, channelTable, tmpTable, channelType, ocupyCount1);
                        break;

                    case "3": //通道机
                        SetChannel(orderTable,lineOrderTable,channelTable, tmpTable, channelType, ocupyCount);
                        break;
                }
            }
        }

        /// <summary>
        /// 固定烟道分拣
        /// </summary>
        /// <param name="orderTable"></param>
        /// <param name="channelTable"></param>
        /// <param name="channelType"></param>
        private void SetFixedChannel(DataTable orderTable, DataTable channelTable, string channelType)
        {
            //固定烟道品牌
            foreach (DataRow orderRow in orderTable.Rows)
            {
                //取当前品牌固定烟道
                DataRow[] channelRows = channelTable.Select(string.Format("CHANNELTYPE = '{0}' AND CIGARETTECODE = '{1}'  AND STATUS='1'", channelType, orderRow["CIGARETTECODE"]), "CHANNELORDER");

                if (channelRows.Length > 1)//占用多少烟道
                {
                    foreach (DataRow channelRow in channelRows)
                    {
                        channelRow["CIGARETTECODE"] = orderRow["CIGARETTECODE"];
                        channelRow["CIGARETTENAME"] = orderRow["CIGARETTENAME"];
                        //++q                       
                        channelRow["GROUPNO"] = orderRow["QUANTITY"];
                        //++q
                    }
                    orderRow["QUANTITY"] = 0;
                }
                else if (channelRows.Length == 1) //只占用一个烟道
                {
                    channelRows[0]["CIGARETTECODE"] = orderRow["CIGARETTECODE"];
                    channelRows[0]["CIGARETTENAME"] = orderRow["CIGARETTENAME"];
                    channelRows[0]["GROUPNO"] = orderRow["QUANTITY"];//++q
                    orderRow["QUANTITY"] = 0;
                }                
            }
        }

        /// <summary>
        /// 分配通道机非固定烟道
        /// </summary>
        /// <param name="orderTable"></param>
        /// <param name="channelTable"></param>
        /// <param name="tmpTable"></param>
        /// <param name="channelType"></param>
        /// <param name="sortSplitProduct"></param>
        private void SetChannel(DataTable orderTable, DataTable lineOrderTable, DataTable channelTable, DataTable tmpTable, string channelType, int sortSplitProduct)
        {

            //非固定通道机品牌
            DataRow[] orderRows = orderTable.Select("QUANTITY > 0", "QUANTITY DESC");
            foreach (DataRow orderRow in orderRows)
            {
                //取未被占用通道机烟道
                int count = Convert.ToInt32(channelTable.Compute("COUNT(CIGARETTECODE)", string.Format("STATUS = '1' AND CIGARETTECODE='{0}'", orderRow["CIGARETTECODE"])));
                DataRow[] channelRows = channelTable.Select(string.Format("CHANNELTYPE = '{0}' AND CIGARETTECODE=''  AND STATUS='1'", channelType), "CHANNELORDER");

                if (count == 0)
                {
                    if (channelRows.Length != 0)
                    {
                        if (sortSplitProduct-- > 0 && channelRows.Length >= 2)
                        {
                            channelRows[0]["CIGARETTECODE"] = orderRow["CIGARETTECODE"];
                            channelRows[0]["CIGARETTENAME"] = orderRow["CIGARETTENAME"];

                            channelRows[1]["CIGARETTECODE"] = orderRow["CIGARETTECODE"];
                            channelRows[1]["CIGARETTENAME"] = orderRow["CIGARETTENAME"];

                            //++q       
                            object objQuantity = lineOrderTable.Compute("SUM(QUANTITY)", string.Format("CIGARETTECODE='{0}'", orderRow["CIGARETTECODE"]));
                            int lineOrderQuantity = Convert.ToInt32(Convert.IsDBNull(objQuantity) ? 0 : objQuantity);
                            channelRows[0]["GROUPNO"] = lineOrderQuantity;
                            channelRows[1]["GROUPNO"] = lineOrderQuantity;
                            //++q
                        }
                        else
                        {
                            DataRow[] tmpRows = tmpTable.Select("", "QUANTITY");
                            foreach (DataRow tmpRow in tmpRows)
                            {
                                //在当前组里查找是否还有烟道，如果没有则在另外一组里进行查询
                                DataRow[] rows = channelTable.Select(string.Format("CIGARETTECODE='' AND CHANNELTYPE='{0}'  AND STATUS='1' AND CHANNELGROUP = {1}", channelType, tmpRow["GROUPNO"]), "CHANNELORDER");
                                if (rows.Length != 0)
                                {
                                    rows[0]["CIGARETTECODE"] = orderRow["CIGARETTECODE"];
                                    rows[0]["CIGARETTENAME"] = orderRow["CIGARETTENAME"];
                                    //++q
                                    object objQuantity = lineOrderTable.Compute("SUM(QUANTITY)", string.Format("CIGARETTECODE='{0}'", orderRow["CIGARETTECODE"]));
                                    int lineOrderQuantity = Convert.ToInt32(Convert.IsDBNull(objQuantity) ? 0 : objQuantity);
                                    rows[0]["GROUPNO"] = lineOrderQuantity;
                                    //++q
                                    tmpRow["QUANTITY"] = Convert.ToInt32(tmpRow["QUANTITY"]) + Convert.ToInt32(orderRow["QUANTITY"]);
                                    break;
                                }
                            }
                        }

                        orderRow["QUANTITY"] = 0;
                    }
                    else
                        break;
                }
            }
        }

        /// <summary>
        /// 分配立式机非固定烟道
        /// </summary>
        /// <param name="orderTable"></param>
        /// <param name="channelTable"></param>
        /// <param name="tmpTable"></param>
        /// <param name="channelType"></param>
        private void SetTowerChannel(DataTable lineOrderTable, DataTable channelTable, DataTable tmpTable, string channelType, int sortSplitProduct)
        {
            DataRow[] orderRows = lineOrderTable.Select("QUANTITY > 0", "QUANTITY DESC");            

            foreach (DataRow orderRow in orderRows)
            {
                int count = Convert.ToInt32(channelTable.Compute("COUNT(CIGARETTECODE)", string.Format("STATUS = '1' AND CIGARETTECODE='{0}'", orderRow["CIGARETTECODE"])));
                if (count == 0)
                {
                    //如果有未被占用的立式机
                    if ((int)channelTable.Compute("COUNT(CHANNELCODE)", string.Format("CIGARETTECODE='' AND CHANNELTYPE='{0}' AND STATUS='1'", channelType)) != 0)
                    {
                        if (sortSplitProduct-- > 0)
                        {
                            DataRow[] channelRows = channelTable.Select(string.Format("CIGARETTECODE='' AND CHANNELTYPE='{0}'  AND STATUS='1' AND CHANNELGROUP = {1}", channelType, 1), "CHANNELORDER");
                            if (channelRows.Length != 0)
                            {
                                channelRows[0]["CIGARETTECODE"] = orderRow["CIGARETTECODE"];
                                channelRows[0]["CIGARETTENAME"] = orderRow["CIGARETTENAME"];
                                channelRows[0]["GROUPNO"] = orderRow["QUANTITY"];//++q                                
                            }

                            channelRows = channelTable.Select(string.Format("CIGARETTECODE='' AND CHANNELTYPE='{0}'  AND STATUS='1' AND CHANNELGROUP = {1}", channelType, 2), "CHANNELORDER");
                            if (channelRows.Length != 0)
                            {
                                channelRows[0]["CIGARETTECODE"] = orderRow["CIGARETTECODE"];
                                channelRows[0]["CIGARETTENAME"] = orderRow["CIGARETTENAME"];
                                channelRows[0]["GROUPNO"] = orderRow["QUANTITY"];//++q                                
                            }

                            orderRow["QUANTITY"] = 0;
                        }
                        else
                        {
                            DataRow[] tmpRows = tmpTable.Select("", "QUANTITY");
                            foreach (DataRow tmpRow in tmpRows)
                            {
                                //在当前组里查找是否还有烟道，如果没有则在另外一组里进行查询
                                DataRow[] channelRows = channelTable.Select(string.Format("CIGARETTECODE='' AND CHANNELTYPE='{0}'  AND STATUS='1' AND CHANNELGROUP = {1}", channelType, tmpRow["GROUPNO"]), "CHANNELORDER");
                                if (channelRows.Length != 0)
                                {
                                    channelRows[0]["CIGARETTECODE"] = orderRow["CIGARETTECODE"];
                                    channelRows[0]["CIGARETTENAME"] = orderRow["CIGARETTENAME"];
                                    channelRows[0]["GROUPNO"] = orderRow["QUANTITY"];//++q

                                    tmpRow["QUANTITY"] = Convert.ToInt32(tmpRow["QUANTITY"]) + Convert.ToInt32(orderRow["QUANTITY"]);
                                    orderRow["QUANTITY"] = 0;
                                    break;
                                }
                            }
                        }
                    }
                    else if (0 == Convert.ToInt32(channelTable.Compute("COUNT(CHANNELCODE)", "STATUS = '1' AND CHANNELTYPE='5'")))
                    {
                        throw new Exception("还有卷烟品牌未分配烟道,请调整烟道设置。");
                    }
                }
            }
        }

        private DataTable GenerateTmpTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("GROUPNO");
            table.Columns.Add("QUANTITY", typeof(Int32));
            for (int i = 1; i <= 2; i++)
            {
                table.Rows.Add(i, 0);
            }
            return table;
        }
    }
}
