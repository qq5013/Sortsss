using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.Dao;
using THOK.AS.Stocking.Util.LED2008;

namespace THOK.AS.Stocking.CommonProcess
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
                Logger.Error("LEDProcess 资源释放失败，原因：" + e.Message);
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
                    Logger.Error(Convert.ToInt32(led.Split(',')[0]) + "号LED屏故障，请检查！IP:[" + led.Split(',')[1] + "]");
                }
            }

            ledUtil.isActiveLeds = isActiveLeds;
        }

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            if (stateItem.ItemName != string.Empty && stateItem.State is LedItem[])
            {
                Show(stateItem.ItemName, (LedItem[])stateItem.State);
            }
        }

        private void Show(string ledCode, LedItem[] ledItems)
        {
            ledUtil.Show(ledCode, ledItems);
        }
    }
}
 