using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.StateManageProcess.Dao;

namespace THOK.AS.Stocking.StateManageProcess
{
    class LedStateProcess : AbstractProcess
    {
        /// <summary>
        /// ״̬�������б�
        /// </summary>
        private IDictionary<string,LedStateManage> ledStateManages = new Dictionary<string,LedStateManage>();
        private LedStateManage GetStateManage(string stateItemCode)
        {
            if (!ledStateManages.ContainsKey(stateItemCode))
            {
                lock (ledStateManages)
                {
                    if (!ledStateManages.ContainsKey(stateItemCode))
                    {
                        ledStateManages[stateItemCode] = new LedStateManage(stateItemCode,this.Context.ProcessDispatcher);
                    }
                }                
            }
            return ledStateManages[stateItemCode];
        }

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*
             * stateItem.Name �� ��Ϣ��Դ ��
             * stateItem.ItemName �� 
             *      ��Ӧ    (0)Init                      �� ��ʼ��������������ʱ�ĳ�ʼ������
             *              (1)Refresh                   �� ˢ������
             *              (2)StateItemCode_LedMoveNext �� ����PLC���ݵ�Ԫ���������ͨ����PLC���õ�ǰ����������ˮ�ţ��������ͨ����
             *              (3)StateItemCode_LedMoveTo   �� ����PLC���ݵ�Ԫ������������ݣ�PLC���õ�ǰ����������ˮ�ţ�����������ݣ�
             *              (4)StateItemCode_LedShowData �� ����PLC���ݵ�Ԫ������ˢ������
             * stateItem.State ������PLC���ݿ����ˮ�š�
             */
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    if (stateItem.ItemName == "Init")
                    {
                        foreach (string stateCode in (new LedStateManage()).GetStateItemCodeList())
                        {
                            GetStateManage(stateCode);
                        }

                        foreach (LedStateManage ledStateManageItem in ledStateManages.Values)
                        {
                            ledStateManageItem.MoveTo(1);
                        }
                        return;
                    }

                    if (stateItem.ItemName == "Refresh")
                    {
                        foreach (LedStateManage ledStateManageItem in ledStateManages.Values)
                        {
                            ledStateManageItem.ShowData();
                        }
                        return;
                    }

                    string stateItemCode = stateItem.ItemName.Split('_')[0];
                    string action = stateItem.ItemName.Split('_')[1];
                    LedStateManage ledStateManage = GetStateManage(stateItemCode);
                    int index = 0;

                    switch (action)
                    {
                        case "LedMoveNext":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && ledStateManage.Check(index))
                            {
                                if (ledStateManage.MoveTo(index))
                                {
                                    if (ledStateManage.MoveNext())
                                    {
                                        if(!ledStateManage.WriteToPlc())
                                        {
                                            Logger.Info(string.Format("{0} ��LED����ͨ����д�����ʧ����ˮ�ţ�[{1}]", stateItemCode, index));
                                        }
                                    }
                                }
                                ledStateManage.ShowData();
                            }
                            break;
                        case "LedMoveTo":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0)
                            {
                                ledStateManage.MoveTo(index);
                                Logger.Info(string.Format("{0} ��LED��У�����,��ˮ�ţ�{1}", stateItemCode, index));
                                ledStateManage.ShowData();
                            }                            
                            break;
                        case "LedShowData":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && ledStateManage.Check(index))
                            {
                                ledStateManage.ShowData(index-1);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("LedStateProcess.StateChanged() ����ʧ�ܣ�ԭ��" + e.Message);
            }         
        }
    }
}
