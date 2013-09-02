using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.AS.Dao;

namespace THOK.AS.Dal
{
    public class EmployeeDal
    {
        public DataTable GetAll(int pageIndex, int pageSize, string filter)
        {
            DataTable table = null;
            using (PersistentManager pm = new PersistentManager())
            {
                EmployeeDao employeeDao = new EmployeeDao();
                table = employeeDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
            return table;
        }


        public int GetCount(string filter)
        {
            int count = 0;
            using (PersistentManager pm = new PersistentManager())
            {
                EmployeeDao employeeDao = new EmployeeDao();
                count = employeeDao.FindCount(filter);
            }
            return count;
        }

        public void Save(string employeeCode, string employeeName, string departmentID, string status, string remark)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                EmployeeDao employeeDao = new EmployeeDao();
                employeeDao.UpdateEntity(employeeCode, employeeName, departmentID, status, remark);
            }
        }

        public void Insert(string employeeCode, string employeeName, string departmentID, string status, string remark)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                EmployeeDao employeeDao = new EmployeeDao();
                employeeDao.InsertEntity(employeeCode, employeeName, departmentID, status, remark);
            }
        }

        public void Delete(string employeeCode)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                EmployeeDao employeeDao = new EmployeeDao();
                employeeDao.DeleteEntity(employeeCode);
            }
        }
    }
}
