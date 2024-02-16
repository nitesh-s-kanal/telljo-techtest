using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IBookmarkService
    {
        Task CreateBookmark(Bookmark bookmark);
        Task<List<Bookmark>> GetAllBookmarksForTheUser(string userId);
        Task<Bookmark> GetBookmark(int Id);
        Task<List<Bookmark>> GetBookmarksForCategory(int categoryId);
        Task UpdateBookmark(Bookmark bookmark);
        Task DeleteBookmark(Bookmark bookmark);
    }
}
