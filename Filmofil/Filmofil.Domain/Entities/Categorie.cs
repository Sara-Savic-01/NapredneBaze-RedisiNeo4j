using System.Collections.Generic;

namespace Filmofil.Domain.Entities
{
    public class Categorie : IEntity
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public List<long> Users { get; set; }
        public List<long> Posts { get; set; }
        public List<long> Messages { get; set; }
    }
}
