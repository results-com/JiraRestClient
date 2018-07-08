using System;

namespace TechTalk.JiraRestClient.Agile
{
	public class Sprint
	{
		public int id { get; set; } 

		public int sequence { get; set; } 

		public string name { get; set; }

		public SprintStateType state { get; set; }

		public int linkedPagesCount { get; set; }

		public DateTime? startDate { get; set; }

		public DateTime? endDate { get; set; }

		public DateTime? completedDate { get; set; }

		public int daysRemaining { get; set; }
	}
}