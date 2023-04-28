namespace API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public string SenderUsername { get; set; }
        //amikor a user nezi a msg-et akkor mutatjuk a kuldo fotojat
        public string SenderPhotourl { get; set; }
        
        public int RecipientId { get; set; }

        public string RecipientUsername { get; set; }

        public string RecipientPhotoUrl { get; set; }

        public string Content { get; set; }

        public DateTime? DateRead { get; set; }
        
        public DateTime MessageSent { get; set; }
    }
}