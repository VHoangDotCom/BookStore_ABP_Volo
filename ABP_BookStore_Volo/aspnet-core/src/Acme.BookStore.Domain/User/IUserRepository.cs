using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Acme.BookStore.User
{
    public interface IUserRepository : IRepository<IdentityUser, Guid>
    {
        Task<IdentityUser> FindByIdAsync(Guid id);
    }
}
