using Filmofil.Domain.Entities;
using System.Collections.Generic;

namespace Filmofil.Business.ChatManagement
{
    public class LoadChatDataOutput
    {
        public List<Message> Messages { get; set; }
        public List<User> Users { get; set; }
    }
}
