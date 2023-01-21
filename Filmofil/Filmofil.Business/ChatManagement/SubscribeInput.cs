using System.Collections.Generic;

namespace Filmofil.Business.ChatManagement
{
    public class SubscribeInput
    {
        public string ConnectionId { get; set; }
        public List<long> Categories { get; set; }
    }
}
