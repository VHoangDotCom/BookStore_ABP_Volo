using Acme.BookStore.Authors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.UserInfos
{
    public interface IUserInfoRepository : IRepository<UserInfo, Guid>
    {
        Task<List<UserInfo>> FindByJobAsync(JobType jobType);
        Task<UserInfo> FindByNameAsync(string fullname);

        Task<UserInfo> FindUserHasInfoAlready(Guid userID);

        Task<List<UserInfo>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            int? jobType,
            string filter = null
        );
    }
}
