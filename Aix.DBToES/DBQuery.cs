using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using System.Linq;
using System.Data.SqlClient;

namespace Aix.DBToES
{
    public class DBQuery
    {
        private DBOptions _dbOptions;
        public DBQuery(DBOptions dBOptions)
        {
            _dbOptions = dBOptions;
        }

        public async Task<List<object>> QueryOld(int pageIndex, int pageSize)
        {
            using (var con = new MySqlConnection(_dbOptions.connectionString))
            {
                int pageStartIndex = pageSize * pageIndex;
                int currentPageCount = pageSize;
                var sql = $"{_dbOptions.Sql} order by {_dbOptions.PrimaryKey}  limit  {pageStartIndex}, {currentPageCount}";
                var p = new { };
                var list = await con.QueryAsync<object>(sql, p);
                return list.ToList();
            }
        }

        public  Task<List<IDictionary<string, object>>> Query(int pageIndex, int pageSize)
        {
            if (_dbOptions.dbType.ToLower() == "mysql")
            {
                return QueryMysql(pageIndex, pageSize);
            }
            else if (_dbOptions.dbType.ToLower() == "mssql")
            {
                return QueryMsSql(pageIndex, pageSize);
            }
            throw new Exception($"非法的数据库类型 :{_dbOptions.dbType}");
        }
        public async Task<List<IDictionary<string, object>>> QueryMysql (int pageIndex, int pageSize)
        {
            using (var con = new MySqlConnection(_dbOptions.connectionString))
            {
                int pageStartIndex = pageSize * pageIndex;
                int currentPageCount = pageSize;
                string orderBy = string.IsNullOrEmpty(_dbOptions.OrderBy) ? $" order by {_dbOptions.PrimaryKey} " : _dbOptions.OrderBy;
                var sql = $"{_dbOptions.Sql}  {orderBy}  limit  {pageStartIndex}, {currentPageCount}";

                List<IDictionary<string, object>> result = new List<IDictionary<string, object>>();
                using (var reader = await con.ExecuteReaderAsync(sql))
                {
                    while (reader.Read())
                    {
                        var dict = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            dict[reader.GetName(i)] = reader.GetValue(i);
                        }
                        result.Add(dict);
                    }
                }

                return result;
            }

        }

        public async Task<List<IDictionary<string, object>>> QueryMsSql(int pageIndex, int pageSize)
        {
            using (var con = new SqlConnection(_dbOptions.connectionString))
            {
                int pageStartIndex = pageSize * pageIndex + 1;
                int pageEndIndex = pageSize * (pageIndex + 1);

                string pageSql =string.Format(" select * from ({0}) as pagetable where RowNumber >={1}  and RowNumber<= {2}  ", _dbOptions.Sql, pageStartIndex, pageEndIndex);
              
                List<IDictionary<string, object>> result = new List<IDictionary<string, object>>();
                using (var reader = await con.ExecuteReaderAsync(pageSql))
                {
                    while (reader.Read())
                    {
                        var dict = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            if (columnName.ToLower() == "rownumber")
                            {
                                continue;
                            }
                            dict[columnName] = reader.GetValue(i);
                        }
                        result.Add(dict);
                    }
                }

                return result;
            }

        }
    }
}
