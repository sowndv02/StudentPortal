namespace SEP_Management.Enums
{
    public class IssueTypeEnum
    {
        public string DisplayActiveName { get; set; }
        public int ActiveKey { get; set; }
        public static List<IssueTypeEnum> GetListActive()
        {
            List<IssueTypeEnum> list = new List<IssueTypeEnum>();
            list.Add(new IssueTypeEnum { DisplayActiveName = "Issue", ActiveKey = 0 });
            list.Add(new IssueTypeEnum { DisplayActiveName = "Incident", ActiveKey = 1 });
            return list.OrderBy(x => x.ActiveKey).ToList();
        }
    }
}
