using System;
using Microsoft.AspNetCore.Http;

namespace BookAPI.ViewModel
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public IFormFile Files { set; get; }
    }
}
