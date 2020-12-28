using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromGemini.Jira.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Issuetype
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
        public int avatarId { get; set; }
    }


public class Project
{
    public string self { get; set; }
    public string id { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public string projectTypeKey { get; set; }
    public bool simplified { get; set; }
}

public class FixVersion
{
    public string self { get; set; }
    public string id { get; set; }
    public string description { get; set; }
    public string name { get; set; }
    public bool archived { get; set; }
    public bool released { get; set; }
    public string releaseDate { get; set; }
}

public class Watches
{
    public string self { get; set; }
    public int watchCount { get; set; }
    public bool isWatching { get; set; }
}

public class Issuerestrictions
{
}

public class Issuerestriction
{
    public Issuerestrictions issuerestrictions { get; set; }
    public bool shouldDisplay { get; set; }
}

public class Color
{
    public string key { get; set; }
}

public class Epic
{
    public int id { get; set; }
    public string key { get; set; }
    public string self { get; set; }
    public string name { get; set; }
    public string summary { get; set; }
    public Color color { get; set; }
    public bool done { get; set; }
}

public class Priority
{
    public string self { get; set; }
    public string iconUrl { get; set; }
    public string name { get; set; }
    public string id { get; set; }
}

public class NonEditableReason
{
    public string reason { get; set; }
    public string message { get; set; }
}

public class Customfield10018
{
    public bool hasEpicLinkFieldDependency { get; set; }
    public bool showField { get; set; }
    public NonEditableReason nonEditableReason { get; set; }
}


    public class Assignee
{
    public string self { get; set; }
    public string accountId { get; set; }
    public string emailAddress { get; set; }
    public string displayName { get; set; }
    public bool active { get; set; }
    public string timeZone { get; set; }
    public string accountType { get; set; }
}

public class StatusCategory
{
    public string self { get; set; }
    public int id { get; set; }
    public string key { get; set; }
    public string colorName { get; set; }
    public string name { get; set; }
}

public class Status
{
    public string self { get; set; }
    public string description { get; set; }
    public string iconUrl { get; set; }
    public string name { get; set; }
    public string id { get; set; }
    public StatusCategory statusCategory { get; set; }
}

public class Component
{
    public string self { get; set; }
    public string id { get; set; }
    public string name { get; set; }
}

public class Timetracking
{
    public string originalEstimate { get; set; }
    public string remainingEstimate { get; set; }
    public string timeSpent { get; set; }
    public int originalEstimateSeconds { get; set; }
    public int remainingEstimateSeconds { get; set; }
    public int timeSpentSeconds { get; set; }
}


    public class Author
{
    public string self { get; set; }
    public string accountId { get; set; }
    public string emailAddress { get; set; }
    public string displayName { get; set; }
    public bool active { get; set; }
    public string timeZone { get; set; }
    public string accountType { get; set; }
}

public class Attachment
{
    public string self { get; set; }
    public string id { get; set; }
    public string filename { get; set; }
    public Author author { get; set; }
    public DateTime created { get; set; }
    public int size { get; set; }
    public string mimeType { get; set; }
    public string content { get; set; }
}


    public class Creator
{
    public string self { get; set; }
    public string accountId { get; set; }
    public string emailAddress { get; set; }
    public string displayName { get; set; }
    public bool active { get; set; }
    public string timeZone { get; set; }
    public string accountType { get; set; }
}

public class StatusCategory2
{
    public string self { get; set; }
    public int id { get; set; }
    public string key { get; set; }
    public string colorName { get; set; }
    public string name { get; set; }
}

public class Status2
{
    public string self { get; set; }
    public string description { get; set; }
    public string iconUrl { get; set; }
    public string name { get; set; }
    public string id { get; set; }
    public StatusCategory2 statusCategory { get; set; }
}

public class Priority2
{
    public string self { get; set; }
    public string iconUrl { get; set; }
    public string name { get; set; }
    public string id { get; set; }
}

public class Issuetype2
{
    public string self { get; set; }
    public string id { get; set; }
    public string description { get; set; }
    public string iconUrl { get; set; }
    public string name { get; set; }
    public bool subtask { get; set; }
    public int avatarId { get; set; }
}

public class Fields2
{
    public string summary { get; set; }
    public Status2 status { get; set; }
    public Priority2 priority { get; set; }
    public Issuetype2 issuetype { get; set; }
}

public class Subtask
{
    public string id { get; set; }
    public string key { get; set; }
    public string self { get; set; }
    public Fields2 fields { get; set; }
}


    public class Reporter
{
    public string self { get; set; }
    public string accountId { get; set; }
    public string emailAddress { get; set; }
    public string displayName { get; set; }
    public bool active { get; set; }
    public string timeZone { get; set; }
    public string accountType { get; set; }
}

public class Aggregateprogress
{
    public int progress { get; set; }
    public int total { get; set; }
    public int percent { get; set; }
}

public class Progress
{
    public int progress { get; set; }
    public int total { get; set; }
    public int percent { get; set; }
}

public class Comment
{
    public List<object> comments { get; set; }
    public int maxResults { get; set; }
    public int total { get; set; }
    public int startAt { get; set; }
}

public class Votes
{
    public string self { get; set; }
    public int votes { get; set; }
    public bool hasVoted { get; set; }
}

public class Author2
{
    public string self { get; set; }
    public string accountId { get; set; }
    public string emailAddress { get; set; }
    public string displayName { get; set; }
    public bool active { get; set; }
    public string timeZone { get; set; }
    public string accountType { get; set; }
}


    public class UpdateAuthor
{
    public string self { get; set; }
    public string accountId { get; set; }
    public string emailAddress { get; set; }
    public string displayName { get; set; }
    public bool active { get; set; }
    public string timeZone { get; set; }
    public string accountType { get; set; }
}

public class Worklog2
{
    public string self { get; set; }
    public Author2 author { get; set; }
    public UpdateAuthor updateAuthor { get; set; }
    public DateTime created { get; set; }
    public DateTime updated { get; set; }
    public DateTime started { get; set; }
    public string timeSpent { get; set; }
    public int timeSpentSeconds { get; set; }
    public string id { get; set; }
    public string issueId { get; set; }
}

public class Worklog
{
    public int startAt { get; set; }
    public int maxResults { get; set; }
    public int total { get; set; }
    public List<Worklog2> worklogs { get; set; }
}

public class Fields
{
    public DateTime statuscategorychangedate { get; set; }
    public Issuetype issuetype { get; set; }
    public int timespent { get; set; }
    public object sprint { get; set; }
    public string customfield_10030 { get; set; }
    public object customfield_10031 { get; set; }
    public Project project { get; set; }
    public List<FixVersion> fixVersions { get; set; }
    public double customfield_10033 { get; set; }
    public string customfield_10034 { get; set; }
    public int aggregatetimespent { get; set; }
    public object resolution { get; set; }
    public string customfield_10035 { get; set; }
    public string customfield_10036 { get; set; }
    public string customfield_10037 { get; set; }
    public object customfield_10028 { get; set; }
    public object resolutiondate { get; set; }
    public int workratio { get; set; }
    public Watches watches { get; set; }
    public DateTime lastViewed { get; set; }
    public Issuerestriction issuerestriction { get; set; }
    public DateTime created { get; set; }
    public object customfield_10020 { get; set; }
    public object customfield_10021 { get; set; }
    public Epic epic { get; set; }
    public object customfield_10022 { get; set; }
    public object customfield_10023 { get; set; }
    public Priority priority { get; set; }
    public object customfield_10024 { get; set; }
    public object customfield_10025 { get; set; }
    public List<object> labels { get; set; }
    public object customfield_10016 { get; set; }
    public object customfield_10017 { get; set; }
    public Customfield10018 customfield_10018 { get; set; }
    public string customfield_10019 { get; set; }
    public int aggregatetimeoriginalestimate { get; set; }
    public int timeestimate { get; set; }
    public List<object> versions { get; set; }
    public List<object> issuelinks { get; set; }
    public Assignee assignee { get; set; }
    public DateTime updated { get; set; }
    public Status status { get; set; }
    public List<Component> components { get; set; }
    public int timeoriginalestimate { get; set; }
    public object description { get; set; }
    public object customfield_10010 { get; set; }
    public string customfield_10014 { get; set; }
    public Timetracking timetracking { get; set; }
    public string customfield_10015 { get; set; }
    public object customfield_10005 { get; set; }
    public object customfield_10006 { get; set; }
    public object security { get; set; }
    public object customfield_10007 { get; set; }
    public object customfield_10008 { get; set; }
    public object customfield_10009 { get; set; }
    public List<Attachment> attachment { get; set; }
    public int aggregatetimeestimate { get; set; }
    public bool flagged { get; set; }
    public string summary { get; set; }
    public Creator creator { get; set; }
    public List<Subtask> subtasks { get; set; }
    public Reporter reporter { get; set; }
    public Aggregateprogress aggregateprogress { get; set; }
    public string customfield_10000 { get; set; }
    public object customfield_10001 { get; set; }
    public object customfield_10002 { get; set; }
    public object customfield_10003 { get; set; }
    public object customfield_10004 { get; set; }
    public object environment { get; set; }
    public string duedate { get; set; }
    public Progress progress { get; set; }
    public Comment comment { get; set; }
    public Votes votes { get; set; }
    public Worklog worklog { get; set; }
}



public class JiraErmIssue
{
    public string expand { get; set; }
    public string id { get; set; }
    public string self { get; set; }
    public string key { get; set; }
    public Fields fields { get; set; }
}

public class Update
{
}
public class JiraErmAddIssue
{
    public Update update { get; set; }
    public Fields fields { get; set; }
}


}
