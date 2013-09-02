using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class BatchDal
    {
        public void AddBatch(string orderDate, int batchNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                BatchDao batchDao = new BatchDao();
                batchDao.InsertEntity(orderDate, batchNo);
            }
        }

        public DataTable GetAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                BatchDao batchDao = new BatchDao();
                table = batchDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }

        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                BatchDao batchDao = new BatchDao();
                count = batchDao.FindCount(filter);
            }
            return count;
        }

        public DataTable GetBatch(string orderDate, int batchNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                BatchDao batchDao = new BatchDao();
                return batchDao.FindBatch(orderDate, batchNo);
            }
        }

        public DataTable GetBatchNo(string orderDate)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                BatchDao batchDao = new BatchDao();
                table = batchDao.FindBatch(orderDate);
            }
            return table;
        }

        public void SaveExecuter(string user, string ip, string orderDate, int batchNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                BatchDao batchDao = new BatchDao();
                batchDao.UpdateExecuter(user, ip, orderDate, batchNo);
            }
        }

        public void SaveUploadUser(string user, string orderDate, int batchNo)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                BatchDao batchDao = new BatchDao();
                batchDao.UpdateNoOnePro(orderDate, batchNo, user);
            }
        }

        public void Save(string orderDate, string sortBatch, string no1Batch,string no1UpLoadState)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                BatchDao batchDao = new BatchDao();
                batchDao.UpdateEntity(orderDate,sortBatch,no1Batch,no1UpLoadState);
            }
        }
    }
}
