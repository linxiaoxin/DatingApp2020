namespace API.Helper
{
    public class PaginationParams
    {
        private int MaxPageSize = 50;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = _pageSize > MaxPageSize ? MaxPageSize : value;
        }
        public int PageNumber { get; set; } = 1;

    }
}