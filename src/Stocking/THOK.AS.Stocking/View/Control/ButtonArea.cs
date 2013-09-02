using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using THOK.MCP;
using THOK.MCP.View;
using THOK.Util;
using THOK.AS.Stocking.Dao;

namespace THOK.AS.Stocking.View
{
    public partial class ButtonArea : ProcessControl
    {
        public ButtonArea()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (btnStop.Enabled)
            {
                MessageBox.Show("��ֹͣ��������˳�ϵͳ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (DialogResult.Yes == MessageBox.Show("��ȷ��Ҫ�˳��������ϵͳ��", "ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                Util.LogFile.DeleteFile();
                Application.Exit();
            }
        }

        private void btnOperate_Click(object sender, EventArgs e)
        {
            try
            {
                THOK.AF.Config config = new THOK.AF.Config();
                THOK.AF.MainFrame mainFrame = new THOK.AF.MainFrame(config);
                mainFrame.Context = Context;
                mainFrame.ShowInTaskbar = false;
                mainFrame.Icon = new Icon(@"./App.ico");
                mainFrame.ShowIcon = true;
                mainFrame.StartPosition = FormStartPosition.CenterScreen;
                mainFrame.WindowState = FormWindowState.Maximized;
                mainFrame.ShowDialog();
            }
            catch (Exception ee)
            {
                Logger.Error("������ҵ����ʧ�ܣ�ԭ��" + ee.Message);
            }

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {            
            try
            {
                DownloadData();
                Context.ProcessDispatcher.WriteToProcess("LEDProcess", "Refresh", null);
                Context.ProcessDispatcher.WriteToProcess("LedStateProcess", "Refresh", null);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            string text = "�ֹ����¾���������Ϣ��";
            string cigaretteCode = "";
            string barcode = "";

            Scan(text, cigaretteCode, barcode);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                Context.ProcessDispatcher.WriteToProcess("OrderDataStateProcess", "Start", null);
                Context.ProcessDispatcher.WriteToProcess("LEDProcess", "Refresh", null);
                Context.ProcessDispatcher.WriteToProcess("LedStateProcess", "Refresh", null);
                SwitchStatus(true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Context.ProcessDispatcher.WriteToProcess("OrderDataStateProcess", "Stop", null);
            SwitchStatus(false);
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "help.chm");
        }

        private void SwitchStatus(bool isStart)
        {
            btnDownload.Enabled = !isStart;
            btnUpload.Enabled = !isStart;
            btnStart.Enabled = !isStart;
            btnStop.Enabled = isStart;
            btnSimulate.Enabled = !isStart;
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {
            Context.ProcessDispatcher.WriteToProcess("DataRequestProcess", "StockInRequest", 1);
        }

        /// <summary>
        /// �������� ����޸����� 2010-10-30
        /// </summary>
        private void DownloadData()
        {
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    DownloadDataDao downloadDataDao = new DownloadDataDao();
                    StockOutDao stockOutDao = new StockOutDao();
                    SupplyDao supplyDao = new SupplyDao();

                    if (supplyDao.FindCount() != stockOutDao.FindOutQuantity())
                        if (DialogResult.Cancel == MessageBox.Show("����δ��������ݣ���ȷ��Ҫ��������������", "ѯ��", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                            return;

                    using (PersistentManager pmServer = new PersistentManager("ServerConnection"))
                    {
                        ServerDao serverDao = new ServerDao();
                        serverDao.SetPersistentManager(pmServer);

                        //ORDER BY ORDERDATE,BATCHNO  ���ҵ�һ���Σ��������Ż��������ϴ�һ�Ź��̣�δ���ص����Σ�
                        DataTable table = serverDao.FindBatch();
                        if (table.Rows.Count != 0)
                        {                           
                            string batchID = table.Rows[0]["BATCHID"].ToString();
                            string orderDate = table.Rows[0]["ORDERDATE"].ToString();
                            string batchNo = table.Rows[0]["BATCHNO"].ToString();
                            string totalQuantity = supplyDao.FindCount().ToString();

                            //Clear
                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("�����ҵ��", 5, 1));
                            downloadDataDao.Clear();
                            System.Threading.Thread.Sleep(100);

                            //AS_SC_STOCKCHANNEL
                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("���ز����̵���", 5, 2));
                            downloadDataDao.InsertStockChannel(serverDao.FindStockChannel(orderDate, batchNo));
                            System.Threading.Thread.Sleep(100);

                            //AS_SC_STOCKMIXCHANNEL
                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("���ز�������̵���", 5, 3));
                            downloadDataDao.InsertStockMixChannel(serverDao.FindStockMixChannel(orderDate, batchNo));
                            System.Threading.Thread.Sleep(100);
                            //AS_SC_CHANNELUSED
                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("���طּ��̵���", 5, 4));
                            downloadDataDao.InsertChannelUSED(serverDao.FindChannelUSED(orderDate, batchNo));
                            System.Threading.Thread.Sleep(100);
                            //AS_SC_SUPPLY
                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState("���ز����ƻ���", 5, 5));
                            downloadDataDao.InsertSupply(serverDao.FindSupply(orderDate, batchNo));
                            System.Threading.Thread.Sleep(100);

                            serverDao.UpdateBatchStatus(batchID);
                            Context.ProcessDispatcher.WriteToProcess("monitorView", "ProgressState", new ProgressState());

                            string dataAndTip = "����������ɣ��������ڣ�[{0}],���κţ�[{1}],��������[{2}]";
                            Logger.Info(string.Format(dataAndTip,orderDate,batchNo,totalQuantity));

                            //��ʼ��PLC���ݣ�������PLC��������PLC��
                            Context.ProcessDispatcher.WriteToService("StockPLC_01", "RestartData", 3);
                            //��ʼ�����ɨ����
                            Context.ProcessDispatcher.WriteToProcess("ScanProcess", "Init", null);
                            //��ʼ��״̬������
                            Context.ProcessDispatcher.WriteToProcess("LedStateProcess", "Init", null);
                            Context.ProcessDispatcher.WriteToProcess("OrderDataStateProcess", "Init", null);
                            Context.ProcessDispatcher.WriteToProcess("ScannerStateProcess", "Init", null);
                            //�������������������
                            Context.ProcessDispatcher.WriteToProcess("StockInRequestProcess", "FirstBatch", null);               
                        }
                        else
                            MessageBox.Show("û�в����ƻ����ݣ�", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("�������ش���ʧ�ܣ�ԭ��" + e.Message);
            }
        }

        public delegate void ProcessStateInMainThread(StateItem stateItem);

        private void ProcessState(StateItem stateItem)
        {
            switch (stateItem.ItemName)
            {
                case "SimulateDialog":
                    string scannerCode = stateItem.State.ToString();
                    THOK.AS.Stocking.View.SimulateDialog simulateDialog = new THOK.AS.Stocking.View.SimulateDialog();
                    simulateDialog.Text = scannerCode + " ��ɨ�����ֹ�ɨ�룡";
                    if (simulateDialog.ShowDialog() == DialogResult.OK)
                    {
                        Dictionary<string, string>  parameters = new Dictionary<string, string>();
                        parameters.Add("barcode", simulateDialog.Barcode);                        
                        Context.ProcessDispatcher.WriteToProcess("ScanProcess", scannerCode, parameters);
                    }
                    Context.ProcessDispatcher.WriteToProcess("ScanProcess","ErrReset", "01");
                    break;
                case "ScanDialog":
                    Dictionary<string, string> scanParam = (Dictionary<string, string>)stateItem.State;
                    Scan(scanParam["text"], scanParam["cigaretteCode"], scanParam["barcode"]);
                    break;
                case "MessageBox":
                    Dictionary<string, object> msgParam = (Dictionary<string, object>)stateItem.State;
                    MessageBox.Show((string)msgParam["msg"], (string)msgParam["title"], (MessageBoxButtons)msgParam["messageBoxButtons"], (MessageBoxIcon)msgParam["messageBoxIcon"]);
                    break;
                default:
                    break;
            }
        }

        public void Scan(string text, string cigaretteCode, string barcode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                StockOutDao outDao = new StockOutDao();
                SupplyDao supplyDao = new SupplyDao();

                if (barcode != string.Empty && supplyDao.Exist(barcode))
                    return;

                DataTable table = supplyDao.FindCigaretteAll(cigaretteCode);

                if (table.Rows.Count > 0)
                {
                    THOK.AS.Stocking.View.ScanDialog scanDialog = new THOK.AS.Stocking.View.ScanDialog(table);
                    scanDialog.setInformation(text, barcode);
                    if (scanDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (scanDialog.IsPass && scanDialog.Barcode.Length == 6)
                        {
                            cigaretteCode = scanDialog.SelectedCigaretteCode;
                            barcode = scanDialog.Barcode;

                            using (PersistentManager pmServer = new PersistentManager("ServerConnection"))
                            {
                                ServerDao serverDao = new ServerDao();
                                serverDao.SetPersistentManager(pmServer);
                                serverDao.UpdateCigaretteToServer(barcode, cigaretteCode);
                            }
                            outDao.UpdateCigarette(barcode, cigaretteCode);
                        }
                        else
                        {
                            MessageBox.Show("��֤�����", "��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        public override void Process(StateItem stateItem)
        {
            base.Process(stateItem);
            this.BeginInvoke(new ProcessStateInMainThread(ProcessState), stateItem);
        }

    }
}
