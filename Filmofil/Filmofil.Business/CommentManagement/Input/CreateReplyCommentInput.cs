namespace Filmofil.Business.CommentManagement.Input
{
    public class CreateReplyCommentInput
    {
        public string Content { get; set; }
        public long PostId { get; set; }
        public long AuthorId { get; set; }
        public long ParentCommentId { get; set; }
        public string CategorieTitle { get; set; }
    }
}
