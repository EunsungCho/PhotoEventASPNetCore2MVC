using System;
using System.Collections.Generic;

namespace PhotoEventTest.Models
{
    public partial class EventUserPhotos
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
        public string UserIdToVote { get; set; }
        public byte[] ThumbnailPhoto { get; set; }
        public byte[] Photo { get; set; }

        public int? VoteScore { get; set; }
        public DateTime? PhotoUploadDate { get; set; }
        public string PhotoTitle { get; set; }

        public virtual Events Event { get; set; }
        public virtual Users User { get; set; }
    }
}
