namespace API.Helper
{
    public class UserParams
    {
        private int MaxPageSize = 50;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = _pageSize > MaxPageSize ? MaxPageSize : value;
        }
        public int PageNumber { get; set; } = 1;

        public string Gender { get; set; }

        public int MaxAge { get; set; } = 150;

        public int MinAge { get; set; } = 19;

        public string UserName { get; set; }

        public string OrderBy { get; set; } = "lastActive";
    }
}