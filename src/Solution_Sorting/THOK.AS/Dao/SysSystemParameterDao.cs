using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
namespace THOK.AS.Dao
{
    public class SysSystemParameterDao : BaseDao
    {
        public DataTable FindAll()
        {
            string sql = "SELECT PARAMETERNAME,PARAMETERVALUE,PARAMETERTEXT FROM SYS_SYSTEMPARAMETER WHERE PARAMETERTYPE='0' AND STATE=1";
            return ExecuteQuery(sql).Tables[0];
        }

        public DataTable FindFormatParameter()
        {
            string sql = "SELECT PARAMETERNAME,PARAMETERVALUE,PARAMETERTEXT FROM SYS_SYSTEMPARAMETER WHERE PARAMETERTYPE='1' AND PARAMETERNAME IN('DDL_FORMATDATETIMEMODE','DDL_FORMATMONEYMODE','DDL_FORMATNUMBERMODE','SYS_SESSIONTIMEOUT')";
            return ExecuteQuery(sql).Tables[0];
        }

        public void UpdateEntity(string name, string value)
        {
            string sql = string.Format("UPDATE SYS_SYSTEMPARAMETER SET PARAMETERVALUE='{0}'  WHERE PARAMETERNAME='{1}'", value, name);
            ExecuteNonQuery(sql);
        }

        public DataSet FindAllOptionParameter()
        {
            string sql = " SELECT SYSTEMPARAMETERID , PARAMETERNAME,PARAMETERVALUE , PARAMETERTEXT, DESCRIPTION , STATE" +
                " FROM SYS_SYSTEMPARAMETER  WHERE PARAMETERTYPE='1' ORDER BY PARAMETERNAME";
            return ExecuteQuery(sql);
        }

        public DataSet FindSystemParameterName()
        {
            string sql = "SELECT DISTINCT(PARAMETERNAME),DESCRIPTION FROM SYS_SYSTEMPARAMETER WHERE PARAMETERTYPE=1";
            return ExecuteQuery(sql);
        }

    }
}
