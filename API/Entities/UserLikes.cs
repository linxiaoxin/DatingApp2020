namespace API.Entities
{
    public class UserLikes
    {
        public AppUser likedByUser { get; set; }

        public int likedByUserId { get; set; }

        public AppUser likedUser { get; set; }  

        public int likedUserId { get; set; }
    }
}