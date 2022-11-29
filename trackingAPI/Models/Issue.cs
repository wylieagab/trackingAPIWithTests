using System.ComponentModel.DataAnnotations;

namespace trackingAPI.Models
{
    public class Issue
    {
        public Issue(int id, string title, string description, Priority priority, IssueType issueType, DateTime created, DateTime? completed)
        {
            Id = id;
            Title = title;
            Description = description;
            Priority = priority;
            IssueType = issueType;
            Created = created;
            Completed = completed;
        }

        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public Priority Priority { get; set; } 
        public IssueType IssueType { get; set;  }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }

        
    }

    public enum Priority
    {
        Low, Medium, High
    }

    public enum IssueType
    {
        Feature, Bug, Documentation
    }

}
