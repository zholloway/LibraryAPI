using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.SqlClient;

namespace LibraryAPI.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int YearPublished { get; set; }
        public string Genre { get; set; }
        public bool IsCheckedOut { get; set; }
        public DateTime LastCheckedOutDate { get; set; }
        public DateTime DueBackDate { get; set; }

        public static List<String> GetAllBooks(SqlConnection connection)
        {
            var bookList = new List<String>();

            var cmd = new SqlCommand("SELECT Title, Author FROM Book", connection);
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                bookList.Add($"{reader["Title"]} by {reader["Author"]}");
            }
            connection.Close();

            return bookList;
        }

        public static void AddNewBook(Book newBook, string pathToDatabase)
        {
            using (var connection = new SqlConnection(pathToDatabase))
            {
                var cmd = new SqlCommand(@"INSERT INTO Book (Title, YearPublished) "
                            + "Values (@Title, @YearPublished)", connection);

                cmd.Parameters.AddWithValue("@Title", newBook.Title);
                cmd.Parameters.AddWithValue("@YearPublished", newBook.YearPublished);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }      
        }
    }   
}