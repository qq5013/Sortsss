using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class SysUserDao : BaseDao
    {
        public DataSet FindAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT * FROM V_SYS_USERLIST " + where + " ORDER BY USERNAME";
            return ExecuteQuery(sql, "V_SYS_USERLIST", startRecord, pageSize);
        }

        public DataTable FindAll()
        {
            string sql = "SELECT USERNAME ,GROUPNAME ,USERID FROM SYS_USERLIST U LEFT JOIN SYS_GROUPLIST G ON U.GROUPID=G.GROUPID ";
            return ExecuteQuery(sql).Tables[0];
        }

        public DataTable FindUser(string groupID)
        {
            string sql = "SELECT USERID, USERNAME ,GROUPNAME  FROM SYS_USERLIST U LEFT JOIN SYS_GROUPLIST G ON U.GROUPID=G.GROUPID WHERE U.GROUPID=" + groupID;
            return ExecuteQuery(sql).Tables[0];
        }

        public void UpdateEntity(int userID)
        {
            string sql = "UPDATE  SYS_USERLIST SET GROUPID='' WHERE USERID=" + userID.ToString();
            ExecuteNonQuery(sql);
        }

        public void UpdateEntity(int groupID, string users)
        {
            string sql = string.Format("UPDATE SYS_USERLIST SET GROUPID={0} WHERE USERID IN ({1})", groupID, users);
            ExecuteNonQuery(sql);
        }

        public void UpdateEntity(string userName, string password)
        {
            string sql = string.Format("UPDATE SYS_USERLIST SET USERPASSWORD='{0}' WHERE USERNAME='{1}'", password, userName);
            ExecuteNonQuery(sql);
        }

        public DataTable FindUserByName(string userName)
        {
            string sql = string.Format("SELECT A.*," +
                           " D.SYS_PAGECOUNT,D.GRID_COLUMNTITLEFONT,D.GRID_CONTENTFONT," +
                           " D.GRID_COLUMNTEXTALIGN,D.GRID_CONTENTTEXTALIGN,D.GRID_NUMBERCOLUMNALIGN," +
                           " D.GRID_MONEYCOLUMNALIGN,D.GRID_SELECTMODE,D.GRID_ODDROWCOLOR," +
                           " D.GRID_EVENROWCOLOR,D.GRID_ISREFRESHBEFOREADD,D.GRID_ISREFRESHBEFOREUPDATE," +
                           " D.GRID_ISREFRESHBEFOREDELETE,D.SYS_PRINTFORM,D.PAGER_SHOWPAGEINDEX," +
                           " E.PARAMETERVALUE AS SESSIONTIMEOUT" +
                           " FROM SYS_USERLIST A" +
                           " LEFT JOIN SYS_USERCONFIGPLAN B ON A.USERID=B.USERID " +
                           " LEFT JOIN SYS_CONFIGPLAN C ON B.CONFIGPLANID=C.CONFIGPLANID " +
                           " LEFT JOIN SYS_USERSYSINFO D ON A.USERID=D.USERID " +
                           " LEFT JOIN SYS_SYSTEMPARAMETER E ON E.PARAMETERNAME='SYS_SESSIONTIMEOUT' " +
                           " WHERE USERNAME='{0}'", userName);
            return ExecuteQuery(sql).Tables[0];
        }

        public void InsertEntity(DataSet dataSet)
        {
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (dataRow.RowState == DataRowState.Added)
                {
                    SqlCreate sqlCreate = new SqlCreate("SYS_USERLIST", SqlType.INSERT);
                    sqlCreate.AppendQuote("USERNAME", dataRow["USERNAME"]);
                    sqlCreate.AppendQuote("USERPASSWORD", dataRow["USERPASSWORD"]);
                    sqlCreate.AppendQuote("EMPLOYEECODE", dataRow["EMPLOYEECODE"]);
                    sqlCreate.AppendQuote("MEMO", dataRow["MEMO"].ToString().Replace("\'", "\''"));
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
                    string sqlUpdate = string.Format("UPDATE SYS_USERLIST SET USERNAME='{0}',EMPLOYEECODE='{1}',MEMO='{2}' WHERE USERID={3}"
                        , dataRow["USERNAME"], dataRow["EMPLOYEECODE"], dataRow["MEMO"].ToString().Replace("\'", "\''"), dataRow["USERID"]);
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
                    ExecuteNonQuery("DELETE SYS_USERLIST WHERE USERID='" + dataRow["USERID", DataRowVersion.Original] + "'");
                }
            }
        }

        public int GetRowCount(string filter)
        {
            string sql = string.Format("SELECT COUNT(*) FROM V_SYS_USERLIST WHERE {0} ", filter);
            return (int)ExecuteScalar(sql);
        }

        public DataSet FindUserOperateModule(string userName)
        {
            string sql = "SELECT DISTINCT(N2.MENUTITLE),N2.ID " +
                           " FROM SYS_GROUPOPERATIONLIST O   " +
                           " LEFT JOIN SYS_MODULELIST  M ON M.MODULEID=O.MODULEID " +
                           " LEFT JOIN SYS_GROUPLIST G ON G.GROUPID=O.GROUPID " +
                           " LEFT JOIN SYS_USERLIST  U ON U.GROUPID=G.GROUPID " +
                           " LEFT JOIN SYS_MENU      N ON N.MENUCODE=M.SUBMODULECODE " +
                           " LEFT JOIN SYS_MENU      N2 ON N2.MENUCODE=SUBSTRING(N.MENUCODE,1,8) " +
                            " WHERE USERNAME='" + userName + "'ORDER BY N2.MENUTITLE";
            return ExecuteQuery(sql);
        }

        public DataSet FindUserOperateSubModule(string userName)
        {
            string sql = "SELECT DISTINCT(M.SUBMODULECODE),N.MENUPARENT,N.MENUTITLE,N.ID " +
                           " FROM SYS_GROUPOPERATIONLIST O   " +
                           " LEFT JOIN SYS_MODULELIST  M ON M.MODULEID=O.MODULEID " +
                           " LEFT JOIN SYS_GROUPLIST G ON G.GROUPID=O.GROUPID " +
                           " LEFT JOIN SYS_USERLIST  U ON U.GROUPID=G.GROUPID " +
                           " LEFT JOIN SYS_MENU      N ON N.MENUCODE=M.SUBMODULECODE " +
                           " WHERE USERNAME='" + userName + "'ORDER BY M.SUBMODULECODE ";
            return ExecuteQuery(sql);
        }

        public DataSet FindUserQuickDesktop(int userID)
        {
            string sql = "SELECT M.MENUPARENT, M.MENUTITLE, Q.MODULEID, M.MENUIMAGE, M.DESTOPIMAGE, M.MENUURL " +
                         "FROM DBO.SYS_QUICKDESTOP AS Q LEFT OUTER JOIN  DBO.SYS_MENU AS M ON Q.MODULEID = M.ID " +
                         "WHERE     (Q.USERID = " + userID + ") AND (Q.MODULEID IN " +
                         "(SELECT DISTINCT N.ID " +
                         " FROM DBO.SYS_GROUPOPERATIONLIST AS O LEFT OUTER JOIN " +
                         "DBO.SYS_MODULELIST AS M ON M.MODULEID = O.MODULEID LEFT OUTER JOIN " +
                         "DBO.SYS_GROUPLIST AS G ON G.GROUPID = O.GROUPID LEFT OUTER JOIN " +
                         "DBO.SYS_USERLIST AS U ON U.GROUPID = G.GROUPID LEFT OUTER JOIN " +
                         " DBO.SYS_MENU AS N ON N.MENUCODE = M.SUBMODULECODE " +
                         "WHERE (U.USERID = " + userID + "))) ";
            return ExecuteQuery(sql);
        }

        public void DeleteUserQuickDesktop(string userID)
        {
            string sql = string.Format("DELETE FROM SYS_QUICKDESTOP WHERE USERID={0}", userID);
            ExecuteNonQuery(sql);
        }

        public void InsertUserQuickDesktop(string userID, string moduleID)
        {
            string sql = string.Format("INSERT INTO SYS_QUICKDESTOP (USERID,MODULEID) VALUES ('{0}','{1}')", userID, moduleID);
            ExecuteNonQuery(sql);
        }
        
    }
}
