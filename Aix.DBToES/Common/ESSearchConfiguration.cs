using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.DBToES.Common
{
    public class ESSearchConfiguration
    {

        private readonly ESOptions _option;

        private static ElasticClient _client;
        private static object lockObject = new object();

        public ESSearchConfiguration(ESOptions option)
        {
            if (option == null || option.Urls == null || option.Urls.Length == 0)
            {
                throw new ArgumentNullException("ElasticHostOption", "Elastic配置信息为空");
            }
            _option = option;
        }


        public static string RelicIndexAlias => "ny_subject";

        public ElasticClient GetClient()
        {
            if (_client == null)
            {
                lock (lockObject)
                {
                    if (_client == null)
                    {
                        var connectionPool = CreateConnectionPool();
                        var settings = new ConnectionSettings(connectionPool)
                            .DefaultIndex(RelicIndexAlias);

                        _client = new ElasticClient(settings);
                    }
                }
            }
            return _client;
        }

        private IConnectionPool CreateConnectionPool()
        {
            Uri[] uris = new Uri[_option.Urls.Length];
            for (int i = 0; i < _option.Urls.Length; i++)
            {
                uris[i] = new Uri(_option.Urls[i]);
            }

            IConnectionPool connectionPool = null;
            if (uris.Length == 1)
            {
                connectionPool = new SingleNodeConnectionPool(uris[0]);
            }
            else
            {
                connectionPool = new SniffingConnectionPool(uris);
            }

            return connectionPool;
        }


    }
}
