using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filmofil.Business.UserManagement.Input
{
    public class FollowUnfollowInput
    {
        public long FollowerId { get; set; }
        public long FollowingId { get; set; }
    }
}
