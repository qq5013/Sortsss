using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Schedule;
using THOK.AS.Dao;
using THOK.Util;
using THOK.Optimize;
using System.Threading;

namespace THOK.AS.Schedule
{
    public class AutoSchedule
    {
        int batchCount = 0;
        public event ScheduleEventHandler OnSchedule = null;

        /// <summary>
        /// 清除指定批次数据
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void ClearSchedule(string orderDate, int batchNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                //AS_BI_BATCH
                BatchDao batchDao = new BatchDao();
                batchDao.UpdateExecuter("0", "0", orderDate, batchNo);
                batchDao.UpdateIsValid(orderDate, batchNo, "0");

                //AS_SC_CHANNELUSED
                ChannelScheduleDao csDao = new ChannelScheduleDao();
                csDao.DeleteSchedule(orderDate, batchNo);

                //AS_SC_LINE
                LineScheduleDao lsDao = new LineScheduleDao();
                lsDao.DeleteSchedule(orderDate, batchNo);

                //AS_SC_PALLETMASTER,AS_SC_ORDER
                OrderScheduleDao osDao = new OrderScheduleDao();
                osDao.DeleteSchedule(orderDate, batchNo);

                //AS_I_ORDERDETAIL，AS_I_ORDERMASTER
                OrderDao orderDao = new OrderDao();
                //orderDao.DeleteOrder(orderDate, batchNo);

                //AS_SC_STOCKMIXCHANNEL
                StockChannelDao scDao = new StockChannelDao();
                scDao.DeleteSchedule(orderDate, batchNo);

                //AS_SC_SUPPLY
                SupplyDao supplyDao = new SupplyDao();
                supplyDao.DeleteSchedule(orderDate, batchNo);

                //AS_SC_HANDLESUPPLY
                HandleSupplyDao handleSupplyDao = new HandleSupplyDao();
                handleSupplyDao.DeleteHandleSupply(orderDate, batchNo);

            }
        }

        /// <summary>
        /// 清除历史数据，并下载数据。
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void DownloadData(string orderDate, int batchNo,string dataBase)
        {
            //try
            //{
            //    DateTime dtOrder = DateTime.Parse(orderDate);
            //    string historyDate = dtOrder.AddDays(-7).ToShortDateString();
            //    using (PersistentManager pm = new PersistentManager())
            //    {
            //        BatchDao batchDao = new BatchDao();
            //        using (PersistentManager ssPM = new PersistentManager(dataBase))
            //        {
            //            SalesSystemDao ssDao = new SalesSystemDao();
            //            ssDao.SetPersistentManager(ssPM);
            //            try
            //            {
            //                pm.BeginTransaction();

            //                //AS_BI_BATCH
            //                batchDao.DeleteHistory(historyDate);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 1, 14));

            //                //AS_SC_CHANNELUSED
            //                ChannelScheduleDao csDao = new ChannelScheduleDao();
            //                csDao.DeleteHistory(historyDate);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 2, 14));

            //                //AS_SC_LINE
            //                LineScheduleDao lsDao = new LineScheduleDao();
            //                lsDao.DeleteHistory(historyDate);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 3, 14));

            //                //AS_SC_PALLETMASTER ,AS_SC_ORDER
            //                OrderScheduleDao osDao = new OrderScheduleDao();
            //                osDao.DeleteHistory(historyDate);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 4, 14));

            //                //AS_I_ORDERMASTER,AS_I_ORDERDETAIL,
            //                OrderDao orderDao = new OrderDao();
            //                orderDao.DeleteHistory(historyDate);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 5, 14));

            //                //AS_SC_STOCKMIXCHANNEL
            //                StockChannelDao scDao = new StockChannelDao();
            //                scDao.DeleteHistory(historyDate);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 6, 14));

            //                //AS_SC_SUPPLY
            //                SupplyDao supplyDao = new SupplyDao();
            //                supplyDao.DeleteHistory(historyDate);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 7, 14));

            //                //AS_SC_HANDLESUPPLY
            //                HandleSupplyDao handleSupplyDao = new HandleSupplyDao();
            //                handleSupplyDao.DeleteHistory(historyDate);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 8, 14));

            //                ClearSchedule(orderDate, batchNo);

            //                //////////////////////////////////////////////////////////////////////////

            //                //下载区域表
            //                AreaDao areaDao = new AreaDao();
            //                DataTable areaTable = ssDao.FindArea();
            //                areaDao.SynchronizeArea(areaTable);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 9, 14));

            //                //下载配送线路表
            //                RouteDao routeDao = new RouteDao();
            //                DataTable routeTable = ssDao.FindRoute();
            //                routeDao.SynchronizeRoute(routeTable);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 10, 14));

            //                //下载客户表
            //                CustomerDao customerDao = new CustomerDao();
            //                DataTable customerTable = ssDao.FindCustomer(dtOrder);
            //                customerDao.SynchronizeCustomer(customerTable);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 11, 14));

            //                //下载卷烟表 进行同步
            //                CigaretteDao cigaretteDao = new CigaretteDao();
            //                DataTable cigaretteTable = ssDao.FindCigarette(dtOrder);
            //                cigaretteDao.SynchronizeCigarette(cigaretteTable);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 12, 14));

            //                //查询已优化过的线路，以进行排除。
            //                string routes = lsDao.FindRoutes(orderDate);

            //                //下载订单主表
            //                DataTable masterTable = ssDao.FindOrderMaster(dtOrder, batchNo, routes);
            //                orderDao.BatchInsertMaster(masterTable);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 13, 14));

            //                //下载订单明细
            //                DataTable detailTable = ssDao.FindOrderDetail(dtOrder, batchNo, routes);
            //                orderDao.BatchInsertDetail(detailTable);
            //                if (OnSchedule != null)
            //                    OnSchedule(this, new ScheduleEventArgs(1, "数据清除与下载", 14, 14));

            //                pm.Commit();
            //            }
            //            catch (Exception e)
            //            {
            //                pm.Rollback();
            //                throw e;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ee)
            //{
            //    if (OnSchedule != null)
            //        OnSchedule(this, new ScheduleEventArgs(OptimizeStatus.ERROR, ee.Message));
            //    throw ee;
            //}
        }

        /// <summary>
        /// 开始优化
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void GenSchedule(string orderDate, int batchNo)
        {
            try
            {
                //分拣线优化
                GenLineSchedule(orderDate, batchNo);                

                //分拣线烟道优化
                GenChannelSchedule(orderDate, batchNo);
                
                //拆分订单
                GenSplitOrder(orderDate, batchNo);

                //订单优化
                GenOrderSchedule(orderDate, batchNo);

                //备货烟道优化
                GenStockChannelSchedule(orderDate, batchNo);

                //补货优化
                GenSupplySchedule(orderDate, batchNo);

                //手工补货优化
                GenHandleSupplySchedule(orderDate, batchNo);

                //更新为已优化
                using (PersistentManager pm = new PersistentManager())
                {
                    BatchDao batchDao = new BatchDao();
                    if (!batchDao.CheckOrder(orderDate, batchNo))
                    {
                        throw new Exception("优化过程出错，请检查！");
                    }
                    batchDao.SelectBalanceIntoHistory(orderDate, batchNo);
                    batchDao.UpdateIsValid(orderDate, batchNo, "1");
                    batchCount= batchDao.BatchCount(orderDate,batchNo);
                }

                if (OnSchedule != null)
                    OnSchedule(this, new ScheduleEventArgs(OptimizeStatus.COMPLETE,Convert.ToString(batchCount)));
                
            }
            catch (Exception e)
            {
                ClearSchedule(orderDate, batchNo);
                if (OnSchedule != null)
                    OnSchedule(this, new ScheduleEventArgs(OptimizeStatus.ERROR,"[" + e.TargetSite + "]\n" + e.Message));
            }
        }



        #region  优化方法

        /// <summary>
        /// 生产线优化  2008-12-11修改 
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void GenLineSchedule(string orderDate, int batchNo)
        {
            using (THOK.Util.PersistentManager pm = new THOK.Util.PersistentManager())
            {
                LineInfoDao lineDao = new LineInfoDao();
                OrderDao detailDao = new OrderDao();
                LineScheduleDao lineScDao = new LineScheduleDao();
                THOK.Optimize.LineOptimize lineSchedule = new THOK.Optimize.LineOptimize();

                DataTable routeTable = detailDao.FindRouteQuantity(orderDate, batchNo).Tables[0];
                DataTable lineTable = lineDao.GetAvailabeLine("2").Tables[0];
                DataTable scLineTable = new DataTable();
                if (lineTable.Rows.Count > 0)
                {
                    scLineTable = lineSchedule.Optimize(routeTable, lineTable, orderDate, batchNo);
                    lineScDao.SaveLineSchedule(scLineTable);
                }
                else
                    throw new Exception("没有可用的分拣线！");


                routeTable = detailDao.FindRouteQuantity(orderDate, batchNo).Tables[0];
                lineTable = lineDao.GetAvailabeLine("3").Tables[0];
                if (lineTable.Rows.Count > 0)
                {
                    scLineTable = lineSchedule.Optimize(routeTable, lineTable, orderDate, batchNo);
                    lineScDao.SaveLineSchedule(scLineTable);
                }

                if (OnSchedule != null)
                    OnSchedule(this, new ScheduleEventArgs(2, "生产线优化", 1, 1));
            }     
        }

        /// <summary>
        /// 生产线烟道优化
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void GenChannelSchedule(string orderDate, int batchNo)
        {
            using (THOK.Util.PersistentManager pm = new THOK.Util.PersistentManager())
            {
                ChannelDao channelDao = new ChannelDao();
                OrderDao detailDao = new OrderDao();                
                LineDeviceDao deviceDao = new LineDeviceDao();
                ChannelOptimize channelSchedule = new ChannelOptimize();

                THOK.AS.Dao.SysParameterDao parameterDao = new SysParameterDao();
                Dictionary<string, string> parameter = parameterDao.FindParameters();

                LineScheduleDao lineDao = new LineScheduleDao();
                DataTable lineTable = lineDao.FindAllLine(orderDate, batchNo).Tables[0];

                LineInfoDao lineDao1 = new LineInfoDao();
                DataTable lineTable1 = lineDao1.GetAvailabeLine("3").Tables[0];
                bool isUseWholePiecesSortLine = lineTable1.Rows.Count > 0;
            
                int currentCount = 0;
                int totalCount = lineTable.Rows.Count;

                foreach (DataRow lineRow in lineTable.Rows)
                {
                    string lineCode = lineRow["LINECODE"].ToString();

                    //查询分拣线烟道表
                    DataTable channelTable = channelDao.FindAvailableChannel(lineCode).Tables[0];
                    //查询分拣线设备参数表            
                    DataTable deviceTable = deviceDao.FindLineDevice(lineCode).Tables[0];

                    //通道机两条分拣线品牌必须一致，立式机两条分拣线则不需要一致
                    //取所有订单品牌及总数量
                    DataTable orderTable = detailDao.FindAllCigaretteQuantity(orderDate, batchNo, isUseWholePiecesSortLine).Tables[0];
                    //取每条线订单品牌及总数量
                    DataTable lineOrderTable = detailDao.FindLineCigaretteQuantity(orderDate, batchNo, lineCode, isUseWholePiecesSortLine).Tables[0];

                    bool isUseSynchronizeOptimize = Convert.ToBoolean(parameter["IsUseSynchronizeOptimize"]);

                    channelSchedule.Optimize(isUseSynchronizeOptimize ? orderTable : lineOrderTable, lineOrderTable, channelTable, deviceTable, parameter);

                    channelDao.SaveChannelSchedule(channelTable, orderDate, batchNo);

                    if (OnSchedule != null)
                        OnSchedule(this, new ScheduleEventArgs(3, "正在优化" + lineRow["LINECODE"].ToString() + "分拣线烟道", ++currentCount, totalCount));
                }

                currentCount = 0;
                totalCount = lineTable1.Rows.Count;

                foreach (DataRow lineRow in lineTable1.Rows)
                {
                    string lineCode = lineRow["LINECODE"].ToString();

                    //查询分拣线烟道表
                    DataTable channelTable = channelDao.FindAvailableChannel(lineCode).Tables[0];
                    channelDao.SaveChannelSchedule(channelTable, orderDate, batchNo);

                    if (OnSchedule != null)
                        OnSchedule(this, new ScheduleEventArgs(3, "正在优化" + lineRow["LINECODE"].ToString() + "分拣线烟道", ++currentCount, totalCount));
                }
            }
        }

        /// <summary>
        /// 订单拆分2010-07-05
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void GenSplitOrder(string orderDate, int batchNo)
        {
            using (THOK.Util.PersistentManager pm = new THOK.Util.PersistentManager())
            {

                OrderDao orderDao = new OrderDao();
                ChannelDao channelDao = new ChannelDao();
                OrderScheduleDao orderScheduleDao = new OrderScheduleDao();
                LineScheduleDao lineDao = new LineScheduleDao();

                OrderSplitOptimize orderSchedule = new OrderSplitOptimize();

                THOK.AS.Dao.SysParameterDao parameterDao = new SysParameterDao();
                Dictionary<string, string> parameter = parameterDao.FindParameters();

                //查询分拣线表
                DataTable lineTable = lineDao.FindAllLine(orderDate, batchNo).Tables[0];

                //清理临时表
                orderScheduleDao.ClearSplitOrder();

                LineInfoDao lineDao1 = new LineInfoDao();
                DataTable lineTable1 = lineDao1.GetAvailabeLine("3").Tables[0];
                bool isUseWholePiecesSortLine = lineTable1.Rows.Count > 0;

                foreach (DataRow lineRow in lineTable.Rows)
                {
                    string lineCode = lineRow["LINECODE"].ToString();

                    //查询烟道信息表
                    DataTable channelTable = channelDao.FindChannelSchedule(orderDate, batchNo, lineCode, Convert.ToInt32(parameter["RemainCount"])).Tables[0];
                    //查询订单主表
                    DataTable masterTable = orderDao.FindOrderMaster(orderDate, batchNo, lineCode).Tables[0];                    
                    //查询订单明细表
                    DataTable orderTable = orderDao.FindOrderDetail(orderDate, batchNo, lineCode).Tables[0];

                    DataTable orderDetail = null;
                    HashTableHandle hashTableHandle = new HashTableHandle(orderTable);

                    int sortNo = 1;
                    int currentCount = 0;
                    int totalCount = masterTable.Rows.Count;
                    orderSchedule.moveToMixChannelProducts.Clear();

                    foreach (DataRow masterRow in masterTable.Rows)
                    {
                        DataRow[] orderRows = null;

                        orderDetail = hashTableHandle.Select("ORDERID", masterRow["ORDERID"]);
                        orderRows = orderDetail.Select(string.Format("ORDERID = '{0}'", masterRow["ORDERID"]), "QUANTITY");

                        DataSet ds = orderSchedule.Optimize(masterRow, orderRows, channelTable, lineCode, ref sortNo, parameter,isUseWholePiecesSortLine);
                        orderScheduleDao.SaveSplitOrder(ds);

                        if (OnSchedule != null)
                            OnSchedule(this, new ScheduleEventArgs(4, "正在优化" + lineRow["LINECODE"].ToString() + "分拣线订单", ++currentCount, totalCount));
                    }

                    channelDao.UpdateQuantity(channelTable, Convert.ToBoolean(parameter["IsUseBalance"]));
                }
            }
        }

        /// <summary>
        /// 订单优化2010-07-07
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void GenOrderSchedule(string orderDate, int batchNo)
        {
            using (THOK.Util.PersistentManager pm = new THOK.Util.PersistentManager())
            {

                OrderDao orderDao = new OrderDao();
                ChannelDao channelDao = new ChannelDao();
                OrderScheduleDao orderScheduleDao = new OrderScheduleDao();                
                SupplyDao supplyDao = new SupplyDao();
                OrderOptimize orderSchedule = new OrderOptimize();

                SysParameterDao parameterDao = new SysParameterDao();
                Dictionary<string, string> parameter = parameterDao.FindParameters();

                LineScheduleDao lineDao = new LineScheduleDao();
                DataTable lineTable = lineDao.FindAllLine(orderDate, batchNo).Tables[0];

                foreach (DataRow lineRow in lineTable.Rows)
                {
                    string lineCode = lineRow["LINECODE"].ToString();

                    //查询订单主表
                    DataTable masterTable = orderDao.FindTmpMaster(orderDate, batchNo, lineCode);
                    //查询订单明细表
                    DataTable orderTable = orderDao.FindTmpDetail(orderDate, batchNo, lineCode);
                    //查询分拣烟道表
                    DataTable channelTable = channelDao.FindChannelSchedule(orderDate, batchNo, lineCode, Convert.ToInt32(parameter["RemainCount"])).Tables[0];

                    int sortNo = 1;
                    int currentCount = 0;
                    int totalCount = masterTable.Rows.Count;

                    foreach (DataRow masterRow in masterTable.Rows)
                    {
                        DataRow[] orderRows = null;

                        //查询当前订单明细
                        orderRows = orderTable.Select(string.Format("SORTNO = '{0}'", masterRow["SORTNO"]), "CHANNELGROUP, CHANNELCODE");
                        DataSet ds = orderSchedule.Optimize(masterRow, orderRows, channelTable, ref sortNo);

                        orderScheduleDao.SaveOrder(ds);
                        supplyDao.InsertSupply(ds.Tables["SUPPLY"], lineCode, orderDate, batchNo);

                        if (OnSchedule != null)
                            OnSchedule(this, new ScheduleEventArgs(5, "正在优化" + lineRow["LINECODE"].ToString() + "分拣线订单", ++currentCount, totalCount));
                    }

                    channelDao.Update(channelTable);
                }

                LineInfoDao lineDao1 = new LineInfoDao();
                DataTable lineTable1 = lineDao1.GetAvailabeLine("3").Tables[0];

                foreach (DataRow lineRow in lineTable1.Rows)
                {
                    string lineCode = lineRow["LINECODE"].ToString();

                    //查询烟道信息表
                    DataTable channelTable = channelDao.FindChannelSchedule(orderDate, batchNo, lineCode, Convert.ToInt32(parameter["RemainCount"])).Tables[0];
                    //查询订单主表
                    DataTable masterTable = orderDao.FindOrderMaster(orderDate, batchNo, lineCode).Tables[0];
                    //查询订单明细表
                    DataTable orderTable = orderDao.FindOrderDetail(orderDate, batchNo, lineCode).Tables[0];

                    int sortNo = 1;
                    int currentCount = 0;
                    int totalCount = masterTable.Rows.Count;

                    foreach (DataRow masterRow in masterTable.Rows)
                    {
                        DataRow[] orderRows = null;
                        //查询当前订单明细
                        orderRows = orderTable.Select(string.Format("ORDERID = '{0}'", masterRow["ORDERID"]), "CIGARETTECODE");
                        DataSet ds = orderSchedule.OptimizeUseWholePiecesSortLine(masterRow, orderRows, channelTable, ref sortNo);
                        orderScheduleDao.SaveOrder(ds);

                        if (OnSchedule != null)
                            OnSchedule(this, new ScheduleEventArgs(5, "正在优化" + lineRow["LINECODE"].ToString() + "分拣线订单", ++currentCount, totalCount));
                    }

                    channelDao.UpdateQuantity(channelTable,false);
                }
            }
        }

        /// <summary>
        /// 备货烟道优化2010-07-08
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void GenStockChannelSchedule(string orderDate, int batchNo)
        {
            using (THOK.Util.PersistentManager pm = new THOK.Util.PersistentManager())
            {
                StockChannelDao schannelDao = new StockChannelDao();
                OrderDao orderDao = new OrderDao();
                OrderDao detailDao = new OrderDao(); 

                SysParameterDao parameterDao = new SysParameterDao();
                Dictionary<string, string> parameter = parameterDao.FindParameters();
                
                //每天分拣结束后备货烟道是否为空
                if (parameter["ClearStockChannel"] == "1")
                    schannelDao.ClearCigarette();

                //查询补货烟道表
                DataTable channelTable = schannelDao.FindChannel();
                //查询通道机卷烟数量信息表
                DataTable orderCTable = orderDao.FindCigaretteQuantityFromChannelUsed(orderDate, batchNo, "3");
                //查询立式机卷烟数量信息表（应加上混合烟道问题）
                DataTable orderTTable = orderDao.FindCigaretteQuantityFromChannelUsed(orderDate, batchNo, "2");
                //取所有订单品牌及总数量
                DataTable orderTable = detailDao.FindAllCigaretteQuantity(orderDate, batchNo,false).Tables[0];

                StockOptimize stockOptimize = new StockOptimize();

                bool isUseSynchronizeOptimize = Convert.ToBoolean(parameter["IsUseSynchronizeOptimize"]);
                DataTable mixTable = stockOptimize.Optimize(isUseSynchronizeOptimize, channelTable, isUseSynchronizeOptimize ? orderCTable : orderTable, isUseSynchronizeOptimize ? orderTTable : orderTable, orderDate, batchNo);

                schannelDao.UpdateChannel(channelTable);
                schannelDao.InsertStockChannelUsed(orderDate, batchNo,channelTable);
                schannelDao.InsertMixChannel(mixTable);

                if (OnSchedule != null)
                    OnSchedule(this, new ScheduleEventArgs(6, "备货烟道优化", 1, 1));
            }
        }

        /// <summary>
        /// 补货优化2010-04-19
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void GenSupplySchedule(string orderDate, int batchNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                OrderDao orderDao = new OrderDao();
                ChannelDao channelDao = new ChannelDao();
                LineScheduleDao lineDao = new LineScheduleDao();
                SupplyDao supplyDao = new SupplyDao();
                SupplyOptimize supplyOptimize = new SupplyOptimize();

                SysParameterDao parameterDao = new SysParameterDao();
                Dictionary<string, string> parameter = parameterDao.FindParameters();

                DataTable lineTable = lineDao.FindAllLine(orderDate, batchNo).Tables[0];

                int currentCount = 0;
                int totalCount = lineTable.Rows.Count;

                foreach (DataRow lineRow in lineTable.Rows)
                {
                    string lineCode = lineRow["LINECODE"].ToString();

                    int channelGroup = 1;
                    int channelType = 2;
                    int aheadCount = Convert.ToInt32(parameter[string.Format("SupplyAheadCount-{0}-{1}-{2}",lineCode,channelGroup,channelType)]);                   
                    supplyDao.AdjustSortNo(orderDate, batchNo, lineCode, channelGroup, channelType, aheadCount);

                    channelGroup = 1;
                    channelType = 3;
                    aheadCount = Convert.ToInt32(parameter[string.Format("SupplyAheadCount-{0}-{1}-{2}", lineCode, channelGroup, channelType)]);   
                    supplyDao.AdjustSortNo(orderDate, batchNo, lineCode, channelGroup, channelType, aheadCount);

                    channelGroup = 2;
                    channelType = 2;
                    aheadCount = Convert.ToInt32(parameter[string.Format("SupplyAheadCount-{0}-{1}-{2}", lineCode, channelGroup, channelType)]);       
                    supplyDao.AdjustSortNo(orderDate, batchNo, lineCode, channelGroup, channelType, aheadCount);

                    channelGroup = 2;
                    channelType = 3;
                    aheadCount = Convert.ToInt32(parameter[string.Format("SupplyAheadCount-{0}-{1}-{2}", lineCode, channelGroup, channelType)]);      
                    supplyDao.AdjustSortNo(orderDate, batchNo, lineCode, channelGroup, channelType, aheadCount);

                    DataTable channelTable = channelDao.FindChannelSchedule(orderDate, batchNo, lineCode, Convert.ToInt32(parameter["RemainCount"])).Tables[0];
                    DataTable supplyTable = supplyOptimize.Optimize(channelTable);
                    supplyDao.InsertSupply(supplyTable, Convert.ToBoolean(parameter["IsSupplyOrderbyCigaretteCode"]));

                    if (OnSchedule != null)
                        OnSchedule(this, new ScheduleEventArgs(7, "正在优化" + lineRow["LINECODE"].ToString() + "补货计划", currentCount++, totalCount));
                }

                LineInfoDao lineDao1 = new LineInfoDao();
                DataTable lineTable1 = lineDao1.GetAvailabeLine("3").Tables[0];

                foreach (DataRow lineRow in lineTable1.Rows)
                {
                    string lineCode = lineRow["LINECODE"].ToString();

                    //查询烟道信息表
                    DataTable channelTable = channelDao.FindChannelSchedule(orderDate, batchNo, lineCode, Convert.ToInt32(parameter["RemainCount"])).Tables[0];
                    //查询订单主表
                    DataTable masterTable = orderDao.FindOrderMaster(orderDate, batchNo, lineCode).Tables[0];
                    //查询订单明细表
                    DataTable orderTable = orderDao.FindOrderDetail(orderDate, batchNo, lineCode).Tables[0];

                    int serialNo = 1;
                    currentCount = 0;
                    totalCount = masterTable.Rows.Count;

                    foreach (DataRow masterRow in masterTable.Rows)
                    {
                        DataRow[] orderRows = null;
                        //查询当前订单明细
                        orderRows = orderTable.Select(string.Format("ORDERID = '{0}'", masterRow["ORDERID"]), "CIGARETTECODE");
                        DataTable supplyTable = supplyOptimize.Optimize(channelTable, orderRows,ref serialNo);
                        supplyDao.InsertSupply(supplyTable,true);

                        if (OnSchedule != null)
                            OnSchedule(this, new ScheduleEventArgs(7, "正在优化" + lineRow["LINECODE"].ToString() + "分拣线订单补货！", ++currentCount, totalCount));
                    }
                }
            }
        }

        /// <summary>
        /// 手工补货优化
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void GenHandleSupplySchedule(string orderDate, int batchNo) 
        {
            using (PersistentManager pm = new PersistentManager())
            {

                HandleSupplyOptimize handleSupplyOptimize = new THOK.Optimize.HandleSupplyOptimize();
                ScOrderDao scOrderDao = new ScOrderDao();
                ChannelDao channelDao = new ChannelDao();

                Dao.LineScheduleDao lineDao = new LineScheduleDao();
                DataTable lineTable = lineDao.FindAllLine(orderDate, batchNo).Tables[0];

                int currentCount = 0;
                int totalCount = lineTable.Rows.Count;

                foreach (DataRow lineRow in lineTable.Rows)
                {
                    string lineCode = lineRow["LINECODE"].ToString();

                    DataTable handSupplyOrders = scOrderDao.FindHandleSupplyOrder(orderDate, batchNo, lineCode);
                    DataTable multiBrandChannel = channelDao.FindMultiBrandChannel(lineCode);

                    AddColumnForChannelTable(multiBrandChannel, multiBrandChannel.Rows.Count);

                    //返回新的手工补货订单表
                    DataTable newSupplyOrders = handleSupplyOptimize.Optimize(handSupplyOrders, multiBrandChannel);

                    //保存烟道空仓作业的SortNo
                    channelDao.Update(multiBrandChannel,orderDate, batchNo);

                    //删除sc_order原来的手工补货定单
                    scOrderDao.DeleteOldSupplyOrders(orderDate, batchNo, lineCode);
                    //在sc_order中插入新的手工补货定单
                    scOrderDao.InsertNewSupplyOrders(newSupplyOrders);


                    //在AS_SC_HANDLESUPPLY中插入新的手工补货定单
                    scOrderDao.InsertHandSupplyOrders(newSupplyOrders);

                    if (OnSchedule != null)
                        OnSchedule(this, new ScheduleEventArgs(8, "正在优化" + lineRow["LINECODE"].ToString() + "分拣线手工补货定单订单", ++currentCount, totalCount));
                }
            }
        }

        private DataTable AddColumnForChannelTable(DataTable channel, int channelCount)
        {
            for (int i = 0; i < channelCount; i++)
            {
                channel.Rows[i]["QUANTITY"] = 0;
            }

            return channel;
        } 

        #endregion
    }
}