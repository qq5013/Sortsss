using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.StateManageProcess.Dao;

namespace THOK.AS.Stocking.StateManageProcess
{
    class ScannerStateProcess : AbstractProcess
    {
        /// <summary>
        /// ״̬�������б�
        /// </summary>
        private IDictionary<string, ScannerStateManage> scannerStateManages = new Dictionary<string, ScannerStateManage>();
        private ScannerStateManage GetStateManage(string stateItemCode)
        {
            if (!scannerStateManages.ContainsKey(stateItemCode))
            {
                lock (scannerStateManages)
                {
                    if (!scannerStateManages.ContainsKey(stateItemCode))
                    {
                        scannerStateManages[stateItemCode] = new ScannerStateManage(stateItemCode,this.Context.ProcessDispatcher);
                    }
                }
            }
            return scannerStateManages[stateItemCode];
        }

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*
             * stateItem.Name �� ��Ϣ��Դ 
             * stateItem.ItemName �� 
             *      ��Ӧ 
             *           (0)Init                              �� ��ʼ��������������ʱ�ĳ�ʼ������             * 
             *           (1)StateItemCode_ScannerMoveNext     �� ����PLC���ݵ�Ԫ���������ͨ����PLC���õ�ǰ����������ˮ�ţ��������ͨ����
             *           (2)StateItemCode_ScannerMoveTo       �� ����PLC���ݵ�Ԫ������������ݣ�PLC���õ�ǰ����������ˮ�ţ�����������ݣ�
             *           (3)StateItemCode_ScannerShowData     �� ����PLC���ݵ�Ԫ��������ʾ���ݣ�PLC���õ�ǰ����������ˮ�ţ�������ʾ���ݣ�
             *           (4)StateItemCode(��stateItem.Name = ��Scanner��) �� ����ɨ��������Ϣ������Ϊ��ǰɨ��������
             * stateItem.State ����Ϊ��ǰɨ��������
             *  
             */
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    string stateItemCode = "";
                    string action = "";

                    if (stateItem.ItemName == "Init")
                    {
                        foreach (string stateCode in (new ScannerStateManage()).GetStateItemCodeList())
                        {
                            GetStateManage(stateCode);
                        }

                        foreach (ScannerStateManage scannerStateManagesItem in scannerStateManages.Values)
                        {
                            scannerStateManagesItem.MoveTo(1);
                        }
                        return;
                    }

                    if (stateItem.ItemName == "Refresh")
                    {
                        foreach (ScannerStateManage scannerStateManagesItem in scannerStateManages.Values)
                        {
                            scannerStateManagesItem.ShowData();
                        }
                        return;
                    }

                    if (stateItem.Name == "Scanner")
                    {
                        stateItemCode = stateItem.ItemName;
                        action = "Scan";
                    }
                    else
                    {
                        stateItemCode = stateItem.ItemName.Split('_')[0];
                        action = stateItem.ItemName.Split('_')[1];
                    }

                    ScannerStateManage scannerStateManage = GetStateManage(stateItemCode);
                    int index = 0;
                    switch (action)
                    {
                        case "Scan":                                
                            if (stateItem.State is Dictionary<string, string> && ((Dictionary<string, string>)stateItem.State).ContainsKey("barcode"))
                            {
                                string barcode = ((Dictionary<string, string>)stateItem.State)["barcode"];
                                if (scannerStateManage.Check(barcode))
                                {
                                    if (scannerStateManage.MoveNext())
                                    {
                                        scannerStateManage.WriteToPlc(this.Context);
                                    }
                                    scannerStateManage.ShowData();
                                }
                            }
                            break;
                        case "ScannerMoveNext":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && scannerStateManage.Check(index))
                            {
                                if (scannerStateManage.MoveTo(index))
                                {
                                    if (scannerStateManage.MoveNext())
                                    {
                                        scannerStateManage.WriteToPlc(this.Context);
                                    }
                                }
                                scannerStateManage.ShowData();
                            }
                            break;
                        case "ScannerMoveTo":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0)
                            {
                                scannerStateManage.MoveTo(index);
                                Logger.Info(string.Format("{0} ��ɨ������У�����,��ˮ�ţ�{1}", stateItemCode, index));
                                scannerStateManage.ShowData();
                            }
                            break;
                        case "ScannerShowData":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && scannerStateManage.Check(index))
                            {
                                scannerStateManage.ShowData(index-1);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ScannerStateProcess.StateChanged() ����ʧ�ܣ�ԭ��" + e.Message + e.StackTrace);
            }
        }
    }
}
