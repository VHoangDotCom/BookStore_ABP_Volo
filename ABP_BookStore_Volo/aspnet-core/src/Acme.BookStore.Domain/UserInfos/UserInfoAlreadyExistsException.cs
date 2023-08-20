using Volo.Abp;

namespace Acme.BookStore.UserInfos
{
    public class UserInfoAlreadyExistsException : BusinessException
    {
        public UserInfoAlreadyExistsException(string fullName)
            :base(BookStoreDomainErrorCodes.UserInfoAlreadyExists)
        {
            WithData("fullName", fullName);
        }
    }
}
