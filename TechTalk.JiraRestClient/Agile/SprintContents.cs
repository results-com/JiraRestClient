using System.Collections.Generic;

namespace TechTalk.JiraRestClient.Agile
{
	public class SprintContents
	{
		public SprintContents()
		{
			completedIssues = new List<SprintIssue>();
			incompletedIssues = new List<SprintIssue>();
			puntedIssues = new List<SprintIssue>();
			issueKeysAddedDuringSprint = new Dictionary<string, string>();
			issuesNotCompletedInCurrentSprint = new List<SprintIssue>();
			issuesCompletedInAnotherSprint = new List<SprintIssue>();
		}

		public SprintStatisticEstimateSum completedIssuesEstimateSum { get; set; }

		public SprintStatisticEstimateSum incompletedIssuesEstimateSum { get; set; }

		public SprintStatisticEstimateSum allIssuesEstimateSum { get; set; }

		public SprintStatisticEstimateSum puntedIssuesEstimateSum { get; set; }

		public List<SprintIssue> completedIssues { get; set; }

		public List<SprintIssue> incompletedIssues { get; set; }

		public List<SprintIssue> puntedIssues { get; set; }

		public Dictionary<string, string> issueKeysAddedDuringSprint { get; set; }

		/// <remarks>
		/// Newer APIs uses this. Try first, then <see cref="incompletedIssues"/>.
		/// </remarks>
		public List<SprintIssue> issuesNotCompletedInCurrentSprint { get; set; }

		/// <remarks>
		/// Newer APIs method.
		/// </remarks>
		public List<SprintIssue> issuesCompletedInAnotherSprint { get; set; }

		/// <remarks>
		/// Newer APIs method.
		/// </remarks>
		public SprintStatisticEstimateSum completedIssuesInitialEstimateSum { get; set; }

		/// <remarks>
		/// Newer APIs method.
		/// </remarks>
		public SprintStatisticEstimateSum issuesNotCompletedInitialEstimateSum { get; set; }

		/// <remarks>
		/// Newer APIs uses this. Try first, then <see cref="incompletedIssuesEstimateSum"/>.
		/// </remarks>
		public SprintStatisticEstimateSum issuesNotCompletedEstimateSum { get; set; }

		/// <remarks>
		/// Newer APIs method.
		/// </remarks>
		public SprintStatisticEstimateSum puntedIssuesInitialEstimateSum { get; set; }

		/// <remarks>
		/// Newer APIs method.
		/// </remarks>
		public SprintStatisticEstimateSum issuesCompletedInAnotherSprintInitialEstimateSum { get; set; }

		/// <remarks>
		/// Newer APIs method.
		/// </remarks>
		public SprintStatisticEstimateSum issuesCompletedInAnotherSprintEstimateSum { get; set; }
	}
}