using System.Collections.Generic;

namespace Service.ResponseModels
{
    public class CurrentUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public IList<string> Role { get; set; }
    }
}
