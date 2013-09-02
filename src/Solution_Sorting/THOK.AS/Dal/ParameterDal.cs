using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.AS.Dao;
using THOK.Util;

namespace THOK.AS.Dal
{
    public class ParameterDal
    {
        public Dictionary<string, string> FindParameter()
        {
            Dictionary<string, string> d = null;
            using (PersistentManager pm = new PersistentManager())
            {
                SysParameterDao parameterDao = new SysParameterDao();
                d = parameterDao.FindParameters();
            }
            return d;
        }

        public void SaveParameter(Dictionary<string, string> parameters)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                SysParameterDao parameterDao = new SysParameterDao();
                parameterDao.UpdateEntity(parameters);
            }
        }

        /// <summary>
        /// 2011-11-21 wu
        /// </summary>
        /// <param name="parameterValue"></param>
        /// <param name="parameterName"></param>
        public void UpdateParameter(string parameterValue,string parameterName)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                SysParameterDao parameterDao = new SysParameterDao();
                parameterDao.UpdateParameter(parameterValue,parameterName);
            }
        }
    }
}
