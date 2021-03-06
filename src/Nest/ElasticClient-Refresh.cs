﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nest.Thrift;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Fasterflect;
using Newtonsoft.Json.Converters;
using Nest.DSL;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Nest
{
	public partial class ElasticClient
	{
		/// <summary>
		///  refreshes all
		/// </summary>
		/// <returns></returns>
		public IndicesShardResponse Refresh()
		{
			return this.Refresh("_all");
		}
		/// <summary>
		/// Refresh an index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IndicesShardResponse Refresh(string index)
		{
			index.ThrowIfNull("index");
			return this.Refresh(new []{ index });
		}
		/// <summary>
		/// Refresh multiple indices at once.
		/// </summary>
		/// <param name="indices"></param>
		/// <returns></returns>
		public IndicesShardResponse Refresh(IEnumerable<string> indices)
		{
			indices.ThrowIfNull("indices");
			string path = this.CreatePath(string.Join(",", indices)) + "_refresh";
			return this._Refresh(path);
		}
		/// <summary>
		/// refresh the connection settings default index for type T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IndicesShardResponse Refresh<T>() where T : class
		{
			var index = this.Settings.DefaultIndex;
			index.ThrowIfNullOrEmpty("Cannot infer default index for current connection.");

			return Refresh(index);
		}
		private IndicesShardResponse _Refresh(string path)
		{
			var status = this.Connection.GetSync(path);
			var r = this.ToParsedResponse<IndicesShardResponse>(status);
			return r;
		}

	}
}
