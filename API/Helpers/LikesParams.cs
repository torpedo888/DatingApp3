namespace API.Helpers
{
    public class LikesParams : PaginationParams
    {
        public int UserId { get; set; }

        //predicate: liked - ki lett lajkolva, likedby - ki likolta a belogolt usert. 
        public string Predicate { get; set; }

    }
}