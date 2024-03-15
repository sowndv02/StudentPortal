namespace SEP_Management.Enums
{
    public class ClassActiveEnum
    {
        public string DisplayActiveName { get; set; }
        public int ActiveKey { get; set; }
        public static List<ClassActiveEnum> GetListActive()
        {
            List<ClassActiveEnum> list = new List<ClassActiveEnum>();
            list.Add(new ClassActiveEnum { DisplayActiveName = "All", ActiveKey = -1 });
            list.Add(new ClassActiveEnum { DisplayActiveName = "Closed", ActiveKey = 0 });
            list.Add(new ClassActiveEnum { DisplayActiveName = "Started", ActiveKey = 1 });
            list.Add(new ClassActiveEnum { DisplayActiveName = "Pending", ActiveKey = 2 });
            return list.OrderBy(x => x.ActiveKey).ToList();
        }
    }
}
