using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using PennState.Models;
using PennState.ViewModels;
using AutoMapper;
using System.Data.Entity.Validation;
using Dapper;

namespace PennState.Controllers
{
    public class ItemController : Controller
    {
        private SqlConnection con;
        private string constr;
        private ContextModel _context;
        private List<Tbl_File> files;
        private List<Tbl_Photo> photos;
        private Tbl_CatagoryLocations catLoc2;

        public ItemController()
        {
            _context = new ContextModel();
            files = new List<Tbl_File>();
            photos = new List<Tbl_Photo>();
            catLoc2 = new Tbl_CatagoryLocations();

        }
        // GET: Item
        public ActionResult Index()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            using (ContextModel _context = new ContextModel())
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Edit(int cid)
        {
            var model = new EditItemViewModel();
            //Files objFile = new Files();
            //Photos objPhoto = new Photos();
            using (ContextModel db = new ContextModel())
            {
                model.Files.GetFileList = db.Tbl_File.Select(x => new Files { Id = x.Id, ItemFileName = x.ItemFileName }).ToList();
                model.Photos.GetPhotoList = db.Tbl_Photo.Select(x => new Photos { Id = x.Id, PhotoName = x.PhotoName }).ToList();
                model.Item = Mapper.Map<Tbl_Items, Item>(db.Tbl_Items.Where(x => x.Id == cid).Include(i => i.Photos).Include(i => i.Files).FirstOrDefault());
            }
            //model.Files = objFile;
            //model.Photos = objPhoto;
            return PartialView("Edit", model);
        }

        [HttpGet]
        public ActionResult ItemDetails(int cid)
        {
            var model = new ItemDetailsModel();
            using (ContextModel db = new ContextModel())
            {
                model.Item = Mapper.Map<Tbl_Items, Item>(db.Tbl_Items.Where(x => x.Id == cid).Include(i => i.Photos).Include(i => i.Files).FirstOrDefault());
            }
            return PartialView("ItemDetails", model);
        }

        [HttpPost]
        public ActionResult Edit(EditItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                Tbl_Items modelItem = _context.Tbl_Items.Find(model.Item.Id);
                List<Image> images = null;
                List<byte[]> photoDatas = null;

                if (model.Files != null)
                {
                    if (model.Files.FileList != null)
                    {
                        foreach (var fileId in model.Files.FileList)
                        {
                            var fId = Convert.ToInt32(fileId);
                            var existingfile = _context.Tbl_File.Where(x => x.Id == fId).FirstOrDefault();
                            modelItem.Files.Add(existingfile);
                        }
                    }
                }

                if (model.Photos != null)
                {
                    if (model.Photos.PhotoList != null)
                    {
                        foreach (var photoId in model.Photos.PhotoList)
                        {
                            var pId = Convert.ToInt32(photoId);
                            var existingPhoto = _context.Tbl_Photo.Where(x => x.Id == pId).FirstOrDefault();
                            modelItem.Photos.Add(existingPhoto);
                        }
                    }
                }

                if (model.FileUpload != null)
                {
                    foreach (HttpPostedFileBase file in model.FileUpload)
                    {
                        if (file != null && file.ContentLength > 0)
                        {

                            var varFile = new Files()
                            {
                                ItemFileName = System.IO.Path.GetFileName(file.FileName)
                            };

                            using (var reader = new System.IO.BinaryReader(file.InputStream))
                            {
                                varFile.DataStream = reader.ReadBytes(file.ContentLength);
                            }

                            files.Add(Mapper.Map<Files, Tbl_File>(varFile));
                        }
                    }
                }

                if (model.PhotoUpload != null)
                {
                    images = new List<Image>();
                    photoDatas = new List<byte[]>();
                    foreach (HttpPostedFileBase photo in model.PhotoUpload)
                    {
                        if (photo != null && photo.ContentLength > 0)
                        {
                            var varPhoto = new Photos()
                            {
                                PhotoName = System.IO.Path.GetFileName(photo.FileName)
                            };

                            using (var preader = new System.IO.BinaryReader(photo.InputStream))
                            {
                                var byteArray = preader.ReadBytes(photo.ContentLength);
                                photoDatas.Add(byteArray);
                                varPhoto.DataStream = byteArray;
                                using (var ms = new MemoryStream(byteArray))
                                {
                                    images.Add(Image.FromStream(ms));
                                }
                            }
                            photos.Add(Mapper.Map<Photos, Tbl_Photo>(varPhoto));
                        }
                    }
                }
                var id = new Tbl_Users();
                id = _context.Tbl_Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

                Tbl_Locations location = null;
                Tbl_SubLocations sublocation = null;
                if (_context.Tbl_Locations.Where(x => x.LocationName == model.Item.Location.LocationName).Any())
                {
                    //if this item has an existing location and sublocation get the location object
                    var loc = _context.Tbl_Locations.Where(x => x.LocationName == model.Item.Location.LocationName).FirstOrDefault();
                    var sub = _context.Tbl_SubLocations.Where(x => x.SubLocationName == model.Item.SubLocation.SubLocationName && x.Location.LocationName == model.Item.Location.LocationName).FirstOrDefault();
                    //assign the changed location name to the item
                    modelItem.Location = loc;
                    modelItem.LocId = loc.Id;
                    //if there are any sublocations that have the same location name in the database
                    if (sub != null)
                    {
                        modelItem.SubLocation = sub;
                        modelItem.SubId = sub.Id;
                    }
                    else
                    {
                        sublocation = new Tbl_SubLocations()
                        {
                            SubLocationName = model.Item.SubLocation.SubLocationName,
                            Location = loc
                        };
                        modelItem.SubLocation = sublocation;
                    }
                }
                //else if neither location and sublocation exist
                else
                {
                    //create a new location
                    location = new Tbl_Locations()
                    {
                        LocationName = model.Item.Location.LocationName    //Create new location and assign it to the new location name
                    };

                    //create a new sublocation for the location
                    sublocation = new Tbl_SubLocations()
                    {
                        SubLocationName = model.Item.SubLocation.SubLocationName,     //Take the sublocation name and assign it to a new sublocation
                        Location = location
                    };

                    //add these to the item
                    modelItem.Location = location;
                    modelItem.SubLocation = sublocation;
                }



                if (files.Count > 0)
                {
                    if (modelItem.Files.Count > 0)
                    {
                        modelItem.Files = modelItem.Files.Concat(files).ToList();
                    }
                    else
                    {
                        if (model.Item.Files != null)
                            modelItem.Files = modelItem.Files.Concat(Mapper.Map<IEnumerable<Tbl_File>>(model.Item.Files)).Concat(files).ToList();
                        else
                            modelItem.Files = files;
                    }
                }
                if (photos.Count > 0)
                {
                    if (modelItem.Photos.Count > 0)
                    {
                        modelItem.Photos = modelItem.Photos.Concat(photos).ToList();
                    }
                    else
                    {
                        if (model.Item.Photos != null)
                            modelItem.Photos = modelItem.Photos.Concat(Mapper.Map<IEnumerable<Tbl_Photo>>(model.Item.Photos)).Concat(photos).ToList();
                        else
                            modelItem.Photos = photos;
                    }
                }
                modelItem.Id = model.Item.Id;
                modelItem.ItemNotes = model.Item.ItemNotes;
                modelItem.Manufacturer = modelItem.Manufacturer;
                modelItem.UsrId = id.Id;
                modelItem.UpdatedBy = id.FirstName + " " + id.LastName;
                modelItem.Added = DateTime.Now;
                modelItem.Updated = DateTime.Now;
                modelItem.AmountInStock = model.Item.AmountInStock;
                modelItem.CatalogNumber = model.Item.CatalogNumber;
                modelItem.ItemName = model.Item.ItemName;
                modelItem.ItemType = model.Item.ItemType;
                modelItem.LocationComments = model.Item.LocationComments;
                modelItem.PurchaseDate = model.Item.PurchaseDate;
                modelItem.PurchasePrice = model.Item.PurchasePrice;
                modelItem.Vendor = model.Item.Vendor;
                modelItem.WebAddress = model.Item.WebAddress;
                try
                {
                    _context.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }


                var pathString1 = "";
                var pathString2 = "";
                var pathString3 = "";
                var pathString4 = "";
                var pathString5 = "";
                var path = "";
                var path2 = "";
                var path4 = "";
                int i = 0;
                if (model.PhotoUpload != null)
                {
                    foreach (var photoObj in model.PhotoUpload)
                    {
                        //Create necessary directories
                        var originalDir = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                        var imageName = photoObj.FileName;

                        var subLocId = (from s in _context.Tbl_SubLocations
                                        join it in _context.Tbl_Items on s.Id equals it.SubId
                                        where it.Id == modelItem.Id
                                        select s).FirstOrDefault();

                        //Check if file was uploaded
                        pathString1 = Path.Combine(originalDir.ToString(), "SubLocations");
                        pathString2 = Path.Combine(originalDir.ToString(), "SubLocations\\" + subLocId.Id.ToString());
                        pathString3 = Path.Combine(originalDir.ToString(), "SubLocations\\" + subLocId.Id.ToString() + "\\Thumbs");
                        pathString4 = Path.Combine(originalDir.ToString(), "SubLocations\\" + subLocId.Id.ToString() + "\\Gallery");
                        pathString5 = Path.Combine(originalDir.ToString(), "SubLocations\\" + subLocId.Id.ToString() + "\\Gallery\\Thumbs");

                        if (!Directory.Exists(pathString1))
                            Directory.CreateDirectory(pathString1);

                        if (!Directory.Exists(pathString2))
                            Directory.CreateDirectory(pathString2);

                        if (!Directory.Exists(pathString3))
                            Directory.CreateDirectory(pathString3);

                        if (!Directory.Exists(pathString4))
                            Directory.CreateDirectory(pathString4);

                        if (!Directory.Exists(pathString5))
                            Directory.CreateDirectory(pathString5);

                        path = string.Format("{0}\\{1}", pathString2, imageName);
                        ImageConverter imageConverter = new System.Drawing.ImageConverter();
                        System.Drawing.Image image = imageConverter.ConvertFrom(photoDatas.ElementAt(i)) as System.Drawing.Image;
                        image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);

                        path2 = string.Format("{0}\\{1}", pathString3, imageName);
                        var img = images.ElementAt(i);
                        img = resizeImage(img, new Size(200, 200));
                        img.Save(path2);

                        path4 = string.Format("{0}\\{1}", pathString4, imageName);
                        var img2 = images.ElementAt(i);
                        img2 = resizeImage(img2, new Size(500, 375));
                        img2.Save(path4);

                        i++;
                    }
                }

                var testLoc = _context.Tbl_CatagoryLocations.Where(x => x.LocationName == model.Item.Location.LocationName).FirstOrDefault();
                var catLoc = new Tbl_CatagoryLocations();
                if (testLoc == null)
                {
                    if (model.Item.SubLocation.SubLocationName != null)
                    {
                        catLoc.LocationName = model.Item.Location.LocationName;
                        catLoc.Pid = null;
                        catLoc.HasChildren = true;
                    }
                    else
                    {
                        catLoc.LocationName = model.Item.Location.LocationName;
                        catLoc.Pid = null;
                        catLoc.HasChildren = false;
                    }

                    _context.Tbl_CatagoryLocations.Add(catLoc);
                    _context.SaveChanges();
                }
                //Get Parent Locations that Equal the current model's Location Name
                var parentId = _context.Tbl_CatagoryLocations.Where(x => x.LocationName == model.Item.Location.LocationName).FirstOrDefault().Id;

                //Determine if there are any sublocations that 
                //1) have a parent 
                //2) have the sublocation name as the model 
                //3) have a parent Id that is not equal to the current model's Id
                //4) where the parent referenced from the database is not equal to null
                bool noLocations = _context.Tbl_CatagoryLocations.Where(x => x.LocationName == model.Item.SubLocation.SubLocationName && x.Pid == parentId).Any();


                if (noLocations == false)
                {
                    var catLoc2 = new Tbl_CatagoryLocations
                    {
                        LocationName = model.Item.SubLocation.SubLocationName,
                        Pid = parentId
                    };
                    _context.Tbl_CatagoryLocations.Add(catLoc2);
                    _context.SaveChanges();
                }

                var owners = _context.Tbl_CatagoryOwners.Where(x => x.OwnerName == id.FirstName + " " + id.LastName).FirstOrDefault();
                if (owners == null)
                {
                    var owner = new Tbl_CatagoryOwners
                    {
                        OwnerName = id.FirstName + " " + id.LastName,
                        Pid = null,
                        HasChildren = false
                    };
                    _context.Tbl_CatagoryOwners.Add(owner);
                    _context.SaveChanges();
                }

                var types = _context.Tbl_CatagoryTypes.Where(x => x.TypeName == model.Item.ItemType).FirstOrDefault();
                if (types == null)
                {
                    var type = new Tbl_CatagoryTypes
                    {
                        TypeName = model.Item.ItemType,
                        Pid = null,
                        HasChildren = false
                    };
                    _context.Tbl_CatagoryTypes.Add(type);
                    _context.SaveChanges();
                }

                var vendors = _context.Tbl_CatagoryVendors.Where(x => x.VendorName == model.Item.Vendor).FirstOrDefault();
                if (vendors == null)
                {
                    var vendor = new Tbl_CatagoryVendors
                    {
                        VendorName = model.Item.Vendor,
                        Pid = null,
                        HasChildren = false
                    };
                    _context.Tbl_CatagoryVendors.Add(vendor);
                    _context.SaveChanges();
                }

            }
            return RedirectToAction("GetAllItems", "Item");
        }

        [HttpGet]
        public ActionResult AddItem()
        {
            AddItemViewModel model = new AddItemViewModel();

            using (ContextModel db = new ContextModel())
            {
                model.Files.GetFileList = db.Tbl_File.Select(x => new Files { Id = x.Id, ItemFileName = x.ItemFileName }).ToList();
                model.Photos.GetPhotoList = db.Tbl_Photo.Select(x => new Photos { Id = x.Id, PhotoName = x.PhotoName }).ToList();
            }
            
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddItem(AddItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var modelItem = new Tbl_Items();
                List<Image> images = null;
                List<byte[]> photoDatas = null;
                if (model.Files.FileList != null)
                {
                    foreach (var fileId in model.Files.FileList)
                    {
                        var fId = Convert.ToInt32(fileId);
                        var existingfile = _context.Tbl_File.Where(x => x.Id == fId).FirstOrDefault();
                        modelItem.Files.Add(existingfile);
                    }
                }

                if (model.Photos.PhotoList != null)
                {
                    foreach (var photoId in model.Photos.PhotoList)
                    {
                        var pId = Convert.ToInt32(photoId);
                        var existingPhoto = _context.Tbl_Photo.Where(x => x.Id == pId).FirstOrDefault();
                        modelItem.Photos.Add(existingPhoto);
                    }
                }

                if (model.FileUpload != null)
                {
                    foreach (HttpPostedFileBase file in model.FileUpload)
                    {
                        if (file != null && file.ContentLength > 0)
                        {

                            var varFile = new Tbl_File()
                            {
                                ItemFileName = System.IO.Path.GetFileName(file.FileName)
                            };

                            using (var reader = new System.IO.BinaryReader(file.InputStream))
                            {
                                varFile.DataStream = reader.ReadBytes(file.ContentLength);
                            }

                            files.Add(varFile);
                        }
                    }
                }

                if (model.PhotoUpload != null)
                {
                    images = new List<Image>();
                    photoDatas = new List<byte[]>();
                    foreach (HttpPostedFileBase photo in model.PhotoUpload)
                    {
                        if (photo != null && photo.ContentLength > 0)
                        {
                            var varPhoto = new Tbl_Photo()
                            {
                                PhotoName = System.IO.Path.GetFileName(photo.FileName)
                            };

                            using (var preader = new System.IO.BinaryReader(photo.InputStream))
                            {
                                var byteArray = preader.ReadBytes(photo.ContentLength);
                                photoDatas.Add(byteArray);
                                varPhoto.DataStream = byteArray;
                                using (var ms = new MemoryStream(byteArray))
                                {
                                    images.Add(Image.FromStream(ms));
                                }
                            }
                            photos.Add(varPhoto);
                        }
                    }
                }

                var id = new Tbl_Users();

                id = _context.Tbl_Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

                //var item = new Item();
                Tbl_Locations location = null;
                Tbl_SubLocations sublocation = null;
                //Check for new location input
                if (_context.Tbl_Locations.Where(x => x.LocationName == model.Item.Location.LocationName).Any())
                {
                    //if this item has an existing location and sublocation get the location object
                    var loc = _context.Tbl_Locations.Where(x => x.LocationName == model.Item.Location.LocationName).FirstOrDefault();
                    var sub = _context.Tbl_SubLocations.Where(x => x.SubLocationName == model.Item.SubLocation.SubLocationName && x.Location.LocationName == model.Item.Location.LocationName).FirstOrDefault();
                    //assign the changed location name to the item
                    modelItem.Location = loc;
                    modelItem.LocId = loc.Id;
                    //if there are any sublocations that have the same location name in the database
                    if (sub != null)
                    {
                        modelItem.SubLocation = sub;
                        modelItem.SubId = sub.Id;
                    }
                    else
                    {
                        sublocation = new Tbl_SubLocations()
                        {
                            SubLocationName = model.Item.SubLocation.SubLocationName,
                            Location = loc
                        };
                        modelItem.SubLocation = sublocation;
                    }
                }
                //else if neither location and sublocation exist
                else
                {
                    //create a new location
                    location = new Tbl_Locations()
                    {
                        LocationName = model.Item.Location.LocationName    //Create new location and assign it to the new location name
                    };

                    //create a new sublocation for the location
                    sublocation = new Tbl_SubLocations()
                    {
                        SubLocationName = model.Item.SubLocation.SubLocationName,     //Take the sublocation name and assign it to a new sublocation
                        Location = location
                    };

                    //add these to the item
                    modelItem.Location = location;
                    modelItem.SubLocation = sublocation;
                }



                if (files.Count > 0)
                {
                    if (modelItem.Files.Count > 0)
                    {
                        modelItem.Files = modelItem.Files.Concat(files).ToList();
                    }
                    else
                    {
                        modelItem.Files = files;
                    }
                }
                if (photos.Count > 0)
                {
                    if (modelItem.Photos.Count > 0)
                    {
                        modelItem.Photos = modelItem.Photos.Concat(photos).ToList();
                    }
                    else
                    {
                        modelItem.Photos = photos;
                    }
                }

                modelItem.UsrId = id.Id;
                modelItem.Added = DateTime.Now;
                modelItem.Updated = DateTime.Now;
                modelItem.AmountInStock = model.Item.AmountInStock;
                modelItem.CatalogNumber = model.Item.CatalogNumber;
                modelItem.ItemName = model.Item.ItemName;
                modelItem.ItemType = model.Item.ItemType;
                modelItem.LocationComments = model.Item.LocationComments;
                modelItem.PurchaseDate = model.Item.PurchaseDate;
                modelItem.PurchasePrice = model.Item.PurchasePrice;
                modelItem.Vendor = model.Item.Vendor;
                modelItem.WebAddress = model.Item.WebAddress;
                try
                {
                    _context.Tbl_Items.Add(modelItem);
                    _context.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                var pathString1 = "";
                var pathString2 = "";
                var pathString3 = "";
                var pathString4 = "";
                var pathString5 = "";
                var path = "";
                var path2 = "";
                var path4 = "";
                int i = 0;
                if (model.PhotoUpload != null)
                {
                    foreach (var photoObj in model.PhotoUpload)
                    {
                        //Create necessary directories
                        var originalDir = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                        var imageName = photoObj.FileName;

                        var subLocId = (from s in _context.Tbl_SubLocations
                                        join it in _context.Tbl_Items on s.Id equals it.SubId
                                        where it.Id == modelItem.Id
                                        select s).FirstOrDefault();

                        //Check if file was uploaded
                        pathString1 = Path.Combine(originalDir.ToString(), "SubLocations");
                        pathString2 = Path.Combine(originalDir.ToString(), "SubLocations\\" + subLocId.Id.ToString());
                        pathString3 = Path.Combine(originalDir.ToString(), "SubLocations\\" + subLocId.Id.ToString() + "\\Thumbs");
                        pathString4 = Path.Combine(originalDir.ToString(), "SubLocations\\" + subLocId.Id.ToString() + "\\Gallery");
                        pathString5 = Path.Combine(originalDir.ToString(), "SubLocations\\" + subLocId.Id.ToString() + "\\Gallery\\Thumbs");

                        if (!Directory.Exists(pathString1))
                            Directory.CreateDirectory(pathString1);

                        if (!Directory.Exists(pathString2))
                            Directory.CreateDirectory(pathString2);

                        if (!Directory.Exists(pathString3))
                            Directory.CreateDirectory(pathString3);

                        if (!Directory.Exists(pathString4))
                            Directory.CreateDirectory(pathString4);

                        if (!Directory.Exists(pathString5))
                            Directory.CreateDirectory(pathString5);

                        path = string.Format("{0}\\{1}", pathString2, imageName);
                        ImageConverter imageConverter = new System.Drawing.ImageConverter();
                        System.Drawing.Image image = imageConverter.ConvertFrom(photoDatas.ElementAt(i)) as System.Drawing.Image;
                        image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);

                        path2 = string.Format("{0}\\{1}", pathString3, imageName);                    
                        var img = images.ElementAt(i);
                        img = resizeImage(img, new Size(200, 200));
                        img.Save(path2);

                        path4 = string.Format("{0}\\{1}", pathString4, imageName);
                        var img2 = images.ElementAt(i);
                        img2 = resizeImage(img2, new Size(500, 375));
                        img2.Save(path4);
                        i++;
                    }
                }

                var testLoc = _context.Tbl_CatagoryLocations.Where(x => x.LocationName == model.Item.Location.LocationName).FirstOrDefault();
                var catLoc = new Tbl_CatagoryLocations();
                if (testLoc == null)
                {
                    if (model.Item.SubLocation.SubLocationName != null)
                    {
                        catLoc.LocationName = model.Item.Location.LocationName;
                        catLoc.Pid = null;
                        catLoc.HasChildren = true;
                    }
                    else
                    {
                        catLoc.LocationName = model.Item.Location.LocationName;
                        catLoc.Pid = null;
                        catLoc.HasChildren = false;
                    }

                    _context.Tbl_CatagoryLocations.Add(catLoc);
                    _context.SaveChanges();
                }


                //Get Parent Locations that Equal the current model's Location Name
                var parentId = _context.Tbl_CatagoryLocations.Where(x => x.LocationName == model.Item.Location.LocationName).FirstOrDefault().Id;

                //Determine if there are any sublocations that 
                //1) have a parent 
                //2) have the sublocation name as the model 
                //3) have a parent Id that is not equal to the current model's Id
                //4) where the parent referenced from the database is not equal to null
                bool noLocations = _context.Tbl_CatagoryLocations.Where(x => x.LocationName == model.Item.SubLocation.SubLocationName && x.Pid == parentId).Any();


                if (noLocations == false)
                {
                    var catLoc2 = new Tbl_CatagoryLocations
                    {
                        LocationName = model.Item.SubLocation.SubLocationName,
                        Pid = parentId
                    };
                    _context.Tbl_CatagoryLocations.Add(catLoc2);
                    _context.SaveChanges();
                }

                var owners = _context.Tbl_CatagoryOwners.Where(x => x.OwnerName == id.FirstName + " " + id.LastName).FirstOrDefault();
                if (owners == null)
                {
                    var owner = new Tbl_CatagoryOwners
                    {
                        OwnerName = id.FirstName + " " + id.LastName,
                        Pid = null,
                        HasChildren = false
                    };
                    _context.Tbl_CatagoryOwners.Add(owner);
                    _context.SaveChanges();
                }

                var types = _context.Tbl_CatagoryTypes.Where(x => x.TypeName == model.Item.ItemType).FirstOrDefault();
                if (types == null)
                {
                    var type = new Tbl_CatagoryTypes
                    {
                        TypeName = modelItem.ItemType,
                        Pid = null,
                        HasChildren = false
                    };
                    try
                    {
                        _context.Tbl_CatagoryTypes.Add(type);
                        _context.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {

                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }

                    }
                }

                var vendors = _context.Tbl_CatagoryVendors.Where(x => x.VendorName == model.Item.Vendor).FirstOrDefault();
                if (vendors == null)
                {
                    var vendor = new Tbl_CatagoryVendors
                    {
                        VendorName = modelItem.Vendor,
                        Pid = null,
                        HasChildren = false
                    };
                    _context.Tbl_CatagoryVendors.Add(vendor);
                    _context.SaveChanges();
                }
                catLoc = null;

                files = null;
                photos = null;
                modelItem = null;


                //Get file extension
            }
            //verify extension
            return RedirectToAction("GetAllItems", "Item");
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        [HttpPost]
        public JsonResult DeleteItem(int id)
        {
            bool result = false;
            using (ContextModel db = new ContextModel())
            {
                Tbl_Items item = db.Tbl_Items.SingleOrDefault(x => x.Id == id && x.IsDeleted == false);
                if (item != null)
                {
                    item.IsDeleted = true;
                    _context.SaveChanges();
                    result = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllItems()
        {
            var model = new ItemViewModel();
            using (ContextModel db = new ContextModel())
            {
                model.Owners = Mapper.Map<IEnumerable<CatagoryOwner>>(db.Tbl_CatagoryOwners.Where(x => !x.Pid.HasValue).ToList());
                model.Types = Mapper.Map<IEnumerable<CatagoryType>>(db.Tbl_CatagoryTypes.Where(x => !x.Pid.HasValue).ToList());
                model.LocationsC = Mapper.Map<IEnumerable<CatagoryLocation>>(db.Tbl_CatagoryLocations.Where(x => !x.Pid.HasValue).ToList());
                model.Vendors = Mapper.Map<IEnumerable<CatagoryVendor>>(db.Tbl_CatagoryVendors.Where(x => !x.Pid.HasValue).ToList());
                model.Items = GetItems(names: null);
                //var pagedList = new PagedList<Item>(model.Items, 1, 1);
                //model.PagedList = pagedList;
            }
            ViewBag.ItemList = model.Items.ToList();
            return View("GetItemList", model);
        }

        List<Item> CatQuery(List<Item> list, string[] array)
        {
            var listItem = new List<Item>();
            var test = array[0];
            var test2 = "";
                if (_context.Tbl_Items.Where(x => x.Location.LocationName == test).Any())
                {
                    for (int i = 0; i < array.Length; i += 2)
                    {
                        test = array[i];
                        test2 = array[i + 1];
                        listItem = listItem.Concat(Mapper.Map<IEnumerable<Item>>(_context.Tbl_Items.Where(a => a.Location.LocationName == test && a.SubLocation.SubLocationName == test2).ToList())).ToList();
                    }
                }
                else if (_context.Tbl_Items.Where(x => x.ItemType == test).Any())
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        test = array[i];
                        var query = list.AsQueryable().Where(a => a.ItemType == test);
                        listItem = listItem.Concat(query.ToList()).ToList();
                    }
                }
                else if (_context.Tbl_Items.Where(x => x.Vendor == test).Any())
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        test = array[i];
                        var query = list.AsQueryable().Where(a => a.Vendor == test);
                        listItem = listItem.Concat(query.ToList()).ToList();
                    }
                }
                else
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        test = array[i];
                        var query = list.AsQueryable().Where(a => a.User.FirstName + " " + a.User.LastName == test);
                        listItem = listItem.Concat(query.ToList()).ToList();
                    }
                }
 
            return listItem;
        }

        public ActionResult NextPage(int? page)
        {
            int pageNumber = page ?? 1;
            var pageItems = TempData["List"] as List<Item>;
            var modelItem = new ItemViewModel();
            var list = new List<Item>();
            foreach (var item in pageItems)
            {
                using (ContextModel db = new ContextModel())
                {
                    var single = db.Tbl_Items.Where(x => x.Id == item.Id).FirstOrDefault();
                    list.Add(Mapper.Map<Item>(single));
                }
                
            }
            modelItem.Items = list;
            ViewBag.ItemList = list;
            //var pagedList = new PagedList<Item>(list, pageNumber, 1);
            //modelItem.PagedList = pagedList;
            return View("GetItemList", modelItem);
        }

        [HttpGet]
        public ActionResult GetItemList(string location, string type, string vendor, string owner)
        {
            var pageNumber = 1;

            if (location == "null")
            {
                location = null;
            }
            if (type == "null")
            {
                type = null;
            }
            if (vendor == "null")
            {
                vendor = null;
            }
            if (owner == "null")
            {
                owner = null;
            }

            var itemList = new List<Item>();
            var test = "";
            string[] loc = null;
            if (location != null)
            {
                loc = location.Split(',');
                itemList = CatQuery(null, loc);
            }

                string[] typeAr = null;
                if (type != null)
                {
                    typeAr = type.Split(',');
                    if (location != null)
                    {
                        itemList = CatQuery(itemList, typeAr);
                    }
                    else
                    {
                        for (int i = 0; i < typeAr.Length; i++)
                        {
                            test = typeAr[i];
                            itemList = itemList.Concat(Mapper.Map<IEnumerable<Item>>(_context.Tbl_Items.Where(x => x.ItemType == test).ToList())).ToList();
                        }
                    }
                }

                string[] ven = null;
                if (vendor != null)
                {
                    ven = vendor.Split(',');
                    if (location != null || type != null)
                    {
                        itemList = CatQuery(itemList, ven);
                    }
                    else
                    {
                        for (int i = 0; i < ven.Length; i++)
                        {
                            test = ven[i];
                            itemList = itemList.Concat(Mapper.Map<IEnumerable<Item>>(_context.Tbl_Items.Where(x => x.Vendor == test).ToList())).ToList();
                        }
                    }
                }

                string[] own = null;
                if (owner != null)
                {
                    own = owner.Split(',');
                    if (location != null || type != null || vendor != null)
                    {
                        itemList = CatQuery(itemList, own);
                    }
                    else
                    {
                        for (int i = 0; i < own.Length; i++)
                        {
                            test = own[i];
                            itemList = itemList.Concat(Mapper.Map<IEnumerable<Item>>(_context.Tbl_Items.Where(a => a.User.FirstName + " " + a.User.LastName == test).ToList())).ToList();
                        }
                    }
                }

            itemList = itemList.Where(x => x.IsDeleted == false).ToList();
            var model = new ItemViewModel();
            //var pagedList = new PagedList<Item>(itemList, pageNumber, 1);
            //model.PagedList = pagedList;
            model.Items = itemList;
            ViewBag.ItemList = itemList;

            return View(model);

        }

        private IEnumerable<Item> GetItems(IEnumerable<string> names)
        {
            using (ContextModel db = new ContextModel())
            {
                var items = (from I in db.Tbl_Items
                             join U in db.Tbl_Users
                             on I.UsrId equals U.Id
                             where I.IsDeleted == false
                             select I).ToList();

                return Mapper.Map<IEnumerable<Item>>(items);
            }
        }

        public ActionResult Gallery()
        {
            return View();
        }

        [Route("item/bytype/{type}")]
        public ActionResult ByType(string type)
        {
            return Content(type);
        }

        [Route("item/getterm/{text}")]
        public JsonResult GetTerm(string text)
        {
            using (ContextModel db = new ContextModel())
            {
                var ItemType = (from N in db.Tbl_Items.ToList()
                                where N.ItemType.ToLower().Contains(text.ToLower())
                                orderby N.ItemType ascending
                                select new { N.ItemType }).Distinct();
                return Json(ItemType.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [Route("item/getloc/{text}")]
        public JsonResult GetLoc(string text)
        {
            using (ContextModel db = new ContextModel())
            {
                var LocType = (from N in db.Tbl_Locations.ToList()
                               where N.LocationName.ToLower().Contains(text.ToLower())
                               orderby N.LocationName ascending
                               select new { N.LocationName }).Distinct();
                return Json(LocType.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [Route("item/getsub/{text}")]
        public JsonResult GetSub(string text)
        {
            using (ContextModel db = new ContextModel())
            {
                var SubType = (from N in db.Tbl_SubLocations.ToList()
                               where N.SubLocationName.ToLower().Contains(text.ToLower())
                               orderby N.SubLocationName ascending
                               select new { N.SubLocationName }).Distinct();
                return Json(SubType.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [Route("item/getven/{text}")]
        public JsonResult GetVen(string text)
        {
            using (ContextModel db = new ContextModel())
            {
                var Ven = (from N in db.Tbl_Items.ToList()
                           where N.Vendor.ToLower().Contains(text.ToLower())
                           orderby N.Vendor ascending
                           select new { N.Vendor }).Distinct();
                return Json(Ven.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
            List<Tbl_File> ObjFiles = GetFileList();

            var FileById = (from FC in ObjFiles
                            where FC.Id.Equals(id)
                            select new { FC.ItemFileName, FC.DataStream }).ToList().FirstOrDefault();

            return File(FileById.DataStream, "application/pdf", FileById.ItemFileName);
        }

        [HttpPost]
        public JsonResult DeleteFile(int id, int itemId)
        {
            DbConnection();
            con.Open();
            using (SqlCommand command = new SqlCommand("DELETE FROM Tbl_ItemFiles WHERE Files_Id = " + id + " AND Items_Id = " + itemId, con))
            {
                command.ExecuteNonQuery();
            }
            using (SqlCommand command = new SqlCommand("SELECT Files_Id FROM Tbl_ItemFiles WHERE Files_Id = " + id, con))
            {
                var query = command.ExecuteScalar();
                using (ContextModel _context = new ContextModel())
                {
                    Tbl_File file = _context.Tbl_File.Find(id);
                    if (file != null && query == null)
                    {
                        _context.Tbl_File.Remove(file);
                        _context.SaveChanges();
                    }

                }
            }
            con.Close();
            bool result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePhoto(int photoId, int itemId, int subId, string name)
        {
                    DbConnection();
                    con.Open();
                    using (SqlCommand command = new SqlCommand("DELETE FROM Tbl_PhotoItems WHERE Photos_Id = " + photoId + " AND Items_Id = " + itemId, con))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand("SELECT Photos_Id FROM Tbl_PhotoItems WHERE Photos_Id = " + photoId, con))
                    {
                        var query = command.ExecuteScalar();
                        using (ContextModel _context = new ContextModel())
                        {
                            Tbl_Photo photo = _context.Tbl_Photo.Find(photoId);
                            if (photo != null && query == null)
                            {
                                _context.Tbl_Photo.Remove(photo);
                                _context.SaveChanges();
                                string mappedPath1 = Request.MapPath(@"~/Images/Uploads/SubLocations/" + subId + '/' + name);
                                string mappedPath2 = Request.MapPath(@"~/Images/Uploads/SubLocations/" + subId + "/Thumbs/" + name);
                                string mappedPath3 = Request.MapPath(@"~/Images/Uploads/SubLocations/" + subId + "/Gallery/" + name);
                                if (System.IO.File.Exists(mappedPath1))
                                {
                                    System.IO.File.Delete(mappedPath1);
                                }
                                if (System.IO.File.Exists(mappedPath2))
                                {
                                    System.IO.File.Delete(mappedPath2);
                                }
                                if (System.IO.File.Exists(mappedPath3))
                                {
                                    System.IO.File.Delete(mappedPath3);
                                }
                    }
                        }
                    }
                con.Close();
      
                bool result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void EmptyFolder(DirectoryInfo directory)
        {

            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subdirectory in directory.GetDirectories())
            {
                EmptyFolder(subdirectory);
                subdirectory.Delete();
            }

        }

        private List<Tbl_File> GetFileList()
        {
            List<Tbl_File> DetList = new List<Tbl_File>();

            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<Tbl_File>(con, "GetFileDetails", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }

        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["ContextModel"].ToString();
            if (constr.ToLower().StartsWith("metadata="))
            {
                System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder efBuilder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(constr);
                constr = efBuilder.ProviderConnectionString;
            }
            con = new SqlConnection(constr);

        }

        //public void SaveItem(Tbl_Items item)
        //{
        //    DbConnection();
        //    SqlCommand cmd = new SqlCommand("spSaveItem", con);
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    SqlParameter paramId = new SqlParameter();
        //    paramId.ParameterName = "@Id";
        //    paramId.Value = item.Id;
        //    cmd.Parameters.Add(paramId);

        //    SqlParameter paramName = new SqlParameter();
        //    paramName.ParameterName = "@ItemName";
        //    paramName.Value = item.ItemName;
        //    cmd.Parameters.Add(paramName);

        //    SqlParameter paramStock = new SqlParameter();
        //    paramStock.ParameterName = "@AmountInStock";
        //    paramStock.Value = item.AmountInStock;
        //    cmd.Parameters.Add(paramStock);

        //    SqlParameter paramLocCom = new SqlParameter();
        //    paramLocCom.ParameterName = "@LocationComments";
        //    paramLocCom.Value = item.LocationComments;
        //    cmd.Parameters.Add(paramLocCom);

        //    SqlParameter paramMan = new SqlParameter();
        //    paramMan.ParameterName = "@Manufacturer";
        //    paramMan.Value = item.Manufacturer;
        //    cmd.Parameters.Add(paramMan);

        //    SqlParameter paramCat = new SqlParameter();
        //    paramCat.ParameterName = "@CatalogNumber";
        //    paramCat.Value = item.CatalogNumber;
        //    cmd.Parameters.Add(paramCat);

        //    SqlParameter paramWeb = new SqlParameter();
        //    paramWeb.ParameterName = "@WebAddress";
        //    paramWeb.Value = item.WebAddress;
        //    cmd.Parameters.Add(paramWeb);

        //    SqlParameter paramId = new SqlParameter();
        //    paramId.ParameterName = "@Id";
        //    paramId.Value = item.Id;
        //    cmd.Parameters.Add(paramId);
        //}
    }
}