using Aix.DBToES.Common;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aix.DBToES
{
    public class SyncToES
    {
        private ESOptions _esOptions;
        private DBOptions _dbOptions;
        public DBQuery _dBQuery;
        private ILogger<SyncToES> _logger;
        private ESSearchConfiguration _eSSearchConfiguration;
        public SyncToES(ESOptions eSOptions, DBOptions dBOptions, DBQuery dBQuery
            , ILogger<SyncToES> logger
            , ESSearchConfiguration eSSearchConfiguration)
        {
            _esOptions = eSOptions;
            _dbOptions = dBOptions;

            _dBQuery = dBQuery;
            _logger = logger;
            _eSSearchConfiguration = eSSearchConfiguration;
        }
        public async Task Sync()
        {
            _logger.LogInformation($"开始同步:{DateTime.Now.ToString("HH:mm:ss")}...");
            int total = 0;
            try
            {
                var hasData = true;
                int padeIndex = 0;
                int pageSize = _esOptions.PerBatchSize;
                while (hasData)
                {
                    var list = await _dBQuery.Query(padeIndex, pageSize);
                    await ToES(list);

                    if (list.Count > 0)
                    {
                        hasData = true;
                        padeIndex++;
                    }
                    else
                    {
                        hasData = false;
                        break;
                    }

                    total += list.Count;
                    _logger.LogInformation($"同步成功:{list.Count}条");
                }

                _logger.LogInformation($"同步结束:{DateTime.Now.ToString("HH:mm:ss")},一共同步:{total}条");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "同步出错");
            }
        }

        private async Task ToES(List<IDictionary<string, object>> list)
        {
            if (list.Count == 0) return;
            var saveList = new List<object>();
            foreach (var item in list)
            {
                string id = GetId(item, _dbOptions.PrimaryKey);
                if (string.IsNullOrEmpty(id)) throw new Exception("id为空");
                switch (_esOptions.opType.ToLower())
                {
                    case "create":
                        saveList.Add(new { create = new { _index = _esOptions.Index, _type = _esOptions.Type, _id = id } });
                        saveList.Add(item);
                        break;
                    case "index":
                        saveList.Add(new { index = new { _index = _esOptions.Index, _type = _esOptions.Type, _id = id } });
                        saveList.Add(item);
                        break;
                    case "update":
                        saveList.Add(new { update = new { _index = _esOptions.Index, _type = _esOptions.Type, _id = id } });
                        saveList.Add(new { doc = item });
                        break;
                    case "delete":
                        saveList.Add(new { delete = new { _index = _esOptions.Index, _type = _esOptions.Type, _id = id } });
                        break;
                    default:
                        throw new Exception("参数syncType配置错误");
                }
            }
            var searchResponse = await _eSSearchConfiguration.GetClient().LowLevel.BulkAsync<StringResponse>(PostData.MultiJson(saveList));
            var success = searchResponse.Success;
            var successOrKnownError = searchResponse.SuccessOrKnownError;
            var exception = searchResponse.OriginalException;
            if (exception != null)
                throw exception;
            //    string responseStream = indexResponse.Body;

            await Task.CompletedTask;
        }

        private string GetId(IDictionary<string, object> model, string idName)
        {
            if (model.TryGetValue(idName, out object value))
            {
                return value?.ToString();
            }

            //var property = model.GetType().GetProperty("idName");
            //if (property == null) throw new Exception($"主键列不存在:{idName}");

            //var value = property.GetValue(model);
            //return value?.ToString();
            return null;
        }

    }
}
