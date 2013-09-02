using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.AS.Dao
{
    public class UploadSortingDao : BaseDao
    {
        //查询分拣主表
        public DataTable FindOrderMaster(string orderDate)
        {
            string sql = @"SELECT A.ORDERID AS ORDER_ID,ORGCODE AS ORG_CODE ,A.AREACODE AS SALE_REG_CODE,ORDERDATE AS ORDER_DATE,'1' AS ORDER_TYPE,
                            B.CUSTOMERCODE AS CUST_CODE ,B.CUSTOMERNAME AS CUST_NAME,C.QUANTITY AS QUANTITY_SUM ,C.AMOUNT_SUM ,
                            DETAILNUM AS DETAIL_NUM,'' AS DELIVER_ORDER,'1' AS ISACTIVE,A.IS_IMPORT AS IS_IMPORT 
                            FROM AS_I_ORDERMASTER A
                            LEFT JOIN AS_BI_CUSTOMER B ON A.CUSTOMERCODE=B.CUSTOMERCODE
                            LEFT JOIN (SELECT ORDERID,ISNULL(SUM(QUANTITY),0) AS QUANTITY,ISNULL(SUM(AMOUNT),0) AS AMOUNT_SUM FROM DBO.AS_I_ORDERDETAIL 
                            WHERE ORDERDATE='{0}' AND IS_IMPORT=0  GROUP BY ORDERID) C ON A.ORDERID=C.ORDERID 
                            WHERE ORDERDATE='{0}' AND IS_IMPORT=0 ORDER BY A.ORDERID";
            sql = string.Format(sql,orderDate);
            return this.ExecuteQuery(sql).Tables[0];
        }

        //查询分拣细表
        public DataTable FindOrderDetail(string orderDate)
        {
            string sql = @"SELECT ORDERDETAILID AS ORDER_DETAIL_ID,ORDERID AS ORDER_ID,CIGARETTECODE AS BRAND_CODE ,CIGARETTENAME AS BRAND_NAME,
                            UTINNAME AS BRAND_UNIT_NAME,QTYDEMAND AS QTY_DEMAND, QUANTITY AS QUANTITY ,PRICE AS PRICE,AMOUNT AS AMOUNT,IS_IMPORT
                            FROM DBO.AS_I_ORDERDETAIL WHERE ORDERDATE='{0}' AND IS_IMPORT='0' ORDER BY ORDERID,CIGARETTECODE";
            sql = string.Format(sql, orderDate);
            return this.ExecuteQuery(sql).Tables[0];
        }

        //查询分拣线表
        public DataTable FindSortingIdps()
        {
            string sql = "SELECT * FROM AS_BI_LINEINFO WHERE IS_IMPORT=0";
            return this.ExecuteQuery(sql).Tables[0];
        }

        //获取上报状态
        public DataTable GetUploadState(string orderDate)
        {
            string sql = string.Format("SELECT UPLOADSTATE FROM AS_RECORD_UPLOAD WHERE UPLOADORDERDATE='{0}'", orderDate);
            return this.ExecuteQuery(sql).Tables[0];
        }

        public void setData(string sql)
        {
            this.ExecuteNonQuery(sql);
        }
    }
}
