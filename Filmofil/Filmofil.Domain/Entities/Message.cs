namespace Filmofil.Domain.Entities
{
    public class Message : IEntity
    {
        public long Id { get; set; }
        public long Sender { get; set; }
        public long Categorie { get; set; }
        public string Content { get; set; }
    }
}
