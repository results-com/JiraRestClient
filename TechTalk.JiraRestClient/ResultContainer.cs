using System.Collections.Generic;

namespace TechTalk.JiraRestClient
{
    internal class ResultContainer<T> where T : new()
    {
        public int maxResults { get; set; }
        public int total { get; set; }
		public int startAt { get; set; }
		public bool isLast { get; set; }

        public List<T> values { get; set; }
    }
}
