using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.AS.Dao
{
    public class LineDeviceDao : BaseDao
    {
        /// <summary>
        /// 2010-11-19
        /// </summary>
        /// <param name="lineCode"></param>
        /// <returns></returns>
        public DataSet FindLineDevice(string lineCode)
        {
            string sql = "SELECT * FROM AS_BI_LINEDEVICE WHERE LINECODE = '{0}' ORDER BY PRIORITY";
            return ExecuteQuery(string.Format(sql, lineCode));
        }

        public DataSet FindLineDevice(string orderdate,string batchno)
        {
            string sql = @" SELECT a.linecode,a.batchno,CONVERT(varchar(10),a.orderdate,120) AS orderdate
                                ,(COUNT(DISTINCT b.cigarettecode)-c.DealedCigaretteCount) cigarettetypes 
                                ,c.channelcount,c.occupyfourtower,c.occupythreetower,c.occupytwotower 
                            FROM  as_SC_line a 
                            LEFT JOIN 
                                (SELECT distinct a.orderdate,b.batchno,b.routecode, a.cigarettecode 
                                    FROM as_I_orderdetail a,as_i_ordermaster b WHERE a.orderid=b.orderid AND a.cigarettecode 
                                    NOT IN (SELECT CIGARETTECODE FROM as_bi_cigarette WHERE ISABNORMITY='1')
                                ) b
                            ON a.orderdate=b.orderdate AND a.batchno=b.batchno AND a.routecode=b.routecode
                            LEFT JOIN 
                                (SELECT E.linecode,E.channelcount,ISNULL
                                    (
                                    CASE WHEN CONVERT(INT,F.CHANNELCOUNT)>CONVERT(INT,F.SORTCOUNT) 
                                    THEN F.SORTCOUNT 
                                    ELSE F.CHANNELCOUNT 
                                    END,0) DealedCigaretteCount ,E.occupyfourtower,E.occupythreetower,E.occupytwotower 
                                    FROM (SELECT * FROM as_BI_linedevice WHERE channeltype='2'
                                    )E
                                LEFT JOIN 
                                    (SELECT * FROM as_BI_linedevice WHERE channeltype='3')F
                                    ON E.linecode=F.linecode
                                ) c 
                            ON a.linecode=c.linecode 
                            WHERE a.orderdate='{0}' AND a.batchno='{1}'  
                            GROUP BY a.orderdate,a.batchno,a.linecode,c.DealedCigaretteCount ,c.occupyfourtower,c.channelcount,c.occupythreetower,c.occupytwotower ";
            return ExecuteQuery(string.Format(sql, orderdate, batchno));
        }
        public void UpdateLineCHPara(string linecode,string OccupyFourCh,string OccupyThreeCh,string OccupyTwoCh)
        {
            string strSQL = "update AS_BI_LINEDEVICE set OCCUPYFOURTOWER='"+OccupyFourCh+"',OCCUPYTHREETOWER='"+OccupyThreeCh+"',OCCUPYTWOTOWER='"+OccupyTwoCh+"' WHERE LINECODE='"+linecode+"' AND CHANNELTYPE='2'";
            ExecuteNonQuery(strSQL);
        }
        public string GetLineCHPara(string linecode,string OccupyCount)
        {
            string strSQL = "";            
            switch (OccupyCount.Trim())
            { 
                case "4":
                    strSQL = "SELECT isnull(OCCUPYFOURTOWER,0) FROM AS_BI_LINEDEVICE WHERE LINECODE='" + linecode + "' AND CHANNELTYPE='2'";
                    break;
                case "3":
                    strSQL = "SELECT isnull(OCCUPYTHREETOWER,0) FROM AS_BI_LINEDEVICE WHERE LINECODE='" + linecode + "' AND CHANNELTYPE='2'";
                    break;
                case "2":
                    strSQL = "SELECT isnull(OCCUPYTWOTOWER,0) FROM AS_BI_LINEDEVICE WHERE LINECODE='" + linecode + "' AND CHANNELTYPE='2'";
                    break;
                default:
                    return "0";                    
            }
            DataSet DS = ExecuteQuery(strSQL);
            return DS.Tables[0].Rows[0][0].ToString();

        
        }
    }
}
