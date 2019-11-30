using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShortcutUrlApp.Data;
using ShortcutUrlApp.Domain;

namespace WebApp.Controllers
{
    public class UrlsController : Controller
    {
        private readonly ShortcutUrlContext _context;

        public UrlsController(ShortcutUrlContext context)
        {
            _context = context;
        }

        // GET: Urls
        public async Task<IActionResult> Index()
        {
            return View(await _context.Urls.ToListAsync());
        }

        // GET: Urls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = await _context.Urls
                .SingleOrDefaultAsync(m => m.Id == id);
            if (url == null)
            {
                return NotFound();
            }

            return View(url);
        }

        // GET: Urls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Urls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Original,Shortened,ConversionCount")] Url url)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(url);
                    _context.Entry(url).Property("Created").CurrentValue = DateTime.Now;
                    _context.Entry(url).Property("LastModified").CurrentValue = DateTime.Now;
                    _context.SaveChanges();
                }
                catch
                {
                    ModelState.AddModelError("Created", "Ошибка");
                    return View();
                }

                    return RedirectToAction(nameof(Index));
            }
            return View(url);
        }

        // GET: Urls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _context.Urls.SingleOrDefault(m => m.Id == id);
            if (url == null)
            {
                return NotFound();
            }
            return View(url);
        }

        // POST: Urls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Url url)
        {
            if (id != url.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(url).Property("LastModified").CurrentValue = DateTime.Now;
                    _context.Update(url);
                    _context.SaveChanges();
                }
                catch 
                {
                    if (!UrlExists(url.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("Created", "Ошибка");
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(url);
        }

        // GET: Urls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = await _context.Urls
                .SingleOrDefaultAsync(m => m.Id == id);
            if (url == null)
            {
                return NotFound();
            }

            return View(url);
        }

        // POST: Urls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var url = await _context.Urls.SingleOrDefaultAsync(m => m.Id == id);
            _context.Urls.Remove(url);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool UrlExists(int id)
        {
            return _context.Urls.Any(e => e.Id == id);
        }


        public async Task<IActionResult> ConversionCount()
        {

            ViewData["Message"] = "Подсчет переходов";

            string path = HttpContext.Request.Path.Value;
            path = path.Substring(1, 6);

            var urlObject = _context.Urls.SingleOrDefault(u => u.Shortened == path);
            


            if (urlObject == null)
            {
                ViewData["Message"] = "Не найден адрес " + path;
                //return NotFound();
            }

            else
            {
                urlObject.ConversionCount = urlObject.ConversionCount + 1;
                _context.Update(urlObject);
                _context.SaveChanges();
                ViewData["Message"] = "Текущий адрес " + path;

            }

            return View();
        }


        [HttpPost]
        public JsonResult getUniqueShortcut()
        {

            string shortcut;
            Url urlObject;


            do
            {
                shortcut = getRandomShortcut();
                urlObject = _context.Urls.Where(u => u.Shortened == shortcut).FirstOrDefault();
            }
            while  (urlObject != null);


            return Json(shortcut);

        }


        private string getRandomShortcut()
        {
            Random random = new Random();
            string dictionaryString = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder resultStringBuilder = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                resultStringBuilder.Append(dictionaryString[random.Next(35)]);
            }
            return resultStringBuilder.ToString();
        }


    }
}
