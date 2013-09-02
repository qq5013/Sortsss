using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.AS.Stocking.Dao;
using System.Data;
using System.Windows.Forms;
using THOK.Util;

namespace THOK.AS.Stocking.StockInProcess
{
    class StockInRequestProcess : AbstractProcess
    {
        private static string strLock = string.Empty;
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*  �������
             * 
             *  stateItem.ItemName ��
             *      Init - ��ʼ����
             *      FirstBatch - ���ɵ�һ�������������
             *      StockInRequest - �������������������
             * 
             *  stateItem.State ������ - ����ľ��̱��롣        
            */
            string cigaretteCode = "";
            try
            {
                lock (strLock)
                {
                    switch (stateItem.ItemName)
                    {
                        case "Init":
                            break;
                        case "FirstBatch":
                            if (AddFirstBatch())
                            {
                                WriteToProcess("LEDProcess", "Refresh", null);
                            }
                            break;
                        case "StockInRequest":
                            cigaretteCode = Convert.ToString(stateItem.State);
                            if (Request(cigaretteCode) || RequestAll())
                            {
                                WriteToProcess("LEDProcess", "Refresh", null);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("������������������ɴ���ʧ�ܣ�ԭ��" + e.Message + e.StackTrace);
            }
        }

        private bool AddFirstBatch()
        {
            bool bResult = false;
            using (PersistentManager pm = new PersistentManager())
            {
                SupplyDao supplyDao = new SupplyDao();              
                ChannelDao channelDao = new ChannelDao();

                //��������ѯ��Ҫ�������Ĳ����ƻ���
                bool b1 = Convert.ToBoolean(Context.Attributes["B1"]);
                bool b2 = Convert.ToBoolean(Context.Attributes["B2"]);
                DataTable cigaretteTable = channelDao.FindChannelForCigaretteCode(b1,b2);
                int stockInCapacityQuantity =  Convert.ToInt32(Context.Attributes["StockInCapacityQuantity"]);
                IDictionary<string, int> stockInQuantityDic = new Dictionary<string, int>();
                if (cigaretteTable.Rows.Count !=0)
                {
                    foreach (DataRow row in cigaretteTable.Rows)
                    {
                        string channelCode = row["CHANNELCODE"].ToString();
                        string cigaretteCode = row["CIGARETTECODE"].ToString();
                        string cigaretteName = row["CIGARETTENAME"].ToString();
                        bool isStockIn = row["ISSTOCKIN"].ToString() == "1" ? true : false;
                        int remainQuantity = Convert.ToInt32(row["REMAINQUANTITY"]);
                        string barcode = row["BARCODE"].ToString();
                        int quantity = Convert.ToInt32(row["QUANTITY"]) - (stockInQuantityDic.ContainsKey(cigaretteCode) ? stockInQuantityDic[cigaretteCode] : 0);

                        if (quantity + remainQuantity >= stockInCapacityQuantity)
                        {
                            StockInRequest(channelCode, cigaretteCode, cigaretteName, barcode, stockInCapacityQuantity, remainQuantity, isStockIn);
                            if (!stockInQuantityDic.ContainsKey(cigaretteCode))
                            {
                                stockInQuantityDic.Add(cigaretteCode, 0);
                            }
                            stockInQuantityDic[cigaretteCode] += stockInCapacityQuantity - remainQuantity;
                            bResult = true;
                        }
                        else if (quantity + remainQuantity > 0)
                        {
                            StockInRequest(channelCode, cigaretteCode, cigaretteName, barcode, quantity + remainQuantity, remainQuantity, isStockIn);
                            bResult = true;
                        }
                        else if (Convert.ToInt32(row["QUANTITY_1"]) == 0)
                        {
                            //��������̬���������̵���
                            bool b3 = Convert.ToBoolean(Context.Attributes["B3"]);
                            if (b3)
                            {
                                ReSetStockChannel(channelCode);
                            }
                        }
                    }

                    Logger.Info("������һ�����������ɹ�");
                    WriteToProcess("LEDProcess", "Refresh", null);
                }
            }
            return bResult;
        }

        private bool RequestAll()
        {
            bool bResult = false;
            using (PersistentManager pm = new PersistentManager())
            {
                ChannelDao channelDao = new ChannelDao();

                //��������ѯ��Ҫ�������Ĳ����ƻ���
                bool b1 = Convert.ToBoolean(Context.Attributes["B1"]);
                bool b2 = Convert.ToBoolean(Context.Attributes["B2"]);
                DataTable cigaretteTable = channelDao.FindChannelForCigaretteCode(b1, b2);

                if (cigaretteTable.Rows.Count != 0)
                {
                    foreach (DataRow row in cigaretteTable.Rows)
                    {
                        string cigaretteCode = row["CIGARETTECODE"].ToString();
                        bResult = Request(cigaretteCode) ? true : bResult;
                    }
                }
            }
            return bResult;
        }

        private bool Request(string cigaretteCode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                StockInBatchDao stockInBatchDao = new StockInBatchDao();
                DataTable stockInBatchTable = stockInBatchDao.FindStockInBatch(cigaretteCode);

                if (stockInBatchTable.Rows.Count == 0 )
                {
                    if (StockInRequest(cigaretteCode))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool StockInRequest(string cigaretteCode)
        {
            bool bResult = false;
            using (PersistentManager pm = new PersistentManager())
            {
                SupplyDao supplyDao = new SupplyDao();
                ChannelDao channelDao = new ChannelDao();

                //��������ѯ��Ҫ�������Ĳ����ƻ���
                bool b1 = Convert.ToBoolean(Context.Attributes["B1"]);
                bool b2 = Convert.ToBoolean(Context.Attributes["B2"]);
                DataTable cigaretteTable = channelDao.FindChannelForCigaretteCode(cigaretteCode, b1, b2);
                int stockInCapacityQuantity = Convert.ToInt32(Context.Attributes["StockInCapacityQuantity"]);
                IDictionary<string, int> stockInQuantityDic = new Dictionary<string, int>();
                if (cigaretteTable.Rows.Count != 0)
                {
                    foreach (DataRow row in cigaretteTable.Rows)
                    {
                        if (Convert.ToInt32(row["QUANTITY_1"]) <= Convert.ToInt32(Context.Attributes["StockInRequestRemainQuantity"]))
                        {
                            string channelCode = row["CHANNELCODE"].ToString();
                            string cigaretteName = row["CIGARETTENAME"].ToString();
                            bool isStockIn = row["ISSTOCKIN"].ToString() == "1" ? true : false;
                            int remainQuantity = Convert.ToInt32(row["REMAINQUANTITY"]);
                            string barcode = row["BARCODE"].ToString();
                            int quantity = Convert.ToInt32(row["QUANTITY"]) - (stockInQuantityDic.ContainsKey(cigaretteCode) ? stockInQuantityDic[cigaretteCode] : 0);

                            if (quantity + remainQuantity >= stockInCapacityQuantity)
                            {
                                StockInRequest(channelCode, cigaretteCode, cigaretteName, barcode, stockInCapacityQuantity, 0, isStockIn);
                                if (!stockInQuantityDic.ContainsKey(cigaretteCode) )
                                {
                                    stockInQuantityDic.Add(cigaretteCode, 0);
                                }
                                stockInQuantityDic[cigaretteCode] += stockInCapacityQuantity - remainQuantity;
                                bResult = true;
                            }
                            else if (quantity + remainQuantity > 0)
                            {
                                StockInRequest(channelCode, cigaretteCode, cigaretteName, barcode, quantity + remainQuantity, 0, isStockIn);
                                bResult = true;
                            }
                            else if (Convert.ToInt32(row["QUANTITY_1"]) == 0)
                            {
                                //��������̬���������̵���
                                bool b3 = Convert.ToBoolean(Context.Attributes["B3"]);
                                if (b3)
                                {
                                    ReSetStockChannel(channelCode);
                                }
                            }
                        }
                    }                    
                }
            }
            return bResult;
        }

        private void ReSetStockChannel(string channelCode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                ChannelDao channelDao = new ChannelDao();

                //��������ѯ��Ҫ�������Ĳ����ƻ���
                bool b1 = Convert.ToBoolean(Context.Attributes["B1"]);
                bool b2 = Convert.ToBoolean(Context.Attributes["B2"]);
                DataTable cigaretteTable = channelDao.FindChannelForCigaretteCode(b1, b2);
                DataRow[] cigaretteRows = cigaretteTable.Select("", "QUANTITY DESC");

                if (cigaretteRows.Length > 0)
                {
                    int count = 1;
                    while (count < 4)
                    {
                        foreach (DataRow cigaretteRow in cigaretteRows)
                        {
                            string cigaretteCode = cigaretteRow["CIGARETTECODE"].ToString();
                            int quantity = Convert.ToInt32(cigaretteRow["QUANTITY"]);
                            int counttmp = Convert.ToInt32(cigaretteTable.Compute("COUNT(CIGARETTECODE)", string.Format("CIGARETTECODE='{0}'", cigaretteCode)));
                            if (count == counttmp && quantity > 0)
                            {
                                channelDao.ReSetStockChannel(channelCode, cigaretteRow);
                                return;
                            }
                        }
                        count++;
                    }
                }                
            }
        }

        private void StockInRequest(string channelCode, string cigaretteCode, string cigaretteName, string barcode, int quantity, int stockRemainQuantity, bool isStockIn)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                try
                {
                    pm.BeginTransaction();

                    StockInBatchDao stockInBatchDao = new StockInBatchDao();
                    StockInDao stockInDao = new StockInDao();

                    int batchNo = stockInBatchDao.FindMaxBatchNo() + 1;
                    stockInBatchDao.InsertBatch(batchNo, channelCode, cigaretteCode, cigaretteName, quantity, isStockIn ? stockRemainQuantity : 0);

                    int stockInID = stockInDao.FindMaxInID();
                    for (int i = 1; i <= quantity; i++)
                    {
                        stockInID = stockInID + 1;
                        stockInDao.Insert(stockInID, batchNo, channelCode, cigaretteCode, cigaretteName, barcode, (isStockIn && stockRemainQuantity-- > 0) ? "1" : "0");
                    }

                    pm.Commit();
                    Logger.Info(string.Format("�������ƻ���ɣ� {0}-{1}-{2}-{3}",channelCode, cigaretteCode, cigaretteName, barcode));                    
                }
                catch (Exception ex)
                {
                    pm.Rollback();
                    Logger.Error("�������ƻ�ʧ�ܣ����飺" + ex.Message);
                }
            }
        }
    
    }
}
