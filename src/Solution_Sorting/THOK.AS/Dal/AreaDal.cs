using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.AS.Dao;

namespace THOK.AS.Dal
{
    public class AreaDal
    {
        public DataTable GetAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                AreaDao areaDao = new AreaDao();
                table = areaDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                AreaDao areaDao = new AreaDao();
                count = areaDao.FindCount(filter);
            }
            return count;
        }

        public void Save(string sortID, string areaCode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                AreaDao areaDao = new AreaDao();
                areaDao.UpdateEntity(sortID, areaCode);
            }
        }

        public void Insert(DataTable areaTable)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                AreaDao areaDao = new AreaDao();
                areaDao.BatchInsertArea(areaTable);
            }
        }
    }
}
