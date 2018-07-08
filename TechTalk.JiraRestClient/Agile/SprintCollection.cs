using System.Collections.Generic;

namespace TechTalk.JiraRestClient.Agile
{
	public class SprintCollection
	{
		public int rapidViewId { get; set; }

		public List<Sprint> sprints { get; set; }
	}
}