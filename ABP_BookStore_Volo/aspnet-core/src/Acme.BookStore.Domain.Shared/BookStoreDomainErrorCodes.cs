namespace Acme.BookStore;

public static class BookStoreDomainErrorCodes
{
    public const string AuthorAlreadyExists = "BookStore:00001";
    public const string UserInfoAlreadyExists = "BookStore:00002";
    public const string UserAlreadyExists = "BookStore:00003";
    public const string UserHasUserInfoAlready = "BookStore:00004";
}
