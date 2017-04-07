using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LibraryAPI.Models;
using System.Data.SqlClient;
using LibraryAPI.Controllers;

namespace LibraryAPI.Controllers
{
    public class CheckoutController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetBooks(string IsCheckedOut)
        {
            using (var connection = new SqlConnection(Setting.PathToLibraryDatabase))
            {
                return Ok(Book.GetBooks(connection, IsCheckedOut));
            }
        }
    }
}
