using FBT.Models;

namespace FBT.Repository
{
    public class Account
    {
        private readonly FbtContext _context;

        public Account (FbtContext context)
        {
            _context = context;
        }

    }
}
