using System.Runtime.CompilerServices;
using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}