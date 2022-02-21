using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore5CRUD.Models;
using DotNetCore5CRUD.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace DotNetCore5CRUD.Controllers
{
    public class AnimesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private new List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _MaxAllowedPosterSize = 1048576;

        public AnimesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // Index
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Animes.OrderByDescending(m => m.Rate).ToListAsync();

            return View(movies);
        }


        // Create 
        public async Task<IActionResult> Create()
        {
            var viewModel = new AnimesFormViewModel
            {
                Categories = await _context.Categories.OrderBy(a => a.Name).ToListAsync()
            };
            return View("AnimeForm", viewModel);
        }

        // Create Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnimesFormViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.OrderBy(a => a.Name).ToListAsync();
                return View("AnimeForm", model);
            }

            var files = Request.Form.Files;

            if(!files.Any())
            {
                model.Categories = await _context.Categories.OrderBy(a => a.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please Select Anime Poster!");
                return View("AnimeForm", model);
            }

            var poster = files.FirstOrDefault();
            

            if(!_allowedExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Categories = await _context.Categories.OrderBy(a => a.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .png & jpg images are allowed!");
                return View("AnimeForm", model);
            }

            if(poster.Length > _MaxAllowedPosterSize)
            {
                model.Categories = await _context.Categories.OrderBy(a => a.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Size of poster can not be more than 1MB!");
                return View("AnimeForm", model);
            }

            using var dataStream = new MemoryStream();
            await poster.CopyToAsync(dataStream);

            var animes = new Anime
            {
                Title = model.Title,
                CategoryId = model.CategoryId,
                Year = model.Year,
                Rate = model.Rate,
                Story = model.Story,
                Poster = dataStream.ToArray()
            };

            _context.Animes.Add(animes);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Anime Created Successfully");

            return RedirectToAction("Index");
        }


        // Edit 
        public async Task<ActionResult> Edit(int? id)
        {
            if(id == null)
            
                return BadRequest();
            var anime = await _context.Animes.FindAsync(id);

            if (anime == null)
                return NotFound();

            var viewModel = new AnimesFormViewModel
            {
                Id = anime.Id,
                Title = anime.Title,
                CategoryId = anime.CategoryId,
                Year = anime.Year,
                Rate = anime.Rate,
                Story = anime.Story, 
                Categories = await _context.Categories.OrderBy(a => a.Name).ToListAsync()
            };

            return View("AnimeForm", viewModel);
        }

        // Edit Post 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AnimesFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.OrderBy(a => a.Name).ToListAsync();
                return View("AnimeForm", model);
            }

           
            var anime = await _context.Animes.FindAsync(model.Id);

            if (anime == null)
                return NotFound();

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();

                using var dataStream = new MemoryStream();

                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

                if (!_allowedExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Categories = await _context.Categories.OrderBy(a => a.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .png & jpg images are allowed!");
                    return View("AnimeForm", model);
                }

                if (poster.Length > _MaxAllowedPosterSize)
                {
                    model.Categories = await _context.Categories.OrderBy(a => a.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Size of poster can not be more than 1MB!");
                    return View("AnimeForm", model);
                }

                anime.Poster = model.Poster;
            }
            
            anime.Title = model.Title;
            anime.CategoryId = model.CategoryId;
            anime.Year = model.Year;
            anime.Rate = model.Rate;
            anime.Story = model.Story;

            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Anime Updated Successfully");

            return RedirectToAction("Index");
        }
    
        // Details 
        public async Task<ActionResult> Details(int? id) 
        {
            if (id == null)
                return BadRequest();

            var anime = await _context.Animes.Include(m => m.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (anime == null)
                return NotFound();

            return View(anime);
        }


        // Delete 
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var anime = await _context.Animes.FindAsync(id);

            if (anime == null)
                return NotFound();


            _context.Animes.Remove(anime);
            _context.SaveChanges();

            return Ok();
        }
    }
}
