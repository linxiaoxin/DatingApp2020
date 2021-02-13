namespace API.Helper
{
    public class LikesParams: PaginationParams
    {
        public string Predicate { get; set; } = "like";

        public int UserId { get; set; }
    }
}