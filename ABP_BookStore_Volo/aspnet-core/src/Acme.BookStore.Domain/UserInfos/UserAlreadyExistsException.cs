using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.BookStore.UserInfos
{
    public class UserAlreadyExistsException : BusinessException
    {
        public UserAlreadyExistsException(string fullName)
            :base(BookStoreDomainErrorCodes.UserAlreadyExists)
        {
            WithData("firstName" + "lastName", fullName);
        }
    }
}
