using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            var tvShows = _context.TVShows.ToList();
            var viewModel = new MovieTVShowViewModel
            {
                Movies = movies,
                TVShows = tvShows
            };
            return View(viewModel);
        }

        // Додано нове дію для пошуку
        public IActionResult Search(string query)
        {
            // Якщо пошуковий запит порожній, повертаємо всі фільми та ТВ-шоу
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index");
            }

            // Фільтрація фільмів за назвою
            var movies = _context.Movies
                .Where(m => m.Title.Contains(query))
                .ToList();

            // Фільтрація ТВ-шоу за назвою
            var tvShows = _context.TVShows
                .Where(t => t.Title.Contains(query))
                .ToList();

            // Створення ViewModel з фільмами та ТВ-шоу, які відповідають пошуковому запиту
            var viewModel = new MovieTVShowViewModel
            {
                Movies = movies,
                TVShows = tvShows
            };

            return View("Index", viewModel); // Повертаємо результат на ту ж саму сторінку
        }

        public IActionResult MovieDetail(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        public IActionResult TVShowDetail(int id)
        {
            var tvShow = _context.TVShows.FirstOrDefault(t => t.Id == id);
            if (tvShow == null)
            {
                return NotFound();
            }
            return View(tvShow);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
