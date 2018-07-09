using System;
using System.Collections.Generic;
using TechTalk.JiraRestClient.Agile;

namespace TechTalk.JiraRestClient
{
	public partial interface IJiraClient<TIssueFields>
	{
		/// <summary>Returns all issues of the specified type for the given project</summary>
		IEnumerable<Issue<TIssueFields>> GetIssues(string projectKey, IEnumerable<string> issueTypes, IEnumerable<string> assignees = null, string parentId = null, Func<Issue<TIssueFields>, bool> filter = null);

		/// <summary>Returns a count of all issues of the specified type for the given project</summary>
		int GetIssueCount(string projectKey, string issueType = null, IEnumerable<string> assignees = null, string parentId = null, Func<Issue<TIssueFields>, bool> filter = null);

		/// <summary>Returns a count of all issues of the specified type for the given project</summary>
		int GetIssueCount(string projectKey, IEnumerable<string> issueTypes = null, IEnumerable<string> assignees = null, string parentId = null, Func<Issue<TIssueFields>, bool> filter = null);

		/// <summary>Returns all statuses</summary>
		IEnumerable<Status> GetStatuses();

		IEnumerable<Dashboard> GetDashboards(string projectKey);

		IEnumerable<Project> GetProjects();

		IEnumerable<Version> GetVersions(string projectKey);

		VersionRelatedIssueCounts GetVersionRelatedIssueCounts(string versionId);

		VersionUnresolvedIssueCount GetVersionUnresolvedIssueCount(string versionId);

		IEnumerable<Sprint> GetSprints(int rapidViewId, bool includeFutureSprints = true, bool includeHistoricSprints = true);

		IEnumerable<RapidView> GetRapidViews();

		SprintReport GetCompletedSprintStatistics(int rapidViewId, int sprintId);

		IEnumerable<Field> GetFields();

		/// <summary>Returns all users</summary>
		IEnumerable<User> GetUsers();
	}
}
