using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.AS.Dao;

namespace THOK.AS.Dal
{
    public class CigaretteDal
    {
        public DataTable GetAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                CigaretteDao cigaretteDao = new CigaretteDao();
                table = cigaretteDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                CigaretteDao cigaretteDao = new CigaretteDao();
                count = cigaretteDao.FindCount(filter);
            }
            return count;
        }

        public void Save(string cigaretteCode, string cigaretteName,string isAbnormity, string barcode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                CigaretteDao cigaretteDao = new CigaretteDao();
                cigaretteDao.UpdateEntity(cigaretteCode,cigaretteName,isAbnormity, barcode);
            }
        }
    }
}
