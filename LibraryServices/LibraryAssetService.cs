using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private readonly LibraryContext _context;
        //create a constructor to save the context instance

        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location);
        }



        public LibraryAsset GetById(int id)
        {
            //old way
            //return _context.LibraryAssets
            //     .Include(asset => asset.Status)
            //     .Include(asset => asset.Location)
            //     .FirstOrDefault(asset => asset.Id == id);

            //after refactoring:new way

            return GetAll()
                .FirstOrDefault(asset => asset.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id).Location;
            //return _context.LibraryAssets.First(asset => asset.Id == id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }
            else
            {
                return "";
            }
        }

        public string GetIsbn(int id)
        {
            if (_context.Books.Any(Book => Book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).ISBN;
            }
            else
            {
                return "";
            }
        }

        public LibraryCard GetLibraryCardByAssetId(int id)
        {
            return _context.LibraryCards
                .FirstOrDefault(c => c.Checkouts
                    .Select(a => a.LibraryAsset)
                    .Select(v => v.Id).Contains(id));
        }

        public string GetTitle(int id)
        {
            return _context.Books
                .FirstOrDefault(book => book.Id == id)
                .Title;
        }

        public string GetType(int id)
        {
            var book = _context.LibraryAssets.OfType<Book>()
                .Where(b => b.Id == id);
            return book.Any() ? "Book" : "Video";
        }
        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>()
                 .Where(asset => asset.Id == id).Any();

            var isVideo = _context.LibraryAssets.OfType<Video>()
                 .Where(asset => asset.Id == id).Any();

            return isBook ?
                _context.Books.FirstOrDefault(book => book.Id == id).Author :
                _context.Videos.FirstOrDefault(video => video.Id == id).Director
                ?? "Unknown"; //?? is a Null coalescence operator


        }
        public void Delete(LibraryAsset existingAsset)
        {
            _context.Remove(existingAsset);
            _context.SaveChanges();
        }

        //public void Update(LibraryAsset existingAsset)
        //{
        //    _context.Entry(existingAsset).State = EntityState.Modified;
        //    _context.SaveChanges();

        //}

       
        }
    }

