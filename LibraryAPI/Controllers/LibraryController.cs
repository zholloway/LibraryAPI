using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LibraryAPI.Models;
using System.Data.SqlClient;

namespace LibraryAPI.Controllers
{
    public class LibraryController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetAllBooks()
        {
            return Ok(Book.GetAllBooks(Setting.PathToLibraryDatabase));
        }
        
        [HttpPut]
        public IHttpActionResult AddNewBook(Book newBook)
        {
            Book.AddNewBook(newBook, Setting.PathToLibraryDatabase);
            return Ok("The book was successfully added.");           
        }

        [HttpDelete]
        public IHttpActionResult DeleteBook(string ID)
        {
            Book.DeleteBook(Setting.PathToLibraryDatabase, ID);
            return Ok($"BookID = {ID} was successfully deleted.");
        }
    }
}
