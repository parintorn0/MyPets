using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyPets.Models;

namespace Pet.Controllers
{
    // [Route("[controller]")]
    public class PetController : Controller
    {
        private readonly MyPetsDbContext _context;
        private readonly IWebHostEnvironment environment;

        // public MyPetsController(ILogger<MyPetsController> logger)
        // {
        //     _logger = logger;
        // }

        public PetController(MyPetsDbContext myPetsDbContext,IWebHostEnvironment environment){
            _context = myPetsDbContext;
            this.environment = environment;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page, int? pageSize) {
            int pageNumber = page ?? 1;
            int defaultPageSize = pageSize ?? 4;
            int totalRecords = await _context.MyPets.CountAsync();
            var myPetsList = await _context.MyPets.OrderByDescending(x => x.Id).Skip((pageNumber - 1) * defaultPageSize).Take(defaultPageSize).ToListAsync();
            foreach(var myPets in myPetsList){
                if(myPets.PhotoFileName == ""|myPets.PhotoFileName==null){
                    myPets.PhotoFileName = "nodata.png";
                }
            }
            var pagedMyPetsList = new PagedList<MyPetsViewModel>(myPetsList, pageNumber, defaultPageSize, totalRecords);
            return View(pagedMyPetsList);
        }

        [HttpGet] //show blank form
        public IActionResult Create(){
            return View();
        }
        [HttpPost] //filled form
        public async Task<IActionResult> Create(MyPetsViewModel addMyPetsViewModel, IFormFile ImageFile){
            try{
                string? strImageFile = "nodata.png";
                if(ImageFile!=null){
                    string strDateTime = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    strImageFile = $"{strDateTime}_{ImageFile.FileName}";
                    string? PhotoFullPath = $"{this.environment.WebRootPath}/images/{strImageFile}";
                    using(var fileStream = new FileStream(PhotoFullPath, FileMode.Create)){
                        await ImageFile.CopyToAsync(fileStream);
                    }
                }
                MyPetsViewModel myPetsViewModel = new MyPetsViewModel() {
                    Id = addMyPetsViewModel.Id,
                    PetName = addMyPetsViewModel.PetName,
                    Gender = addMyPetsViewModel.Gender,
                    DateOfBirth = addMyPetsViewModel.DateOfBirth,
                    PhotoFileName = strImageFile,
                };
                await _context.AddAsync(myPetsViewModel);
                await _context.SaveChangesAsync();
                TempData["successMessage"] = $"New Pet Added ({addMyPetsViewModel.PetName})";
                return RedirectToAction(nameof(Index));
            } catch (Exception ex){
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id){
            try{
                var myPets = await _context.MyPets.SingleOrDefaultAsync(n => n.Id == id);
                TempData["PhotoFilePath"] = $"/images/{myPets.PhotoFileName}";
                return View(myPets);
            }
            catch (Exception ex){
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(MyPetsViewModel myPetsViewModel, IFormFile ImageFile) {
            try {
                var myPets = await _context.MyPets.SingleOrDefaultAsync(n => n.Id == myPetsViewModel.Id);
                if (myPets == null) {
                    return View("No data");
                }
                else {
                    myPets.PetName = myPetsViewModel.PetName;
                    myPets.Gender = myPetsViewModel.Gender;
                    myPets.DateOfBirth = myPetsViewModel.DateOfBirth;
                    //for photo
                    if(ImageFile!=null) {
                        string strDateTime = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                        string strImageFile = strDateTime + "_" + ImageFile.FileName;
                        string? PhotoFullPath = $"{this.environment.WebRootPath}/images/{strImageFile}";
                        using(var fileStream = new FileStream(PhotoFullPath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }
                        myPets.PhotoFileName = strImageFile;//use new photo file data
                    }
                    else {
                        myPets.PhotoFileName = myPetsViewModel.PhotoFileName;//use existing img file data
                    }
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"{myPetsViewModel.PetName} was Edited";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex) {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id) {
            try {
                var myPets = await _context.MyPets.SingleOrDefaultAsync(n => n.Id == id);
                TempData["PhotoFilePath"] = $"/images/{myPets.PhotoFileName}";
                return View(myPets);
            }
            catch(Exception ex) {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(MyPetsViewModel myPetsViewModel) {
            try {
                var myPets = await _context.MyPets.SingleOrDefaultAsync(n => n.Id == myPetsViewModel.Id);
                if (myPets == null) {
                    TempData["errorMessage"] = $"Product Not Found with Id {myPetsViewModel.Id}";
                    return View("No data");
                }
                else {
                    var name = myPetsViewModel.PetName;
                    _context.MyPets.Remove(myPets);
                    await _context.SaveChangesAsync();
                    TempData["successMessage"] = $"{name} was Deleted";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(Exception ex) {
                TempData["errorMessage"] = ex.Message + "<br/>" + ex.StackTrace;
                return View();
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}