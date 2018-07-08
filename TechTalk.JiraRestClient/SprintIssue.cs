namespace TechTalk.JiraRestClient
{
	public class SprintIssue : IssueRef
	{
		public string summary { get; set; }

		public Status status { get; set; }
	}
}