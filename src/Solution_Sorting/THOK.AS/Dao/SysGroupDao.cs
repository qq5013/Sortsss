using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class SysGroupDao : BaseDao
    {
        public DataTable FindAll()
        {
            string sql = "SELECT * FROM V_SYS_GROUPLIST ORDER BY GROUPID";
            return ExecuteQuery(sql).Tables[0];
        }

        public int FindCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM V_SYS_GROUPLIST " + where;
            return (int)ExecuteScalar(sql);
        }

        public DataSet FindAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT * FROM V_SYS_GROUPLIST " + where;
            return ExecuteQuery(sql, "V_SYS_GROUPLIST", startRecord, pageSize);
        }

        public int FindGroupMemberCount(int GroupID)
        {
            string sql=string.Format("SELECT COUNT(*) FROM SYS_USERLIST WHERE GROUPID={0}",GroupID);
            return (int)ExecuteScalar(sql);
        }

        public DataSet FindSystemModules()
        {
            string sql = "SELECT DISTINCT(MODULECODE) ,MENUTITLE FROM SYS_MODULELIST  M LEFT JOIN SYS_MENU N  ON M.MODULECODE=N.MENUCODE ORDER BY MODULECODE";
            return ExecuteQuery(sql);
        }

        public DataSet FindSystemSubModules()
        {
            string sql = "SELECT DISTINCT(N.ID),MENUTITLE,M.SUBMODULECODE,MENUPARENT,MODULECODE FROM SYS_MODULELIST M LEFT JOIN SYS_MENU N ON M.SUBMODULECODE=N.MENUCODE ORDER BY N.ID";
            return ExecuteQuery(sql);
        }

        public DataSet FindSystemOperation()
        {
            string sql = "SELECT SUBMODULENAME,SUBMODULECODE,OPERATORDESCRIPTION,MODULEID FROM SYS_MODULELIST ORDER BY SUBMODULECODE";
            return ExecuteQuery(sql);
        }

        public DataSet FindGroupOperation(int groupID)
        {
            string sql = "SELECT MODULEID FROM SYS_GROUPOPERATIONLIST WHERE GROUPID=" + groupID;
            return ExecuteQuery(sql);
        }

        public void DeleteGroupOperation(int groupID)
        {
            string sql = string.Format("DELETE FROM SYS_GROUPOPERATIONLIST WHERE GROUPID={0}", groupID);
            ExecuteNonQuery(sql);
        }

        public void InsertGroupOperation(int groupID, string moduleID)
        {
            string sql = string.Format("INSERT INTO SYS_GROUPOPERATIONLIST (GROUPID,MODULEID) VALUES ('{0}','{1}')", groupID, moduleID);
            ExecuteNonQuery(sql);
        }

        public DataSet FindRole(int groupID)
        {
            string sql = "select distinct(SubModuleCode),m.ModuleID,OperatorCode,n.MenuCode,n.MenuTitle,n.MenuParent,n.MenuUrl,n.MenuImage,n2.MenuImage as ParentImage" +
                               " from sys_GroupOperationList o  " +
                               " left join sys_ModuleList  m on m.ModuleID=o.ModuleID " +
                               " left join sys_GroupList g on g.GroupID=o.GroupID " +
                               " left join sys_UserLIst  u on u.GroupID=g.GroupID " +
                               " left join sys_Menu      n on n.MenuCode=m.SubModuleCode" +
                               " left join sys_Menu      n2 on n2.MenuCode=substring(n.MenuCode,1,8)" +
                               " where g.GroupID=" + groupID + " order by SubModuleCode ";
            return ExecuteQuery(sql);
        }

        public void InsertEntity(DataSet dataSet)
        {
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (dataRow.RowState == DataRowState.Added)
                {
                    SqlCreate sqlCreate = new SqlCreate("sys_GroupList", SqlType.INSERT);
                    sqlCreate.AppendQuote("GroupName", dataRow["GroupName"]);
                    sqlCreate.AppendQuote("Memo", dataRow["Memo"].ToString().Replace("\'", "\''"));
                    ExecuteNonQuery(sqlCreate.GetSQL());
                }
            }
        }

        public void UpdateEntity(DataSet dataSet)
        {
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (dataRow.RowState == DataRowState.Modified)
                {
                    string sqlUpdate = string.Format("update sys_GroupList set GroupName='{0}',Memo='{1}' where GroupID={2}"
                        , dataRow["GroupName"].ToString(), dataRow["Memo"].ToString().Replace("\'", "\''"), dataRow["GroupID"].ToString());
                    ExecuteNonQuery(sqlUpdate);
                }
            }
        }

        public void DeleteEntity(DataSet dataSet)
        {
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (dataRow.RowState == DataRowState.Deleted)
                {
                    ExecuteNonQuery("delete sys_GroupList WHERE GroupID='" + dataRow["GroupID", DataRowVersion.Original] + "'");
                }
            }
        }

        internal string FindNavigation(string menuCode)
        {
            string sql = "SELECT MenuParent + '>>' + MenuTitle FROM sys_Menu WHERE MENUCODE = '{0}'";
            return (string)ExecuteScalar(string.Format(sql,menuCode));
        }
    }
}
