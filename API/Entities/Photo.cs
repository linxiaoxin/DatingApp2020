using System;

namespace API.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool isMain { get; set; }
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }
}