using MoviesProject.Models;
using MoviesProject.Models.Context;
using MoviesProject.VeiwModel;
using MoviesProject.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace MoviesProject.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController()
        {
            _context = new ApplicationDbContext();
        }

        public async Task<ActionResult> Index()
        {
            var movies = await _context.Movies.Include(m => m.Genre).ToListAsync();
            return View(movies);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string searchbyMovie, string searchbyYear,
            string searchbyGenre)
        {
            if (string.IsNullOrEmpty(searchbyMovie) && string.IsNullOrEmpty(searchbyYear) && string.IsNullOrEmpty(searchbyGenre))
            {
                var movies = await _context.Movies.Include(m => m.Genre).ToListAsync();
                return View(movies);
            }
            else if (!string.IsNullOrEmpty(searchbyMovie) && !string.IsNullOrEmpty(searchbyGenre) && !string.IsNullOrEmpty(searchbyYear))
            {
                var movies = await _context.Movies.Include(m => m.Genre).Where(m => m.Name.StartsWith(searchbyMovie)).Where(m => m.Genre.Name.StartsWith(searchbyGenre)).Where(m => m.Year.StartsWith(searchbyYear)).ToListAsync();
                return View(movies);
            }
            else if (!string.IsNullOrEmpty(searchbyMovie) && !string.IsNullOrEmpty(searchbyGenre))
            {
                var movies = await _context.Movies.Include(m => m.Genre).Where(m => m.Name.StartsWith(searchbyMovie)).Where(m => m.Genre.Name.StartsWith(searchbyGenre)).ToListAsync();
                return View(movies);
            }
            else if (!string.IsNullOrEmpty(searchbyMovie) && !string.IsNullOrEmpty(searchbyYear))
            {
                var movies = await _context.Movies.Include(m => m.Genre).Where(m => m.Name.StartsWith(searchbyMovie)).Where(m => m.Year.StartsWith(searchbyYear)).ToListAsync();
                return View(movies);
            }
            else if (!string.IsNullOrEmpty(searchbyGenre) && !string.IsNullOrEmpty(searchbyYear))
            {
                var movies = await _context.Movies.Include(m => m.Genre).Where(m => m.Genre.Name.StartsWith(searchbyGenre)).Where(m => m.Year.StartsWith(searchbyYear)).ToListAsync();
                return View(movies);
            }
            else
            {
                if (!string.IsNullOrEmpty(searchbyMovie))
                {
                    var movies = await _context.Movies.Include(m => m.Genre).Where(m => m.Name.StartsWith(searchbyMovie)).ToListAsync();
                    return View(movies);
                }
                else if (!string.IsNullOrEmpty(searchbyGenre))
                {
                    var movies = await _context.Movies.Include(m => m.Genre).Where(m => m.Genre.Name.StartsWith(searchbyGenre)).ToListAsync();
                    return View(movies);
                }
                else if (!string.IsNullOrEmpty(searchbyYear))
                {
                    var movies = await _context.Movies.Include(m => m.Genre).Where(m => m.Year.StartsWith(searchbyYear)).ToListAsync();
                    return View(movies);
                }

                return HttpNotFound();

            }
            //if (string.IsNullOrEmpty(searchbyMovie))
            //{
            //    var movies = await _context.Movies.Include(m => m.Genre).ToListAsync();
            //    return View(movies);
            //}
            //else
            //{
            //    var movies = await _context.Movies.Include(m => m.Genre).Where(m => m.Name.StartsWith(searchbyMovie)).ToListAsync();
            //    return View(movies);
            //}
        }

        public async Task<JsonResult> GetMoviesAsync(string term)
        {
            List<string> movies = await _context.Movies.Where(m => m.Name.StartsWith(term)).Select(x => x.Name).ToListAsync();

            return Json(movies, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetGenresAsync(string term)
        {
            List<string> genres = await _context.Genres.Where(m => m.Name.StartsWith(term)).Select(x => x.Name).ToListAsync();

            return Json(genres, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetYearAsync(string term)
        {
            List<string> years = await _context.Movies.Where(m => m.Year.StartsWith(term)).Select(x => x.Year).Distinct().ToListAsync();

            return Json(years, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Create()
        {
            var viewModel = new MovieViewModel
            {
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("Create", model);
            }

            var movies = new Movies
            {
                Name = model.Name,
                GenreId = model.GenreId,
                Year = model.Year,
                
            };

            _context.Movies.Add(movies);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return HttpNotFound();

            var viewModel = new MovieViewModel
            {
                Id = movie.Id,
                Name = movie.Name,
                GenreId = movie.GenreId,
                Year = movie.Year,
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("Edit", model);
            }

            var movie = await _context.Movies.FindAsync(model.Id);

            if (movie == null)
                return HttpNotFound();
               
            model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
            movie.Name = model.Name;
            movie.GenreId = model.GenreId;
            movie.Year = model.Year;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return HttpNotFound();

            return View(movie);
        }
        [HttpDelete]

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return HttpNotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return View(nameof(Index));
        }
    }
}