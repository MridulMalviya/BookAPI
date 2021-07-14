using BookAPI.Models;
using BookAPI.Repositories;
using BookAPI.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private static IWebHostEnvironment _hostingEnvironment;

        public BooksController(IBookRepository bookRepository, IWebHostEnvironment environment)
        {
            _bookRepository = bookRepository;
            _hostingEnvironment = environment;
        }

        [HttpGet]
        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _bookRepository.Get();
        }

        //[HttpGet]
        //public async Task<ActionResult> GetBooks1()
        //{
        //    try
        //    {
        //        return Ok(await _bookRepository.Get());

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,"Error retriving data from database");
        //    }
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBooks(int id)
        {
            return await _bookRepository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> PostBooks([FromForm] BookViewModel book)
        {
             string uniqueFileName = UploadedFile(book);

            Book book1 = new Book();
            book1.Id = book.Id;
            book1.Title = book.Title;
            book1.Author = book.Author;
            book1.Description = book.Description;
            book1.BookImage = uniqueFileName;
            var newBook = await _bookRepository.Create(book1);
            return CreatedAtAction(nameof(GetBooks), new { id = newBook.Id }, newBook);
        }

        private string UploadedFile(BookViewModel model)
        {
            string filePath = null;

            if (model.Files != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "images");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Files.FileName;
                filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Files.CopyTo(fileStream);
                }
            }
            return filePath;
        }

        //[HttpPost]
        //public async Task<string> UploadFile([FromForm] IFormFile file)
        //{
        //    string fName = file.FileName;
        //    string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Images/" + fName);
        //    using (var stream = new FileStream(path, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }
        //    return file.FileName;
   // }

        [HttpPut]
        public async Task<ActionResult> PutBooks(int id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            await _bookRepository.Update(book);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var bookToDelete = await _bookRepository.Get(id);
            if (bookToDelete == null)
                return NotFound();

            await _bookRepository.Delete(bookToDelete.Id);
            return NoContent();
        }
    }
}
