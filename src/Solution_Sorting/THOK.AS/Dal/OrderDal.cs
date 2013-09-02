using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class OrderDal
    {
        public DataTable GetRouteAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderDao orderDao = new OrderDao();
                table = orderDao.FindRouteAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }

        public int GetRouteCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderDao orderDao = new OrderDao();
                count = orderDao.FindRouteCount(filter);
            }
            return count;
        }

        public int GetMasterCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderDao orderDao = new OrderDao();
                count = orderDao.FindMasterCount(filter);
            }
            return count;
        }

        public DataTable GetMasterAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderDao orderDao = new OrderDao();
                table = orderDao.FindMasterAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }

        public int GetDetailCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderDao orderDao = new OrderDao();
                count = orderDao.FindDetailCount(filter);
            }
            return count;
        }

        public DataTable GetDetailAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderDao orderDao = new OrderDao();
                table = orderDao.FindDetailAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }

        public DataTable GetOrderRoute(string orderDate, int batchNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                OrderDao orderDao = new OrderDao();
                return orderDao.FindOrderRoute(orderDate, batchNo);
            }
        }

        public void DeleteNoUseOrder(string orderDate, int batchNo, string routes)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                OrderDao orderDao = new OrderDao();
                orderDao.DeleteNoUseOrder(orderDate, batchNo, routes);
            }
        }

    }
}
