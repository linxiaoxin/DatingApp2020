using API.Entities;

namespace API.DTO
{
    public class PhotoDTO
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool isMain { get; set; }
        
    }
}