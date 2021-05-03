using System;

namespace API.DTO
{
    public class PhotoForModerationDTO
    {
        public int id { get; set; }
        public string url { get; set; }
        public bool isApproved { get; set; }
        public DateTime? moderateDate { get; set; }
        public string userName { get; set; }
        public string knownAs { get; set; }

    }
}