using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace THOK.AS.Stocking.Util.LED2008
{
    public class LedItem
    {
        public string Name;
        public int Count = 0;
        public override string ToString()
        {
            return string.Format("{0}-{1}",Count>0? Count.ToString().PadLeft(2, ' '):"", Name);
        }
    }

    public class LEDUtil
    {
        private LedCollection leds = new LedCollection();
        public Dictionary<int, string> isActiveLeds = new Dictionary<int, string>();

        public LEDUtil()
        {
            //��ʼ��LED��            
            leds.DelAllProgram();
        }

        public void Release()
        {
            //�ͷ�LED����Դ
            leds = null;
        } 

        public void RefreshStockInLED(DataTable table,string ledNo)
        {
            int cardNum = Convert.ToInt32(ledNo);

            if (!IsOnLineLed(cardNum))
            {
                return;
            }

            int i = 1;
            leds.DelAllProgram();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    string cigaretteName = row["CIGARETTENAME"].ToString().Trim();
                    cigaretteName = cigaretteName.Replace("(", StringUtil.ToSBC(""));
                    cigaretteName = cigaretteName.Replace(")", StringUtil.ToSBC(""));
                    cigaretteName = cigaretteName.Replace("��", StringUtil.ToSBC(""));
                    cigaretteName = cigaretteName.Replace("��", StringUtil.ToSBC(""));
                    cigaretteName = cigaretteName.Replace(" ", "");
                    cigaretteName = cigaretteName.Replace("  ", "");
                    leds.AddTextToProgram(cardNum, 0, (i - 1) * 16, 16, 128, row["QUANTITY"].ToString() + "-" + cigaretteName.ToString(), i == 1 ? LED2008.GREEN : LED2008.RED, false);
                    i++;
                }
            }
            else
            {
                leds.AddTextToProgram(cardNum, 0, (i - 1) * 16, 16, 128, "��ǰ������������", LED2008.GREEN, false);
            }       
                
            leds.SendToScreen();
        }

        public void RefreshStockInLED( string ledNo,string cigaretteName)
        {
            int cardNum = Convert.ToInt32(ledNo);

            if (!IsOnLineLed(cardNum))
            {
                return;
            }
            cigaretteName = cigaretteName.Replace("(", StringUtil.ToSBC(""));
            cigaretteName = cigaretteName.Replace(")", StringUtil.ToSBC(""));
            cigaretteName = cigaretteName.Replace("��", StringUtil.ToSBC(""));
            cigaretteName = cigaretteName.Replace("��", StringUtil.ToSBC(""));
            cigaretteName = cigaretteName.Replace(" ", "");
            cigaretteName = cigaretteName.Replace("  ", "");
            int i = 1;
            leds.DelAllProgram();

            leds.AddTextToProgram(cardNum, 0, (i - 1) * 16, 16, 128, "ȱ������,����⣺", LED2008.RED, false);
            i++;
            leds.AddTextToProgram(cardNum, 0, (i - 1) * 16, 16, 128, cigaretteName.ToString(), LED2008.GREEN, false);

            leds.SendToScreen();
        }

        private bool IsOnLineLed(int ledNo)
        {
            if (isActiveLeds.ContainsKey(ledNo))
            {
                Microsoft.VisualBasic.Devices.Network network = new Microsoft.VisualBasic.Devices.Network();
                if (!network.Ping(isActiveLeds[ledNo]))
                {
                    THOK.MCP.Logger.Error(ledNo + "��LED�����ϣ����飡IP:[" + isActiveLeds[ledNo] + "]");
                    return false;
                }
                else
                    return true;
            }
            else 
                return false;
        }

        internal void Show(string ledCode, LedItem[] ledItems)
        {
            int ledno = 0;
            if (int.TryParse(ledCode, out ledno))
            {
                if (IsOnLineLed(ledno))
                {
                    leds.DelAllProgram();

                    int i = 1;
                    if (ledItems.Length > 0)
                    {
                        foreach (LedItem item in ledItems)
                        {
                            string  cigaretteName = item.ToString();
                            cigaretteName = cigaretteName.Replace("(", StringUtil.ToSBC(""));
                            cigaretteName = cigaretteName.Replace(")", StringUtil.ToSBC(""));
                            cigaretteName = cigaretteName.Replace("��", StringUtil.ToSBC(""));
                            cigaretteName = cigaretteName.Replace("��", StringUtil.ToSBC(""));
                            cigaretteName = cigaretteName.Replace(" ", "");
                            cigaretteName = cigaretteName.Replace("  ", "");
                            leds.AddTextToProgram(ledno, 0, (i - 1) * 16, 16, 128, cigaretteName.ToString(), i == 1 ? LED2008.GREEN : LED2008.RED, false);
                            i++;
                        }
                    }
                    else
                        leds.AddTextToProgram(ledno, 0, (i - 1) * 16, 16, 128, "��ǰ��������", LED2008.GREEN, false);

                    leds.SendToScreen();
                }
            }
        }
    }
}
