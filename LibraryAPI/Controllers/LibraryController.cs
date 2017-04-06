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
        const string PathToLibraryDatabase = @"Server=localhost\SQLEXPRESS;Database=Library;Trusted_Connection=True;";

        [HttpGet]
        public IHttpActionResult GetAllBooks()
        {
            using (var connection = new SqlConnection(PathToLibraryDatabase))
            {
                return Ok(Book.GetAllBooks(connection));
            }
        }

        [HttpPut]
        public IHttpActionResult AddNewBook(Book newBook)
        {
            Book.AddNewBook(newBook, PathToLibraryDatabase);
            return Ok("The book was successfully added.");           
        }
    }
}
