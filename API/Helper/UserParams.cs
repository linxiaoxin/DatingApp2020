namespace API.Helper
{
    public class UserParams: PaginationParams
    {
        public string Gender { get; set; }

        public int MaxAge { get; set; } = 150;

        public int MinAge { get; set; } = 19;

        public string UserName { get; set; }

        public string OrderBy { get; set; } = "lastActive";
    }
}