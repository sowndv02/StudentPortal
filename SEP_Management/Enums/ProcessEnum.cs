namespace SEP_Management.Enums
{
    public class ProcessEnum
    {
        public string DisplayProcessName { get; set; }
        public int ActiveKey { get; set; }
        public static List<ProcessEnum> GetListActive()
        {
            List<ProcessEnum> list = new List<ProcessEnum>();
            list.Add(new ProcessEnum { DisplayProcessName = "All", ActiveKey = -1 });
            list.Add(new ProcessEnum { DisplayProcessName = "Close", ActiveKey = 0 });
            list.Add(new ProcessEnum { DisplayProcessName = "Open", ActiveKey = 1 });
            return list.OrderBy(x => x.ActiveKey).ToList();
        }
    }
}
