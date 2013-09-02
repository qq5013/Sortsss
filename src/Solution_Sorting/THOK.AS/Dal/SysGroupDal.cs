using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class SysGroupDal
    {
        public DataSet GetAll(int pageIndex, int pageSize, string filter)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                return groupDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
        }

        public DataTable GetAll()
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                return groupDao.FindAll();
            }
        }

        public int GetCount(string filter)
        {
            using (PersistentManager pm = new PersistentManager())
            {
               SysGroupDao groupDao = new SysGroupDao();
                return groupDao.FindCount(filter);
            }
        }

        public int GetGroupMemberCount(int groupID)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                return groupDao.FindGroupMemberCount(groupID);
            }
        }

        public bool Insert(DataSet dataSet)
        {
            bool flag = false;
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao GroupDao = new SysGroupDao();
                GroupDao.InsertEntity(dataSet);
                flag = true;
            }
            return flag;
        }

        public bool Update(DataSet dataSet)
        {
            bool flag = false;
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao GroupDao = new SysGroupDao();
                GroupDao.UpdateEntity(dataSet);
                flag = true;
            }
            return flag;
        }

        public bool Delete(DataSet dataSet)
        {
            bool flag = false;
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao GroupDao = new SysGroupDao();
                GroupDao.DeleteEntity(dataSet);
                flag = true;
            }
            return flag;
        }

        public DataSet GetSystemModules()
        {
            
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                return groupDao.FindSystemModules();
            }
        }

        public DataSet GetSystemSubModules()
        {
            
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                return groupDao.FindSystemSubModules();
            }
        }

        public DataSet GetSystemOperations()
        {
            
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao groupDal = new SysGroupDao();
                return groupDal.FindSystemOperation();
            }
        }

        public DataSet GetGroupOperation(int groupID)
        {
            
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                return groupDao.FindGroupOperation(groupID);
            }
        }

        public void DeleteGroupOperation(int groupID)
        {

            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                groupDao.DeleteGroupOperation(groupID);
            }
        }

        public void InsertGroupOperation(int groupID, string moduleID)
        {

            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                groupDao.InsertGroupOperation(groupID, moduleID);
            }
        }

        public DataSet GetGroupRole(int groupID)
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                return groupDao.FindRole(groupID);
            }
        }

        public string GetNavigation(string menuCode)
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysGroupDao groupDao = new SysGroupDao();
                return groupDao.FindNavigation(menuCode);
            }
        }
    }
}
