using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class AbnormityCigaretteDal
    {
        public DataTable GetLineAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                AbnormityCigaretteDao acDao = new AbnormityCigaretteDao();
                table = acDao.FindLineAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetLineCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                AbnormityCigaretteDao acDao = new AbnormityCigaretteDao();
                count = acDao.FindLineCount(filter);
            }
            return count;
        }

        public DataTable GetRouteAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                AbnormityCigaretteDao acDao = new AbnormityCigaretteDao();
                table = acDao.FindRouteAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetRouteCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                AbnormityCigaretteDao acDao = new AbnormityCigaretteDao();
                count = acDao.FindRouteCount(filter);
            }
            return count;
        }
    }
}
