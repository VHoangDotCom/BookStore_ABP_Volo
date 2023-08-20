using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Acme.BookStore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace Acme.BookStore.UserInfos
{
    public class EfCoreUserInfoRepository
        : EfCoreRepository<BookStoreDbContext, UserInfo, Guid>,
         IUserInfoRepository
    {
        public EfCoreUserInfoRepository(
            IDbContextProvider<BookStoreDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
            
        }

        public async Task<List<UserInfo>> FindByJobAsync(JobType job)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x => x.Job == job && !x.IsDeleted).ToListAsync();
        }

        public async Task<UserInfo> FindByNameAsync(string fullname)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => (x.FirstName + " " + x.LastName).ToLower() == fullname);
        }

        public async Task<UserInfo> FindUserHasInfoAlready(Guid userID)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => x.UserId == userID);
        }

        public async Task<List<UserInfo>> GetListAsync(
            int skipCount, 
            int maxResultCount, 
            string sorting, 
            int? job,
            string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            int? jobInt = (int?)job;
            return await dbSet
                .WhereIf(
                    !filter.IsNullOrWhiteSpace(),
                    userInfo => userInfo.FirstName.Contains(filter)
                    )
                .WhereIf(job.HasValue, x => (int)x.Job == job.Value)
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }
    }
}
