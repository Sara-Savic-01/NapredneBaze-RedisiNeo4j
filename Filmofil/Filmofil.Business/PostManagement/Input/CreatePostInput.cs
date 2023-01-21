namespace Filmofil.Business.PostManagement.Input
{
    public class CreatePostInput
    {
        public string Content { get; set; }
        public string CategorieTitle { get; set; }
        public long AuthorId { get; set; }
    }
}
