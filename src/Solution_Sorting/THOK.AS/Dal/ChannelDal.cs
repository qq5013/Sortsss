using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.AS.Dao;

namespace THOK.AS.Dal
{
    public class ChannelDal
    {
        public DataTable GetAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                ChannelDao channelDao = new ChannelDao();
                table = channelDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                ChannelDao channelDao = new ChannelDao();
                count = channelDao.FindCount(filter);
            }
            return count;
        }

        public void Save(string channelID, string cigaretteCode, string cigaretteName, string status)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                ChannelDao channelDao = new ChannelDao();
                channelDao.UpdateEntity(channelID, cigaretteCode, cigaretteName,status);
            }
        }
    }
}
