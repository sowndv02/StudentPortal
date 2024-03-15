namespace SEP_Management.Models
{
    public class RoleClaim
    {
        public int Id { set; get; }
        public string ClaimType { set; get; }
        public string ClaimValue { set; get; }
        public Role Role { set; get; }
    }
}
