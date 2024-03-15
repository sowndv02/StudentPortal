namespace SEP_Management.Models
{
    public partial class IssueSetting
    {
        public IssueSetting()
        {
            IssueLabels = new HashSet<IssueLabel>();
        }

        public int SettingId { get; set; }
        public int? SubjectId { get; set; }
        public int? ClassId { get; set; }
        public int? ProjectId { get; set; }
        public string? Title { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public int? GitlabId { get; set; }
        public string? Color { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public byte? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Class? Class { get; set; }
        public virtual Project? Project { get; set; }
        public virtual Subject? Subject { get; set; }
        public virtual ICollection<IssueLabel> IssueLabels { get; set; }
    }
}
