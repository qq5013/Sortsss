using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.Dao;

namespace THOK.AS.Stocking.StockOutProcess
{
    public class DataRequestProcess: AbstractProcess
    {
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    StockOutBatchDao stockOutBatchDao = new StockOutBatchDao();
                    StockOutDao stockOutDao = new StockOutDao();
                    StockInDao stockInDao = new StockInDao();
                    stockOutBatchDao.SetPersistentManager(pm);
                    stockOutDao.SetPersistentManager(pm);
                    stockInDao.SetPersistentManager(pm);

                    try
                    {
                        //��������ѯ��Ҫ�������Ĳ����ƻ���
                        bool b1 = Convert.ToBoolean(Context.Attributes["B1"]);
                        bool b2 = Convert.ToBoolean(Context.Attributes["B2"]);
                        DataTable outTable = stockOutDao.FindNoSupplyOrder(b1,b2);
                        DataTable stockInTable = stockInDao.FindStockInForIsInAndNotOut();                        

                        if (outTable.Rows.Count > 0)
                        {
                            pm.BeginTransaction();

                            for (int i = 0; i < outTable.Rows.Count; i++)
                            {                                
                                WriteToProcess("StockInRequestProcess", "StockInRequest", outTable.Rows[i]["CIGARETTECODE"].ToString());

                                DataRow[] stockInRows = stockInTable.Select(string.Format("CIGARETTECODE='{0}' AND STATE ='1' AND ( STOCKOUTID IS NULL OR STOCKOUTID = 0 )", outTable.Rows[i]["CIGARETTECODE"].ToString()), "STOCKINID");
                                if (stockInRows.Length > 0)
                                {
                                    stockInRows[0]["STOCKOUTID"] = outTable.Rows[i]["STOCKOUTID"].ToString();
                                    outTable.Rows[i]["STATE"] = 1;
                                }
                                else
                                {
                                    Logger.Error(string.Format("[{0}] [{1}] ��治�㣡", outTable.Rows[i]["CIGARETTECODE"].ToString(), outTable.Rows[i]["CIGARETTENAME"].ToString()));
                                    WriteToProcess("LEDProcess", "StockInRequestShow", outTable.Rows[0]["CIGARETTENAME"]);
                                    break;
                                }
                            }

                            stockOutDao.UpdateStatus(outTable);
                            stockInDao.UpdateStockOutIdToStockIn(stockInTable);

                            pm.Commit();
                            Logger.Info("����������ݳɹ���");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("�����������ʧ�ܣ�ԭ��" + e.Message);
                        pm.Rollback();
                    }
                }                
            }
            catch (Exception e)
            {
                Logger.Error("�����������ʧ�ܣ�ԭ��" + e.Message);
            }
        }
    }
}
