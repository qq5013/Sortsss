using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class SalesSystemDao : BaseDao
    {
        public string dbTypeName = "zybzjk-mssql";
        public SalesSystemDao()
        {
            THOK.AS.Dao.SysParameterDao parameterDao = new SysParameterDao();
            Dictionary<string, string> parameter = parameterDao.FindParameters();

            //分拣订单业务数据接口服务器数据库类型
            if (parameter["SalesSystemDBType"] != "")
                dbTypeName = parameter["SalesSystemDBType"];
        }

        /// <summary>
        /// 营销系统区域表
        /// </summary>
        /// <returns></returns>
        public DataTable FindArea()
        {
            string sql = "";

            switch (dbTypeName)
            {
                case "zybzjk-mssql":
                    sql = @"SELECT SALE_REG_CODE AS AREACODE,SALE_REG_NAME AS AREANAME,0 AS SORTID " +
                            " FROM DWV_ORG_SALE_REGION";
                    break;
                case "dyyc-db2":
                    sql = @"SELECT AREACODE, AREANAME,0 AS SORTID " +
                            " FROM OUKANG.OUKANG_REGION";
                    break;
                case "kfyc-mssql":
                    sql = @"SELECT DISTINCT COMPANY AS AREACODE,LTRIM(RTRIM(COMPNAME)) AS AREANAME, 0 AS SORTID " +
                            " FROM TC.V_DELIVEDATA";
                    break;
                case "kfyc1-mssql":
                    sql = @"SELECT XGSBM AS AREACODE,LTRIM(RTRIM(XGSMC)) AS AREANAME, 0 AS SORTID " +
                            " FROM VIWMS_SALEDEPT";
                    break;
                case "ayyc-db2":
                    sql = @"SELECT DISTINCT DIST_STA_CODE AS AREACODE,LTRIM(RTRIM(DIST_STA_NAME)) AS AREANAME, 0 AS SORTID " +
                            " FROM IC.V_WMS_DIST_STATION WHERE ISACTIVE ='1'";
                    break;
                case "ayyc-mssql":
                    sql = @"SELECT DISTINCT DIST_STA_CODE AS AREACODE,LTRIM(RTRIM(DIST_STA_NAME)) AS AREANAME, 0 AS SORTID " +
                           " FROM V_WMS_DIST_STATION WHERE ISACTIVE ='1'";
                    break;
                case "ncyc-db2":
                    sql = @"SELECT AREACODE, AREANAME,0 AS SORTID " +
                            " FROM OUKANG.OUKANG_REGION";
                    break;
                default:
                    sql = @"SELECT SALE_REG_CODE AS AREACODE,SALE_REG_NAME AS AREANAME,0 AS SORTID " +
                            " FROM DWV_ORG_SALE_REGION";
                    break;
            }

            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 营销系统线路表
        /// </summary>
        /// <returns></returns>
        public DataTable FindRoute()
        {
            string sql = "";

            switch (dbTypeName)
            {
                case "zybzjk-mssql":
                    sql = @"SELECT DELIVER_LINE_CODE AS ROUTECODE, DELIVER_LINE_NAME AS ROUTENAME, '', DELIVER_LINE_ORDER AS SORTID, '' " +
                            " FROM DWV_CAR_DELIVER_LINE";
                    break;
                case "dyyc-db2":
                    sql = @"SELECT ROUTECODE,ROUTENAME, AREACODE, SORTID " +
                            " FROM OUKANG.OUKANG_RUT";
                    break;
                case "kfyc-mssql":
                    sql = @"SELECT DISTINCT DELIROUTE AS ROUTECODE,QUICKCODE AS ROUTENAME,COMPANY AS AREACODE, 0 AS SORTID " +
                            " FROM TC.V_DELIVEDATA";
                    break;
                case "kfyc1-mssql":
                    sql = @"SELECT SHXLBM AS ROUTECODE,LTRIM(RTRIM(SHXLMC)) AS ROUTENAME,XGSBM AS AREACODE, SHXLSX AS SORTID " +
                            " FROM VIWMS_ROUTE";
                    break;
                case "ayyc-db2":
                    sql = @"SELECT DISTINCT DELIVER_LINE_CODE AS ROUTECODE,DELIVER_LINE_NAME AS ROUTENAME,DIST_STA_CODE AS AREACODE, DELIVER_LINE_ORDER AS SORTID " +
                            " FROM IC.V_WMS_DELIVER_LINE WHERE ISACTIVE = '1'";
                    break;
                case "ayyc-mssql":
                    sql = @"SELECT DISTINCT DELIVER_LINE_CODE AS ROUTECODE,DELIVER_LINE_NAME AS ROUTENAME,DIST_STA_CODE AS AREACODE, DELIVER_LINE_ORDER AS SORTID " +
                            " FROM V_WMS_DELIVER_LINE WHERE ISACTIVE = '1'";
                    break;
                case "ncyc-db2":
                    sql = @"SELECT ROUTECODE,ROUTENAME, AREACODE, SORTID " +
                            " FROM OUKANG.OUKANG_RUT";
                    break;
                default:
                    sql = @"SELECT DELIVER_LINE_CODE AS ROUTECODE, DELIVER_LINE_NAME AS ROUTENAME, '', DELIVER_LINE_ORDER AS SORTID, '' " +
                            " FROM DWV_CAR_DELIVER_LINE";
                    break;
            }
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 营销系统客户表
        /// </summary>
        /// <returns></returns>
        public DataTable FindCustomer(DateTime orderDate)
        {
            string sql = "";

            switch (dbTypeName)
            {
                case "zybzjk-mssql":
                    sql = @"SELECT CUST_CODE AS CUSTOMERCODE,CUST_NAME AS CUSTOMERNAME,DELIVER_LINE_CODE AS ROUTECODE, " +
                            "SALE_REG_CODE AS AREACODE,LICENSE_CODE AS LICENSENO,DELIVER_ORDER AS SORTID, " +
                            "PRINCIPAL_TEL AS TELNO,PRINCIPAL_ADDRESS AS ADDRESS FROM DWV_ORG_CUSTOMER";
                    break;
                case "dyyc-db2":
                    sql = @"SELECT A.CUSTOMERCODE, A.CUSTOMERNAME, A.ROUTECODE, " +
                            " B.AREACODE AS AREACODE,A.LICENSENO,A.SORTID, A.TELNO," +
                            " A.ADDRESS FROM OUKANG.OUKANG_CUST A " +
                            " LEFT JOIN OUKANG.OUKANG_RUT B ON A.ROUTECODE = B.ROUTECODE";
                    break;
                case "kfyc-mssql":
                    sql = @"SELECT CUSTOMER AS CUSTOMERCODE, CUSTNAME AS CUSTOMERNAME," +
                            " DELIROUTE AS ROUTECODE,COMPANY AS AREACODE,LICENCENUM AS LICENSENO," +
                            " MIN(DELISEQUENCE) AS SORTID,TEL AS TELNO,ADDR AS ADDRESS " +
                            " FROM TC.V_DELIVEDATA " +
                            " WHERE BIZDATE = '{0}'" +
                            " GROUP BY CUSTOMER,CUSTNAME,DELIROUTE,COMPANY,LICENCENUM,TEL,ADDR";

                    sql = string.Format(sql, orderDate.ToString("yyyyMMdd"));
                    break;
                case "kfyc1-mssql":
                    sql = @"SELECT KHBM AS CUSTOMERCODE,LTRIM(RTRIM(JYZXM)) AS CUSTOMERNAME," +
                            " SHXLBM AS ROUTECODE,XGSBM AS AREACODE,XKZH AS LICENSENO," +
                            " SHSX AS SORTID,LXDH AS TELNO,SHDZ AS ADDRESS " +
                            " FROM VIWMS_CUST";
                    break;
                case "ayyc-db2":
                    sql = @"SELECT CUST_CODE AS CUSTOMERCODE,LTRIM(RTRIM(PRINCIPAL_NAME)) AS CUSTOMERNAME," +
                            " A.DELIVER_LINE_CODE AS ROUTECODE,B.DIST_STA_CODE AS AREACODE,LICENSE_CODE AS LICENSENO," +
                            " DELIVER_ORDER AS SORTID,DIST_PHONE AS TELNO,DIST_ADDRESS AS ADDRESS,N_CUST_CODE " +
                            " FROM IC.V_WMS_CUSTOMER A" +
                            " LEFT JOIN IC.V_WMS_DELIVER_LINE B ON A.DELIVER_LINE_CODE = B.DELIVER_LINE_CODE" +
                            " WHERE A.ISACTIVE ='1'";
                    break;
                case "ayyc-mssql":
                    sql = @"SELECT CUST_CODE AS CUSTOMERCODE,LTRIM(RTRIM(PRINCIPAL_NAME)) AS CUSTOMERNAME," +
                            " A.DELIVER_LINE_CODE AS ROUTECODE,B.DIST_STA_CODE AS AREACODE,LICENSE_CODE AS LICENSENO," +
                            " DELIVER_ORDER AS SORTID,DIST_PHONE AS TELNO,DIST_ADDRESS AS ADDRESS,N_CUST_CODE " +
                            " FROM V_WMS_CUSTOMER A" +
                            " LEFT JOIN V_WMS_DELIVER_LINE B ON A.DELIVER_LINE_CODE = B.DELIVER_LINE_CODE" +
                            " WHERE A.ISACTIVE ='1'";
                    break;
                case "ncyc-db2":
                    sql = @"SELECT A.CUSTOMERCODE, A.CUSTOMERNAME, A.ROUTECODE, " +
                            " B.AREACODE AS AREACODE,A.LICENSENO,A.SORTID, A.TELNO," +
                            " A.ADDRESS,A.LICENSENO AS N_CUST_CODE  FROM OUKANG.OUKANG_CUST A " +
                            " LEFT JOIN OUKANG.OUKANG_RUT B ON A.ROUTECODE = B.ROUTECODE";
                    break;
                default:
                    sql = @"SELECT CUST_CODE AS CUSTOMERCODE,CUST_NAME AS CUSTOMERNAME,DELIVER_LINE_CODE AS ROUTECODE, " +
                            "SALE_REG_CODE AS AREACODE,LICENSE_CODE AS LICENSENO,DELIVER_ORDER AS SORTID, " +
                            "PRINCIPAL_TEL AS TELNO,PRINCIPAL_ADDRESS AS ADDRESS FROM DWV_ORG_CUSTOMER";
                    break;
            }

            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 营销系统卷烟表
        /// </summary>
        /// <returns></returns>
        public DataTable FindCigarette(DateTime orderDate)
        {
            string sql = "";

            switch (dbTypeName)
            {
                case "zybzjk-mssql":
                    sql = @"SELECT BRAND_CODE AS CIGARETTECODE,BRAND_NAME AS CIGARETTENAME," +
                            " IS_ABNORMITY_BRAND AS ISABNORMITY,RIGHT(BARCODE_PIECE,6) AS BARCODE" +
                            " FROM DWV_INF_BRAND";
                    break;
                case "dyyc-db2":
                    sql = @"SELECT CIGARETTECODE, CIGARETTENAME,ISABNORMITY,RIGHT(ltrim(rtrim(BARCODE)),6)  BARCODE " +
                            " FROM OUKANG.OUKANG_ITEM";
                    break;
                case "kfyc-mssql":
                    sql = @"SELECT DISTINCT LTRIM(RTRIM(MERCH)) AS CIGARETTECODE,LTRIM(RTRIM(MERCHNAME)) AS  CIGARETTENAME," +
                            " 0  AS ISABNORMITY," +
                            " '' AS BARCODE " +
                            " FROM TC.V_DELIVEDATA " +
                            " WHERE BIZDATE = '{0}'";
                    sql = string.Format(sql, orderDate.ToString("yyyyMMdd"));
                    break;
                case "kfyc1-mssql":
                    sql = @"SELECT LTRIM(RTRIM(SPBM)) AS CIGARETTECODE,LTRIM(RTRIM(SPMC)) AS  CIGARETTENAME," +
                            " YXY AS ISABNORMITY," +
                            " LTRIM(RTRIM(TXM_J)) BARCODE " +
                            " FROM VIWMS_MERCH";
                    break;
                case "ayyc-db2":
                    sql = @"SELECT LTRIM(RTRIM(BRAND_CODE)) AS CIGARETTECODE,LTRIM(RTRIM(BRAND_NAME)) AS  CIGARETTENAME," +
                            " IS_ABNORMITY_BRAND AS ISABNORMITY," +
                            " LTRIM(RTRIM(BARCODE_PIECE)) BARCODE " +
                            " FROM IC.V_WMS_BRAND WHERE ISACTIVE ='1'";
                    break;
                case "ayyc-mssql":
                    sql = @"SELECT LTRIM(RTRIM(BRAND_CODE)) AS CIGARETTECODE,LTRIM(RTRIM(BRAND_NAME)) AS  CIGARETTENAME," +
                            " IS_ABNORMITY_BRAND AS ISABNORMITY," +
                            " LTRIM(RTRIM(BARCODE_PIECE)) BARCODE " +
                            " FROM V_WMS_BRAND WHERE ISACTIVE ='1'";
                    break;
                case "ncyc-db2":
                    sql = @"SELECT CIGARETTECODE, CIGARETTENAME,ISABNORMITY,RIGHT(ltrim(rtrim(BARCODE)),6)  BARCODE " +
                            " FROM OUKANG.OUKANG_ITEM";
                    break;
                default:
                    sql = @"SELECT BRAND_CODE AS CIGARETTECODE,BRAND_NAME AS CIGARETTENAME," +
                            " IS_ABNORMITY_BRAND AS ISABNORMITY,RIGHT(BARCODE_PIECE,6) AS BARCODE" +
                            " FROM DWV_INF_BRAND";
                    break;
            }

            return ExecuteQuery(sql).Tables[0];
        }


        /// <summary>
        /// 营销系统订单主表,20110905 修改
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="ExistAreaRoutes"></param>
        /// <returns></returns>
        public DataTable FindOrderMaster(DateTime orderDate, int batchNo, string routes)
        {
            string sql = "";

            switch (dbTypeName)
            {
                case "zybzjk-mssql":
                    sql = @"SELECT '{0}',{1}, ORDER_ID AS ORDERID, SALE_REG_CODE AS AREACODE,DELIVER_LINE_CODE AS ROUTECODE, " +
                            " CUST_CODE AS CUSTOMERCODE,DELIVER_ORDER AS SORTID " +
                            " FROM DWV_ORD_ORDER " +
                            " WHERE ORDER_DATE LIKE '{2}%' AND DELIVER_LINE_CODE NOT IN ({3})";
                    break;
                case "dyyc-db2":
                    sql = @"SELECT '{0}', {1}, A.ORDERID AS ORDERID,A.AREACODE AS AREACODE,A.RUTCODE AS ROUTECODE," +
                            " A.CUSTOMERCODE AS CUSTOMERCODE,B.SORTID AS SORTID " +
                            " FROM OUKANG.OUKANG_CO A " +
                            " LEFT JOIN OUKANG_CUST B ON A.CUSTOMERCODE = B.CUSTOMERCODE " +
                            " WHERE A.ORDERDATE = '{2}' AND B.ROUTECODE NOT IN ({3}) " +
                            " GROUP BY ORDERID,A.CUSTOMERCODE, A.RUTCODE ,AREACODE,B.SORTID";
                    break;
                case "kfyc-mssql":
                    sql = @"SELECT DISTINCT '{0}', {1}, ORDERID AS ORDERID,'01'AS ORGCODE,COMPANY AS AREACODE," +
                            " DELIROUTE AS ROUTECODE,CUSTOMER AS CUSTOMERCODE,0 AS SORTID,COUNT(*) AS DETAILNUM,'0' AS IS_IMPORT " +
                            " FROM TC.V_DELIVEDATA " +
                            " WHERE BIZDATE = '{2}' AND DELIROUTE NOT IN ({3}) GROUP BY ORDERID,COMPANY,CUSTOMER,DELIROUTE";
                    break;
                case "kfyc1-mssql":
                    sql = @"SELECT '{0}', {1}, DDBM AS ORDERID,XGSBM AS AREACODE," +
                            " SHXLBM AS ROUTECODE,KHBM AS CUSTOMERCODE,KHSHSX AS SORTID " +
                            " FROM VIWMS_ORDERMASTER" +
                            " WHERE DDRQ = '{2}' AND SHXLBM NOT IN ({3})";
                    break;
                case "ayyc-db2":
                    sql = @"SELECT '{0}', {1}, ORDER_ID AS ORDERID,ORG_CODE AS ORGCODE,DIST_STA_CODE AS AREACODE," +
                            " DELIVER_LINE_CODE AS ROUTECODE,CUST_CODE AS CUSTOMERCODE,DELIVER_ORDER AS SORTID ,DETAIL_NUM AS DETAILNUM,'0' AS IS_IMPORT " +
                            " FROM IC.V_WMS_SORT_ORDER" +
                            " WHERE ORDER_DATE = '{2}' AND DELIVER_LINE_CODE NOT IN ({3}) AND ISACTIVE ='1' ";
                    break;
                case "ayyc-mssql":
                    sql = @"SELECT '{0}', {1}, ORDER_ID AS ORDERID,ORG_CODE AS ORGCODE,DIST_STA_CODE AS AREACODE," +
                            " DELIVER_LINE_CODE AS ROUTECODE,CUST_CODE AS CUSTOMERCODE,DELIVER_ORDER AS SORTID ,DETAIL_NUM AS DETAILNUM,'0' AS IS_IMPORT " +
                            " FROM V_WMS_SORT_ORDER" +
                            " WHERE ORDER_DATE = '{2}' AND DELIVER_LINE_CODE NOT IN ({3}) AND ISACTIVE ='1' ";
                    break;
                case "ncyc-db2":
                    sql = @"SELECT '{0}',{1}, A.ORDERID AS ORDERID,'01'AS ORGCODE,A.AREACODE AS AREACODE,A.RUTCODE AS ROUTECODE,
                             A.CUSTOMERCODE AS CUSTOMERCODE,B.SORTID AS SORTID,'0' AS DETAILNUM,'0' AS IS_IMPORT
                            FROM OUKANG.OUKANG_CO A 
                            LEFT JOIN OUKANG_CUST B ON A.CUSTOMERCODE = B.CUSTOMERCODE 
                             WHERE A.ORDERDATE = '{2}' AND B.ROUTECODE NOT IN ({3}) 
                             GROUP BY ORDERID,A.CUSTOMERCODE, A.RUTCODE ,AREACODE,B.SORTID";
                    break;
                default:
                    sql = @"SELECT '{0}',{1}, ORDER_ID AS ORDERID, SALE_REG_CODE AS AREACODE,DELIVER_LINE_CODE AS ROUTECODE, " +
                            " CUST_CODE AS CUSTOMERCODE,DELIVER_ORDER AS SORTID " +
                            " FROM DWV_ORD_ORDER WHERE ORDER_DATE LIKE '{2}%' AND DELIVER_LINE_CODE NOT IN ({3})";
                    break;
            }

            return ExecuteQuery(string.Format(sql, orderDate.ToString("yyyy-MM-dd"), batchNo, orderDate.ToString("yyyy-MM-dd"), routes)).Tables[0];
        }

        /// <summary> 
        /// 营销系统订单明细表
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        /// <param name="ExistAreaRoutes"></param>
        /// <returns></returns>
        public DataTable FindOrderDetail(DateTime orderDate, int batchNo, string routes)
        {
            string sql = "";

            switch (dbTypeName)
            {
                case "zybzjk-mssql":
                    sql = @"SELECT ORDER_ID AS ORDERID, BRAND_CODE AS CIGARETTECODE, BRAND_NAME AS CIGARETTENAME,QUANTITY,0,0,'{0}',{1}" +
                            " FROM DWV_ORD_ORDER_DETAIL WHERE ORDER_ID IN (SELECT ORDER_ID FROM DWV_ORD_ORDER WHERE ORDER_DATE LIKE '{2}%' " +
                            " AND DELIVER_LINE_CODE NOT IN ({3}))";
                    break;
                case "dyyc-db2":
                    sql = @"SELECT A.ORDERID AS ORDERID, A.CIGARETTECODE AS CIGARETTECODE, B.CIGARETTENAME AS CIGARETTENAME,A.QUANTITY,0,0,'{0}',{1}" +
                        " FROM OUKANG.OUKANG_CO A " +
                        " LEFT JOIN OUKANG.OUKANG_ITEM B ON A.CIGARETTECODE = B.CIGARETTECODE" +
                        " LEFT JOIN OUKANG_CUST C ON A.CUSTOMERCODE = C.CUSTOMERCODE " +
                        " WHERE ORDERDATE = '{2}' AND C.ROUTECODE NOT IN ({3})";
                    break;
                case "kfyc-mssql":
                    sql = @"SELECT ORDERID ||RIGHT('000'||CAST(SEQNO AS CHAR(5)),4) AS ORDERDETAILID,ORDERID AS ORDERID,LTRIM(RTRIM(MERCH)) AS CIGARETTECODE, " +
                            " LTRIM(RTRIM(MERCHNAME)) AS CIGARETTENAME,'条' AS UTINNAME,QUANTITY AS QUANTITY,0,0,'{0}',{1},REQQTY AS QTYDEMAND," +
                            " SALEPRICE AS PRICE,AMOUNT AS AMOUNT,'0' AS IS_IMPORT FROM TC.V_DELIVEDATA" +
                            " WHERE BIZDATE = '{2}' AND DELIROUTE NOT IN ({3})";
                    break;
                case "kfyc1-mssql":
                    sql = @"SELECT A.DDBM AS ORDERID,ltrim(rtrim(A.SPBM)) AS CIGARETTECODE, " +
                            " LTRIM(RTRIM(A.SPMC)) AS CIGARETTENAME,A.SL AS QUANTITY,0,0,'{0}',{1}" +
                            " FROM VIWMS_ORDERDETAIL A " +
                            " LEFT JOIN VIWMS_ORDERMASTER B ON A.DDBM = B.DDBM" +
                            " WHERE B.DDRQ = '{2}' AND B.SHXLBM NOT IN ({3})";
                    break;
                case "ayyc-db2":
                    sql = @"SELECT A.ORDER_DETAIL_ID AS ORDERDETAILID,A.ORDER_ID AS ORDERID,LTRIM(RTRIM(A.BRAND_CODE)) AS CIGARETTECODE, " +
                            " LTRIM(RTRIM(A.BRAND_NAME)) AS CIGARETTENAME,'条' AS UTINNAME,A.QUANTITY AS QUANTITY,0,0,'{0}',{1}," +
                            " QTY_DEMAND AS QTYDEMAND,PRICE AS PRICE,AMOUNT AS AMOUNT,'0' AS IS_IMPORT,A.QUANTITY AS ORDER_QUANTITY " +
                            " FROM IC.V_WMS_SORT_ORDER_DETAIL A " +
                            " LEFT JOIN IC.V_WMS_SORT_ORDER B ON A.ORDER_ID = B.ORDER_ID" +
                            " WHERE B.ORDER_DATE = '{2}' AND B.DELIVER_LINE_CODE NOT IN ({3}) AND A.QUANTITY > 0 ";
                    break;
                case "ayyc-mssql":
                    sql = @"SELECT A.ORDER_DETAIL_ID AS ORDERDETAILID,A.ORDER_ID AS ORDERID,LTRIM(RTRIM(A.BRAND_CODE)) AS CIGARETTECODE, " +
                            " LTRIM(RTRIM(A.BRAND_NAME)) AS CIGARETTENAME,'条' AS UTINNAME,A.QUANTITY AS QUANTITY,0,0,'{0}',{1}," +
                            " QTY_DEMAND AS QTYDEMAND,PRICE AS PRICE,AMOUNT AS AMOUNT,'0' AS IS_IMPORT,A.QUANTITY AS ORDER_QUANTITY " +
                            " FROM V_WMS_SORT_ORDER_DETAIL A " +
                            " LEFT JOIN V_WMS_SORT_ORDER B ON A.ORDER_ID = B.ORDER_ID" +
                            " WHERE B.ORDER_DATE = '{2}' AND B.DELIVER_LINE_CODE NOT IN ({3}) AND A.QUANTITY > 0 ";
                    break;
                case "ncyc-db2":
                    sql = @" SELECT 0 AS ORDERDETAILID, A.ORDERID AS ORDERID, A.CIGARETTECODE AS CIGARETTECODE, B.CIGARETTENAME AS CIGARETTENAME,'条' AS UTINNAME,A.QUANTITY,0,0,'{0}',{1},
                         0 AS QTYDEMAND,0 AS PRICE,0 AS AMOUNT,0 AS IS_IMPORT,A.QUANTITY AS ORDER_QUANTITY 
                         FROM OUKANG.OUKANG_CO A
                         LEFT JOIN OUKANG.OUKANG_ITEM B ON A.CIGARETTECODE = B.CIGARETTECODE
                        LEFT JOIN OUKANG_CUST C ON A.CUSTOMERCODE = C.CUSTOMERCODE 
                        WHERE ORDERDATE = '{2}' AND C.ROUTECODE NOT IN ({3})";
                    break;
                default:
                    sql = @"SELECT ORDER_ID AS ORDERID, BRAND_CODE AS CIGARETTECODE, BRAND_NAME AS CIGARETTENAME,QUANTITY,0,0,'{0}',{1}" +
                            " FROM DWV_ORD_ORDER_DETAIL WHERE ORDER_ID IN (SELECT ORDER_ID FROM DWV_ORD_ORDER WHERE ORDER_DATE LIKE '{2}%' " +
                            " AND DELIVER_LINE_CODE NOT IN ({3}))";
                    break;
            }
            return ExecuteQuery(string.Format(sql, orderDate.ToString("yyyy-MM-dd"), batchNo, orderDate.ToString("yyyy-MM-dd"), routes)).Tables[0];
        }
    }
}
