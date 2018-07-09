namespace TechTalk.JiraRestClient
{
	public class User
	{
		public string self { get; set; }

		public string key { get; set; }

		public string accountId { get; set; }

		public string name { get; set; }

		public string emailAddress { get; set; }

		public string displayName { get; set; }

		public bool active { get; set; }

		public string timeZone { get; set; }
	}
}