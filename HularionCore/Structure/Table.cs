#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HularionCore.Structure
{
    /// <summary>
    /// A class used to construct object tables.
    /// </summary>
    /// <remarks>
    /// The types for the columns and rows must be defined, but the values can be any type.  Values that are not set will be treated as null.
    /// </remarks>
    /// <typeparam name="ColumnType">The type used to reference columns.  If the type is an enum, each value of the enum will be added as a column automatically.</typeparam>
    /// <typeparam name="RowType">The type used to reference rows.  If the type is an enum, each value of the enum will be added as a row automatically.</typeparam>
    public class Table<ColumnType, RowType>
    {

        #region PrivateMembers
        /// <summary>
        /// The dictionary that contains the columns.
        /// </summary>
        private Dictionary<object, uint> columns = new Dictionary<object, uint>();
        /// <summary>
        /// The dictionary that contains the rows.
        /// </summary>
        private Dictionary<object, uint> rows = new Dictionary<object, uint>();
        /// <summary>
        /// The dictionary that contains the table data.
        /// </summary>
        private Dictionary<ulong, object> values = new Dictionary<ulong, object>();
        /// <summary>
        /// The dictionary that contains the data associated with a column.
        /// </summary>
        private Dictionary<uint, HashSet<ulong>> columnValues = new Dictionary<uint, HashSet<ulong>>();
        /// <summary>
        /// The dictionary that contains the data associated with a column.
        /// </summary>
        private Dictionary<uint, HashSet<ulong>> rowValues = new Dictionary<uint, HashSet<ulong>>();
        /// <summary>
        /// The counter used to index the columns.
        /// </summary>
        private uint columnCounter = 0;
        /// <summary>
        /// The counter used to index the rows.
        /// </summary>
        private uint rowCounter = 0;
        /// <summary>
        /// A list of keys from columns that were deleted.  These keys will be recycled.
        /// </summary>
        private List<uint> availableColumnKeys = new List<uint>();
        /// <summary>
        /// A list of keys from rows that were deleted.  These keys will be recycled.
        /// </summary>
        private List<uint> availableRowKeys = new List<uint>();
        #endregion

        #region PublicMembers
        /// <summary>
        /// A list of the columns in the table.
        /// </summary>
        public List<ColumnType> Columns { get; private set; }
        /// <summary>
        /// A list of the rows in the table.
        /// </summary>
        public List<RowType> Rows { get; private set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public Table()
        {
            Columns = new List<ColumnType>();
            Rows = new List<RowType>();
            if (typeof(ColumnType).IsEnum)
            {
                foreach (var v in Enum.GetValues(typeof(ColumnType)))
                {
                    AddColumn((ColumnType)v);
                }
            }
            if (typeof(RowType).IsEnum)
            {
                foreach (var v in Enum.GetValues(typeof(RowType)))
                {
                    AddRow((RowType)v);
                }
            }
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="table">The Table to copy.</param>
        public Table(Table<ColumnType, RowType> table)
        {
            Copy(table);
        }

        #endregion

        #region PublicMethods

        /// <summary>
        /// Adds the column to the table.
        /// </summary>
        /// <param name="column">The column to add to the table.</param>
        public void AddColumn(ColumnType column)
        {
            if (columns.ContainsKey(column))
            {
                return;
            }
            uint columnKey;
            if (availableColumnKeys.Count > 0)
            {
                columnKey = availableColumnKeys[0];
                availableColumnKeys.Remove(columnKey);
            }
            else
            {
                columnKey = columnCounter;
                columnCounter++;
            }
            columns.Add(column, columnKey);
            Columns.Add(column);
        }

        /// <summary>
        /// Adds all of the provided columns to the table.
        /// </summary>
        /// <param name="columns">The columns to add to the table.</param>
        public void AddColumns(params ColumnType[] columns)
        {
            for(int i = 0; i < columns.Length; i++)
            {
                AddColumn(columns[i]);
            }
        }

        /// <summary>
        /// Removes the column from the table.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        public void RemoveColumn(ColumnType column)
        {
            if (!columns.ContainsKey(column))
            {
                return;
            }
            ClearColumnValues(column);
            uint columnKey;
            columns.TryGetValue(column, out columnKey);
            availableColumnKeys.Add(columnKey);
            columns.Remove(column);
            Columns.Remove(column);
        }

        /// <summary>
        /// Removes the provided columns from the table.
        /// </summary>
        /// <param name="columns">The columns to remove.</param>
        public void RemoveColumns(params ColumnType[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                RemoveColumn(columns[i]);
            }
        }

        /// <summary>
        /// Returns true if the column exists in the table and false otherwise.
        /// </summary>
        /// <param name="column">The column to search for.</param>
        /// <returns>true: The column exists in the table.  false: The column does not exist in the table.</returns>
        public bool ContainsColumn(ColumnType column)
        {
            return columns.ContainsKey(column);
        }

        /// <summary>
        /// Changes the column so that the column will be referenced by the new ColumnType object instead of the old one.
        /// </summary>
        /// <exception cref="ArgumentException">Throws an exception if the current column does not exist in the table.</exception>
        /// <param name="currentColumn">The current ColumnType used to reference the column.</param>
        /// <param name="newColumn">The new ColumnType object used to reference the column.</param>
        public void ChangeColumn(ColumnType currentColumn, ColumnType newColumn)
        {
            if (!columns.ContainsKey(currentColumn)) throw new ArgumentException("The specified column is not in the table.");
            uint columnKey;
            columns.TryGetValue(currentColumn, out columnKey);
            columns.Remove(currentColumn);
            columns.Add(newColumn, columnKey);
            Columns.Insert(Columns.IndexOf(currentColumn), newColumn);
            Columns.Remove(currentColumn);
        }

        /// <summary>
        /// Adds the row to the table.
        /// </summary>
        /// <param name="row">The row to add to the table.</param>
        public void AddRow(RowType row)
        {
            if (rows.ContainsKey(row))
            {
                return;
            }
            uint rowKey;
            if (availableRowKeys.Count > 0)
            {
                rowKey = availableRowKeys[0];
                availableRowKeys.Remove(rowKey);
            }
            else
            {
                rowKey = rowCounter;
                rowCounter++;
            }
            rows.Add(row, rowKey);
            Rows.Add(row);
        }

        /// <summary>
        /// Adds the provided rows to the table.
        /// </summary>
        /// <param name="rows">The rows to add to the table.</param>
        public void AddRows(params RowType[] rows)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                AddRow(rows[i]);
            }
        }

        /// <summary>
        /// Removes the row from the table.
        /// </summary>
        /// <param name="row">The row to remove.</param>
        public void RemoveRow(RowType row)
        {
            if (!rows.ContainsKey(row))
            {
                return;
            }
            ClearRowValues(row);
            uint rowKey;
            rows.TryGetValue(row, out rowKey);
            availableRowKeys.Add(rowKey);
            rows.Remove(row);
            Rows.Remove(row);
        }

        /// <summary>
        /// Removes the provided rows to the table.
        /// </summary>
        /// <param name="rows">The rows to remove to the table.</param>
        public void RemoveRows(params RowType[] rows)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                RemoveRow(rows[i]);
            }
        }

        /// <summary>
        /// Returns true if the row exists in the table and false otherwise.
        /// </summary>
        /// <param name="row">The row to search for.</param>
        /// <returns>true: The row exists in the table. false: The row does not exist in the table.</returns>
        public bool ContainsRow(RowType row)
        {
            return rows.ContainsKey(row);
        }

        /// <summary>
        /// Changes the row so that the row will be referenced by the new RowType object instead of the old one.
        /// </summary>
        /// <param name="currentRow">The current RowType used to reference the row.</param>
        /// <param name="newRow">The new RowType object used to reference the row.</param>
        public void ChangeRow(RowType currentRow, RowType newRow)
        {
            if (!rows.ContainsKey(currentRow)) throw new ArgumentException("The specified row is not in the table.");
            rows.TryGetValue(currentRow, out uint rowKey);
            rows.Remove(currentRow);
            rows.Add(newRow, rowKey);
            Rows.Insert(Rows.IndexOf(currentRow), newRow);
            Rows.Remove(currentRow);
        }

        /// <summary>
        /// Sets the object at the given Column and Row location.
        /// </summary>
        /// <param name="column">The column in which to place the value.</param>
        /// <param name="row">The row in which to place the value.</param>
        /// <param name="Value">The object to store in the Column/Row location.</param>
        public void SetValue(ColumnType column, RowType row, object Value)
        {
            if (!columns.ContainsKey(column))
            {
                AddColumn(column);
            }
            if (!rows.ContainsKey(row))
            {
                AddRow(row);
            }
            ulong key = MakeKey(column, row);
            if (values.ContainsKey(key)) values.Remove(key);
            values.Add(key, Value);
            var columnKey = GetColumnKey(key);
            if (!columnValues.ContainsKey(columnKey))
            {
                columnValues.Add(columnKey, new HashSet<ulong>());
            }
            columnValues[columnKey].Add(key);
            var rowKey = GetRowKey(key);
            if(!rowValues.ContainsKey(rowKey))
            {
                rowValues.Add(rowKey, new HashSet<ulong>());
            }
            rowValues[rowKey].Add(key);
        }

        /// <summary>
        /// Gets the object stored at the given Column and Row location.
        /// </summary>
        /// <param name="column">The column to search.</param>
        /// <param name="row">The row to search.</param>
        /// <param name="value">The object at the Column/Row location. The object is null if no value was found.</param>
        /// <returns>true: A value was found.  false: A value was not found.</returns>
        public bool GetValue(ColumnType column, RowType row, out object value)
        {
            if (!columns.ContainsKey(column) || !rows.ContainsKey(row))
            {
                value = null;
                return false;
            }
            ulong key = MakeKey(column, row);
            if (!values.ContainsKey(key))
            {
                value = null;
                return false;
            }
            values.TryGetValue(key, out value);
            return true;
        }

        /// <summary>
        /// Returns the entry of the table at the provided column and row.
        /// </summary>
        /// <param name="column">The column of the entry.</param>
        /// <param name="row">The row of the entry.</param>
        /// <returns>The entry at the indicated column and row or null if the column or row do not exist.</returns>
        public TableEntry<ColumnType, RowType> GetEntry(ColumnType column, RowType row)
        {
            return new TableEntry<ColumnType, RowType>() { Column = column, Row = row, Value = GetValue<object>(column, row) };
        }

        /// <summary>
        /// Returns the entry of the table at the provided column and row.
        /// </summary>
        /// <param name="column">The column of the entry.</param>
        /// <param name="row">The row of the entry.</param>
        /// <typeparam name="T">The type of value in the entry.</typeparam>
        /// <returns>The entry at the indicated column and row or null if the column or row do not exist.</returns>
        public TableEntry<ColumnType, RowType, T> GetEntry<T>(ColumnType column, RowType row)
        {
            return new TableEntry<ColumnType, RowType, T>() { Column = column, Row = row, Value = GetValue<T>(column, row) };
        }

        /// <summary>
        /// Gets the object stored at the given Column and Row location.
        /// </summary>
        /// <typeparam name="T">The type of object to return.  This must be the type of the object that is stored.</typeparam>
        /// <param name="column">The column to search.</param>
        /// <param name="row">The row to search.</param>
        /// <returns>The object or the object's default value give the type T if the object is null.</returns>
        public T GetValue<T>(ColumnType column, RowType row)
        {
            object o;
            GetValue(column, row, out o);
            if(o == null)
            {
                return default(T);
            }
            return (T)o;
        }

        /// <summary>
        /// Gets all of the values associated with a column.
        /// </summary>
        /// <param name="column">The column of values to get.</param>
        /// <returns>All of the values associated with the provided column.</returns>
        public IEnumerable<object> GetColumnValues(ColumnType column)
        {
            if (!columns.ContainsKey(column))
            {
                return new List<object>();
            }
            var columnKey = columns[column];
            var keys = columnValues[columnKey].ToList();
            var result = new List<object>();
            foreach (var key in keys)
            {
                result.Add(values[key]);
            }
            return result;
        }

        /// <summary>
        /// Gets all of the values associated with a column.
        /// </summary>
        /// <param name="column">The column of values to get.</param>
        /// <typeparam name="T">The type of values to return.</typeparam>
        /// <returns>All of the values associated with the provided column.</returns>
        public IEnumerable<T> GetColumnValues<T>(ColumnType column)
        {
            var values = GetColumnValues(column);
            return values.Select(x => x == null ? default(T) : (T)x).ToList();
        }

        /// <summary>
        /// Gets all of the values associated with a row.
        /// </summary>
        /// <param name="row">The row of values to get.</param>
        /// <returns>All of the values associated with the provided row.</returns>
        public IEnumerable<object> GetRowValues(RowType row)
        {
            if (!rows.ContainsKey(row))
            {
                return new List<object>();
            }
            var rowKey = rows[row];
            var keys = rowValues[rowKey].ToList();
            var result = new List<object>();
            foreach (var key in keys)
            {
                result.Add(values[key]);
            }
            return result;
        }

        /// <summary>
        /// Gets all of the values associated with a row.
        /// </summary>
        /// <param name="row">The row of values to get.</param>
        /// <typeparam name="T">The type of values to return.</typeparam>
        /// <returns>All of the values associated with the provided row.</returns>
        public IEnumerable<T> GetRowValues<T>(RowType row)
        {
            var values = GetRowValues(row);
            return values.Select(x => x == null ? default(T) : (T)x).ToList();
        }

        /// <summary>
        /// Gets all of the entries associated with the provided column.
        /// </summary>
        /// <param name="column">The column of entries to get.</param>
        /// <returns>All of the entries associated with the provided column.</returns>
        public IEnumerable<TableEntry<ColumnType, RowType>> GetColumnEntries(ColumnType column)
        {
            var entries = GetColumnEntries<object>(column);
            return entries;
        }

        /// <summary>
        /// Gets all of the entries associated with the provided column.
        /// </summary>
        /// <param name="column">The column of entries to get.</param>
        /// <typeparam name="T">The type of the value in the entries.</typeparam>
        /// <returns>All of the entries associated with the provided column.</returns>
        public IEnumerable<TableEntry<ColumnType, RowType, T>> GetColumnEntries<T>(ColumnType column)
        {
            if (!columns.ContainsKey(column))
            {
                return new List<TableEntry<ColumnType, RowType,T>>();
            }
            var columnKey = columns[column];
            var keys = columnValues[columnKey];
            var result = new List<TableEntry<ColumnType, RowType, T>>();
            foreach (var key in keys)
            {
                var rowKey = GetRowKey(key);
                var row = (RowType)rows.Where(x => x.Value == rowKey).First().Key;
                result.Add(GetEntry<T>(column, row));
            }
            return result;
        }

        /// <summary>
        /// Gets all of the entries in the provided row.
        /// </summary>
        /// <param name="row">The row of entries to get.</param>
        /// <returns>All of the entries in the provided row.</returns>
        public IEnumerable<TableEntry<ColumnType, RowType>> GetRowEntries(RowType row)
        {
            var entries = GetRowEntries<object>(row);
            return entries;
        }

        /// <summary>
        /// Gets all of the entries in the provided row.
        /// </summary>
        /// <typeparam name="T">The type of the value of the entries.</typeparam>
        /// <param name="row">The row of entries to get.</param>
        /// <returns>All of the entries in the provided row.</returns>
        public IEnumerable<TableEntry<ColumnType, RowType, T>> GetRowEntries<T>(RowType row)
        {
            if (!rows.ContainsKey(row))
            {
                return new List<TableEntry<ColumnType, RowType, T>>();
            }
            var rowKey = rows[row];
            var keys = rowValues[rowKey];
            var result = new List<TableEntry<ColumnType, RowType, T>>();
            foreach (var key in keys)
            {
                var columnKey = GetColumnKey(key);
                var column = (ColumnType)columns.Where(x => x.Value == columnKey).First().Key;
                result.Add(GetEntry<T>(column, row));
            }
            return result;
        }

        /// <summary>
        /// Gets all of the entries in the table.
        /// </summary>
        /// <typeparam name="T">The type of the entry value.</typeparam>
        /// <returns>All of the entries in the table.</returns>
        public IEnumerable<TableEntry<ColumnType, RowType, T>> GetAllEntries<T>()
        {
            var entries = new List<TableEntry<ColumnType, RowType, T>>();
            var reverseColumns = columns.ToDictionary(x => x.Value, x => x.Key);
            var reverseRows = rows.ToDictionary(x => x.Value, x => x.Key);
            foreach (var value in values)
            {
                var keys = ReverseKey(value.Key);
                entries.Add(new TableEntry<ColumnType, RowType, T>()
                {
                    Column = (ColumnType)reverseColumns[keys.Item1],
                    Row = (RowType)reverseRows[keys.Item2],
                    Value = (T)value.Value
                });
            }
            return entries;
        }

        /// <summary>
        /// Gets all of the entries in the table.
        /// </summary>
        /// <returns>All of the entries in the table.</returns>
        public IEnumerable<TableEntry<ColumnType, RowType>> GetAllEntries()
        {
            var entries = new List<TableEntry<ColumnType, RowType>>();
            var reverseColumns = columns.ToDictionary(x => x.Value, x => x.Key);
            var reverseRows = rows.ToDictionary(x => x.Value, x => x.Key);
            foreach (var value in values)
            {
                var keys = ReverseKey(value.Key);
                entries.Add(new TableEntry<ColumnType, RowType>()
                {
                    Column = (ColumnType)reverseColumns[keys.Item1],
                    Row = (RowType)reverseRows[keys.Item2],
                    Value = value.Value
                });
            }
            return entries;
        }

        /// <summary>
        /// Gets all of the rows that share a value with the provided column.
        /// </summary>
        /// <param name="column">The column whose associated rows will be returned.</param>
        /// <returns>All of the rows that share a value with the provided column.</returns>
        public IEnumerable<RowType> GetColumnConnectedRows(ColumnType column)
        {
            if (!columns.ContainsKey(column))
            {
                return new List<RowType>();
            }
            var columnKey = columns[column];
            var rowKeys = values.Where(x => GetColumnKey(x.Key) == columnKey).Select(x => GetRowKey(x.Key)).ToList();
            return rows.Where(x => rowKeys.Contains(x.Value)).Select(x => (RowType)x.Key).ToList();
        }

        /// <summary>
        /// Gets all of the columns that share a value with the provided row.
        /// </summary>
        /// <param name="row">The row whose associated columns will be returned.</param>
        /// <returns>All of the columns that share a value with the provided row.</returns>
        public IEnumerable<ColumnType> GetRowConnectedColumns(RowType row)
        {
            if (!rows.ContainsKey(row))
            {
                return new List<ColumnType>();
            }
            var rowKey = rows[row];
            var columnKeys = values.Where(x => GetRowKey(x.Key) == rowKey).Select(x => GetColumnKey(x.Key)).ToList();
            return columns.Where(x => columnKeys.Contains(x.Value)).Select(x => (ColumnType)x.Key).ToList();
        }

        /// <summary>
        /// Returns true if there is a value at the given Column and Row location and false otherwise.
        /// </summary>
        /// <param name="column">The column to search.</param>
        /// <param name="row">The row to search.</param>
        /// <returns>true: A value exists at the Column/Row location.  false: A value does not exist at the Column Row location.</returns>
        public bool ContainsValue(ColumnType column, RowType row)
        {
            if (!columns.ContainsKey(column)) { return false; }
            if (!rows.ContainsKey(row)) { return false; }
            ulong key = MakeKey(column, row);
            return values.ContainsKey(key);
        }

        /// <summary>
        /// Removes the value with the indicated column and row from the table.
        /// </summary>
        /// <param name="column">The column of the value to remove.</param>
        /// <param name="row">The row of the value to remove.</param>
        public void ClearValue(ColumnType column, RowType row)
        {
            if(!columns.ContainsKey(column) || !rows.ContainsKey(row))
            {
                return;
            }
            var key = MakeKey(column, row);
            var columnKey = GetColumnKey(key);
            var rowKey = GetRowKey(key);
            values.Remove(key);
            columnValues[columnKey].Remove(key);
            rowValues[rowKey].Remove(key);
        }

        /// <summary>
        /// Clears the values from the table.
        /// </summary>
        public void ClearValues()
        {
            values.Clear();
            columnValues.Select(x => { x.Value.Clear(); return false; });
            rowValues.Select(x => { x.Value.Clear(); return false; });
        }

        /// <summary>
        /// Removes all of the values in the specified column.
        /// </summary>
        /// <param name="column">The column from which to remove the values.</param>
        public void ClearColumnValues(ColumnType column)
        {
            var columnKey = columns[column];
            var keys = columnValues[columnKey];
            foreach (ulong key in keys)
            {
                values.Remove(key);
            }
            columnValues[columnKey].Clear();
        }

        /// <summary>
        /// Removes all of the values in the specified row.
        /// </summary>
        /// <param name="row">The row from which to remove the values.</param>
        public void ClearRowValues(RowType row)
        {
            var rowKey = columns[row];
            var keys = rowValues[rowKey];
            foreach (ulong key in keys)
            {
                values.Remove(key);
            }
            rowValues[rowKey].Clear();
        }

        /// <summary>
        /// Gets all of the values.
        /// </summary>
        /// <returns>All o fthe values.</returns>
        public IEnumerable<object> GetAllValues()
        {
            return values.Values.ToList();
        }

        /// <summary>
        /// Gets all of the values cast as the specified type.
        /// </summary>
        /// <typeparam name="ValueType">The type to cast the values to.</typeparam>
        /// <returns>All of the values as the given type.</returns>
        public IEnumerable<ValueType> GetAllValues<ValueType>()
        {
            return values.Values.Select(x=>(ValueType)x).ToList();
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Copies the given Table T to this Table.
        /// </summary>
        /// <param name="table">The table to copy.</param>
        private void Copy(Table<ColumnType, RowType> table)
        {
            columns = new Dictionary<object, uint>(table.columns);
            rows = new Dictionary<object, uint>(table.rows);
            values = new Dictionary<ulong, object>(table.values);
            columnCounter = table.columnCounter;
            rowCounter = table.rowCounter;
            Columns = new List<ColumnType>(table.Columns);
            Rows = new List<RowType>(table.Rows);
        }

        /// <summary>
        /// Creates a ulong key for a table value given a column name and a row name.
        /// </summary>
        /// <param name="column">The name of the column.</param>
        /// <param name="row">The name of the row.</param>
        /// <returns>A key that represents the value at the given column and row.</returns>
        private ulong MakeKey(ColumnType column, RowType row)
        {
            uint columnKey;
            uint rowKey;
            columns.TryGetValue(column, out columnKey);
            rows.TryGetValue(row, out rowKey);
            return ((ulong)columnKey << 32) | (ulong)rowKey;
        }

        /// <summary>
        /// Creates column and row keys given a value key.
        /// </summary>
        /// <param name="key">The key of a value.</param>
        /// <returns>A column and row keys given a value key.</returns>
        private Tuple<uint,uint> ReverseKey(ulong key)
        {
            return new Tuple<uint, uint>((uint)(key >> 32), (uint)key);
        }

        /// <summary>
        /// Gets a column key given a table key.
        /// </summary>
        /// <param name="key">The table key.</param>
        /// <returns>The column key.</returns>
        private uint GetColumnKey(ulong key)
        {
            return (uint) (key >> 32);
        }

        /// <summary>
        /// Gets the row key given a table key.
        /// </summary>
        /// <param name="key">The table key.</param>
        /// <returns>The row key.</returns>
        private uint GetRowKey(ulong key)
        {
            return (uint)key;
        }

        #endregion


    }

    /// <summary>
    /// An entry within a table.
    /// </summary>
    /// <typeparam name="ColumnType">The type of the table's columns.</typeparam>
    /// <typeparam name="RowType">The type of the table's rows</typeparam>
    public class TableEntry<ColumnType, RowType>
    {
        /// <summary>
        /// A column in the table.
        /// </summary>
        public ColumnType Column { get; set; }
        /// <summary>
        /// A row in the table.
        /// </summary>
        public RowType Row { get; set; }
        /// <summary>
        /// The value in the table at the column and row.
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Casts the value to the indicated type.
        /// </summary>
        /// <typeparam name="T">The type to cast the value to.</typeparam>
        /// <returns>The value of the table entry casted to the indicated type.</returns>
        public T GetValue<T>()
        {
            return Value == null ? default(T) : (T)Value;
        }
    }
    /// <summary>
    /// An entry within a table.
    /// </summary>
    /// <typeparam name="ColumnType">The type of the table's columns.</typeparam>
    /// <typeparam name="RowType">The type of the table's rows</typeparam>
    /// <typeparam name="ValueType">The type of the value of the entry.</typeparam>
    public class TableEntry<ColumnType, RowType, ValueType> : TableEntry<ColumnType, RowType>
    {
        /// <summary>
        /// The value in the table at the column and row.
        /// </summary>
        public new ValueType Value { get; set; }
    }

}
