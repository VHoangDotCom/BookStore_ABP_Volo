using Acme.BookStore.Authors;
using Acme.BookStore.Books;
using Acme.BookStore.UserInfos;
using AutoMapper;
using Volo.Abp.Identity;

namespace Acme.BookStore;

public class BookStoreApplicationAutoMapperProfile : Profile
{
    public BookStoreApplicationAutoMapperProfile()
    {
        CreateMap<Book, BookDto>();
        CreateMap<CreateUpdateBookDto, Book>();

        CreateMap<Author, AuthorDto>();
        CreateMap<Author, AuthorLookupDto>();

        CreateMap<UserInfo, UserInfoDto>();
        CreateMap<UserInfo, UpdateUserInfoDto>();
        CreateMap<UserInfo, CreateUserInfoDto>();
        CreateMap<IdentityUser, UserLookupDto>();
    }
}
