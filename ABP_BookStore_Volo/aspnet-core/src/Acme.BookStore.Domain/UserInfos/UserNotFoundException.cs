using System;
using Volo.Abp;

namespace Acme.BookStore.UserInfos
{
    public class UserNotFoundException : BusinessException
    {
        public UserNotFoundException(Guid id)
            : base(BookStoreDomainErrorCodes.UserAlreadyExists)
        {
            WithData("userId", id);
        }
    }
}
