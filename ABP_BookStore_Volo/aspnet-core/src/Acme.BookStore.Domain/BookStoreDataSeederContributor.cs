using Acme.BookStore.Authors;
using Acme.BookStore.Books;
using Acme.BookStore.UserInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore
{
    public class BookStoreDataSeederContributor
     : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Book, Guid> _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly AuthorManager _authorManager;
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly UserInfoManager _userInfoManager;

        public BookStoreDataSeederContributor(
            IRepository<Book, Guid> bookRepository,
            IAuthorRepository authorRepository,
            AuthorManager authorManager,
            UserInfoManager userInfoManager,
            IUserInfoRepository userInfoRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _authorManager = authorManager;
            _userInfoManager = userInfoManager;
            _userInfoRepository = userInfoRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _bookRepository.GetCountAsync() == 0)
            {
                var orwell = await _authorRepository.InsertAsync(
                    await _authorManager.CreateAsync(
                        "George Orwell",
                        new DateTime(1903, 06, 25),
                        "Orwell produced literary criticism and poetry, fiction and polemical journalism; and is best known for the allegorical novella Animal Farm (1945) and the dystopian novel Nineteen Eighty-Four (1949)."
                    )
                );

                var douglas = await _authorRepository.InsertAsync(
                    await _authorManager.CreateAsync(
                        "Douglas Adams",
                        new DateTime(1952, 03, 11),
                        "Douglas Adams was an English author, screenwriter, essayist, humorist, satirist and dramatist. Adams was an advocate for environmentalism and conservation, a lover of fast cars, technological innovation and the Apple Macintosh, and a self-proclaimed 'radical atheist'."
                    )
                );

                await _bookRepository.InsertAsync(
                    new Book
                    {
                        AuthorId = orwell.Id,
                        Name = "1984",
                        Type = BookType.Dystopia,
                        PublishDate = new DateTime(1949, 6, 8),
                        Price = 19.84f
                    },
                    autoSave: true
                );

                await _bookRepository.InsertAsync(
                    new Book
                    {
                        AuthorId = douglas.Id,
                        Name = "The Hitchhiker's Guide to the Galaxy",
                        Type = BookType.ScienceFiction,
                        PublishDate = new DateTime(1995, 9, 27),
                        Price = 42.0f
                    },
                    autoSave: true
                );
            }

            if (await _userInfoRepository.GetCountAsync() == 0)
            {
                await _userInfoRepository.InsertAsync(
                    await _userInfoManager.CreateAsync(
                        "Viet",
                        "Hoang",
                        "https://res.cloudinary.com/dduv8pom4/image/upload/v1692182729/test_level_1/test_level_2/thresh.jpg",
                        new DateTime(1952, 03, 11),
                        JobType.Developer,
                        Guid.TryParse("18FE9275-8A5B-8D2A-B612-3A0D018AD38C", out var parsedGuid) ? parsedGuid : Guid.Empty,
                        "Hai Duong"
                    )
                );

                await _userInfoRepository.InsertAsync(
                    await _userInfoManager.CreateAsync(
                        "Lam",
                        "Anh",
                        "https://res.cloudinary.com/dduv8pom4/image/upload/v1692167636/yep6vqkyudbsrg1awzv9.jpg",
                        new DateTime(1952, 03, 11),
                        JobType.Teacher,
                        Guid.TryParse("8BB34EBA-65AC-62C1-8F83-3A0D10F93F9F", out var parsedTesterGuid) ? parsedTesterGuid : Guid.Empty,
                        "Hai Duong"
                    )
                );
            }

        }

    }
}
