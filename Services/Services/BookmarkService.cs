using Data;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookmarkService : IBookmarkService
    {
        private ReadLaterDataContext _ReadLaterDataContext;
        public BookmarkService(ReadLaterDataContext readLaterDataContext) 
        {
            _ReadLaterDataContext = readLaterDataContext;            
        }

        public Task<List<Bookmark>> GetAllBookmarksForTheUser(string userId)
        {
            return Task.FromResult(_ReadLaterDataContext.Bookmark.Where(c => c.Category.UserId == userId).ToList());
        }

        public Task<Bookmark> GetBookmark(int Id)
        {
            return Task.FromResult(_ReadLaterDataContext.Bookmark.Where(c => c.ID == Id).FirstOrDefault());
        }

        public Task<List<Bookmark>> GetBookmarksForCategory(int categoryId)
        {
            return Task.FromResult(_ReadLaterDataContext.Bookmark.Where(c => c.CategoryId == categoryId).ToList());
        }

        public Task UpdateBookmark(Bookmark bookmark)
        {
            _ReadLaterDataContext.Update(bookmark);
            _ReadLaterDataContext.SaveChanges();

            return Task.CompletedTask;
        }

        public Task DeleteBookmark(Bookmark bookmark)
        {
            _ReadLaterDataContext.Bookmark.Remove(bookmark);
            _ReadLaterDataContext.SaveChanges();

            return Task.CompletedTask;
        }

        public Task CreateBookmark(Bookmark bookmark)
        {
            _ReadLaterDataContext.Add(bookmark);
            _ReadLaterDataContext.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
