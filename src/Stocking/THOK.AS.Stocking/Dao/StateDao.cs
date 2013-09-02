using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Stocking.Dao
{
    public class StateDao:BaseDao
    {
        #region//ɨ��״̬��������ѯ
        /// <summary>
        /// ��ѯ����ɨ��״̬������
        /// </summary>
        /// <returns></returns>
        public DataTable FindScannerListTable()
        {
            string sql = "SELECT STATECODE,STATECODE + '|' + REMARK AS STATENAME FROM AS_STATEMANAGER_SCANNER";
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// ����ɨ��״̬��������Ų�����Ӧ��ROW_INDEX
        /// </summary>
        /// <param name="stateCode">״̬���������</param>
        /// <returns></returns>
        public DataTable FindScannerIndexNoByStateCode(string stateCode)
        {
            string sql = string.Format("SELECT ROW_INDEX,VIEWNAME FROM dbo.AS_STATEMANAGER_SCANNER WHERE STATECODE='{0}'",stateCode);
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// ����ROW_INDEX��ѯɨ������������Ϣ
        /// </summary>
        /// <param name="indexNo">��ˮ��</param>
        /// <returns></returns>
        public DataTable FindScannerStateByIndexNo(string indexNo,string viewName)
        {
            string sql = string.Format(@"SELECT ROW_INDEX,LINECODE,STOCKOUTID,CIGARETTECODE,CIGARETTENAME,CHANNELCODE,SORTCHANNELCODE,CHANNELNAME,
                            CASE CHANNELTYPE 
                                WHEN '5' THEN '�����ʽ��'
                                WHEN '4' THEN '���ͨ����'
                                WHEN '3' THEN 'ͨ����'
                                WHEN '2' THEN '��ʽ��'
                                END CHANNELTYPENAME,
                            CASE 
                                WHEN ROW_INDEX < {0} THEN '��ɨ��'
                                WHEN ROW_INDEX = {0} THEN '��ɨ��'
                                WHEN ROW_INDEX > {0} THEN 'δɨ��'
                                END STATE
                            FROM {1} ", indexNo,viewName);
            return ExecuteQuery(sql).Tables[0];
        } 
       #endregion

        #region//LED״̬��������ѯ
        /// <summary>
        /// ��ѯ����LED״̬������
        /// </summary>
        /// <returns></returns>
        public DataTable FindLedListTable()
        {
            string sql = "SELECT STATECODE,STATECODE + '|' + REMARK AS STATENAME FROM AS_STATEMANAGER_LED";
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// ����LED״̬��������Ų�����Ӧ��ROW_INDEX����ͼ
        /// </summary>
        /// <param name="stateCode">״̬���������</param>
        /// <returns></returns>
        public DataTable FindLedIndexNoByStateCode(string stateCode)
        {
            string sql = string.Format("SELECT ROW_INDEX,VIEWNAME FROM dbo.AS_STATEMANAGER_LED WHERE STATECODE='{0}'", stateCode);
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// ����ROW_INDEX��ѯLED��������Ϣ
        /// </summary>
        /// <param name="indexNo">��ˮ��</param>
        /// <returns></returns>
        public DataTable FindLedStateByIndexNo(string indexNo, string viewName)
        {
            string sql = string.Format(@"SELECT ROW_INDEX,LINECODE,STOCKOUTID,CIGARETTECODE,CIGARETTENAME,CHANNELCODE,SORTCHANNELCODE,CHANNELNAME,
                            CASE CHANNELTYPE 
                                WHEN '5' THEN '�����ʽ��'
                                WHEN '4' THEN '���ͨ����'
                                WHEN '3' THEN 'ͨ����'
                                WHEN '2' THEN '��ʽ��'
                                END CHANNELTYPENAME,
                            CASE 
                                WHEN ROW_INDEX < {0} THEN '��ͨ��'
                                WHEN ROW_INDEX = {0} THEN '��ͨ��'
                                WHEN ROW_INDEX > {0} THEN 'δͨ��'
                                END STATE
                            FROM {1} ", indexNo,viewName);
            return ExecuteQuery(sql).Tables[0];
        }

        #endregion

        #region//����״̬��������ѯ

        /// <summary>
        /// ��ѯ���ж���״̬��������Ϣ
        /// </summary>
        /// <returns></returns>
        public DataTable FindOrderListTable()
        {
            string sql = "SELECT STATECODE,STATECODE + '|' + REMARK AS STATENAME FROM AS_STATEMANAGER_ORDER";
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// ���ݶ���״̬��������Ų�����Ӧ��ROW_INDEX
        /// </summary>
        /// <param name="stateCode">״̬���������</param>
        /// <returns></returns>
        public DataTable FindOrderIndexNoByStateCode(string stateCode)
        {
            string sql = string.Format("SELECT ROW_INDEX,VIEWNAME FROM dbo.AS_STATEMANAGER_ORDER WHERE STATECODE='{0}'", stateCode);
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// ����ROW_INDEX��ѯ��������Ϣ
        /// </summary>
        /// <param name="indexNo">��ˮ��</param>
        /// <returns></returns>
        public DataTable FindOrderStateByIndexNo(string indexNo,string viewName)
        {
            string sql = string.Format(@"SELECT ROW_INDEX,LINECODE,STOCKOUTID,CIGARETTECODE,CIGARETTENAME,CHANNELCODE,SORTCHANNELCODE,CHANNELNAME,
                            CASE CHANNELTYPE 
                                WHEN '5' THEN '�����ʽ��'
                                WHEN '4' THEN '���ͨ����'
                                WHEN '3' THEN 'ͨ����'
                                WHEN '2' THEN '��ʽ��'
                                END CHANNELTYPENAME,
                            CASE 
                                WHEN ROW_INDEX < {0} THEN '���µ�'
                                WHEN ROW_INDEX = {0} THEN '���µ�'
                                WHEN ROW_INDEX > {0} THEN 'δ�µ�'
                                END STATE
                            FROM {1} ", indexNo, viewName);
            return ExecuteQuery(sql).Tables[0];
        }

        #endregion
    }
}
