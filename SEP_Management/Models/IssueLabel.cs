namespace SEP_Management.Models
{
    public class IssueLabel
    {
        public int IssueLabelId { get; set; }
        public int IssueId { get; set; }
        public int LabelId { get; set; }
        public short? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public short? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Issue? Issue { get; set; } = null!;
        public virtual IssueSetting? Label { get; set; } = null!;
    }
}
