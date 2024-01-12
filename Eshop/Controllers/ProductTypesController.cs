using Eshop.Data;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Controllers
{
    public class ProductTypesController : Controller
    {
        private EshopContext _context;

        public ProductTypesController(EshopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var ListProductType = _context.ProductTypes.ToList();
            return View(ListProductType);
        }
        public IActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var productType = _context.ProductTypes.Find(Id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductType productType)
        {
            _context.ProductTypes.Add(productType);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int Id)
        {
            var LayID = _context.ProductTypes.Find(Id);
            return View(LayID);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,Name,Status")] ProductType productType)
        {
            if (id != productType.Id)
            {
                return NotFound();
            }
            _context.ProductTypes.Update(productType);
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
            var productType = _context.ProductTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }
            _context.ProductTypes.Remove(productType);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
