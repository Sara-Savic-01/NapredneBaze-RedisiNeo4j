using System;
using System.Collections.Generic;
using System.Text;

namespace Filmofil.Business.PostManagement.Input
{
    public class CreateCommentInput
    {
        public string Content { get; set; }

        public long AuthorId { get; set; }

        public long PostId { get; set; }
    }
}
