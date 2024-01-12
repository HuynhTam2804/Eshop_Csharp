using Microsoft.AspNetCore.Mvc;
using Eshop.Data;
using Microsoft.EntityFrameworkCore;
using Eshop.Models;
using NuGet.Protocol.Plugins;

namespace Eshop.Controllers
{
    public class AccountsController : Controller
    {
        private EshopContext _context;
        private IWebHostEnvironment _environment;
        public AccountsController(EshopContext context, IWebHostEnvironment enviroment)
        {
            _context = context;
            _environment = enviroment;
        }
        public IActionResult Index()
        {
            var ListAccount = _context.Accounts.ToList();
            return View(ListAccount);

        }
        public IActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var account = _context.Accounts.Find(Id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create([Bind("Username,Password,Email,Phone,Address,FullName,IsAdmin,AvatarFile,Status")] Account account)
        {
            if (_context.Accounts.Any(a => a.Username == account.Username))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại.";
                return View();
            }
            _context.Accounts.Add(account);
            _context.SaveChanges();

            if (account.AvatarFile != null)
            {
                if (!account.AvatarFile.ContentType.StartsWith("image"))
                {
                    ViewBag.ErrorMessage = "Chỉ được upload file hình ảnh";
                    return View(account);
                }

                if (account.AvatarFile.Length > 800 * 1024)
                {
                    ViewBag.ErrorMessage = "File vượt quá dung lượng cho phép (800 KB)";
                    return View(account);
                }
                var fileName = account.Id.ToString()+Path.GetExtension(account.AvatarFile.FileName);
                var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "avatar");
                var uploadPath = Path.Combine(uploadFolder, fileName);
                using (FileStream fs = System.IO.File.Create(uploadPath))
                {
                    account.AvatarFile.CopyTo(fs);
                    fs.Flush();
                }
                account.Avatar = fileName;
                _context.Accounts.Update(account);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
            
        }

        public IActionResult Edit(int Id)
        {
            var account = _context.Accounts.Find(Id);
            return View(account);
        }
        [HttpPost]
        public IActionResult Edit(int id, [Bind("Username,Password,Email,Phone,Address,FullName,IsAdmin,AvatarFile,Status")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }
            if (account.Password == null)
            {
                account.Password = _context.Accounts.AsNoTracking()
                    .FirstOrDefault(a => a.Id == id).Password;
            }
            account.Avatar = _context.Accounts.AsNoTracking()
                .FirstOrDefault(a => a.Id == id).Avatar;
            _context.Accounts.Update(account);
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
            var account = _context.Accounts.Find(id);
            if (account == null)
            {
                return NotFound();
            }
            _context.Accounts.Remove(account);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
