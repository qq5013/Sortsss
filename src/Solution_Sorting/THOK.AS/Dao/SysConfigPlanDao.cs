using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Dao
{
    public class SysConfigPlanDao : BaseDao
    {
        public void InsertEntity(DataSet dataSet)
        {
            foreach (DataRow dataRow in dataSet.Tables["AS_SYS_BARCODE"].Rows)
            {
                if (dataRow.RowState == DataRowState.Added)
                {
                    SqlCreate sqlCreate = new SqlCreate("AS_SYS_BARCODE", SqlType.UPDATE);
                    sqlCreate.AppendQuote("CIGARETTECODE", dataRow["CIGARETTECODE"]);
                    ExecuteNonQuery(sqlCreate.GetSQL());
                }
            }
        }

        public void UpdateEntity(DataSet dataSet)
        {
            foreach (DataRow dataRow in dataSet.Tables["AS_SYS_BARCODE"].Rows)
            {
                if (dataRow.RowState == DataRowState.Modified)
                {
                    SqlCreate sqlCreate = new SqlCreate("AS_BI_CHANNEL", SqlType.UPDATE);
                    sqlCreate.AppendQuote("CIGARETTECODE", dataRow["CIGARETTECODE"]);
                    sqlCreate.AppendWhereQuote("CHANNELID", dataRow["CHANNELID", DataRowVersion.Original]);
                    ExecuteNonQuery(sqlCreate.GetSQL());
                }
            }
        }

        public void DeleteEntity(DataSet dataSet)
        {
            foreach (DataRow dataRow in dataSet.Tables["AS_SYS_BARCODE"].Rows)
            {
                if (dataRow.RowState == DataRowState.Deleted)
                {
                    ExecuteNonQuery("DELETE FROM AS_SYS_BARCODE WHERE FIELDCODE='" + dataRow["FIELDCODE"] + "'");
                }
            }
        }
    }
}
