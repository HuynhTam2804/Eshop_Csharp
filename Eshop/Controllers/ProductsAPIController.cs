using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;

namespace Eshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly EshopContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductsAPIController(EshopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        // GET: api/ProductsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(p => p.ProductType).ToListAsync();
        }

        // GET: api/ProductsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }
        [HttpGet("search/{keyword}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct(string keyword)
        {
            var result = await _context.Products
                .Include(p => p.ProductType)
                .Where(p => p.Name.Contains(keyword))
                .ToListAsync();
            return result;
        }
        // PUT: api/ProductsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProductsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] Product product)
        {
            product.Image = "";
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            if (product.ImageFile != null && product.ImageFile.ContentType.StartsWith("image"))
            {
                var fileName = product.Id.ToString() + DateTime.Now.ToString() + Path.GetExtension(product.ImageFile.FileName);
                var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "product");
                var uploadPath = Path.Combine(uploadFolder, fileName);
                System.IO.Directory.CreateDirectory(uploadPath);
                using (FileStream fs = System.IO.File.Create(uploadPath))
                {
                    await product.ImageFile.CopyToAsync(fs);
                    fs.Flush();
                }
                product.Image = fileName;
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            await _context.Entry(product).Reference(p => p.ProductType).LoadAsync();
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/ProductsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
