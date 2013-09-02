using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace THOK.Optimize
{
    /// <summary>
    /// 双主线垂直式半自动订单优化
    /// </summary>
    public class OrderSplitOptimize
    {
        private int splitOrderQuantity = 25;

        private int[] exportQuantity = new int[2];
        private int[] channelGroupQuantity = new int[2];

        private int noMoveTixChannelQuantity = 0;
        private bool isOneChannelMoveToMixChannel = false;
        private bool isTwoChannelMoveToMixChannel = false;
        private int moveToMixChannelProductsCount = 0;
        //public IList<string> moveToMixChannelProducts = new List<string>();
        public IDictionary<string,int> moveToMixChannelProducts = new Dictionary<string,int>();
        
        
        private bool isCombineOrder = false;

        public class PackerInfo
        {
            public int packMode = 0; //(0：正常PE包装；1：周转箱连续客户合装包装；2：动态包装)
            public string lineCode = "";//当前折单优化分拣线
            public string routeCode = "";//当前折单优化线路
            public string orderId = "";//当前折单优化订单号
            public int packNo = 0;//当前折单优化包装包号
            public int exportNo = 1;//当前折单优化包装机方向（1：1号包装机；2：2号包装机；）
            public int quantity = 0;//当前折单优化包装数量
            public int splitPackQuantity = 0; //包装折包的每包数量
            public int lastOrderNo = 0;//A线尾数单的OrderNo
            public int lastOrderNo1 = 0;//B线尾数单的OrderNo
        }
        private PackerInfo packerInfo = new PackerInfo();

        //优化开始；
        public DataSet Optimize(DataRow masterRow, DataRow[] orderRows, DataTable channelTable, string lineCode, ref int sortNo, Dictionary<string, string> param, bool isUseWholePiecesSortLine)
        {
            double splitParam = Convert.ToDouble(param["SplitParam"]);
            splitOrderQuantity = Convert.ToInt32(param["SplitOrderQuantity"]);

            noMoveTixChannelQuantity = Convert.ToInt32(param["NoMoveTixChannelQuantity"]);
            isOneChannelMoveToMixChannel = Convert.ToBoolean(param["IsOneChannelMoveToMixChannel-" + lineCode]);
            isTwoChannelMoveToMixChannel = Convert.ToBoolean(param["IsTwoChannelMoveToMixChannel-" + lineCode]);
            moveToMixChannelProductsCount = Convert.ToInt32(param["MoveToMixChannelProductsCount-" + lineCode]);

            isCombineOrder = Convert.ToBoolean(param["IsCombineOrder"]);
            packerInfo.packMode = Convert.ToInt32(param["PackMode"]);
            packerInfo.splitPackQuantity = Convert.ToInt32(param["SplitPackQuantity"]);

            Dictionary<string, int> product = new Dictionary<string, int>();
            DataTable tmpDetail = GetEmptyOrderDetail();
            int[] groupQuantity = new int[2];

            int orderQuantity = 0;
            
            foreach (DataRow orderRow in orderRows)
            {
                string cigaretteCode = orderRow["CIGARETTECODE"].ToString();
                string cigaretteName = orderRow["CIGARETTENAME"].ToString();
                int quantity = isUseWholePiecesSortLine ? Convert.ToInt32(orderRow["QUANTITY"]) % 50 : Convert.ToInt32(orderRow["QUANTITY"]);

                DataRow[] channelRows = channelTable.Select(string.Format("STATUS = '1' AND CIGARETTECODE = '{0}'", cigaretteCode));
                int channelGroup = 0;
                switch (channelRows.Length)
                {
                    case 0:
                        channelRows = channelTable.Select(string.Format("STATUS = '1' AND CHANNELTYPE = '{0}'", "5"));
                        if (channelRows.Length != 0)
                        {
                            channelRows[0]["CIGARETTECODE"] = cigaretteCode;
                            channelRows[0]["CIGARETTENAME"] = cigaretteName;
                            AddDetailRow(masterRow, tmpDetail, -1, lineCode, channelRows[0], quantity);                          
                            channelRows[0]["CIGARETTECODE"] = "";
                            channelRows[0]["CIGARETTENAME"] = channelRows[0]["CHANNELNAME"];

                            channelGroup = Convert.ToInt32(channelRows[0]["CHANNELGROUP"]);
                            groupQuantity[channelGroup - 1] += quantity; 
                            orderQuantity += quantity;
                        }
                        else
                        {
                            throw new Exception(string.Format("没有为品牌'{0}'找到分拣烟道，订单号：{1}", cigaretteCode, masterRow["ORDERID"]));
                        }

                        break;
                    case 1://当前品牌只占一个烟道 //++q                       
                        if (!OneChannelMoveToMixChannel(groupQuantity, cigaretteCode, quantity, lineCode, masterRow, channelTable, tmpDetail))
                        {                           
                            AddDetailRow(masterRow, tmpDetail, -1, lineCode, channelRows[0], quantity);
                            channelGroup = Convert.ToInt32(channelRows[0]["CHANNELGROUP"]);
                            groupQuantity[channelGroup - 1] += quantity;                            
                        }
                        orderQuantity += quantity;
                        break;
                    case 2://当前品牌占两个烟道,在下面的代码中才进行处理
                        if (!TwoChannelMoveToMixChannel(groupQuantity, cigaretteCode, quantity, lineCode, masterRow, channelTable, tmpDetail))
                        {
                            product.Add(cigaretteCode, quantity);
                        }
                        orderQuantity += quantity;
                        break;
                    default:
                        //todo
                        if (!TwoChannelMoveToMixChannel(groupQuantity, cigaretteCode, quantity, lineCode, masterRow, channelTable, tmpDetail))
                        {
                            product.Add(cigaretteCode, quantity);
                        }
                        orderQuantity += quantity;
                        break;
                }
            }

            #region 合并订单
            
            //++
            if ((isCombineOrder || orderQuantity <= 3) && orderQuantity <= splitOrderQuantity && (groupQuantity[0] == 0 || groupQuantity[1] == 0))
            {
                int channelGroup = 1;

                if (groupQuantity[0] != 0)
                {
                    channelGroup = 1;
                }
                else if (groupQuantity[1] != 0)
                {
                    channelGroup = 2;
                }
                else
                {
                    if (channelGroupQuantity[0] < channelGroupQuantity[1])
                    {
                        channelGroup = 1;
                    }
                    else
                    {
                        channelGroup = 2;
                    }
                }

                Dictionary<string, int> productDel = new Dictionary<string, int>();

                foreach (string cigaretteCode in product.Keys)
                {
                    DataRow[] channelRows = channelTable.Select(string.Format("STATUS = '1' AND CIGARETTECODE = '{0}' AND CHANNELGROUP = {1}", cigaretteCode, channelGroup));
                    if (channelRows.Length > 0)
                    {
                        channelGroup = Convert.ToInt32(channelRows[0]["CHANNELGROUP"]);
                        AddDetailRow(masterRow, tmpDetail, -1, lineCode, channelRows[0], product[cigaretteCode]);
                        groupQuantity[channelGroup - 1] += product[cigaretteCode];
                        productDel.Add(cigaretteCode, product[cigaretteCode]);
                    }
                }

                foreach (string cigaretteCode in productDel.Keys)
                {
                    product.Remove(cigaretteCode);
                }
            }
            //++

            #endregion

            //调整订单，使当前订单的两条主线，分拣量尽量平均。
            AdjustOrder(groupQuantity, product, lineCode, masterRow, channelTable, tmpDetail);

            channelGroupQuantity[0] += groupQuantity[0];
            channelGroupQuantity[1] += groupQuantity[1];

            if ( Convert.ToInt32(Convert.IsDBNull(tmpDetail.Compute("SUM(QUANTITY)","CHANNELGROUP = 1"))?0:tmpDetail.Compute("SUM(QUANTITY)","CHANNELGROUP = 1")) != groupQuantity[0])
            {
                throw new Exception("优化过程出错，请检查！");
            }
            if (Convert.ToInt32(Convert.IsDBNull(tmpDetail.Compute("SUM(QUANTITY)", "CHANNELGROUP = 2")) ? 0 : tmpDetail.Compute("SUM(QUANTITY)", "CHANNELGROUP = 2")) != groupQuantity[1])
            {
                throw new Exception("优化过程出错，请检查！");
            }

            #region 平衡出口压力    
            int exportNoLast = 2;
            if (exportQuantity[0] < exportQuantity[1] * splitParam)
            {
                exportNoLast = 1;
            }
            else
            {
                exportNoLast = 2;
            }

            if ((groupQuantity[0] + groupQuantity[1]) % 25 < 5)
            {
                if (groupQuantity[0] >= 25)
                {
                    exportNoLast = 1;
                }
                else if (groupQuantity[1] >= 25)
                {
                    exportNoLast = 2;
                }
                else
                {
                    exportNoLast = 1;
                }
            }

            if (exportNoLast == 1)
            {
                exportQuantity[0] += (groupQuantity[0] / splitOrderQuantity) * splitOrderQuantity + groupQuantity[0] % splitOrderQuantity + groupQuantity[1] % splitOrderQuantity;
                exportQuantity[1] += (groupQuantity[1] / splitOrderQuantity) * splitOrderQuantity;
            }
            else
            {
                exportQuantity[0] += (groupQuantity[0] / splitOrderQuantity) * splitOrderQuantity;
                exportQuantity[1] += (groupQuantity[1] / splitOrderQuantity) * splitOrderQuantity + groupQuantity[0] % splitOrderQuantity + groupQuantity[1] % splitOrderQuantity;
            }
            #endregion

            DataSet result = SplitOrder(masterRow, tmpDetail, groupQuantity, ref sortNo, lineCode,exportNoLast);

            return result;
        }

        //单一烟道将尾数移到混合烟道；
        private bool OneChannelMoveToMixChannel(int[] groupQuantity, string cigaretteCode, int quantity, string lineCode, DataRow masterRow, DataTable channelTable, DataTable detailTable)
        {
            DataRow[] channelRows = channelTable.Select(string.Format("STATUS = '1' AND CIGARETTECODE = '{0}'", cigaretteCode));

            //判断是否为固定烟道，如果是则不可移；
            if (channelRows[0]["CIGARETTECODE"].ToString() == channelRows[0]["D_CIGARETTECODE"].ToString() )
            {
                return false;
            }

            //判断是否为通道机，且已进入尾数折单优化，如果是则进入移仓算法；
            if (channelRows[0]["CHANNELTYPE"].ToString() == "3" && Convert.ToInt32(channelRows[0]["QUANTITY"]) + quantity > Convert.ToInt32(channelRows[0]["GROUPNO"]) / 50 * 50)
            {
                int quantity1 = Convert.ToInt32(channelRows[0]["GROUPNO"]) / 50 * 50 - Convert.ToInt32(channelRows[0]["QUANTITY"]);
                if (quantity1 < 0)
                {
                    return false;
                }

                int quantity2 = quantity - quantity1;
                
                int channelGroup = 0;

                DataRow[] channelRows1 = channelTable.Select(string.Format("STATUS = '1' AND CIGARETTECODE = '' AND CHANNELTYPE = '{0}'", "2"));
                DataRow[] channelRows2 = channelTable.Select(string.Format("STATUS = '1' AND CHANNELTYPE = '{0}'", "5"));

                if (channelRows1.Length > 0 || (channelRows2.Length > 0 && isOneChannelMoveToMixChannel && ( moveToMixChannelProducts.ContainsKey(cigaretteCode) || moveToMixChannelProducts.Count < moveToMixChannelProductsCount)))
                {
                    if (quantity1 > 0)
                    {                        
                        AddDetailRow(masterRow, detailTable, -1, lineCode, channelRows[0], quantity1);
                        channelGroup = Convert.ToInt32(channelRows[0]["CHANNELGROUP"]);
                        groupQuantity[channelGroup - 1] += quantity1;                        
                    }

                    if (quantity2 > 0)
                    {
                        if (channelRows1.Length > 0)
                        {
                            //移仓到立式空仓烟道
                            channelRows1[0]["CIGARETTECODE"] = cigaretteCode;
                            channelRows1[0]["CIGARETTENAME"] = channelRows[0]["CIGARETTENAME"];
                            channelRows1[0]["GROUPNO"] = channelRows[0]["GROUPNO"];
                            AddDetailRow(masterRow, detailTable, -1, lineCode, channelRows1[0], quantity2);

                            channelGroup = Convert.ToInt32(channelRows1[0]["CHANNELGROUP"]);
                            groupQuantity[channelGroup - 1] += quantity2;
                        }
                        else
                        {
                            int channelQuantityTotal = Convert.ToInt32(channelTable.Compute("SUM(QUANTITY)", string.Format("STATUS = '1' AND CIGARETTECODE='{0}'", cigaretteCode)));
                            channelQuantityTotal += moveToMixChannelProducts.ContainsKey(cigaretteCode) ? moveToMixChannelProducts[cigaretteCode] : 0;
                            bool isLastNoMove = Convert.ToInt32(channelRows[0]["GROUPNO"]) - channelQuantityTotal - quantity2 <= noMoveTixChannelQuantity;
                            if (isLastNoMove)
                            {
                                int q1 = Convert.ToInt32(channelRows[0]["GROUPNO"]) - channelQuantityTotal - noMoveTixChannelQuantity;
                                q1 = (q1 > 0 ? q1 : 0);//可移仓到混合烟道的数量
                                int q2 = quantity2 - q1;//不移仓数量
                                if (q2 > 0)
                                {
                                    AddDetailRow(masterRow, detailTable, -1, lineCode, channelRows[0], q2);
                                    channelGroup = Convert.ToInt32(channelRows[0]["CHANNELGROUP"]);
                                    groupQuantity[channelGroup - 1] += q2;
                                    quantity2 -= q2;
                                }
                            }

                            //移仓到混合烟道
                            if (quantity2 > 0)
                            {
                                if (!moveToMixChannelProducts.ContainsKey(cigaretteCode))
                                {
                                    moveToMixChannelProducts.Add(cigaretteCode, 0);
                                }

                                channelRows2[0]["CIGARETTECODE"] = cigaretteCode;
                                channelRows2[0]["CIGARETTENAME"] = channelRows[0]["CIGARETTENAME"];
                                AddDetailRow(masterRow, detailTable, -1, lineCode, channelRows2[0], quantity2);
                                channelRows2[0]["CIGARETTECODE"] = "";
                                channelRows2[0]["CIGARETTENAME"] = channelRows2[0]["CHANNELNAME"];

                                channelGroup = Convert.ToInt32(channelRows2[0]["CHANNELGROUP"]);
                                groupQuantity[channelGroup - 1] += quantity2;

                                moveToMixChannelProducts[cigaretteCode] += quantity2;
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            else
                return false;
        }
        //多烟道将尾数移到混合烟道；
        private bool TwoChannelMoveToMixChannel(int[] groupQuantity, string cigaretteCode, int quantity, string lineCode, DataRow masterRow, DataTable channelTable, DataTable detailTable)
        {
            //++q
            DataRow[] channelRows = channelTable.Select(string.Format("STATUS = '1' AND CIGARETTECODE = '{0}'", cigaretteCode), "CHANNELTYPE,QUANTITY");
            //判断是否为固定烟道，如果是则不可移；
            if (channelRows[0]["CIGARETTECODE"].ToString() == channelRows[0]["D_CIGARETTECODE"].ToString())
            {
                return false;
            }

            int channelQuantityTotal = Convert.ToInt32(channelTable.Compute("SUM(QUANTITY)", string.Format("STATUS = '1' AND CIGARETTECODE='{0}'", cigaretteCode)));
            int OrderQuantityTotal = quantity;
            if (channelQuantityTotal + OrderQuantityTotal > Convert.ToInt32(channelRows[0]["GROUPNO"]) / 50 * 50 - 50 * channelRows.Length)
            {
                //循环将非整烟道补整；
                foreach (DataRow row in channelRows)
                {
                    if (Convert.ToInt32(row["QUANTITY"]) % 50 != 0 && OrderQuantityTotal > 0)
                    {
                        int temp1 = (50 - Convert.ToInt32(row["QUANTITY"]) % 50);
                        int temp2 = OrderQuantityTotal >= temp1 ? temp1 : OrderQuantityTotal;
                        AddDetailRow(masterRow, detailTable, -1, lineCode, row, temp2);
                        OrderQuantityTotal -= temp2;

                        int channelGroup2 = Convert.ToInt32(row["CHANNELGROUP"]);
                        groupQuantity[channelGroup2 - 1] += temp2;
                    }
                }

                channelQuantityTotal = Convert.ToInt32(channelTable.Compute("SUM(QUANTITY)", string.Format("STATUS = '1' AND CIGARETTECODE='{0}'", cigaretteCode)));
                //循环将非整烟道补整；
                while (OrderQuantityTotal > 0 && Convert.ToInt32(channelRows[0]["GROUPNO"]) - channelQuantityTotal >= 50)
                {
                    int temp1 = OrderQuantityTotal > 50 ? 50 : OrderQuantityTotal;
                    AddDetailRow(masterRow, detailTable, -1, lineCode, channelRows[0], temp1);
                    OrderQuantityTotal -= temp1;

                    int channelGroup2 = Convert.ToInt32(channelRows[0]["CHANNELGROUP"]);
                    groupQuantity[channelGroup2 - 1] += temp1;

                    channelQuantityTotal = Convert.ToInt32(channelTable.Compute("SUM(QUANTITY)", string.Format("STATUS = '1' AND CIGARETTECODE='{0}'", cigaretteCode)));
                }

                //移仓到立式空仓烟道
                DataRow[] channelRows1 = channelTable.Select(string.Format("STATUS = '1' AND CIGARETTECODE = '' AND CHANNELTYPE = '{0}'", "2"));
                if (OrderQuantityTotal > 0 && channelRows1.Length > 0 && channelRows[0]["CHANNELTYPE"].ToString() == "3")
                {
                    channelRows1[0]["CIGARETTECODE"] = cigaretteCode;
                    channelRows1[0]["CIGARETTENAME"] = channelRows[0]["CIGARETTENAME"];
                    channelRows1[0]["GROUPNO"] = channelRows[0]["GROUPNO"];
                    AddDetailRow(masterRow, detailTable, -1, lineCode, channelRows1[0], OrderQuantityTotal);

                    int channelGroup2 = Convert.ToInt32(channelRows1[0]["CHANNELGROUP"]);
                    groupQuantity[channelGroup2 - 1] += OrderQuantityTotal;
                    OrderQuantityTotal = 0;
                }

                //移仓到混合烟道(部份不移仓)
                channelQuantityTotal += moveToMixChannelProducts.ContainsKey(cigaretteCode) ? moveToMixChannelProducts[cigaretteCode] : 0;
                DataRow[] channelRows2 = channelTable.Select(string.Format("STATUS = '1' AND CHANNELTYPE = '{0}'", "5"));
                bool isLastNoMove = Convert.ToInt32(channelRows[0]["GROUPNO"]) - channelQuantityTotal - OrderQuantityTotal <= noMoveTixChannelQuantity;
                if (isLastNoMove)
                {
                    int q1 = Convert.ToInt32(channelRows[0]["GROUPNO"]) - channelQuantityTotal - noMoveTixChannelQuantity;
                    q1 = (q1 > 0 ? q1 : 0);//可移仓到混合烟道的数量
                    int q2 = OrderQuantityTotal - q1;//不移仓数量
                    if (q2 > 0)
                    {
                        AddDetailRow(masterRow, detailTable, -1, lineCode, channelRows[0], q2);

                        int channelGroup2 = Convert.ToInt32(channelRows[0]["CHANNELGROUP"]);
                        groupQuantity[channelGroup2 - 1] += q2;
                        OrderQuantityTotal -= q2;
                    }
                }
                //移仓到混合烟道
                if (OrderQuantityTotal > 0 && isTwoChannelMoveToMixChannel && channelRows[0]["CHANNELTYPE"].ToString() == "3" && channelRows2.Length > 0 && (moveToMixChannelProducts.ContainsKey(cigaretteCode) || moveToMixChannelProducts.Count < moveToMixChannelProductsCount))
                {
                    if (!moveToMixChannelProducts.ContainsKey(cigaretteCode))
                    {
                        moveToMixChannelProducts.Add(cigaretteCode,0);
                    }

                    channelRows2[0]["CIGARETTECODE"] = cigaretteCode;
                    channelRows2[0]["CIGARETTENAME"] = channelRows[0]["CIGARETTENAME"];
                    AddDetailRow(masterRow, detailTable, -1, lineCode, channelRows2[0], OrderQuantityTotal);
                    channelRows2[0]["CIGARETTECODE"] = "";
                    channelRows2[0]["CIGARETTENAME"] = channelRows2[0]["CHANNELNAME"];

                    int channelGroup2 = Convert.ToInt32(channelRows2[0]["CHANNELGROUP"]);
                    groupQuantity[channelGroup2 - 1] += OrderQuantityTotal;

                    moveToMixChannelProducts[cigaretteCode] += OrderQuantityTotal;
                }
                else if (OrderQuantityTotal > 0)
                {
                    //不移仓
                    AddDetailRow(masterRow, detailTable, -1, lineCode, channelRows[0], OrderQuantityTotal);

                    int channelGroup2 = Convert.ToInt32(channelRows[0]["CHANNELGROUP"]);
                    groupQuantity[channelGroup2 - 1] += OrderQuantityTotal;
                    OrderQuantityTotal = 0;
                }

                return true;
            }
            else
                return false;
            //++q
        }
        
                
        //调整订单，使当前订单的两条主线，分拣量尽量平均。
        private void AdjustOrder(int[] groupQuantity, Dictionary<string, int> product, string lineCode, DataRow masterRow, DataTable channelTable, DataTable detailTable)
        {
            //如果一个品牌占两个烟道，则用这些品牌来调整两个组的分拣量，目的是使两个组的分拣量均衡
            foreach (string cigaretteCode in product.Keys)
            {
                int gruopCount1 = Convert.ToInt32(channelTable.Compute("COUNT(CHANNELCODE)", string.Format("CHANNELGROUP=1 AND CIGARETTECODE='{0}'", cigaretteCode)));
                int groupCount2 = Convert.ToInt32(channelTable.Compute("COUNT(CHANNELCODE)", string.Format("CHANNELGROUP=2 AND CIGARETTECODE='{0}'", cigaretteCode)));

                //占用多个烟道的品牌在同一条分拣主线上
                if (gruopCount1 == 0 || groupCount2 == 0)
                {
                    DataRow[] channelRows = channelTable.Select(string.Format("STATUS = '1' AND CIGARETTECODE='{0}'", cigaretteCode), "QUANTITY");

                    //当前主线此品牌占用多个烟道
                    AdjustChannel(masterRow, detailTable, channelRows, product[cigaretteCode], cigaretteCode);

                    int channelGroup = gruopCount1 != 0 ? 0 : 1;
                    groupQuantity[channelGroup] += product[cigaretteCode];
                }
                else//占用多个烟道品牌在两条分拣主线上
                {
                    //计算少多少
                    int quantity = groupQuantity[0] > groupQuantity[1] ? groupQuantity[0] - groupQuantity[1] : groupQuantity[1] - groupQuantity[0];
                    //计算哪组少
                    int channelGroup = groupQuantity[0] > groupQuantity[1] ? 1 : 0;

                    int[] productQuantity = new int[2];

                    if (product[cigaretteCode] > quantity)
                    {
                        int tmpQuantity = product[cigaretteCode] - quantity;//计算当前品牌大于两组差额的条数

                        productQuantity[channelGroup] += quantity;

                        //将当前品牌大于两组差额的条数在两个组中平均分配
                        productQuantity[0] += tmpQuantity / 2;
                        productQuantity[1] += tmpQuantity - (tmpQuantity / 2);
                    }
                    else
                    {
                        productQuantity[channelGroup] += product[cigaretteCode];
                    }

                    //++
                    if (isCombineOrder)
                    {
                        int tmp1 = (groupQuantity[0] + productQuantity[0]) % 25;
                        int tmp2 = (groupQuantity[1] + productQuantity[1]) % 25;

                        if (productQuantity[0] >= tmp1 && productQuantity[1] >= tmp2)
                        {
                            if (channelGroupQuantity[0] < channelGroupQuantity[1])
                            {
                                productQuantity[1] -= tmp2;
                                productQuantity[0] += tmp2;
                            }
                            else
                            {
                                productQuantity[0] -= tmp1;
                                productQuantity[1] += tmp1;
                            }
                        }
                        else if (productQuantity[0] >= tmp1)
                        {
                            productQuantity[0] -= tmp1;
                            productQuantity[1] += tmp1;
                        }
                        else if (productQuantity[1] >= tmp2)
                        {
                            productQuantity[1] -= tmp2;
                            productQuantity[0] += tmp2;
                        }
                    }
                    //++                       

                    //两条分拣主线
                    for (int i = 0; i < 2; i++)
                    {
                        if (productQuantity[i] > 0)
                        {
                            AdjustChannel(masterRow, detailTable, channelTable, cigaretteCode, i + 1, productQuantity[i]);
                            groupQuantity[i] += productQuantity[i];
                        }
                    }
                }
            }
        }
        

        //品牌在一条分拣主线上是占用一个烟道还是多个烟道
        private void AdjustChannel(DataRow masterRow, DataTable detailTable, DataTable channelTable, string cigaretteCode, int channelGroup, int quantity)
        {
            DataRow[] channelRows = channelTable.Select(string.Format("STATUS = '1' AND CIGARETTECODE='{0}' AND CHANNELGROUP={1}", cigaretteCode, channelGroup), "QUANTITY");
            //如果当前主线此品牌只占用一个烟道
            if (channelRows.Length == 1)
            {
                AddDetailRow(masterRow, detailTable, -1, "", channelRows[0], quantity);
            }
            else
            {
                //如果当前主线此品牌占用多个烟道
                AdjustChannel(masterRow, detailTable, channelRows, quantity, cigaretteCode);
            }
        }
        //在同一主线多个烟道中平均分拣此品牌
        private void AdjustChannel(DataRow masterRow, DataTable detailTable, DataRow[] channelRows, int quantity, string cigaretteCode)
        {
            int[] quantitys = new int[channelRows.Length];

            int cigaretteQuantity = quantity;
            while (cigaretteQuantity > 0)
            {
                for (int i = 0; i < channelRows.Length; i++)
                {
                    quantitys[i] += 1;
                    if (--cigaretteQuantity == 0)
                        break;
                }
            }

            for (int i = 0; i < channelRows.Length; i++)
            {
                //添加到Detail表
                AddDetailRow(masterRow, detailTable, -1, "", channelRows[i], quantitys[i]);
            }
        }


        //将订单拆分成25条的分拣订单，拆分的原因是分拣出口缓存为25条
        private DataSet SplitOrder(DataRow masterRow, DataTable tmpTable, int[] groupQuantity, ref int sortNo, string lineCode,int exportNoLast)
        {
            DataTable masterTable = GetEmptyOrderMaster();
            DataTable detailTable = GetEmptyOrderDetail();
            int orderNo = 1;
            string sort = "DESC";
            while (groupQuantity[0] > 0 || groupQuantity[1] > 0)
            {
                int[] quantity = new int[2];

                if (sort == "")
                    sort = "DESC";
                else
                    sort = "";

                quantity[1] = SplitOrder(masterRow, detailTable, tmpTable, sortNo, lineCode, groupQuantity, 2, orderNo, groupQuantity[1] > splitOrderQuantity ? 2 : exportNoLast, sort);
                quantity[0] = SplitOrder(masterRow, detailTable, tmpTable, sortNo, lineCode, groupQuantity, 1, orderNo, groupQuantity[0] > splitOrderQuantity ? 1 : exportNoLast, sort);

                AddMasterRow(masterTable, masterRow, sortNo++, orderNo++, quantity,exportNoLast);
            }

            DataRow[] orderMasterRows = masterTable.Select(string.Format("QUANTITY <= {0} AND PACKORDERNO = {1}", splitOrderQuantity, packerInfo.lastOrderNo));
            foreach (DataRow orderMasterRow in orderMasterRows)
            {
                orderMasterRow["PACKNO"] = packerInfo.packNo;
            }

            orderMasterRows = masterTable.Select(string.Format("QUANTITY1 <= {0} AND PACKORDERNO = {1}", splitOrderQuantity, packerInfo.lastOrderNo1));
            foreach (DataRow orderMasterRow in orderMasterRows)
            {
                orderMasterRow["PACKNO1"] = packerInfo.packNo;
            }

            DataRow[] orderRows = detailTable.Select(string.Format("(CHANNELGROUP = 1 AND ORDERNO = {0}) OR (CHANNELGROUP = 2 AND ORDERNO = {1})", packerInfo.lastOrderNo, packerInfo.lastOrderNo1));
            foreach (DataRow orderRow in orderRows)
            {
                orderRow["PACKNO"] = packerInfo.packNo;
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(masterTable);
            ds.Tables.Add(detailTable);
            return ds;
        }
        
        //对一条分拣主线的订单进行拆分
        private int SplitOrder(DataRow masterRow, DataTable detailTable, DataTable tmpTable, int sortNo, string lineCode, int[] groupQuantity, int channelGroup, int orderNo, int exportNo,string sort)
        {
            int packNo = 0;
            int quantity = 0;
            if (packerInfo.packMode == 1)
            {
                if (packerInfo.splitPackQuantity % splitOrderQuantity != 0)
                {
                    throw new Exception("packerInfo.splitPackQuantity 除以 splitOrderQuantity 不是整理数倍！ ");
                }
                if (packerInfo.routeCode != masterRow["ROUTECODE"].ToString())
                {
                    packerInfo.exportNo = 1;
                    packerInfo.quantity = 0;
                    packerInfo.routeCode = masterRow["ROUTECODE"].ToString();
                }
                else if (packerInfo.quantity == packerInfo.splitPackQuantity)
                {
                    packerInfo.exportNo = packerInfo.exportNo == 2 ? 1 : 2;
                    packerInfo.quantity = 0;
                }
                exportNo = packerInfo.exportNo;
                if (packerInfo.splitPackQuantity - packerInfo.quantity >= splitOrderQuantity)
                {
                    quantity = 0;
                }
                else
                {
                    quantity = packerInfo.quantity % splitOrderQuantity;  
                }                
            }
            else if (packerInfo.packMode == 2)
            {
                if (packerInfo.lineCode != masterRow["LINECODE"].ToString())
                {
                    packerInfo.packNo = 0;
                    packerInfo.lineCode = masterRow["LINECODE"].ToString();
                }
                if (packerInfo.orderId != masterRow["ORDERID"].ToString())
                {
                    packerInfo.packNo++;
                    packerInfo.lastOrderNo = 0;
                    packerInfo.lastOrderNo1 = 0;
                    packerInfo.orderId = masterRow["ORDERID"].ToString();
                }

                if (groupQuantity[channelGroup - 1] > splitOrderQuantity)
                {
                    packNo = packerInfo.packNo++;
                }
                else
                {
                    packNo = packerInfo.packNo;
                }
            }
            if (groupQuantity[channelGroup - 1] > 0)
            {          
                if (channelGroup == 1)
                {
                    masterRow["EXPORTNO"] = exportNo;
                    masterRow["PACKNO"] = packNo;
                    if (groupQuantity[channelGroup - 1] <= splitOrderQuantity)
                    {
                        packerInfo.lastOrderNo = orderNo;
                    }
                }
                else
                {
                    masterRow["EXPORTNO1"] = exportNo;
                    masterRow["PACKNO1"] = packNo;
                    if (groupQuantity[channelGroup - 1] <= splitOrderQuantity)
                    {
                        packerInfo.lastOrderNo1 = orderNo;
                    }
                }

                masterRow["PACKORDERNO"] = orderNo;

                DataRow[] orderRows = tmpTable.Select(string.Format("CHANNELGROUP={0} AND QUANTITY > 0", channelGroup),string.Format("CHANNELTYPE {0},QUANTITY DESC,CHANNELORDER DESC",sort));
                foreach (DataRow orderRow in orderRows)
                {
                    int orderQuantity = Convert.ToInt32(orderRow["QUANTITY"]);
                    int tmpQuantity = splitOrderQuantity - quantity;

                    //计算当前拆分订单当前品牌数量
                    int newQuantity = tmpQuantity >= orderQuantity ? orderQuantity : tmpQuantity;

                    //立式机一次最多只能出15条烟 TODO
                    if (orderRow["CHANNELTYPE"].ToString() == "2" && newQuantity > 15 && groupQuantity[channelGroup - 1] - orderQuantity >= splitOrderQuantity - quantity - 15)
                        newQuantity = 15;

                    quantity += newQuantity;
                    packerInfo.quantity += newQuantity;
                    groupQuantity[channelGroup -1 ] -= newQuantity;
                    orderQuantity -= newQuantity;
                    tmpQuantity -= newQuantity;

                    orderRow["QUANTITY"] = orderQuantity;
                    orderRow["ORDERNO"] = orderNo;
                    orderRow["EXPORTNO"] = exportNo;
                    orderRow["PACKNO"] = packNo;//数据库需要增加相应字段；                  

                    AddDetailRow(masterRow, detailTable, sortNo, lineCode, orderRow, newQuantity);
                    if (tmpQuantity == 0)
                        break;
                }
            }
            return quantity;
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

            //table.Columns.Add("TQUANTITY", typeof(Int32));
            table.Columns.Add("QUANTITY", typeof(Int32));
            table.Columns.Add("QUANTITY1", typeof(Int32));

            //table.Columns.Add("PQUANTITY", typeof(Int32));
            //table.Columns.Add("PACKQUANTITY", typeof(Int32));
            //table.Columns.Add("PACKQUANTITY1", typeof(Int32));

            table.Columns.Add("ABNORMITY_QUANTITY", typeof(Int32));
                        
            table.Columns.Add("EXPORTNO", typeof(Int32));
            table.Columns.Add("EXPORTNO1", typeof(Int32));
            table.Columns.Add("PACKNO", typeof(Int32));
            table.Columns.Add("PACKNO1", typeof(Int32));
            table.Columns.Add("PACKORDERNO", typeof(Int32));
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
            table.Columns.Add("CHANNELGROUP", typeof(Int32));
            table.Columns.Add("CHANNELORDER", typeof(Int32));
            table.Columns.Add("CHANNELTYPE");
            table.Columns.Add("EXPORTNO", typeof(Int32));
            table.Columns.Add("PACKNO", typeof(Int32));
            return table;
        }

        private void AddMasterRow(DataTable masterTable, DataRow masterRow, int sortNo, int orderNo, int[] quantity,int exportNoLast)
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

            //newRow["TQUANTITY"] = 0;
            newRow["QUANTITY"] = quantity[0];
            newRow["QUANTITY1"] = quantity[1];

            //newRow["PQUANTITY"] = 0;
            //newRow["PACKQUANTITY"] = 0;
            //newRow["PACKQUANTITY1"] = 0;

            newRow["ABNORMITY_QUANTITY"] = masterRow["ABNORMITY_QUANTITY"];

            newRow["EXPORTNO"] = masterRow["EXPORTNO"];
            newRow["EXPORTNO1"] = masterRow["EXPORTNO1"];
            newRow["PACKNO"] = masterRow["PACKNO"];
            newRow["PACKNO1"] = masterRow["PACKNO1"];
            newRow["PACKORDERNO"] = masterRow["PACKORDERNO"];

            masterTable.Rows.Add(newRow);
        }

        private void AddDetailRow(DataRow masterRow, DataTable detailTable, int sortNo, string lineCode, DataRow channelRow, int quantity)
        {
            if (quantity != 0)
            {
                //sortNo=-1时表示是在AdjustChannel方法中调用此方法，需要汇总每个烟道的分拣总量
                if (sortNo == -1)
                    channelRow["QUANTITY"] = Convert.ToInt32(channelRow["QUANTITY"]) + quantity;

                DataRow newRow = detailTable.NewRow();

                newRow["ORDERDATE"] = masterRow["ORDERDATE"];
                newRow["BATCHNO"] = masterRow["BATCHNO"];
                newRow["LINECODE"] = lineCode;
                newRow["SORTNO"] = sortNo;

                newRow["ORDERID"] = masterRow["ORDERID"];
                if (sortNo != -1)
                    newRow["ORDERNO"] = channelRow["ORDERNO"];
                newRow["CIGARETTECODE"] = channelRow["CIGARETTECODE"];
                newRow["CIGARETTENAME"] = channelRow["CIGARETTENAME"];                
                newRow["QUANTITY"] = quantity;

                newRow["CHANNELCODE"] = channelRow["CHANNELCODE"];
                newRow["CHANNELGROUP"] = channelRow["CHANNELGROUP"];
                newRow["CHANNELORDER"] = channelRow["CHANNELORDER"];
                newRow["CHANNELTYPE"] = channelRow["CHANNELTYPE"];
                if (sortNo != -1)
                    newRow["EXPORTNO"] = channelRow["EXPORTNO"];
                if (sortNo != -1)
                    newRow["PACKNO"] = channelRow["PACKNO"];

                detailTable.Rows.Add(newRow);
            }
        }
    }
}
