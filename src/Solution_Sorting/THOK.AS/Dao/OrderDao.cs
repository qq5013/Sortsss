using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
namespace THOK.AS.Dao
{
    public class OrderDao : BaseDao
    {
        public DataTable FindMasterAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT A.*,B.ROUTENAME,C.CUSTOMERNAME FROM AS_I_ORDERMASTER A " +
                "LEFT JOIN AS_BI_ROUTE B ON A.ROUTECODE=B.ROUTECODE " +
                "LEFT JOIN AS_BI_CUSTOMER C ON A.CUSTOMERCODE=C.CUSTOMERCODE " + where;
            sql += " ORDER BY ORDERDATE,BATCHNO, ORDERID";
            return ExecuteQuery(sql, "AS_I_ORDERMASTER", startRecord, pageSize).Tables[0];
        }

        public int FindMasterCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM AS_I_ORDERMASTER A ";
            sql += where;
            return (int)ExecuteScalar(sql);
        }

        public DataTable FindDetailAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT *,QUANTITY/50 JQUANTITY,QUANTITY%50 TQUANTITY FROM AS_I_ORDERDETAIL " + where;
            sql += " ORDER BY QUANTITY DESC";
            return ExecuteQuery(sql, "AS_I_ORDERDETAIL", startRecord, pageSize).Tables[0];
        }

        public int FindDetailCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM AS_I_ORDERDETAIL ";
            sql += where;
            return (int)ExecuteScalar(sql);
        }


        public DataTable FindRouteAll(int startRecord, int pageSize, string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);
            string sql = "SELECT A.*, C.AREANAME, D.ROUTENAME, CASE WHEN B.QUANTITY % 25 = 0 THEN B.QUANTITY / 25 ELSE B.QUANTITY / 25 + 1 END QUANTITY " +
                "FROM AS_I_ORDERMASTER A LEFT JOIN (SELECT ORDERID, SUM(QUANTITY) QUANTITY FROM AS_I_ORDERDETAIL WHERE CIGARETTECODE NOT IN " +
                "(SELECT CIGARETTECODE FROM AS_BI_CIGARETTE WHERE ISABNORMITY='1') GROUP BY ORDERID) B ON A.ORDERID=B.ORDERID " +
                "LEFT JOIN AS_BI_AREA C ON A.AREACODE=C.AREACODE LEFT JOIN AS_BI_ROUTE D ON A.ROUTECODE=D.ROUTECODE ";
            sql += where;
            sql += " ORDER BY ORDERDATE,BATCHNO,AREACODE,ROUTECODE,SORTID";
            return ExecuteQuery(sql, "AS_BI_ORDER", startRecord, pageSize).Tables[0];
        }

        public int FindRouteCount(string filter)
        {
            string where = " ";
            if (filter != null)
                where += (" WHERE " + filter);

            string sql = "SELECT COUNT(*) FROM (SELECT A.*, C.AREANAME, D.ROUTENAME, CASE WHEN B.QUANTITY % 25 = 0 THEN B.QUANTITY / 25 ELSE B.QUANTITY / 25 + 1 END QUANTITY "+
                "FROM AS_I_ORDERMASTER A LEFT JOIN (SELECT ORDERID, SUM(QUANTITY) QUANTITY FROM AS_I_ORDERDETAIL WHERE CIGARETTECODE NOT IN " +
                "(SELECT CIGARETTECODE FROM AS_BI_CIGARETTE WHERE ISABNORMITY='1') GROUP BY ORDERID) B ON A.ORDERID=B.ORDERID " +
                "LEFT JOIN AS_BI_AREA C ON A.AREACODE=C.AREACODE LEFT JOIN AS_BI_ROUTE D ON A.ROUTECODE=D.ROUTECODE) E "; 
            sql += where;
            return (int)ExecuteScalar(sql);
        }
        
        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public DataSet FindRouteQuantity(string orderDate, int batchNo)
        {
            //排除异型烟
            string sql = "SELECT ROUTECODE, SORTID, SUM(QUANTITY) QUANTITY ,LINECODE"+
                            " FROM (SELECT A.ROUTECODE, D.SORTID, B.QUANTITY ,C.LINECODE"+
                            " 		FROM AS_I_ORDERMASTER A "+
                            " 		LEFT JOIN AS_I_ORDERDETAIL B "+
                            " 			ON A.ORDERID = B.ORDERID "+
                            " 		LEFT JOIN AS_BI_ROUTE C "+
                            " 			ON A.ROUTECODE = C.ROUTECODE AND A.AREACODE=C.AREACODE "+
                            " 		LEFT JOIN AS_BI_AREA D" +
                            " 			ON A.AREACODE=D.AREACODE " +
                            " 		WHERE A.ORDERDATE = '{0}' AND A.BATCHNO = {1} "+
                            " 		AND CIGARETTECODE NOT IN "+
                            " 		(SELECT CIGARETTECODE "+
                            " 			FROM AS_BI_CIGARETTE "+
                            " 			WHERE ISABNORMITY = '1')) D "+
                            " GROUP BY ROUTECODE, SORTID,LINECODE ORDER BY SORTID,QUANTITY DESC";
            return ExecuteQuery(string.Format(sql, orderDate, batchNo));
        }

        /// <summary>
        /// 分拣烟道优化时，取卷烟名称及数量，进行优化。[ZENG]
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="lineCode"></param>
        /// <returns></returns>
        public DataSet FindCigaretteQuantity(string orderDate, int batchNo, string lineCode)
        {
            //排除异型烟
            string sql = "SELECT A.CIGARETTECODE, B.CIGARETTENAME, " +
                            " SUM(A.QUANTITY) QUANTITY, SUM(A.QUANTITY -A.QUANTITY % 5) QUANTITY5" +
                            " FROM AS_I_ORDERDETAIL A" +
                            " LEFT JOIN AS_BI_CIGARETTE B ON A.CIGARETTECODE = B.CIGARETTECODE" +
                            " LEFT JOIN AS_I_ORDERMASTER C ON A.ORDERID = C.ORDERID" +
                            " LEFT JOIN AS_SC_LINE D ON A.ORDERDATE = D.ORDERDATE AND C.BATCHNO = D.BATCHNO AND C.ROUTECODE = D.ROUTECODE" +
                            " WHERE B.ISABNORMITY != '1' AND A.ORDERDATE='{0}' AND C.BATCHNO = '{1}' AND D.LINECODE = '{2}'" +
                            " GROUP BY A.CIGARETTECODE, B.CIGARETTENAME " +
                            " ORDER BY SUM(A.QUANTITY) DESC";

            return ExecuteQuery(string.Format(sql, orderDate, batchNo, lineCode));
        }

        /// <summary>
        /// 补货烟道优化时，取卷烟名称及数量，进行优化。[ZENG]
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public DataTable FindCigaretteQuantity(string orderDate, int batchNo)
        {
            //排除异型烟
            string sql = "SELECT A.CIGARETTECODE,B.CIGARETTENAME, SUM(QUANTITY) QUANTITY" +
                            " FROM AS_I_ORDERDETAIL A" +
                            " LEFT JOIN AS_BI_CIGARETTE B ON A.CIGARETTECODE =B.CIGARETTECODE" +
                            " LEFT JOIN AS_I_ORDERMASTER C ON A.ORDERID = C.ORDERID" +
                            " WHERE B.ISABNORMITY != '1' AND A.ORDERDATE='{0}' AND C.BATCHNO = '{1}' " +
                            " GROUP BY A.CIGARETTECODE, B.CIGARETTENAME HAVING SUM(QUANTITY) >= 50 " +
                            " ORDER BY SUM(QUANTITY) DESC";

            return ExecuteQuery(string.Format(sql, orderDate, batchNo)).Tables[0];
        }

        /// <summary>
        /// 分拣烟道优化时，取卷烟名称及数量，进行优化。[ZENG 2010-11-19]
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public DataSet FindAllCigaretteQuantity(string orderDate, int batchNo,bool isUseWholePiecesSortLine)
        {
            //排除异型烟
            string sql = "SELECT CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY) QUANTITY, SUM(QUANTITY - QUANTITY % 5) QUANTITY5 " +
                "FROM AS_I_ORDERDETAIL WHERE " +
                "CIGARETTECODE NOT IN (SELECT CIGARETTECODE FROM AS_BI_CIGARETTE WHERE ISABNORMITY = '1') AND " +
                "ORDERID IN ( SELECT ORDERID FROM AS_I_ORDERMASTER A WHERE A.ORDERDATE='{0}' AND A.BATCHNO = '{1}' )  " +
                "GROUP BY CIGARETTECODE, CIGARETTENAME ORDER BY SUM(QUANTITY) DESC";
            if (isUseWholePiecesSortLine)
            {
                sql = @"SELECT CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY) QUANTITY, SUM(QUANTITY - QUANTITY % 5) QUANTITY5
                            FROM  (
                            SELECT ORDERID,CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY)%50 QUANTITY, SUM(QUANTITY - QUANTITY % 5) QUANTITY5 
                            FROM AS_I_ORDERDETAIL 
                            WHERE CIGARETTECODE NOT IN (SELECT CIGARETTECODE FROM AS_BI_CIGARETTE WHERE ISABNORMITY = '1') 
                            AND ORDERID IN ( SELECT ORDERID FROM AS_I_ORDERMASTER A WHERE A.ORDERDATE='{0}' AND A.BATCHNO = '{1}' )  
                            GROUP BY ORDERID,CIGARETTECODE, CIGARETTENAME HAVING SUM(QUANTITY)%50  > 0 ) Q
                            GROUP BY CIGARETTECODE, CIGARETTENAME
                            ORDER BY SUM(QUANTITY) DESC";
            }
            return ExecuteQuery(string.Format(sql, orderDate, batchNo));
        }

        /// <summary>
        /// 分拣烟道优化时，取卷烟名称及数量，进行优化。[ZENG 2010-11-19]
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="lineCode"></param>
        /// <returns></returns>
        public DataSet FindLineCigaretteQuantity(string orderDate, int batchNo, string lineCode, bool isUseWholePiecesSortLine)
        {
            //排除异型烟
            string sql = "SELECT CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY) QUANTITY, SUM(QUANTITY - QUANTITY % 5) QUANTITY5 " +
                "FROM AS_I_ORDERDETAIL WHERE " +
                "CIGARETTECODE NOT IN (SELECT CIGARETTECODE FROM AS_BI_CIGARETTE WHERE ISABNORMITY = '1') AND " +
                "ORDERID IN ( SELECT ORDERID FROM AS_I_ORDERMASTER A LEFT JOIN AS_SC_LINE B ON " +
                "A.ROUTECODE = B.ROUTECODE AND A.ORDERDATE = B.ORDERDATE AND A.BATCHNO = B.BATCHNO " +
                "WHERE A.ORDERDATE='{0}' AND A.BATCHNO = '{1}' AND B.LINECODE = '{2}' )  " +
                "GROUP BY CIGARETTECODE, CIGARETTENAME ORDER BY SUM(QUANTITY) DESC";

            if (isUseWholePiecesSortLine)
            {
                sql = @"SELECT CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY) QUANTITY, SUM(QUANTITY - QUANTITY % 5) QUANTITY5
                            FROM  (
                            SELECT ORDERID,CIGARETTECODE, CIGARETTENAME, SUM(QUANTITY)%50 QUANTITY, SUM(QUANTITY - QUANTITY % 5) QUANTITY5 
                            FROM AS_I_ORDERDETAIL 
                            WHERE CIGARETTECODE NOT IN (SELECT CIGARETTECODE FROM AS_BI_CIGARETTE WHERE ISABNORMITY = '1') 
                            AND ORDERID IN ( SELECT ORDERID FROM AS_I_ORDERMASTER A 
                            LEFT JOIN AS_SC_LINE B ON A.ROUTECODE = B.ROUTECODE AND A.ORDERDATE = B.ORDERDATE AND A.BATCHNO = B.BATCHNO
                            WHERE A.ORDERDATE='{0}' AND A.BATCHNO = '{1}' AND B.LINECODE = '{2}' )  
                            GROUP BY ORDERID,CIGARETTECODE, CIGARETTENAME HAVING SUM(QUANTITY)%50  > 0 ) Q
                            GROUP BY CIGARETTECODE, CIGARETTENAME
                            ORDER BY SUM(QUANTITY) DESC";
            }

            return ExecuteQuery(string.Format(sql, orderDate, batchNo, lineCode));
        }

        /// <summary>
        /// 备货烟道优化，取卷烟名称及数量，进行优化。[ZENG 2010-11-19]
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="channelType"></param>
        /// <returns></returns>
        public DataTable FindCigaretteQuantityFromChannelUsed(string orderDate, int batchNo, string channelType)
        {
            string sql = string.Format("SELECT CIGARETTECODE,CIGARETTENAME,SUM(QUANTITY) QUANTITY ,MAX(CHANNELORDER) CHANNELORDER"+
                                            " FROM AS_SC_CHANNELUSED " +
                                            " WHERE ORDERDATE='{0}' AND BATCHNO={1} AND CHANNELTYPE='{2}' AND CIGARETTECODE != '' AND QUANTITY >= 50 " +
                                            " GROUP BY CIGARETTECODE,CIGARETTENAME ORDER BY SUM(QUANTITY) DESC,CHANNELORDER ",
                                            orderDate, batchNo, channelType);
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 2010-11-21 todo
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="lineCode"></param>
        /// <returns></returns>
        public DataSet FindOrderMaster(string orderDate, int batchNo, string lineCode)
        {
            string sql = "SELECT A.ORDERDATE,A.BATCHNO, B.LINECODE, ROW_NUMBER() over (ORDER BY D.SORTID,A.ROUTECODE, E.SORTID) SORTNO, " +
                            " A.ORDERID, A.AREACODE, C.AREANAME, A.ROUTECODE, D.ROUTENAME, A.CUSTOMERCODE, E.CUSTOMERNAME,E.ADDRESS, E.LICENSENO, " +
                            " 0 TQUANTITY, 0 QUANTITY, 0 PQUANTITY, 0 PACKQUANTITY,E.SORTID ORDERNO, 1 EXPORTNO, 1 EXPORTNO1, '0', NULL " +
                            " FROM AS_I_ORDERMASTER A " +
                            " LEFT JOIN AS_SC_LINE B ON A.ROUTECODE = B.ROUTECODE  AND A.ORDERDATE = B.ORDERDATE AND A.BATCHNO = B.BATCHNO " +
                            " LEFT JOIN AS_BI_AREA C ON A.AREACODE = C.AREACODE " +
                            " LEFT JOIN AS_BI_ROUTE D ON A.ROUTECODE = D.ROUTECODE " +
                            " LEFT JOIN AS_BI_CUSTOMER E ON A.CUSTOMERCODE = E.CUSTOMERCODE " +
                            " WHERE A.ORDERDATE = '{0}' AND A.BATCHNO = '{1}' AND B.LINECODE = '{2}' " + 
                            " ORDER BY SORTNO";

            sql = "SELECT A.ORDERDATE,A.BATCHNO, B.LINECODE, " + 
                    //分拣流水号
                    " ROW_NUMBER() over (ORDER BY C.SORTID,D.SORTID,A.ROUTECODE, A.SORTID) SORTNO, " +

                    " A.ORDERID, A.AREACODE, C.AREANAME, A.ROUTECODE, D.ROUTENAME, A.CUSTOMERCODE, E.CUSTOMERNAME,E.ADDRESS, E.LICENSENO, " +
                    " 0 TQUANTITY, 0 QUANTITY, 0 PQUANTITY, 0 PACKQUANTITY, " + 

                    //当前订单异型烟数量
                    " ISNULL((SELECT ISNULL(SUM(F.QUANTITY),0) FROM AS_I_ORDERDETAIL F " +
		                " LEFT JOIN AS_BI_CIGARETTE G ON F.CIGARETTECODE = G.CIGARETTECODE " +
		                " WHERE G.ISABNORMITY = '1' AND F.ORDERID = A.ORDERID " +
                        " GROUP BY F.ORDERID),0) ABNORMITY_QUANTITY," +

                    //配送序号（业务配送序号）
                    //" E.SORTID ORDERNO, " + 

                    //分拣订单在当前分拣线路中的分拣序号（根据业务配送序号分拣生成的连续顺序号）
                    " (SELECT ORDERNO FROM (SELECT A1.*,ROW_NUMBER() over (ORDER BY A1.SORTID) ORDERNO FROM AS_I_ORDERMASTER A1 " +
                        " LEFT JOIN AS_BI_CUSTOMER B1 ON A1.CUSTOMERCODE = B1.CUSTOMERCODE "+
                        " WHERE A1.ROUTECODE = A.ROUTECODE AND A1.ORDERDATE = '{0}' AND A1.BATCHNO = '{1}') TEMP WHERE TEMP.ORDERID = A.ORDERID) ORDERNO," +

                    //分拣订单在全部订单中的分拣序号（根据业务配送序号分拣生成的连续顺序号）
                    " ROW_NUMBER() over (ORDER BY  C.SORTID,D.SORTID,A.ROUTECODE, A.SORTID) CUSTOMERSORTNO," +

                    //包状机号
                    " 1 EXPORTNO, 1 EXPORTNO1, '0', NULL ," +  
                    " 0 PACKNO ,0 PACKNO1,0 PACKORDERNO"+
          
                    " FROM AS_I_ORDERMASTER A " +
                    " LEFT JOIN AS_SC_LINE B ON A.ROUTECODE = B.ROUTECODE  AND A.ORDERDATE = B.ORDERDATE AND A.BATCHNO = B.BATCHNO " +
                    " LEFT JOIN AS_BI_AREA C ON A.AREACODE = C.AREACODE " +
                    " LEFT JOIN AS_BI_ROUTE D ON A.ROUTECODE = D.ROUTECODE " +
                    " LEFT JOIN AS_BI_CUSTOMER E ON A.CUSTOMERCODE = E.CUSTOMERCODE " +

                    " LEFT JOIN (SELECT I.* FROM AS_I_ORDERDETAIL I " +
                                " LEFT JOIN AS_BI_CIGARETTE J ON I.CIGARETTECODE = J.CIGARETTECODE WHERE J.ISABNORMITY NOT IN ('1')) K" +
                            " ON A.ORDERID = K.ORDERID" +
                    //条件
                    " WHERE A.ORDERDATE = '{0}' AND A.BATCHNO = '{1}' AND B.LINECODE = '{2}' " +
                    " AND A.ORDERID NOT IN (SELECT ORDERID FROM AS_HANDLE_SORT_ORDER WHERE ORDERDATE = '{0}') " +
                    //分组
                    " GROUP BY A.ORDERDATE,  A.BATCHNO, B.LINECODE,A.ORDERID, A.AREACODE,C.SORTID,C.AREANAME, A.ROUTECODE," +
                        " D.ROUTENAME, A.CUSTOMERCODE, E.CUSTOMERNAME,E.ADDRESS, E.LICENSENO,D.SORTID,A.ROUTECODE, A.SORTID " +
                    //条件
                    " HAVING ISNULL(SUM(K.QUANTITY),0) > 0 " +
                    //排序
                    " ORDER BY SORTNO";
            sql = string.Format(sql,orderDate, batchNo, lineCode);
            return ExecuteQuery(sql);
        }

        /// <summary>
        /// 2010-11-21
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="lineCode"></param>
        /// <returns></returns>
        public DataSet FindOrderDetail(string orderDate, int batchNo, string lineCode)
        {
            string sql = "SELECT A.*, B.CHANNELCODE " +
                            " FROM (SELECT * FROM AS_I_ORDERDETAIL " +
                                    " WHERE CIGARETTECODE NOT IN (SELECT CIGARETTECODE FROM AS_BI_CIGARETTE WHERE ISABNORMITY = '1') " +
                                    " AND ORDERID IN (SELECT ORDERID FROM AS_I_ORDERMASTER " +
                                                      " WHERE ORDERDATE = '{0}' AND BATCHNO = '{1}' " +
                                                      " AND ROUTECODE IN (SELECT ROUTECODE FROM AS_SC_LINE " +
                                                                          " WHERE ORDERDATE = '{2}' AND BATCHNO = '{3}' AND LINECODE = '{4}'))) A " +
                            " LEFT JOIN (SELECT CIGARETTECODE,MIN(CHANNELCODE) CHANNELCODE " +
                                        " FROM AS_SC_CHANNELUSED " +
                                        " WHERE LINECODE='{4}' AND ORDERDATE = '{0}' AND BATCHNO = {1} " +
                                        " GROUP BY CIGARETTECODE) B " +
                            " ON A.CIGARETTECODE = B.CIGARETTECODE " +
                            " WHERE A.ORDERID NOT IN (SELECT ORDERID FROM AS_HANDLE_SORT_ORDER WHERE ORDERDATE = '{0}')" +
                            " ORDER BY ORDERID,CHANNELCODE";
            sql = string.Format(sql, orderDate, batchNo, orderDate, batchNo, lineCode);
            return ExecuteQuery(sql); 
        }

        public DataTable FindOrderRoute(string orderDate, int batchNo)
        {
            string sql = string.Format("SELECT B.*,C.AREANAME " + 
                " FROM (SELECT DISTINCT ROUTECODE FROM AS_I_ORDERMASTER WHERE ORDERDATE='{0}' AND BATCHNO='{1}') A " +
                " LEFT JOIN AS_BI_ROUTE B ON A.ROUTECODE=B.ROUTECODE " + 
                " LEFT JOIN AS_BI_AREA C ON B.AREACODE=C.AREACODE " +
                " ORDER BY B.AREACODE,B.ROUTECODE", orderDate, batchNo);
            return ExecuteQuery(sql).Tables[0];
        }

        public void BatchInsertMaster(DataTable dtData)
        {
            BatchInsert(dtData, "AS_I_ORDERMASTER");
        }

        public void BatchInsertDetail(DataTable dtData)
        {
            BatchInsert(dtData, "AS_I_ORDERDETAIL");
        }

        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <param name="orderDate"></param>
        public void DeleteHistory(string orderDate)
        {
            string sql = string.Format("DELETE FROM AS_I_ORDERDETAIL WHERE ORDERDATE < '{0}'", orderDate);
            ExecuteNonQuery(sql);

            sql = string.Format("DELETE FROM AS_I_ORDERMASTER WHERE ORDERDATE < '{0}'", orderDate);
            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        public void DeleteOrder(string orderDate, int batchNo)
        {
            string sql = string.Format("DELETE FROM AS_I_ORDERDETAIL WHERE ORDERID IN (SELECT ORDERID FROM AS_I_ORDERMASTER WHERE ORDERDATE = '{0}' AND BATCHNO={1})", orderDate, batchNo);
            ExecuteNonQuery(sql);

            sql = string.Format("DELETE FROM AS_I_ORDERMASTER WHERE ORDERDATE = '{0}' AND BATCHNO = {1}", orderDate, batchNo);
            ExecuteNonQuery(sql);
        }

        public void DeleteNoUseOrder(string orderDate, int batchNo, string routes)
        {
            string sql = "DELETE FROM AS_I_ORDERDETAIL WHERE ORDERDATE='{0}' AND BATCHNO={1} AND ORDERID NOT IN " +
                          "(SELECT ORDERID FROM AS_I_ORDERMASTER WHERE ORDERDATE='{0}' AND BATCHNO={1} AND ROUTECODE IN ({2}))";
            ExecuteNonQuery(string.Format(sql,orderDate, batchNo, routes));
            sql = "DELETE FROM AS_I_ORDERMASTER WHERE ORDERDATE='{0}' AND BATCHNO={1} AND ROUTECODE NOT IN ({2})";
            ExecuteNonQuery(string.Format(sql, orderDate, batchNo, routes));
        }

        public DataTable FindTmpMaster(string orderDate, int batchNo, string lineCode)
        {
            string sql = string.Format("SELECT * FROM AS_TMP_PALLETMASTER WHERE ORDERDATE = '{0}' AND BATCHNO = {1} AND LINECODE = '{2}'", orderDate, batchNo, lineCode);
            return ExecuteQuery(sql).Tables[0];
        }

        public DataTable FindTmpDetail(string orderDate, int batchNo, string lineCode)
        {
            string sql = string.Format("SELECT * FROM AS_TMP_ORDER WHERE ORDERDATE = '{0}' AND BATCHNO = {1} AND LINECODE = '{2}'", orderDate, batchNo, lineCode);
            return ExecuteQuery(sql).Tables[0];
        }
    }
}
