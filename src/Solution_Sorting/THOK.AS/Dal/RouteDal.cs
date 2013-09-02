using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class RouteDal
    {
        public DataTable GetAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                RouteDao routedao = new RouteDao();
                table = routedao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                RouteDao routedao = new RouteDao();
                count = routedao.FindCount(filter);
            }
            return count;
        }

        public void Save(string sortID,string lineCode,string routeCode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                RouteDao routeDao = new RouteDao();
                routeDao.UpdateEntity(sortID, lineCode, routeCode);
            }
        }
    }
}
