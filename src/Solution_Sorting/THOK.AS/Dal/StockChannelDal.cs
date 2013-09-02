using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.AS.Dao;

namespace THOK.AS.Dal
{
    public class StockChannelDal
    {
        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                StockChannelDao stockChannelDao = new StockChannelDao();
                count = stockChannelDao.FindCount(filter);
            }
            return count;
        }
        public DataTable GetAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                StockChannelDao stockChannelDao = new StockChannelDao();
                table = stockChannelDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }
        public void Save(string channelCode, string cigaretteCode, string cigaretteName, int quantity, string status,string isStockIn)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                StockChannelDao stockChannelDao = new StockChannelDao();
                stockChannelDao.UpdateEntity(channelCode, cigaretteCode, cigaretteName, quantity, status, isStockIn);
            }
        }
    }
}
