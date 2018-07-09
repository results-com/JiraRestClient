using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RestSharp;
using RestSharp.Deserializers;

namespace TechTalk.JiraRestClient.Converters
{
	public class CustomFieldSupportedDeserialiser
		: IDeserializer
	{
		private JsonDeserializer DefaultDeserialiser { get; set; }

		public CustomFieldSupportedDeserialiser()
		{
			DefaultDeserialiser = new JsonDeserializer();
		}

		#region Implementation of IDeserializer

		public T Deserialize<T>(IRestResponse response)
		{
			T result = DefaultDeserialiser.Deserialize<T>(response);
			if (typeof (T).IsGenericType)
			{
				Type genericListType = typeof (IssueContainer<>);
				Type parameterType = (typeof (T)).GetGenericArguments()[0];
				if (typeof (IssueFields).IsAssignableFrom(parameterType))
				{
					genericListType = genericListType.MakeGenericType(parameterType);

					if (genericListType.IsAssignableFrom(typeof (T)))
					{
						PropertyInfo issuesPropertyInfo = genericListType.GetProperty("issues");
						var issues = (IEnumerable<object>) issuesPropertyInfo.GetValue(result, null);
						if (!issues.Any())
							return result;

						var actualResult = DefaultDeserialiser.Deserialize<Dictionary<string, object>>(response);
						var actualIssues = ((JsonArray)actualResult["issues"]);
						PropertyInfo fieldsPropertyInfo = null;
						PropertyInfo idPropertyInfo = null;
						foreach (object issueField in issues)
						{
							fieldsPropertyInfo = fieldsPropertyInfo ?? issueField.GetType().GetProperty("fields");
							idPropertyInfo = idPropertyInfo ?? issueField.GetType().GetProperty("id");
							string id = (string) idPropertyInfo.GetValue(issueField, null);
							var issueFields = (IssueFields) fieldsPropertyInfo.GetValue(issueField, null);

							foreach (IDictionary<string, object> issue in actualIssues)
							{
								if ((string)issue["id"] != id)
									continue;

								issueFields.customFields = new Dictionary<int, object>();

								var fields = (IDictionary<string, object>)issue["fields"];
								const string propertyPrefix = "customfield_";
								foreach (string propertyName in fields.Keys.Where(propertyName => propertyName.StartsWith(propertyPrefix)))
								{
									int customFieldId = int.Parse(propertyName.Substring(propertyPrefix.Length));
									issueFields.customFields.Add(customFieldId, fields[propertyName]);
								}
								break;
							}
						}
					}
				}
			}
			return result;
		}

		public string RootElement
		{
			get { return DefaultDeserialiser.RootElement; }
			set { DefaultDeserialiser.RootElement = value; }
		}

		public string Namespace
		{
			get { return DefaultDeserialiser.Namespace; }
			set { DefaultDeserialiser.Namespace = value; }
		}

		public string DateFormat
		{
			get { return DefaultDeserialiser.DateFormat; }
			set { DefaultDeserialiser.DateFormat = value; }
		}

		#endregion
	}
}
