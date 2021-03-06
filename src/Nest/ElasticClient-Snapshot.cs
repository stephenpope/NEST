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

		public IndicesShardResponse Snapshot()
		{
			return this.Snapshot("_all");
		}
		public IndicesShardResponse Snapshot<T>() where T : class
		{
			var index = this.Settings.DefaultIndex;
			index.ThrowIfNullOrEmpty("Cannot infer default index for current connection.");

			return Snapshot(index);
		}
		public IndicesShardResponse Snapshot(string index)
		{
			index.ThrowIfNull("index");
			return this.Snapshot(new[] { index });
		}

		public IndicesShardResponse Snapshot(IEnumerable<string> indices)
		{
			indices.ThrowIfNull("indices");
			string path = this.CreatePath(string.Join(",", indices)) + "_gateway/snapshot";
			return this._Snapshot(path);
		}
		private IndicesShardResponse _Snapshot(string path)
		{
			var status = this.Connection.PostSync(path, "");
			var r = this.ToParsedResponse<IndicesShardResponse>(status);
			return r;
		}

	}
}
