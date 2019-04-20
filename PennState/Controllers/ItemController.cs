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
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Reflection;
using SQL = System.Data;
using System.Text;
using Microsoft.Office.Interop.Excel;
//Test commit
namespace PennState.Controllers
{
    public class ItemController : Controller
    {
        private SqlConnection con;
        private string constr;
        private PennStateDB _context;
        private List<Tbl_File> files;
        private List<Tbl_Photo> photos;
        private Tbl_CatagoryLocations catLoc2;

        public ItemController()
        {
            _context = new PennStateDB();
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
            using (PennStateDB _context = new PennStateDB())
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        public ActionResult Scheduler()
        {
            using (PennStateDB db = new PennStateDB())
            {
                var model = Mapper.Map<IEnumerable<CheckedOut>>(db.Tbl_CheckedOut.Take(100));
                return View(model);
            }                
        }

        public JsonResult DeleteRequest(int id)
        {
            using (PennStateDB db = new PennStateDB())
            {
                Tbl_Requests request = db.Tbl_Requests.Find(id);
                db.Tbl_Requests.Remove(request);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Search(string query)
        {
            if(query == null)
            {
                var items = PopulateList();
                return View("GetItemList", items);
            }
            var text = string.Format("%{0}%", query);
            var lower = query.ToLower();
            var something = _context.Tbl_Items.Where(x => DbFunctions.Like(x.ItemType, text)).ToList();
            
            if (something.Count == 0)
            {
                something = _context.Tbl_Items.Where(x => DbFunctions.Like(x.ItemName, text)).ToList();
            }
            if (something.Count == 0)
            {
                something = _context.Tbl_Items.Where(x => DbFunctions.Like(x.Tbl_Users.FirstName + " " + x.Tbl_Users.LastName, text)).ToList();

            }
            if(something.Count == 0)
            {
                something = _context.Tbl_Items.Where(x => DbFunctions.Like(x.CatalogNumber, text)).ToList();
            }
            if(something.Count == 0)
            {
                something = _context.Tbl_Items.Where(x => DbFunctions.Like(x.Manufacturer, text)).ToList();
            }
            if(something.Count == 0)
            {
                something = _context.Tbl_Items.Where(x => DbFunctions.Like(x.Tbl_SubLocations.SubLocationName, text)).ToList();
            }
            if(something.Count == 0)
            {
                something = _context.Tbl_Items.Where(x => DbFunctions.Like(x.Tbl_Locations.LocationName, text)).ToList();
            }
            if (something.Count == 0)
            {
                something = _context.Tbl_Items.Where(x => DbFunctions.Like(x.Vendor, text)).ToList();
            }

            var model = PopulateNewList();
            model.Items = Mapper.Map<IEnumerable<Item>>(something);
            ViewBag.ItemList = model.Items;
            return View("GetItemList", model);
        }

        [CustomAuthorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult Edit(int cid)
        {
            var model = new EditItemViewModel();
            //Files objFile = new Files();
            //Photos objPhoto = new Photos();
            using (PennStateDB db = new PennStateDB())
            {
                model.Files.GetFileList = db.Tbl_File.Select(x => new Files { Id = x.Id, ItemFileName = x.ItemFileName }).ToList();
                model.Photos.GetPhotoList = db.Tbl_Photo.Select(x => new Photos { Id = x.Id, PhotoName = x.PhotoName }).ToList();
                model.Item = Mapper.Map<Tbl_Items, Item>(db.Tbl_Items.Where(x => x.Id == cid).Include(i => i.Tbl_Photo).Include(i => i.Tbl_File).FirstOrDefault());
            }
            //model.Files = objFile;
            //model.Photos = objPhoto;
            return PartialView("Edit", model);
        }

        [HttpGet]
        public ActionResult ItemDetails(int cid)
        {
            
            var model = new ItemDetailsModel();
            using (PennStateDB db = new PennStateDB())
            {
                
                model.Item = Mapper.Map<Tbl_Items, Item>(db.Tbl_Items.Where(x => x.Id == cid).Include(i => i.Tbl_Photo).Include(i => i.Tbl_File).FirstOrDefault());
                if(model.Item.CheckedOutById != null)
                {
                    model.User = Mapper.Map<Tbl_Users, User>(db.Tbl_Users.Where(x => x.Id == model.Item.CheckedOutById).FirstOrDefault());
                }
            }
            return PartialView("ItemDetails", model);
        }

        [CustomAuthorize(Roles = "Admin,User")]
        [HttpPost]
        public JsonResult MarkDelete(int id, bool check)
        {
            using (PennStateDB db = new PennStateDB())
            {
                Tbl_Items item = db.Tbl_Items.Find(id);
                if(item.MarkedDeleted == false)
                {
                    item.MarkedDeleted = true;
                    check = true;
                    db.SaveChanges();
                    return Json(check, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    item.MarkedDeleted = false;
                    db.SaveChanges();
                    return Json("null", JsonRequestBehavior.AllowGet);
                }

            }
                
            
        }

        [CustomAuthorize(Roles = "Admin,User")]
        [HttpPost]
        public ActionResult Edit(EditItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var photos2 = new List<Tbl_Photo>();
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
                            modelItem.Tbl_File.Add(existingfile);
                            files.Add(existingfile);
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
                            modelItem.Tbl_Photo.Add(existingPhoto);
                            photos2.Add(existingPhoto);
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

                
                var id = _context.Tbl_Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                modelItem = GetLocation(modelItem, model.Item.SubLocation.SubLocationName, model.Item.Location.LocationName);

                if (files.Count > 0)
                {
                    if (modelItem.Tbl_File.Count > 0)
                    {
                        modelItem.Tbl_File = modelItem.Tbl_File.Concat(files).ToList();
                    }
                    else
                    {
                        if (model.Item.Files != null)
                            modelItem.Tbl_File = modelItem.Tbl_File.Concat(Mapper.Map<IEnumerable<Tbl_File>>(model.Item.Files)).Concat(files).ToList();
                        else
                            modelItem.Tbl_File = files;
                    }
                }
                
                modelItem.Id = model.Item.Id;
                modelItem.ItemNotes = model.Item.ItemNotes;
                modelItem.Manufacturer = model.Item.Manufacturer;
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

                Tbl_Items testItem = _context.Tbl_Items.Find(modelItem.Id);

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
                            varPhoto.SubId = testItem.SubId;
                            photos.Add(Mapper.Map<Photos, Tbl_Photo>(varPhoto));
                        }
                    }
                }

                if (photos.Count > 0)
                {
                    if (modelItem.Tbl_Photo.Count > 0)
                    {
                        testItem.Tbl_Photo = modelItem.Tbl_Photo.Concat(photos).ToList();
                    }
                    else
                    {
                        if (model.Item.Photos != null)
                            testItem.Tbl_Photo = modelItem.Tbl_Photo.Concat(Mapper.Map<IEnumerable<Tbl_Photo>>(model.Item.Photos)).Concat(photos).ToList();
                        else
                            testItem.Tbl_Photo = photos;
                    }
                }

                _context.SaveChanges();

                var pathString1 = "";
                var pathString2 = "";
                var pathString3 = "";
                var pathString4 = "";
                var pathString5 = "";
                var path = "";
                var path2 = "";
                var path4 = "";
                var imageName = "";
                               
                if (photos.Count > 0 || photos2.Count > 0)
                {
                    var i = 0;
                    var subLocId = (from s in _context.Tbl_SubLocations
                                    join it in _context.Tbl_Items on s.Id equals it.SubId
                                    where it.Id == modelItem.Id
                                    select s).FirstOrDefault();
                    //Create necessary directories
                    var originalDir = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
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
                    var listImages = new List<Image>();
                    ImageConverter imageConverter = new System.Drawing.ImageConverter();
                    foreach (var photoObj in photos2)
                    {
                        
                        imageName = photoObj.PhotoName;                        
                        path = string.Format("{0}\\{1}", pathString2, imageName);
                        
                        
                        if (model.Photos.PhotoList != null)
                        {
                            var newId = _context.Tbl_Photo.Where(x => x.PhotoName == imageName).FirstOrDefault().SubId;
                            pathString5 = Path.Combine(originalDir.ToString(), "SubLocations\\" + newId.ToString());
                            var newPath = string.Format("{0}\\{1}", pathString5, imageName);
                            var image = Image.FromFile(newPath);
                            image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                            listImages.Add(image);
                        }
                        path2 = string.Format("{0}\\{1}", pathString3, imageName);
                        path4 = string.Format("{0}\\{1}", pathString4, imageName);
                                               

                        if(model.Photos.PhotoList != null)
                        {
                            var img = listImages.ElementAt(i);
                            img = resizeImage(img, new Size(200, 200));
                            img.Save(path2);


                            var img2 = listImages.ElementAt(i);
                            img2 = resizeImage(img2, new Size(500, 375));
                            img2.Save(path4);
                        }

                        i++;
                    }
                    i = 0;
                    foreach(var objPhoto in photos)
                    {                        
                        imageName = objPhoto.PhotoName;
                        path = string.Format("{0}\\{1}", pathString2, imageName);
                        path2 = string.Format("{0}\\{1}", pathString3, imageName);
                        path4 = string.Format("{0}\\{1}", pathString4, imageName);
                        if (model.PhotoUpload != null)
                        {
                            var image = imageConverter.ConvertFrom(photoDatas.ElementAt(i)) as System.Drawing.Image;
                            image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        if (model.PhotoUpload != null)
                        {
                            var img = images.ElementAt(i);
                            img = resizeImage(img, new Size(200, 200));
                            img.Save(path2);

                            var img2 = images.ElementAt(i);
                            img2 = resizeImage(img2, new Size(500, 375));
                            img2.Save(path4);
                        }
                        i++;
                    }
                }

                SaveCatagories(model.Item.Location.LocationName, model.Item.SubLocation.SubLocationName, 
                               id.FirstName + " " + id.LastName, model.Item.ItemType, model.Item.Vendor);                

            }
            return RedirectToAction("GetAllItems", "Item");
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddItem()
        {
            AddItemViewModel model = new AddItemViewModel();

            using (PennStateDB db = new PennStateDB())
            {
                model.Files.GetFileList = db.Tbl_File.Select(x => new Files { Id = x.Id, ItemFileName = x.ItemFileName }).ToList();
                model.Photos.GetPhotoList = db.Tbl_Photo.Select(x => new Photos { Id = x.Id, PhotoName = x.PhotoName }).ToList();
            }
            
            return PartialView(model);
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddItem(AddItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var photos2 = new List<Tbl_Photo>();
                var modelItem = new Tbl_Items();
                List<Image> images = null;
                List<byte[]> photoDatas = null;
                if (model.Files.FileList != null)
                {
                    foreach (var fileId in model.Files.FileList)
                    {
                        var fId = Convert.ToInt32(fileId);
                        var existingfile = _context.Tbl_File.Where(x => x.Id == fId).FirstOrDefault();
                        modelItem.Tbl_File.Add(existingfile);
                        files.Add(existingfile);
                    }
                }

                if (model.Photos.PhotoList != null)
                {
                    foreach (var photoId in model.Photos.PhotoList)
                    {
                        var pId = Convert.ToInt32(photoId);
                        var existingPhoto = _context.Tbl_Photo.Where(x => x.Id == pId).FirstOrDefault();
                        modelItem.Tbl_Photo.Add(existingPhoto);
                        photos2.Add(existingPhoto);
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

                

                var id = new Tbl_Users();

                id = _context.Tbl_Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                modelItem = GetLocation(modelItem, model.Item.SubLocation.SubLocationName, model.Item.Location.LocationName);



                if (files.Count > 0)
                {
                    if (modelItem.Tbl_File.Count > 0)
                    {
                        modelItem.Tbl_File = modelItem.Tbl_File.Concat(files).ToList();
                    }
                    else
                    {
                        modelItem.Tbl_File = files;
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
                Tbl_Items testItem = _context.Tbl_Items.Find(modelItem.Id);

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
                            varPhoto.SubId = testItem.SubId;
                            photos.Add(varPhoto);
                        }
                    }
                }

                if (photos.Count > 0)
                {
                    if (modelItem.Tbl_Photo.Count > 0)
                    {
                        testItem.Tbl_Photo = modelItem.Tbl_Photo.Concat(photos).ToList();
                    }
                    else
                    {
                        testItem.Tbl_Photo = photos;
                    }
                }

                _context.SaveChanges();

                var pathString1 = "";
                var pathString2 = "";
                var pathString3 = "";
                var pathString4 = "";
                var pathString5 = "";
                var path = "";
                var path2 = "";
                var path4 = "";
                var imageName = "";

                if (photos.Count > 0 || photos2.Count > 0)
                {
                    var i = 0;
                    var subLocId = (from s in _context.Tbl_SubLocations
                                    join it in _context.Tbl_Items on s.Id equals it.SubId
                                    where it.Id == modelItem.Id
                                    select s).FirstOrDefault();
                    //Create necessary directories
                    var originalDir = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
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
                    var listImages = new List<Image>();
                    ImageConverter imageConverter = new System.Drawing.ImageConverter();
                    foreach (var photoObj in photos2)
                    {

                        imageName = photoObj.PhotoName;
                        path = string.Format("{0}\\{1}", pathString2, imageName);


                        if (model.Photos.PhotoList != null)
                        {
                            var newId = _context.Tbl_Photo.Where(x => x.PhotoName == imageName).FirstOrDefault().SubId;
                            pathString5 = Path.Combine(originalDir.ToString(), "SubLocations\\" + newId.ToString());
                            var newPath = string.Format("{0}\\{1}", pathString5, imageName);
                            var image = Image.FromFile(newPath);
                            image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                            listImages.Add(image);
                        }
                        path2 = string.Format("{0}\\{1}", pathString3, imageName);
                        path4 = string.Format("{0}\\{1}", pathString4, imageName);


                        if (model.Photos.PhotoList != null)
                        {
                            var img = listImages.ElementAt(i);
                            img = resizeImage(img, new Size(200, 200));
                            img.Save(path2);


                            var img2 = listImages.ElementAt(i);
                            img2 = resizeImage(img2, new Size(500, 375));
                            img2.Save(path4);
                        }

                        i++;
                    }
                    i = 0;
                    foreach (var objPhoto in photos)
                    {
                        imageName = objPhoto.PhotoName;
                        path = string.Format("{0}\\{1}", pathString2, imageName);
                        path2 = string.Format("{0}\\{1}", pathString3, imageName);
                        path4 = string.Format("{0}\\{1}", pathString4, imageName);
                        if (model.PhotoUpload != null)
                        {
                            var image = imageConverter.ConvertFrom(photoDatas.ElementAt(i)) as System.Drawing.Image;
                            image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        if (model.PhotoUpload != null)
                        {
                            var img = images.ElementAt(i);
                            img = resizeImage(img, new Size(200, 200));
                            img.Save(path2);

                            var img2 = images.ElementAt(i);
                            img2 = resizeImage(img2, new Size(500, 375));
                            img2.Save(path4);
                        }
                        i++;
                    }
                }
                SaveCatagories(model.Item.Location.LocationName, model.Item.SubLocation.SubLocationName,
                               id.FirstName + " " + id.LastName, model.Item.ItemType, model.Item.Vendor);
                

                files = null;
                photos = null;
                modelItem = null;


                //Get file extension
            }
            //verify extension
            return RedirectToAction("GetAllItems", "Item");
        }

        public Image imageFromBytes(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                return Image.FromStream(ms);
            }
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult DeleteItem(int id)
        {
            bool result = false;
            using (PennStateDB db = new PennStateDB())
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

        [HttpPost]
        public JsonResult CheckItem(int id, bool check)
        {
            using (PennStateDB db = new PennStateDB())
            {
                var uid = db.Tbl_Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                var checkId = db.Tbl_Items.Where(x => x.CheckedOutById == uid.Id && x.Id == id).FirstOrDefault();
                Tbl_Items item = db.Tbl_Items.SingleOrDefault(x => x.Id == id && x.CheckedOut == false);
                if(item != null)
                {
                    item.CheckedOut = true;
                    item.CheckedOutById = uid.Id;
                    var checkout = new Tbl_CheckedOut();
                    checkout.CheckOutDate = DateTime.Now;
                    checkout.FirstName = uid.FirstName;
                    checkout.LastName = uid.LastName;
                    checkout.ItemId = id;
                    checkout.ItemName = item.ItemName;
                    checkout.UserId = uid.Id;
                    db.Tbl_CheckedOut.Add(checkout);
                    db.SaveChanges();
                    check = true;
                    return Json(check, JsonRequestBehavior.AllowGet);
                }
                else if(checkId != null)
                {
                    checkId.CheckedOut = false;
                    Tbl_CheckedOut checkout = db.Tbl_CheckedOut.Where(x => x.ItemId == id).FirstOrDefault();
                    checkout.CheckInDate = DateTime.Now;
                    checkId.CheckedOutById = null;
                    db.SaveChanges();
                    return Json("null", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(check, JsonRequestBehavior.AllowGet);
                }
            }
                
            
        }

        //[CustomAuthorize(Roles = "Admin,User")]
        //public ActionResult MarkForDeletion(int id)
        //{
        //    return View();
        //}

        public ActionResult GetAllItems()
        {
            var model = PopulateList();
            ViewBag.ItemList = model.Items.ToList();
            return View("GetItemList", model);
        }

        List<Item> CatQuery(List<Item> list, string[] array)
        {
            var listItem = new List<Item>();
            var test = array[0];
            var test2 = "";
                if (_context.Tbl_Items.Where(x => x.Tbl_Locations.LocationName == test).Any())
                {
                    for (int i = 0; i < array.Length; i += 2)
                    {
                        test = array[i];
                        test2 = array[i + 1];
                        listItem = listItem.Concat(Mapper.Map<IEnumerable<Item>>(_context.Tbl_Items.Where(a => a.Tbl_Locations.LocationName == test && a.Tbl_SubLocations.SubLocationName == test2).ToList())).ToList();
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

        [HttpGet]
        public ActionResult GetItemList(string locations, string types, string vendors, string owners)
        {
            var test = "";
            string[] location = null;
            string[] vendor = null;
            string[] type = null;
            string[] owner = null;
            var itemList = new List<Item>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (locations != "null")
            {
                location = js.Deserialize<string[]>(locations);
                itemList = CatQuery(null, location);
            }
            if (types != "null")
            {
                type = js.Deserialize<string[]>(types);
                if (location != null)
                {
                    itemList = CatQuery(itemList, type);
                }
                else
                {
                    for (int i = 0; i < type.Length; i++)
                    {
                        test = type[i];
                        itemList = itemList.Concat(Mapper.Map<IEnumerable<Item>>(_context.Tbl_Items.Where(x => x.ItemType == test).ToList())).ToList();
                    }
                }
            }
            if (vendors != "null")
            {
                vendor = js.Deserialize<string[]>(vendors);
                if (location != null || type != null)
                {
                    itemList = CatQuery(itemList, vendor);
                }
                else
                {
                    for (int i = 0; i < vendor.Length; i++)
                    {
                        test = vendor[i];
                        itemList = itemList.Concat(Mapper.Map<IEnumerable<Item>>(_context.Tbl_Items.Where(x => x.Vendor == test).ToList())).ToList();
                    }
                }
            }
            if (owners != "null")
            {
                owner = js.Deserialize<string[]>(owners);
                if (location != null || type != null || vendor != null)
                {
                    itemList = CatQuery(itemList, owner);
                }
                else
                {
                    for (int i = 0; i < owner.Length; i++)
                    {
                        test = owner[i];
                        itemList = itemList.Concat(Mapper.Map<IEnumerable<Item>>(_context.Tbl_Items.Where(a => a.Tbl_Users.FirstName + " " + a.Tbl_Users.LastName == test).ToList())).ToList();
                    }
                }
            }            
            itemList = itemList.Where(x => x.IsDeleted == false).ToList();
          
            var model = new ItemViewModel();
            model.Items = itemList;
            ViewBag.ItemList = itemList;

            return View(model);

        }

        private IEnumerable<Item> GetItems(IEnumerable<string> names)
        {
            using (PennStateDB db = new PennStateDB())
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
            using (PennStateDB db = new PennStateDB())
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
            using (PennStateDB db = new PennStateDB())
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
            using (PennStateDB db = new PennStateDB())
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
            using (PennStateDB db = new PennStateDB())
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

        [CustomAuthorize(Roles = "Admin")]
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
                using (PennStateDB _context = new PennStateDB())
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

        [CustomAuthorize(Roles = "Admin")]
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
                        using (PennStateDB _context = new PennStateDB())
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
            constr = ConfigurationManager.ConnectionStrings["PennStateDB"].ToString();
            if (constr.ToLower().StartsWith("metadata="))
            {
                System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder efBuilder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(constr);
                constr = efBuilder.ProviderConnectionString;
            }
            con = new SqlConnection(constr);

        }

        public ItemViewModel PopulateNewList()
        {
            var model = new ItemViewModel();
            using (PennStateDB db = new PennStateDB())
            {
                model.Owners = Mapper.Map<IEnumerable<CatagoryOwner>>(db.Tbl_CatagoryOwners.Where(x => !x.Pid.HasValue).ToList());
                model.Types = Mapper.Map<IEnumerable<CatagoryType>>(db.Tbl_CatagoryTypes.Where(x => !x.Pid.HasValue).ToList());
                model.LocationsC = Mapper.Map<IEnumerable<CatagoryLocation>>(db.Tbl_CatagoryLocations.Where(x => !x.Pid.HasValue).ToList());
                model.Vendors = Mapper.Map<IEnumerable<CatagoryVendor>>(db.Tbl_CatagoryVendors.Where(x => !x.Pid.HasValue).ToList());
            }
            return model;
        }

        public ItemViewModel PopulateList()
        {
            var model = new ItemViewModel();
            using (PennStateDB db = new PennStateDB())
            {
                model.Owners = Mapper.Map<IEnumerable<CatagoryOwner>>(db.Tbl_CatagoryOwners.Where(x => !x.Pid.HasValue).ToList());
                model.Types = Mapper.Map<IEnumerable<CatagoryType>>(db.Tbl_CatagoryTypes.Where(x => !x.Pid.HasValue).ToList());
                model.LocationsC = Mapper.Map<IEnumerable<CatagoryLocation>>(db.Tbl_CatagoryLocations.Where(x => !x.Pid.HasValue).ToList());
                model.Vendors = Mapper.Map<IEnumerable<CatagoryVendor>>(db.Tbl_CatagoryVendors.Where(x => !x.Pid.HasValue).ToList());
                model.Items = GetItems(names: null);
                ViewBag.ItemList = model.Items.ToList();               
                //var pagedList = new PagedList<Item>(model.Items, 1, 1);
                //model.PagedList = pagedList;
            }
            return model;
        }

        public void SaveCatagories(string locName, string subName, string ownerName, string typeName, string vendorName)
        {
            var testLoc = _context.Tbl_CatagoryLocations.Where(x => x.LocationName == locName).FirstOrDefault();
            var catLoc = new Tbl_CatagoryLocations();
            if (testLoc == null)
            {
                if (subName != null)
                {
                    catLoc.LocationName = locName;
                    catLoc.Pid = null;
                    catLoc.HasChildren = true;
                }
                else
                {
                    catLoc.LocationName = locName;
                    catLoc.Pid = null;
                    catLoc.HasChildren = false;
                }

                _context.Tbl_CatagoryLocations.Add(catLoc);
                _context.SaveChanges();
            }


            //Get Parent Locations that Equal the current model's Location Name
            var parentId = _context.Tbl_CatagoryLocations.Where(x => x.LocationName == locName).FirstOrDefault().Id;

            //Determine if there are any sublocations that 
            //1) have a parent 
            //2) have the sublocation name as the model 
            //3) have a parent Id that is not equal to the current model's Id
            //4) where the parent referenced from the database is not equal to null
            bool noLocations = _context.Tbl_CatagoryLocations.Where(x => x.LocationName == subName && x.Pid == parentId).Any();


            if (noLocations == false)
            {
                var catLoc2 = new Tbl_CatagoryLocations
                {
                    LocationName = subName,
                    Pid = parentId
                };
                _context.Tbl_CatagoryLocations.Add(catLoc2);
                _context.SaveChanges();
            }

            var owners = _context.Tbl_CatagoryOwners.Where(x => x.OwnerName == ownerName).FirstOrDefault();
            if (owners == null)
            {
                var owner = new Tbl_CatagoryOwners
                {
                    OwnerName = ownerName,
                    Pid = null,
                    HasChildren = false
                };
                _context.Tbl_CatagoryOwners.Add(owner);
                _context.SaveChanges();
            }

            var types = _context.Tbl_CatagoryTypes.Where(x => x.TypeName == typeName).FirstOrDefault();
            if (types == null)
            {
                var type = new Tbl_CatagoryTypes
                {
                    TypeName = typeName,
                    Pid = null,
                    HasChildren = false
                };
                _context.Tbl_CatagoryTypes.Add(type);
                _context.SaveChanges();
            }

            var vendors = _context.Tbl_CatagoryVendors.Where(x => x.VendorName == vendorName).FirstOrDefault();
            if (vendors == null)
            {
                var vendor = new Tbl_CatagoryVendors
                {
                    VendorName = vendorName,
                    Pid = null,
                    HasChildren = false
                };
                _context.Tbl_CatagoryVendors.Add(vendor);
                _context.SaveChanges();
            }
            catLoc = null;
        }

        public Tbl_Items GetLocation(Tbl_Items item,string subName, string locName)
        {
            Tbl_Locations locObject = null;
            Tbl_SubLocations sublocation = null;
            if (_context.Tbl_Locations.Where(x => x.LocationName == locName).Any())
            {
                var subLoc = _context.Tbl_SubLocations.Where(x => x.SubLocationName == subName && x.Tbl_Locations.LocationName == locName).FirstOrDefault();
                var location = _context.Tbl_Locations.Where(x => x.LocationName == locName).FirstOrDefault();
                item.LocId = location.Id;
                
                if (subLoc != null)
                {
                    item.SubId = subLoc.Id;
                }
                else
                {
                        sublocation = new Tbl_SubLocations()
                        {
                            SubLocationName = subName,
                            Tbl_Locations = location
                        };
                        item.Tbl_SubLocations = sublocation;

                }
            }
            else
            {
                //create a new location
                locObject = new Tbl_Locations()
                {
                    LocationName = locName   //Create new location and assign it to the new location name
                };

                //create a new sublocation for the location
                sublocation = new Tbl_SubLocations()
                {
                    SubLocationName = subName,     //Take the sublocation name and assign it to a new sublocation
                    Tbl_Locations = locObject
                };

                //add these to the item
                item.Tbl_Locations = locObject;
                item.Tbl_SubLocations = sublocation;
            }
            return item;
        }

        public List<Tbl_Items> GetItemsUpload(string path)
        {
            //Read data from file
            var application = new Excel.Application();
            var workbook = application.Workbooks.Open(path);
            var sheet = new Excel.Worksheet();
            var firstName = "";
            var lastName = "";
            var loc = "";
            var subLoc = "";
            var index = -1;
            List<Tbl_Items> items = new List<Tbl_Items>();

            for (int j = 2; j <= workbook.Worksheets.Count; j++)
            {
                sheet = (Excel.Worksheet)workbook.Worksheets.get_Item(j);
                Excel.Range row = sheet.UsedRange;
                for(int i = 2; i <= row.Rows.Count; i++)
                {

                    var item = new Tbl_Items();
                    item.ItemName = row.Cells[i, 3].Text;
                    if(row.Cells[i, 4].Text == "")
                    {
                        item.Vendor = "_";
                    }
                    else
                    {
                        item.Vendor = row.Cells[i, 4].Text;
                    }
                    item.CatalogNumber = row.Cells[i, 5].Text;
                    item.Added = DateTime.Today;
                    firstName = row.Cells[i, 6].Text;
                    if(firstName != "")
                    firstName = firstName.Substring(0, firstName.IndexOf(' '));
                    lastName = row.Cells[i, 6].Text;
                    if(lastName != "")
                    lastName = lastName.Substring(lastName.LastIndexOf(' ') + 1);
                    var user = _context.Tbl_Users.Where(x => x.FirstName == firstName && x.LastName == lastName).FirstOrDefault();
                    if (user == null)
                    {
                        user = _context.Tbl_Users.Where(x => x.RoleId == 1).FirstOrDefault();
                        item.UsrId = user.Id;
                    }
                    else
                    {
                        item.UsrId = user.Id;
                    }
                    if (row.Cells[i, 7].Text == "")
                        loc = "_";
                    else
                        loc = row.Cells[i, 7].Text;
                    if (row.Cells[i, 8].Text == "")
                        subLoc = "_";
                    else
                        subLoc = row.Cells[i, 8].Text;
                    var sb = new StringBuilder(subLoc);
                    index = subLoc.IndexOf('/');
                    if(index != -1)
                    {                       
                        sb[index] = '-';
                        subLoc = sb.ToString();
                    }
                    item = GetLocation(item, subLoc, loc);
                    item.LocationComments = row.Cells[i, 9].Text;
                    var price = row.Cells[i, 10].Text;
                    if(price != "")
                    item.PurchasePrice = Convert.ToDecimal(price);
                    var amount = row.Cells[i, 11].Text;
                    if(amount != "" && amount != "Plenty of" && amount != "Many")
                    item.AmountInStock = Convert.ToInt32(amount);
                    item.WebAddress = row.Cells[i, 13].Text;
                    item.ItemNotes = row.Cells[i, 14].Text;
                    item.ItemType = sheet.Name;
                    item.PurchaseDate = null;
                    _context.Tbl_Items.Add(item);
                    _context.SaveChanges();
                    SaveCatagories(loc, subLoc, firstName + " " + lastName, sheet.Name, item.Vendor);
                }
            }
            
            application.Quit();
            if (workbook != null) { Marshal.ReleaseComObject(workbook); } //release each workbook like this
            if (sheet != null) { Marshal.ReleaseComObject(sheet); } //release each worksheet like this
            if (application != null) { Marshal.ReleaseComObject(application); } //release the Excel application
            workbook = null; //set each memory reference to null.
            sheet = null;
            application = null;
            return items;
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Import(FormCollection formCollection)
        {
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if (file == null || file.ContentLength == 0)
                {
                    var model = PopulateList();
                    ViewBag.ItemList = model.Items.ToList();
                    ViewBag.Error = "Please select an excel file";
                    return View("GetItemList", model);
                }
                else
                {
                    if (file.FileName.EndsWith("xls") || file.FileName.EndsWith("xlsx") || file.FileName.EndsWith("xlsm"))
                    {
                        string path = Server.MapPath("~/Item/" + file.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            var items = GetItemsUpload(path);
                                if (items == null)
                                {
                                    var model = PopulateList();
                                    ViewBag.Error = "One or more record values cannot be mapped to the database";
                                    return View("GetItemList", model);
                                }
                                else
                                {
                                    ViewBag.Success = "The database was uploaded successfully";
                                    return RedirectToAction("GetAllItems");
                                }

                        }
                         else
                         {
                                file.SaveAs(path);
                                var items = GetItemsUpload(path);
                                if (items == null)
                                {
                                    var model = PopulateList();
                                    ViewBag.Error = "One or more record values cannot be mapped to the database";
                                    return View("GetItemList", model);
                                }
                                else
                                {
                                    ViewBag.Success = "The database was uploaded successfully";
                                    return RedirectToAction("GetAllItems");
                                }

                        }

                    }
                    else
                    {
                        var model = PopulateList();
                        ViewBag.Error = "Please select .xls, .xlsx, or .xlsm type";
                        return View("GetItemList", model);
                    }
                }
                
            }
            else
            {
                var model = PopulateList();
                ViewBag.Error = "An error occured with the file request";
                return View("GetItemList", model);
            }
        }

        public ActionResult Export()
        {
            
            //string conString = "your connection string";
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * ");
            query.Append("FROM [dbo].[Tbl_Items] ");
            query.Append("ORDER BY ItemType ");
            string cstr = ConfigurationManager.ConnectionStrings["PennStateDB"].ToString();
            if (cstr.ToLower().StartsWith("metadata="))
            {
                System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder efBuilder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(cstr);
                cstr = efBuilder.ProviderConnectionString;
            }
            SQL.DataTable dtItems = new SQL.DataTable();
            using (SqlConnection conn = new SqlConnection(cstr))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    
                    using (SqlDataAdapter da = new SqlDataAdapter(query.ToString(), conn))
                    {
                        da.Fill(dtItems);
                    }
                    conn.Close();
                }
            }

        
            dtItems.Columns["Id"].SetOrdinal(1);
            dtItems.Columns["IsDeleted"].SetOrdinal(2);
            dtItems.Columns["ItemName"].SetOrdinal(3);
            dtItems.Columns["Vendor"].SetOrdinal(4);
            dtItems.Columns["CatalogNumber"].SetOrdinal(5);
            dtItems.Columns.Add("Owner").SetOrdinal(6);
            dtItems.Columns.Add("Location").SetOrdinal(7);
            dtItems.Columns.Add("Sub-location").SetOrdinal(8);
            dtItems.Columns["LocationComments"].SetOrdinal(9);
            dtItems.Columns["PurchasePrice"].SetOrdinal(10);
            dtItems.Columns["AmountInStock"].SetOrdinal(11);
            dtItems.Columns["Manufacturer"].SetOrdinal(12);
            dtItems.Columns["WebAddress"].SetOrdinal(14);
            dtItems.Columns["ItemNotes"].SetOrdinal(15);
            dtItems.Columns.RemoveAt(26);
            dtItems.Columns.RemoveAt(25);
            dtItems.Columns.RemoveAt(24);
            dtItems.Columns.RemoveAt(23);
            dtItems.DefaultView.Sort = "ItemType desc";
            Excel.Application oXL = new Excel.Application();
            Excel._Workbook oWB;
            Excel._Worksheet oSheet;
            oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
            oSheet = (Excel._Worksheet)oWB.ActiveSheet;
            try
            {
                SQL.DataTable dtCatagoryTypes = dtItems.DefaultView.ToTable(true, "ItemType");
                foreach(SQL.DataRow catagory in dtCatagoryTypes.Rows)
                {
                    oSheet = (Excel._Worksheet)oXL.Worksheets.Add();
                    oSheet.Name = catagory[0].ToString();
                    string[] colNames = new string[dtItems.Columns.Count];
                    int col = 0;

                    foreach (SQL.DataColumn dc in dtItems.Columns)
                        colNames[col++] = dc.ColumnName;

                    char lastColumn = (char)(65 + dtItems.Columns.Count - 1);

                    oSheet.get_Range("A1", lastColumn + "1").Value2 = colNames;
                    oSheet.get_Range("A1", lastColumn + "1").Font.Bold = true;
                    oSheet.get_Range("A1", lastColumn + "1").VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    SQL.DataRow[] dr = dtItems.Select(string.Format("ItemType='{0}'",catagory[0].ToString()));

                    string[,] rowData = new string[dr.Count<SQL.DataRow>(), dtItems.Columns.Count];

                    int rowCnt = 0;
                    int redRows = 2;
                    foreach (SQL.DataRow row in dr)
                    {
                        for (col = 0; col < dtItems.Columns.Count; col++)
                        {
                            if (col == 5)
                            {
                                var id = Convert.ToInt32(row[20].ToString());
                                var f = _context.Tbl_Users.Where(x => x.Id == id).FirstOrDefault().FirstName;
                                var l = _context.Tbl_Users.Where(x => x.Id == id).FirstOrDefault().LastName;
                                rowData[rowCnt, col] = f + " " + l;
                            }
                            else if (col == 6)
                            {
                                var id = Convert.ToInt32(row[21].ToString());
                                var l = _context.Tbl_Locations.Where(x => x.Id == id).FirstOrDefault().LocationName;
                                rowData[rowCnt, col] = l;
                            }
                            else if (col == 7)
                            {
                                var id = Convert.ToInt32(row[22].ToString());
                                var s = _context.Tbl_SubLocations.Where(x => x.Id == id).FirstOrDefault().SubLocationName;
                                rowData[rowCnt, col] = s;
                            }
                            else
                            {
                                rowData[rowCnt, col] = row[col].ToString();
                            }
                        }

                        redRows++;
                        rowCnt++;
                    }
                    rowCnt++;
                    oSheet.get_Range("A2", lastColumn + rowCnt.ToString()).Value2 = rowData;

                }
                oXL.Worksheets[oXL.Sheets.Count].Delete();
                oSheet = (Excel._Worksheet)oXL.Worksheets.Add();
                oSheet.Name = "Test";

                oXL.Visible = true;
                oXL.UserControl = true;

                //oWB.SaveAs("Products.xlsx",
                //    AccessMode: Excel.XlSaveAsAccessMode.xlShared);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject(oWB);
            }

            ViewBag.Success = "The database was successfully export to an Excel File.";
            return RedirectToAction("GetAllItems");

        }

        [HttpGet]
        public ActionResult GetRequests()
        {
            RequestViewModel model = new RequestViewModel();
            if (_context.Tbl_Requests.Any())
            {
                var requests = Mapper.Map<IEnumerable<Requests>>(_context.Tbl_Requests.ToList());
                model.Requests = requests;
            }
            else
            {
                TempData["SM"] = "There are no pending requests.";
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult FlagItem(int? id)
        {

            FlagItemViewModel model = new FlagItemViewModel();
            using (PennStateDB db = new PennStateDB())
            {
                if (id != null)
                {
                    model.TheItem = new Item();
                    model.TheRequest = new Requests();
                    model.TheItem = Mapper.Map<Item>(db.Tbl_Items.Where(x => x.Id == id).FirstOrDefault());
                    model.TheRequest.UnitPrice = model.TheItem.PurchasePrice;
                }
                else
                {
                    model.TheItem = null;
                    model.TheRequest = new Requests();
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult FlagItem(FlagItemViewModel model)
        {
            Tbl_Requests request = new Tbl_Requests();
            if (model.TheItem.ItemName != null)
            {
                request.ItemName = model.TheItem.ItemName;
                var item = _context.Tbl_Items.Find(model.TheItem.Id);

                if (model.TheItem.Flagged != null)
                {
                    item.Flagged = model.TheItem.Flagged;
                    request.Message = model.TheItem.Flagged;
                }
                else
                    item.Flagged = "Requested";
            }
            else
            {
                request.ItemName = model.TheRequest.ItemName;
                request.Message = model.TheRequest.Message;
            }
            var id = _context.Tbl_Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (model.TheItem.ItemName != null)
                request.ItemId = model.TheItem.Id;
            else
                request.ItemId = null;
                
            request.UserId = id.Id;   
            if(model.TheRequest.TotalPrice != null)
            {
                var price = Convert.ToDecimal(model.TheRequest.TotalPrice.Substring(1));
                request.TotalPrice = price;
            }
            request.Quantity = model.TheRequest.Quantity;
            request.Status = model.TheRequest.StatEnum.ToString();
            //request.TotalPrice = model.TheRequest.TotalPrice;
            request.UnitPrice = model.TheRequest.UnitPrice;
            _context.Tbl_Requests.Add(request);
            _context.SaveChanges();
            var items = PopulateList();
            return View("GetItemList", items);
        }
    }
}