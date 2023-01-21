using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Filmofil.Domain.Entities
{
    class Follow : IEntity
    {
        public long Id { get; set; }

        public long FollowerId { get; set; }

        public long FollowingId { get; set; }

    }
}
