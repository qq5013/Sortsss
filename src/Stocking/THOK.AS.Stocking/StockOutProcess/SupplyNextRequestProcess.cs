using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Stocking.Dao;
using THOK.MCP;
using THOK.Util;

namespace THOK.AS.Stocking.StockOutProcess
{
    class SupplyNextRequestProcess : AbstractProcess
    {
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            try
            {
                bool needNotify = false;

                string lineCode = stateItem.ItemName.Split("_"[0])[0];
                string channelGroup = stateItem.ItemName.Split("_"[0])[1];
                string channelType = stateItem.ItemName.Split("_"[0])[2];

                object obj = ObjectUtil.GetObject(stateItem.State);
                int sortNo = obj != null ? Convert.ToInt32(obj) : 0;

                if (sortNo==0)
                {
                    return;
                }

                sortNo = sortNo + Convert.ToInt32(Context.Attributes["SupplyAheadCount-" + lineCode + "-" + channelGroup + "-" + channelType]);

                needNotify = AddNextSupply(lineCode, channelGroup, channelType, sortNo);

                if (needNotify)
                {
                    WriteToProcess("LedStateProcess", "Refresh", null);
                    WriteToProcess("ScannerStateProcess", "Refresh", null);
                    WriteToProcess("DataRequestProcess", "SupplyRequest", 1);
                }
            }
            catch (Exception e)
            {
                Logger.Error("�����������ɴ���ʧ�ܣ�ԭ��" + e.Message);
            }
        }

        private bool AddNextSupply(string lineCode, string channelGroup, string channelType, int sortNo)
        {
            bool result = false;
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    StockOutBatchDao batchDao = new StockOutBatchDao();
                    SupplyDao supplyDao = new SupplyDao();
                    StockOutDao outDao = new StockOutDao();
                    batchDao.SetPersistentManager(pm);
                    supplyDao.SetPersistentManager(pm);
                    outDao.SetPersistentManager(pm);

                    DataTable supplyTable = supplyDao.FindNextSupply(lineCode, channelGroup, channelType, sortNo);

                    if (supplyTable.Rows.Count != 0)
                    {
                        Logger.Info(string.Format("�յ��������󣬷ּ��� '{0}'���̵��� '{1}'���̵����� '{2}'����ˮ�� '{3}'", lineCode, channelGroup, channelType, sortNo));
                        try
                        {
                            pm.BeginTransaction();

                            int batchNo = batchDao.FindMaxBatchNo() + 1;
                            batchDao.InsertBatch(batchNo, lineCode, channelGroup, channelType, sortNo, supplyTable.Rows.Count);

                            int outID = outDao.FindMaxOutID();
                            outDao.Insert(outID, supplyTable);

                            pm.Commit();
                            result = true;

                            Logger.Info("���ɳ�������ɹ�");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("���ɳ�������ʧ�ܣ�ԭ��" + e.Message);
                            pm.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("���ɳ�������ʧ�ܣ�ԭ��" + e.Message);
            }

            return result;
        }
    }
}
