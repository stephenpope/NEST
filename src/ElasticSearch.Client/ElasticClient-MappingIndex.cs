﻿using System;
using System.Collections.Generic;
using System.Linq;
using ElasticSearch.Client.Mapping;
using ElasticSearch.Client.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ElasticSearch.Client
{
    public partial class ElasticClient
    {
        private static Regex StripIndex = new Regex(@"^index\.");
        public IndexSettingsResponse GetIndexSettings()
        {
            var index = this.Settings.DefaultIndex;
            return this.GetIndexSettings(index);
        }
        public IndexSettingsResponse GetIndexSettings(string index)
        {
            string path = this.CreatePath(index) + "_settings";
            var status = this.Connection.GetSync(path);

            var response = new IndexSettingsResponse();
            response.Success = false;
            try
            {
                var o = JObject.Parse(status.Result);
                var settingsObject = o.First.First.First.First;
                var settings = JsonConvert
                    .DeserializeObject<IndexSettings>(settingsObject.ToString());

                foreach (JProperty s in settingsObject.Children<JProperty>())
                {
                    settings.Add(StripIndex.Replace(s.Name, ""), s.Value.ToString());
                }
                

                response.Settings = settings;
                response.Success = true;
            }
            catch { }
            response.ConnectionStatus = status;
            return response;
        }





        public IndicesResponse CreateIndex(string index, IndexSettings settings)
        {
            string path = this.CreatePath(index);
            string data =  JsonConvert.SerializeObject(settings, Formatting.None, this.SerializationSettings);
            var status = this.Connection.PostSync(path,data);

            try
            {
                var response = JsonConvert.DeserializeObject<IndicesResponse>(status.Result);
                response.ConnectionStatus = status;
                
                return response;
            } 
            catch
            {
                return new IndicesResponse();
            }
        }
        public IndicesResponse DeleteIndex<T>() where T : class
        {
            return this.DeleteIndex(this.Settings.DefaultIndex);
        }
        public IndicesResponse DeleteIndex(string index)
        {
            string path = this.CreatePath(index);

            var status = this.Connection.DeleteSync(path);
            var response = JsonConvert.DeserializeObject<IndicesResponse>(status.Result);
            response.ConnectionStatus = status;

            return response;
        }

    }    
}