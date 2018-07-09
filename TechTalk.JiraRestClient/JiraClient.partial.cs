using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using RestSharp;
using TechTalk.JiraRestClient.Agile;

namespace TechTalk.JiraRestClient
{
	public partial class JiraClient<TIssueFields>
	{
		public IEnumerable<Issue<TIssueFields>> GetIssues(string projectKey, IEnumerable<string> issueTypes, IEnumerable<string> assignees = null, string parentId = null, Func<Issue<TIssueFields>, bool> filter = null)
		{
			return EnumerateIssues(projectKey, issueTypes, assignees, parentId, filter).ToList();
		}

		public IEnumerable<Issue<TIssueFields>> EnumerateIssues(string projectKey, IEnumerable<string> issueTypes, IEnumerable<string> assignees, string parentId, Func<Issue<TIssueFields>, bool> filter = null)
		{
			try
			{
				IEnumerable<Issue<TIssueFields>> result = EnumerateIssues(projectKey, issueTypes, assignees, parentId, false);
				if (filter != null)
					result = result.Where(filter);
				return result;
			}
			catch (Exception ex)
			{
				Trace.TraceError("EnumerateIssues(projectKey, issueType) error: {0}", ex);
				throw new JiraClientException("Could not load issues", ex);
			}
		}

		public IEnumerable<Issue<TIssueFields>> EnumerateIssues(string projectKey, IEnumerable<string> issueTypes, IEnumerable<string> assignees, string parentId, bool keyFieldsOnly = false)
		{
			return EnumerateIssuesByQuery(CreateCommonJql(projectKey, issueTypes, assignees, parentId), null, 0);
		}

		private string CreateCommonJql(string projectKey, IEnumerable<string> issueTypes, IEnumerable<string> assignees, string parentId)
		{
			var jql = string.Format("project={0}", projectKey);
			var issueTypeList = issueTypes == null ? new List<string>() : issueTypes.ToList();
			var assigneesList = assignees == null ? new List<string>() : assignees.ToList();
			if (issueTypeList.Any())
				jql = string.Format("{0} AND issueType IN ({1})", jql, string.Join(",", issueTypeList));
			if (assigneesList.Any())
				jql = string.Format("{0} AND assignee IN ({1})", jql, string.Join(",", assigneesList));
			if (!string.IsNullOrWhiteSpace(parentId))
				jql = string.Format("{0} AND cf[{1}]={2}", jql, GetFields().Single(field => field.name == "Epic Link").schema["customId"], parentId);
			return jql;
		}

		public int GetIssueCount(string projectKey, string issueType, IEnumerable<string> assignees = null, string parentId = null, Func<Issue<TIssueFields>, bool> filter = null)
		{
			return GetIssueCount(projectKey, issueType == null ? null : new[] { issueType }, assignees, parentId, filter);
		}

		public int GetIssueCount(string projectKey, IEnumerable<string> issueTypes = null, IEnumerable<string> assignees = null, string parentId = null, Func<Issue<TIssueFields>, bool> filter = null)
		{
			return TotalIssuesInternal(projectKey, issueTypes, assignees, parentId, filter);
		}

		private int TotalIssuesInternal(string projectKey, IEnumerable<string> issueTypes, IEnumerable<string> assignees, string parentId, Func<Issue<TIssueFields>, bool> filter)
		{
			if (filter != null)
			{
				return EnumerateIssues(projectKey, issueTypes, assignees, parentId, true)
					.Count(filter);
			}

			const int queryCount = 1;
			const int queryStart = 0;
			while (true)
			{
				var jql = string.Format("project={0}", projectKey);
				if (issueTypes != null && issueTypes.Any())
					jql += string.Format(" AND issueType IN({0})", issueTypes);
				if (!string.IsNullOrWhiteSpace(parentId))
					jql = string.Format("{0} AND cf[{1}]={2}", jql, GetFields().Single(field => field.name == "Epic Link").schema["customId"], parentId);
				var path = string.Format("search?jql={0}&startAt={1}&maxResults={2}", Uri.EscapeUriString(jql), queryStart, queryCount);
				RestRequest request = CreateRequest(Method.GET, path);

				IRestResponse response = ExecuteRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<IssueContainer<TIssueFields>>(response);
				return data.total;
			}
		}

		/// <summary>Returns all statuses</summary>
		public IEnumerable<Status> GetStatuses()
		{
			try
			{
				var request = CreateRequest(Method.GET, "status");
				request.AddHeader("ContentType", "application/json");

				IRestResponse response = ExecuteRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<List<Status>>(response);
				return data;

			}
			catch (Exception ex)
			{
				Trace.TraceError("GetStatuses() error: {0}", ex);
				throw new JiraClientException("Could not load statuses", ex);
			}
		}

		public IEnumerable<Dashboard> GetDashboards(string projectKey)
		{
			var queryCount = 50;
			var resultCount = 0;
			while (true)
			{
				var jql = string.Format("filter={0}", Uri.EscapeUriString(projectKey));
				var path = string.Format("dashboard?jql={0}&startAt={1}&maxResults={2}", jql, resultCount, queryCount);
				RestRequest request = CreateRequest(Method.GET, path);

				IRestResponse response = ExecuteRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<List<Dashboard>>(response);
				return data;
			}
		}

		public IEnumerable<Project> GetProjects()
		{
			var queryCount = 50;
			var resultCount = 0;
			while (true)
			{
				var path = string.Format("project?startAt={0}&maxResults={1}", resultCount, queryCount);
				RestRequest request = CreateRequest(Method.GET, path);

				IRestResponse response = ExecuteRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<List<Project>>(response);
				return data;
			}
		}

		public IEnumerable<Version> GetVersions(string projectKey)
		{
			var queryCount = 50;
			var resultCount = 0;
			while (true)
			{
				var path = string.Format("project/{0}/versions?startAt={1}&maxResults={2}", projectKey, resultCount, queryCount);
				RestRequest request = CreateRequest(Method.GET, path);

				IRestResponse response = ExecuteRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<List<Version>>(response);
				return data;
			}
		}

		public VersionRelatedIssueCounts GetVersionRelatedIssueCounts(string versionId)
		{
			var queryCount = 50;
			var resultCount = 0;
			while (true)
			{
				var path = string.Format("version/{0}/relatedIssueCounts?startAt={1}&maxResults={2}", versionId, resultCount, queryCount);
				RestRequest request = CreateRequest(Method.GET, path);

				IRestResponse response = ExecuteRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<VersionRelatedIssueCounts>(response);
				return data;
			}
		}

		public VersionUnresolvedIssueCount GetVersionUnresolvedIssueCount(string versionId)
		{
			var queryCount = 50;
			var resultCount = 0;
			while (true)
			{
				var path = string.Format("version/{0}/unresolvedIssueCount?startAt={1}&maxResults={2}", versionId, resultCount, queryCount);
				RestRequest request = CreateRequest(Method.GET, path);

				IRestResponse response = ExecuteRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<VersionUnresolvedIssueCount>(response);
				return data;
			}
		}

		private IRestResponse ExecuteSprintRequest(RestRequest request)
		{
			var client = new RestClient(baseApiUrl.Replace("rest/api/2/", "rest/greenhopper/1.0/"));
			return client.Execute(request);
		}

		public IEnumerable<Sprint> GetSprints(int rapidViewId, bool includeFutureSprints = true, bool includeHistoricSprints = true)
		{
			var queryCount = 50;
			var resultCount = 0;
			while (true)
			{
				var path = string.Format("sprintquery/{0}?includeFutureSprints={1}&includeHistoricSprints={2}&startAt={3}&maxResults={4}", rapidViewId, includeFutureSprints, includeHistoricSprints, resultCount, queryCount);
				RestRequest request = CreateRequest(Method.GET, path);

				IRestResponse response = ExecuteSprintRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<SprintCollection>(response);
				return data.sprints;
			}
		}

		public IEnumerable<RapidView> GetRapidViews()
		{
			var queryCount = 50;
			var resultCount = 0;
			while (true)
			{
				var path = string.Format("rapidviews/list?startAt={0}&maxResults={1}", resultCount, queryCount);
				RestRequest request = CreateRequest(Method.GET, path);

				IRestResponse response = ExecuteSprintRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<RapidViewCollection>(response);
				return data.views;
			}
		}

		public SprintReport GetCompletedSprintStatistics(int rapidViewId, int sprintId)
		{
			var queryCount = 50;
			var resultCount = 0;
			while (true)
			{
				var path = string.Format("rapid/charts/sprintreport?rapidViewId={0}&sprintId={1}&startAt={2}&maxResults={3}", rapidViewId, sprintId, resultCount, queryCount);
				RestRequest request = CreateRequest(Method.GET, path);

				IRestResponse response = ExecuteSprintRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<SprintReport>(response);
				return data;
			}
		}

		public IEnumerable<Field> GetFields()
		{
			try
			{
				var request = CreateRequest(Method.GET, "field");
				request.AddHeader("ContentType", "application/json");

				IRestResponse response = ExecuteRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<List<Field>>(response);
				return data;

			}
			catch (Exception ex)
			{
				Trace.TraceError("GetFields() error: {0}", ex);
				throw new JiraClientException("Could not load fields", ex);
			}
		}

		/// <summary>Returns all users</summary>
		public IEnumerable<User> GetUsers()
		{
			try
			{
				var request = CreateRequest(Method.GET, "user/bulk");
				request.AddHeader("ContentType", "application/json");

				IRestResponse response = ExecuteRequest(request);
				AssertStatus(response, HttpStatusCode.OK);

				var data = deserializer.Deserialize<List<User>>(response);
				return data;

			}
			catch (Exception ex)
			{
				Trace.TraceError("GetUsers() error: {0}", ex);
				throw new JiraClientException("Could not load users", ex);
			}
		}
	}
}