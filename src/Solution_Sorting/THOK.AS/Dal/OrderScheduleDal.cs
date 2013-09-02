using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class OrderScheduleDal
    {
        public DataTable GetMasterAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderScheduleDao orderDao = new OrderScheduleDao();
                table = orderDao.FindMasterAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetMasterCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderScheduleDao orderDao = new OrderScheduleDao();
                count = orderDao.FindMasterCount(filter);
            }
            return count;
        }

        public DataTable GetDetailAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderScheduleDao orderDao = new OrderScheduleDao();
                table = orderDao.FindDetailAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetDetailCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderScheduleDao orderDao = new OrderScheduleDao();
                count = orderDao.FindDetailCount(filter);
            }
            return count;
        }

        public DataTable GetLineAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderScheduleDao orderDao = new OrderScheduleDao();
                table = orderDao.FindLineAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetLineCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                OrderScheduleDao orderDao = new OrderScheduleDao();
                count = orderDao.FindLineCount(filter);
            }
            return count;
        }

        public DataTable GetOrder(string orderDate, int batchNo, int mode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                OrderScheduleDao orderDao = new OrderScheduleDao();
                THOK.AS.Dao.SysParameterDao parameterDao = new SysParameterDao();

                Dictionary<string, string> parameter = parameterDao.FindParameters();
                string handleSortLineCode = parameter["handleSortLineCode"].ToString();                
                string wholePiecesSortLineCode = parameter["WholePiecesSortLineCode"].ToString();
                string abnormitySortLineCode = parameter["AbnormitySortLineCode"].ToString();

                switch (mode)
                {
                    case 1://正常分拣打码
                        return orderDao.FindOrder(orderDate, batchNo);
                        break;
                    case 2://手工分拣打码
                        if (handleSortLineCode != string.Empty)                    
                            return orderDao.FindOrderForHandleSort(orderDate, batchNo, handleSortLineCode);
                        else
                            return new DataTable();                          
                        break;
                    case 3://整件分拣打码
                        if (wholePiecesSortLineCode != string.Empty)
                            return orderDao.FindOrderForWholePieces(orderDate, batchNo, wholePiecesSortLineCode);
                        else
                            return new DataTable();                  
                        break;
                    case 4://异形分拣打码
                        if (abnormitySortLineCode != string.Empty)
                            return orderDao.FindOrderForAbnormity(orderDate, batchNo, abnormitySortLineCode);
                        else
                            return new DataTable();    
                        break;
                    default:
                        return new DataTable();
                        break;
                }
            }
        }
    }
}
