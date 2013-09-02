using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace THOK.AS.Schedule
{
    class HashTableHandle
    {
        private Hashtable _hashTable = new Hashtable();

        public HashTableHandle(DataTable table)
        {
            _hashTable = TableToHash(table);
        }

        Hashtable TableToHash(DataTable table)
        {
            Hashtable hashTable = new Hashtable();
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        if (!hashTable.ContainsKey(col.ColumnName))
                        {
                            Hashtable t1 = new Hashtable();
                            hashTable.Add(col.ColumnName, t1);
                        }

                        if (!((Hashtable)hashTable[col.ColumnName]).ContainsKey(row[col]))
                        {
                            Hashtable t3 = new Hashtable();
                            ((Hashtable)hashTable[col.ColumnName]).Add(row[col], t3);
                        }

                        Hashtable t4 = (Hashtable)(hashTable[col.ColumnName]);
                        t4 = (Hashtable)(t4[row[col]]);
                        t4.Add(t4.Count + 1, row);
                    }
                }
            }
            return hashTable;
        }

        public DataTable Select<TValue>(string colName, TValue colValue)
        {
            if (!_hashTable.ContainsKey(colName) || !((Hashtable)_hashTable[colName]).ContainsKey(colValue))
            {
                return new DataTable();
            }
            return HashToTable((Hashtable)((Hashtable)_hashTable[colName])[colValue]);
        }

        DataTable HashToTable(Hashtable hashtable)
        {
            DataTable dt = new DataTable();
            if (hashtable.Count > 0)
            {
                dt = ((DataRow)hashtable[1]).Table.Clone();

                for (int i = 1; i <= hashtable.Count; i++)
                {
                    DataRow row = (DataRow)hashtable[i];
                    dt.Rows.Add(row.ItemArray);
                }
            }
            return dt;
        }
    }
}
