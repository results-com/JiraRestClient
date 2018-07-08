using System;

namespace TechTalk.JiraRestClient
{
	public class Version
	{
		public string id { get; set; }

		public string name { get; set; }

		public string description { get; set; }

		public bool archived { get; set; }

		public bool released { get; set; }

		public DateTime releaseDate { get; set; }

		public bool overdue { get; set; }

		public DateTime userReleaseDate { get; set; }
	}
}
