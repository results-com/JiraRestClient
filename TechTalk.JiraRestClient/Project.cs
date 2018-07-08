using System.Collections.Generic;

namespace TechTalk.JiraRestClient
{
	public class Project
	{
		public Project()
		{
			avatarUrls = new Dictionary<string, string>();
		}

		public Dictionary<string, string> avatarUrls { get; set; }

		public string id { get; set; }

		public string key { get; set; }

		public string name { get; set; }
	}
}
