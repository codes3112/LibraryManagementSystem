using Library.Models.Catalog;
using Library.Models.CheckoutModels;
using LibraryData;
using LibraryData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;


namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        //create a constructor and pass libraryasset interface
        private ILibraryAsset _assets;  //variable to save the instance 
        private ICheckout _checkouts;  //variable to save the instance
        private LibraryContext _context;//creating datacontext instance
        


        public CatalogController(ILibraryAsset assets, ICheckout checkouts,LibraryContext context)
        {
            _assets = assets;
            _checkouts = checkouts;
            _context = context;
        }

        public IActionResult Index()
        {
            var assetModels = _assets.GetAll();
            var listingResult = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id = result.Id,
                    ImageUrl = result.ImageUrl,
                    AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                    Title = result.Title,
                    DewyCallNumber = _assets.GetDeweyIndex(result.Id),
                    Type = _assets.GetType(result.Id)
                });

            var model = new AssetIndexModel()
            {
                //Assets is enum from AssetindexModel
                Assets = listingResult
            };
            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var asset = _assets.GetById(id);
            var currentHolds = _checkouts.GetCurrentHolds(id)
                .Select(a => new AssetHoldModel
                {
                    HoldPlaced = _checkouts.GetCurrentHoldPlaced(a.Id).ToString("d"),//d is for short date format
                    PatronName = _checkouts.GetCurrentHoldPatronName(a.Id)

                });


            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _assets.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurrentLocation = _assets.GetCurrentLocation(id)?.Name,
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                ISBN = _assets.GetIsbn(id),

                CurrentAssociatedLibraryCard = _assets.GetLibraryCardByAssetId(id),
                CheckoutHistory = _checkouts.GetCheckoutHistory(id),
                LatestCheckout = _checkouts.GetLatestCheckout(id),
                CurrentHolds = currentHolds,
                PatronName = _checkouts.GetCurrentCheckoutPatron(id)
            };
            return View(model);
        }

        [Authorize]
        public IActionResult CheckOut(int id)
        {
            var asset = _assets.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkouts.IsCheckedOut(id)
            };
            return View(model);
        }

        [Authorize(Policy = "Admin")]
        public IActionResult CheckIn(int id)
        {
            _checkouts.CheckInItem(id);
            return RedirectToAction("Detail", new { id = id });
        }

        public IActionResult Hold(int id)
        {
            var asset = _assets.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkouts.IsCheckedOut(id),
                HoldCount = _checkouts.GetCurrentHolds(id).Count()
            };
            return View(model);
        }

        public IActionResult MarkLost(int id)
        {
            _checkouts.MarkLost(id);
            return RedirectToAction("Detail", new { id = id });
        }
        public IActionResult MarkFound(int id)
        {
            _checkouts.MarkFound(id);
            return RedirectToAction("Detail", new { id = id });
        }

        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            _checkouts.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkouts.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        

        [Authorize(Policy ="Admin")]
        public IActionResult Delete(int id, int? assetId)
        {
            var asset = _assets.GetById(id);

            _assets.Delete(asset);


            var assetModels = _assets.GetAll();
            var listingResult = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id = result.Id,
                    ImageUrl = result.ImageUrl,
                    AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                    Title = result.Title,
                    DewyCallNumber = _assets.GetDeweyIndex(result.Id),
                    Type = _assets.GetType(result.Id)
                });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };
            return RedirectToAction("Index", new { assetId = assetId });
        }


        //update

        [Authorize(Policy = "Admin")]
        public IActionResult Update(int id)
        {


            var assetModel = _assets.GetById(id);
            var listingResult = new AssetIndexListingModel
            {
                Id = assetModel.Id,
                ImageUrl = assetModel.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(assetModel.Id),
                Title = assetModel.Title,
                DewyCallNumber = _assets.GetDeweyIndex(assetModel.Id),
                Type = _assets.GetType(assetModel.Id)
            };




            var updateVM = listingResult;

            return View(updateVM);
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        public IActionResult Update(AssetIndexListingModel updateVM)
        {
            

            if (ModelState.IsValid)
            {
                var libraryAsset = _context.LibraryAssets.FirstOrDefault(x => x.Id == updateVM.Id);

                if (libraryAsset != default(LibraryAsset))
                {
                    libraryAsset.Title = updateVM.Title;
                }
                _context.Entry(libraryAsset).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index", new { id = updateVM.Id });
            }

            return View(updateVM);
        }
        //[Authorize(Policy = "Admin")]
        //public IActionResult Create()
        //{
        //    var bookVM = new AssetIndexListingModel();

        //    return View(bookVM);
        //}
        //[HttpPost]
        //[Authorize(Policy = "Admin")]
        //public IActionResult Create(AssetIndexListingModel createVM)
        //{
        //    LibraryAsset libraryAsset = new LibraryAsset();
        //    //var asset = _context.LibraryAssets.Where(a => a.Id > 0).Select(x => new LibraryAsset
        //    //{ Title = x.Title, ImageUrl = x.ImageUrl, Cost = x.Cost

        //    //});

        //    //var status=_context.Statuses.Where(a => a.Id > 0).Select(x => new Status
        //    //{
        //    //    Id= x.Id,
        //    //    Name = x.Name
        //    //});
        //    //var result = asset.Union(status);
        //    //var book = _context.Books.Where(a => a.Id > 0).Select(x => new Book
        //    //{
        //    //    Author = x.Author,
        //    //    ISBN = x.ISBN,
        //    //    DeweyIndex=x.DeweyIndex
        //    //});
        //    //var query = from t1 in _context.LibraryAssets
        //    //             join t2 in _context.Books on t1.Id equals t2.Id into listA
        //    //             from t3 in listA.DefaultIfEmpty()
        //    //             select new LibraryAsset
        //    //             {
        //    //                 Title = t3.Title,


        //    //             };

            


        //    Book book = new Book();
        //    //Video video = new Video();
            
        //    if (ModelState.IsValid)
        //    {
        //        if(book != default(Book))
        //        {
        //            book.Author = createVM.AuthorOrDirector;
        //            book.Title = createVM.Title;
        //            book.NumberOfCopies = Convert.ToInt32(createVM.NumberOfCopies);
        //            book.StatusId = Convert.ToInt32(createVM.StatusId);
        //            book.LocationId= Convert.ToInt32(createVM.LocationId);
                    
                    
                    

        //        }
        //        //if (video != default(Video))
        //        //{
        //        //    video.Director = createVM.AuthorOrDirector;
        //        //}                
                
                
        //        _context.Books.Add(book);
        //        //_context.Videos.Add(video);
        //        return RedirectToAction("Index");

        //    }


        //    return View(createVM);

        //}


    }

}
