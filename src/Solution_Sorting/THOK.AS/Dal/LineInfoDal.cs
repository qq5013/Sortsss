using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.AS.Dao;

namespace THOK.AS.Dal
{
    public class LineInfoDal
    {
        public DataTable GetAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                LineInfoDao lineinfoDao = new LineInfoDao();
                table = lineinfoDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                LineInfoDao lineinfoDao = new LineInfoDao();
                count = lineinfoDao.FindCount(filter);
            }
            return count;
        }

        public void Save(string lineCode, string status)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                LineInfoDao lineinfoDao = new LineInfoDao();
                lineinfoDao.UpdateEntity(lineCode, status);
            }
        }
    }
}
