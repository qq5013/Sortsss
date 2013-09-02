using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class SysUserDal
    {
        public DataSet GetUserList(int pageIndex, int pageSize, string filter)
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                return userDao.FindAll((pageIndex - 1) * pageSize, pageSize, filter);
            }
        }

        public DataTable GetAll()
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                return userDao.FindAll();
            }
        }

        public DataTable GetGroupUser(int groupID)
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                return userDao.FindUser(groupID.ToString());
            }
        }

        public bool DeleteUserFromGroup(int userID)
        {
            
            bool flag = false;
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                userDao.UpdateEntity(userID);
                flag = true;
            }
            return flag;
        }

        public bool AddUserToGroup(int groupID, string users)
        {
            bool flag = false;
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                userDao.UpdateEntity(groupID, users);
                flag = true;
            }
            return flag;
        }

        public DataTable GetUser(string userName)
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                return userDao.FindUserByName(userName);
                
            }
        }

        public bool ChangePassword(string userName, string password)
        {
            bool flag = false;
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                userDao.UpdateEntity(userName, password);
                flag = true;
            }
            return flag;
        }

        public bool Insert(DataSet dataSet)
        {
            bool flag = false;
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao UserDao = new SysUserDao();
                UserDao.InsertEntity(dataSet);
                flag = true;
            }
            return flag;
        }

        public bool Update(DataSet dataSet)
        {
            bool flag = false;
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao UserDao = new SysUserDao();
                UserDao.UpdateEntity(dataSet);
                flag = true;
            }
            return flag;
        }

        public bool Delete(DataSet dataSet)
        {
            bool flag = false;
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao UserDao = new SysUserDao();
                UserDao.DeleteEntity(dataSet);
                flag = true;
            }
            return flag;
        }

        public int GetRowCount(string filter)
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                return userDao.GetRowCount(filter);
            }
        }

        public DataSet GetUserOperateModule(string userName)
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                return userDao.FindUserOperateModule(userName);
            }
        }

        public DataSet GetUserOperateSubModule(string userName)
        {
            
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                return userDao.FindUserOperateSubModule(userName);
            }
        }

        public DataSet GetUserQuickDesktop(int userID)
        {
            
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                return userDao.FindUserQuickDesktop(userID);
            }
        }

        public void DeleteUserQuickDesktop(string userID)
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                userDao.DeleteUserQuickDesktop(userID);
            }
        }

        public void InsertUserQuickDesktop(string userID, string moduleID)
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysUserDao userDao = new SysUserDao();
                userDao.InsertUserQuickDesktop(userID, moduleID);
            }
        }
    }
}
