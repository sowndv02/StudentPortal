namespace SEP_Management.Models
{
    public partial class Issue
    {
        public Issue()
        {
            IssueUpdates = new HashSet<IssueUpdate>();
            IssueLabels = new HashSet<IssueLabel>();
        }

        public int IssueId { get; set; }
        public int? ProjectId { get; set; }
        public int? MilestoneId { get; set; }
        public string? IssueName { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? IssueType { get; set; }
        public string? AssigneeId { get; set; }
        public int? StatusId { get; set; }
        public int? ProcessId { get; set; }
        public int? GitlabId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Milestone? Milestone { get; set; }
        public virtual Project? Project { get; set; }
        public virtual User? Assignee { get; set; }
        public virtual ICollection<IssueUpdate> IssueUpdates { get; set; }
        public virtual ICollection<IssueLabel> IssueLabels { get; set; }
    }
}
