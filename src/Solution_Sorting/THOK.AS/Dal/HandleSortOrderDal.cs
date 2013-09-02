using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class HandleSortOrderDal
    {
        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                HandleSortOrderDao handleSortOrderDao = new HandleSortOrderDao();
                count = handleSortOrderDao.FindCount(filter);
            }
            return count;
        }

        public DataTable GetAll(int pageIndex, int PagingSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                HandleSortOrderDao handleSortOrderDao = new HandleSortOrderDao();
                table = handleSortOrderDao.FindAll((pageIndex - 1) * PagingSize, PagingSize, filter);
            }
            return table;
        }

        public void Insert(string orderDate, string orderId)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                HandleSortOrderDao handleSortOrderDao = new HandleSortOrderDao();
                handleSortOrderDao.InsertEntity(orderDate, orderId);
            }
        }

        public void Save(string orderDate, string oldOrderId, string newOrderId)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                HandleSortOrderDao handleSortOrderDao = new HandleSortOrderDao();
                handleSortOrderDao.UpdateEntity(orderDate, oldOrderId, newOrderId);
            }
        }

        public void Delete(string orderDate, string orderId)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                HandleSortOrderDao handleSortOrderDao = new HandleSortOrderDao();
                handleSortOrderDao.DeleteEntity(orderDate, orderId);
            }
        }

        /// <summary>
        /// 根据条件查询出该条记录在分拣订单细表的数量 2011-12-11 wu
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="orderDate">订单日期</param>
        /// <param name="batchNo">批次号</param>
        /// <param name="cigaretteCode">卷烟代码</param>
        public int GetQuantityByValue(string orderId, string orderDate, string batchNo, string cigaretteCode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                HandleSortOrderDao handleSortOrderDao = new HandleSortOrderDao();
                return handleSortOrderDao.GetQuantityByValue(orderId, orderDate, batchNo, cigaretteCode);
            }
        }

        /// <summary>
        ///  根据条件修改改记录的分拣数量 2011-12-11 wu
        /// </summary>
        /// <param name="quantity">实际分拣数量</param>
        /// <param name="orderId">订单号</param>
        /// <param name="orderDate">订单日期</param>
        /// <param name="batchNo">批次号</param>
        /// <param name="cigaretteCode">卷烟代码</param>
        public void updateSortQuantity(int quantity, string orderId, string orderDate, string batchNo, string cigaretteCode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                HandleSortOrderDao handleSortOrderDao = new HandleSortOrderDao();
                handleSortOrderDao.UpdateSortQuantity(quantity, orderId, orderDate, batchNo, cigaretteCode);
            }
        }
    }
}
