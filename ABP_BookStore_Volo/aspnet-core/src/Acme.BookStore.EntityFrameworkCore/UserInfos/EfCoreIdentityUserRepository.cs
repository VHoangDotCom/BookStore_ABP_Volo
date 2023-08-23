using Acme.BookStore.EntityFrameworkCore;
using Acme.BookStore.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace Acme.BookStore.UserInfos
{
    public class EfCoreIdentityUserRepository 
        : EfCoreRepository<BookStoreDbContext, IdentityUser, Guid>,
         IUserRepository
    {
        public EfCoreIdentityUserRepository(
            IDbContextProvider<BookStoreDbContext> dbContextProvider) 
            : base(dbContextProvider)
        {
        }

        public async Task<IdentityUser> FindByIdAsync(Guid id)
        {
            var dbSet = await GetDbSetAsync();
            return dbSet.FirstOrDefault(x => x.Id == id);
        }
    }
}
