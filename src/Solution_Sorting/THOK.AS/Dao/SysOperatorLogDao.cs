using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.AS.Dao
{
    public class SysOperatorLogDao:BaseDao
    {
        public void DeleteEntity(DataSet dataSet)
        {
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (dataRow.RowState == DataRowState.Deleted)
                {
                    ExecuteNonQuery("DELETE sys_OperatorLog WHERE OperatorLogID='" + dataRow["OperatorLogID", DataRowVersion.Original] + "'");
                }
            }
        }

        public void Insert(DateTime operateTime, string OperateUser, string moduleName, string executeOperation)
        {
            string sql = string.Format("insert into sys_OperatorLog ([LoginUser],[LoginTime],[LoginModule],[ExecuteOperator]) values ('{0}','{1}','{2}','{3}')",OperateUser,operateTime.ToString(),moduleName,executeOperation);
            ExecuteNonQuery(sql);
        }

        public void SetData(string sql)
        {
            ExecuteNonQuery(sql);
        }
    }
}
