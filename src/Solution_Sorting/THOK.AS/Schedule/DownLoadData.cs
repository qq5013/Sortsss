using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Schedule
{
    public class DownLoadData
    {
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
                orderDao.DeleteOrder(orderDate, batchNo);

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
        public void DownloadData(string orderDate, int batchNo, string dataBase)
        {
            try
            {
                ProcessState.Status = "PROCESSING";
                ProcessState.TotalCount = 15;
                ProcessState.Step = 1;

                DateTime dtOrder = DateTime.Parse(orderDate);
                string historyDate = dtOrder.AddDays(-7).ToShortDateString();
                using (PersistentManager pm = new PersistentManager())
                {
                    BatchDao batchDao = new BatchDao();
                    using (PersistentManager ssPM = new PersistentManager(dataBase))
                    {
                        SalesSystemDao ssDao = new SalesSystemDao();
                        ssDao.SetPersistentManager(ssPM);
                        try
                        {
                            pm.BeginTransaction();

                            //AS_BI_BATCH
                            batchDao.DeleteHistory(historyDate);
                            ProcessState.CompleteCount = 1;

                            //AS_SC_CHANNELUSED
                            ChannelScheduleDao csDao = new ChannelScheduleDao();
                            csDao.DeleteHistory(historyDate);
                            ProcessState.CompleteCount = 2;

                            //AS_SC_LINE
                            LineScheduleDao lsDao = new LineScheduleDao();
                            lsDao.DeleteHistory(historyDate);
                            ProcessState.CompleteCount = 3;

                            //AS_SC_PALLETMASTER ,AS_SC_ORDER
                            OrderScheduleDao osDao = new OrderScheduleDao();
                            osDao.DeleteHistory(historyDate);
                            ProcessState.CompleteCount = 4;

                            //AS_I_ORDERMASTER,AS_I_ORDERDETAIL,
                            OrderDao orderDao = new OrderDao();
                            orderDao.DeleteHistory(historyDate);
                            ProcessState.CompleteCount = 5;

                            //AS_SC_STOCKMIXCHANNEL
                            StockChannelDao scDao = new StockChannelDao();
                            scDao.DeleteHistory(historyDate);
                            ProcessState.CompleteCount = 6;

                            //AS_SC_SUPPLY
                            SupplyDao supplyDao = new SupplyDao();
                            supplyDao.DeleteHistory(historyDate);
                            ProcessState.CompleteCount = 7;

                            //AS_SC_HANDLESUPPLY
                            HandleSupplyDao handleSupplyDao = new HandleSupplyDao();
                            handleSupplyDao.DeleteHistory(historyDate);
                            ProcessState.CompleteCount = 8;

                            ClearSchedule(orderDate, batchNo);

                            //////////////////////////////////////////////////////////////////////////

                            //下载区域表
                            AreaDao areaDao = new AreaDao();
                            DataTable areaTable = ssDao.FindArea();
                            areaDao.SynchronizeArea(areaTable);
                            ProcessState.CompleteCount = 9;

                            //下载配送线路表
                            RouteDao routeDao = new RouteDao();
                            DataTable routeTable = ssDao.FindRoute();
                            routeDao.SynchronizeRoute(routeTable);
                            ProcessState.CompleteCount = 10;

                            //下载客户表
                            CustomerDao customerDao = new CustomerDao();
                            DataTable customerTable = ssDao.FindCustomer(dtOrder);
                            customerDao.SynchronizeCustomer(customerTable);
                            ProcessState.CompleteCount = 11;

                            //下载卷烟表 进行同步
                            CigaretteDao cigaretteDao = new CigaretteDao();
                            DataTable cigaretteTable = ssDao.FindCigarette(dtOrder);
                            cigaretteDao.SynchronizeCigarette(cigaretteTable);
                            ProcessState.CompleteCount = 12;

                            //查询已优化过的线路，以进行排除。
                            string routes = lsDao.FindRoutes(orderDate);

                            //下载订单主表
                            DataTable masterTable = ssDao.FindOrderMaster(dtOrder, batchNo, routes);
                            orderDao.BatchInsertMaster(masterTable);
                            ProcessState.CompleteCount = 13;

                            //下载订单明细
                            DataTable detailTable = ssDao.FindOrderDetail(dtOrder, batchNo, routes);
                            orderDao.BatchInsertDetail(detailTable);
                            ProcessState.CompleteCount = 14;

                            pm.Commit();
                        }
                        catch (Exception e)
                        {
                            pm.Rollback();
                            throw e;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                ProcessState.Status = "ERROR";
                ProcessState.Message = ee.Message;
            }
        }
    }
}
