﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    [Route("comments")]
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public IActionResult AddComment(int? movieId, int? tvShowId, string author, string content, int rating)
        {
            // Перевірка на наявність фільму або ТВ-шоу
            if (movieId == null && tvShowId == null)
            {
                return BadRequest("Film veya dizi kimliği sağlanmadı.");
            }

            // Перевірка на оцінку
            if (rating < 1 || rating > 5)
            {
                return BadRequest("Geçersiz veri. Lütfen geçerli bir puan girin.");
            }

            var comment = new Comment
            {
                MovieId = movieId,
                TVShowId = tvShowId,
                Author = author,
                Content = content ?? string.Empty, // Якщо коментар порожній, зробимо його порожнім рядком
                Date = DateTime.Now,
                Rating = rating
            };

            try
            {
                _context.Comments.Add(comment);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Veritabanına eklerken bir hata oluştu: " + ex.Message);
            }

            // Переадресація на деталі фільму або ТВ-шоу після додавання коментаря
            if (movieId.HasValue)
            {
                return RedirectToAction("MovieDetail", "Comment", new { id = movieId.Value });
            }
            else
            {
                return RedirectToAction("TVShowDetail", "Comment", new { id = tvShowId.Value });
            }
        }

        [HttpGet("movie/{id}")]
        public IActionResult MovieDetail(int id)
        {
            var movie = _context.Movies.Include(m => m.Comments)
                                       .FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View("MovieDetail", movie);
        }

        [HttpGet("tvshow/{id}")]
        public IActionResult TVShowDetail(int id)
        {
            var tvShow = _context.TVShows.Include(ts => ts.Comments)
                                          .FirstOrDefault(ts => ts.Id == id);
            if (tvShow == null)
            {
                return NotFound();
            }

            return View("TVShowDetail", tvShow);
        }
    }
}
