using System.Collections.Generic;

namespace TechTalk.JiraRestClient
{
	public class Field
	{
		public string id { get; set; }

		public string name { get; set; }

		public bool custom { get; set; }

		public bool orderable { get; set; }

		public bool navigable { get; set; }

		public bool searchable { get; set; }

		public List<string> clauseNames { get; set; }

		public Dictionary<string, string> schema { get; set; }
	}
}
