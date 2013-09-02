using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class SysParameterDal
    {
        public void SaveParameter(Dictionary<string, string> parameters)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                try
                {
                    SysSystemParameterDao parameterDao = new SysSystemParameterDao();
                    pm.BeginTransaction();
                    foreach (string key in parameters.Keys)
                    {
                        parameterDao.UpdateEntity(key, parameters[key]);
                    }
                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                }
            }
        }

        public DataTable GetAll()
        {
            using (PersistentManager pm = new PersistentManager())
            {                
                SysSystemParameterDao parameterDao = new SysSystemParameterDao();
                return parameterDao.FindAll();
            }
        }

        public DataTable GetFormatParameter()
        {
            using (PersistentManager pm = new PersistentManager())
            {
                SysSystemParameterDao parameterDao = new SysSystemParameterDao();
                return parameterDao.FindFormatParameter();
            }
        }

        public DataSet GetAllOptionParameter()
        {
            
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysSystemParameterDao parameterDao = new SysSystemParameterDao();
                return parameterDao.FindAllOptionParameter();
            }
        }

        public DataSet GetSystemParameterName()
        {
            
            using (PersistentManager persistentManager = new PersistentManager())
            {
                SysSystemParameterDao parameterDao = new SysSystemParameterDao();
                return parameterDao.FindSystemParameterName();
            }
        }
    }
}
