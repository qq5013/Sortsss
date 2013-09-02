using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.AS.Dao;

namespace THOK.AS.Dal
{
    public class DepartmentDal
    {
        public DataTable GetAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                DepartmentDao departmentDao = new DepartmentDao();
                table = departmentDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                DepartmentDao departmentDao = new DepartmentDao();
                count = departmentDao.FindCount(filter);
            }
            return count;
        }

        public void Save(string departmentID, string departmentName, string remark)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DepartmentDao departmentDao = new DepartmentDao();
                departmentDao.UpdateEntity(departmentID, departmentName, remark);
            }
        }

        public void Insert(string departmentName, string remark)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DepartmentDao departmentDao = new DepartmentDao();
                departmentDao.InsertEntity(departmentName, remark);
            }
        }

        public void Delete(string departmentID)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DepartmentDao departmentDao = new DepartmentDao();
                departmentDao.DeleteEntity(departmentID);
            }
        }
    }
}
