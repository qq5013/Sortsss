using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.Dao;
using THOK.AS.Stocking.Util.LED2008;

namespace THOK.AS.Stocking.StockInProcess
{
    public class LEDProcess: AbstractProcess
    {
        private LEDUtil ledUtil = new LEDUtil();
        private Dictionary<int, string> isActiveLeds = new Dictionary<int, string>();

        public override void Release()
        {
            try
            {
                ledUtil.Release();
                base.Release();
            }
            catch (Exception e)
            {
                Logger.Error("LEDProcess ��Դ�ͷ�ʧ�ܣ�ԭ��" + e.Message);
            }
        }

        public override void Initialize(Context context)
        {
            base.Initialize(context);

            Microsoft.VisualBasic.Devices.Network network = new Microsoft.VisualBasic.Devices.Network();
            string[] ledConfig = context.Attributes["IsActiveLeds"].ToString().Split(';');

            foreach (string led in ledConfig)
            {
                if (network.Ping(led.Split(',')[1]))
                {
                    isActiveLeds.Add(Convert.ToInt32(led.Split(',')[0]), led.Split(',')[1]);
                }
                else
                {
                    Logger.Error(Convert.ToInt32(led.Split(',')[0]) + "��LED�����ϣ����飡IP:[" + led.Split(',')[1] + "]");
                }
            }

            ledUtil.isActiveLeds = isActiveLeds;
        }

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*  �������
             *  Init����ʼ��
             *  Refresh��ˢ��LED����
             *      ��01����һ���� ��ʾ�������������Ϣ
             *      ��02���������� ��ʾ���󲹻��Ļ���̵�����˳����Ϣ
             */
            string cigaretteName = "";

            switch (stateItem.ItemName)
            {
                case "Refresh":
                    this.Refresh();
                    break;
                case "StockInRequestShow":
                    cigaretteName = Convert.ToString(stateItem.State);
                    this.StockInRequestShow(cigaretteName);
                    break;
                default:
                    if (stateItem.ItemName != string.Empty && stateItem.State is LedItem[])
                    {
                        Show(stateItem.ItemName,(LedItem[])stateItem.State);
                    }                    
                    break;
            }        
        }

        private void Refresh()
        {
            //ˢ��1����
            using (PersistentManager pm = new PersistentManager())
            {
                StockInBatchDao stockInBatchDao = new StockInBatchDao();
                DataTable batchTable = stockInBatchDao.FindStockInTopAnyBatch();
                ledUtil.RefreshStockInLED(batchTable, "1");
            }
        }

        private void StockInRequestShow(string cigaretteName)
        {
            ledUtil.RefreshStockInLED("1",cigaretteName);
            Logger.Info("ȱ�����ѣ������" + cigaretteName);
        }

        internal void Show(string ledCode,LedItem[] ledItems)
        {
            ledUtil.Show(ledCode, ledItems);
        }
    }
}
 