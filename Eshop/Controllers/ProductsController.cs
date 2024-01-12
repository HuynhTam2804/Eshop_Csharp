using Eshop.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers
{
    public class ProductsController : Controller
    {
        private EshopContext _context;
        private IWebHostEnvironment _environment;


        public ProductsController(EshopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View(_context.Products.Include(p => p.ProductType).ToList());
        }
        public IActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var product = _context.Products.Find(Id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        public IActionResult Create()
        {
            ViewBag.ProductTypes = new SelectList(_context.ProductTypes,"Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create([Bind("SKU,Name,Description,Price,Stock,ProductTypeId,ImageFile,Status")] Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            if (product.ImageFile != null)
            {
                if (!product.ImageFile.ContentType.StartsWith("image"))
                {
                    ViewBag.ErrorMessage = "Chỉ được upload file hình ảnh";
                    ViewBag.ProductTypes = new SelectList(_context.ProductTypes, "Id", "Name");
                    return View(product);
                }
                if (product.ImageFile.Length > 800 * 1024)
                {
                    ViewBag.ErrorMessage = "File vượt quá dung lượng cho phép (800 KB)";
                    ViewBag.ProductTypes = new SelectList(_context.ProductTypes, "Id", "Name");
                    return View(product);
                }

                var fileName = product.Id.ToString()+ Path.GetExtension(product.ImageFile.FileName);
                var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "product");
                var uploadPath = Path.Combine(uploadFolder, fileName);
                using (FileStream fs = System.IO.File.Create(uploadPath))
                {
                    product.ImageFile.CopyTo(fs);
                    fs.Flush();
                }

                product.Image = fileName;
                _context.Products.Update(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            var product = _context.Products.Find(id);
            ViewBag.ProductTypes = new SelectList(_context.ProductTypes,
            "Id", "Name", product.Id);
            return View(product);

        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("SKU,Name,Description,Price,Stock,ProductTypeId,ImageFile,Status")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            product.Image= _context.Products.AsNoTracking()
               .FirstOrDefault(a => a.Id == id).Image;
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
