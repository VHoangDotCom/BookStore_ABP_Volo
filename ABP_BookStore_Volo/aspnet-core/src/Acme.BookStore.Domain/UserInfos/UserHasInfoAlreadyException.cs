using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.BookStore.UserInfos
{
    public class UserHasInfoAlreadyException : BusinessException
    {
        public UserHasInfoAlreadyException(string username)
            : base(BookStoreDomainErrorCodes.UserHasUserInfoAlready)
        {
            WithData("username", username);
        }
    }
}
