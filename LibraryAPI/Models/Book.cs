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

        public static List<String> GetAllBooks(string pathToDatabase)
        {
            var bookList = new List<String>();

            using(var connection = new SqlConnection(pathToDatabase))
            {
                var cmd = new SqlCommand("SELECT * FROM Book", connection);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var bookString = $"Book ID {reader["ID"]}: {reader["Title"]} by {reader["Author"]}." +
                        $" A {reader["Genre"]} story published in {reader["YearPublished"]}.";
                    if (reader["IsCheckedOut"].ToString() == "True")
                    {
                        bookString += $" This book is currently checked out. " +
                            $"It was checked out on {reader["LastCheckedOutDate"]}, and its due date is {reader["DueBackDate"]}";
                    }
                    else if (reader["IsCheckedOut"].ToString() == "False" || reader["IsCheckedOut"].ToString() == null)
                    {
                        bookString += "This book is available for checkout.";
                        if (reader["LastCheckedOutDate"].ToString() != String.Empty)
                        {
                            bookString += $" It was last checked out on {reader["LastCheckedOutDate"]}.";
                        }
                    }

                    bookList.Add(bookString);
                }
                connection.Close();
            }
            
            return bookList;
        }

        public static List<String> GetBooks(SqlConnection connection, string IsCheckedOut)
        {
            var bookList = new List<String>();
            var bookString = String.Empty;

            var cmd = new SqlCommand($"SELECT * FROM Book WHERE IsCheckedOut=@IsCheckedOut", connection);
            cmd.Parameters.AddWithValue("@IsCheckedOut", IsCheckedOut);

            connection.Open();
            var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                if (IsCheckedOut == "True")
                {
                    bookString = $"{reader["Title"]} by {reader["Author"]} was " +
                    $"checked out on {reader["LastCheckedOutDate"]} and is due back {reader["DueBackDate"]}.";
                    bookList.Add(bookString);
                }
                else if (IsCheckedOut == "False")
                {
                    bookString = $"{reader["Title"]} by {reader["Author"]} is available for checkout.";
                    bookList.Add(bookString);
                }
                
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

        public static void DeleteBook(string pathToDatabase, string ID)
        {
            using(var connection = new SqlConnection(pathToDatabase))
            {
                var cmd = new SqlCommand("DELETE FROM Book WHERE ID=@ID", connection);
                cmd.Parameters.AddWithValue("@ID", ID);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public static void UpdateBook(string pathToDatabase, string ID, string attribute, string newValue)
        {
            using(var connection = new SqlConnection(pathToDatabase))
            {
                var cmd = new SqlCommand($"UPDATE Book SET [{attribute}] = @newValue " +
                    $"WHERE ID = @ID", connection);
                cmd.Parameters.AddWithValue("@newValue", newValue);
                cmd.Parameters.AddWithValue("@ID", ID);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public static string TryCheckOutBook(string pathToDatabase, string ID)
        {
            var rv = String.Empty;
            var bookName = String.Empty;
            var dueBack = String.Empty;
            var checkedOutStatus = String.Empty;

            using (var connection = new SqlConnection(pathToDatabase))
            {
                var cmd = new SqlCommand($"SELECT * FROM Book WHERE ID=@ID", connection);
                cmd.Parameters.AddWithValue("@ID", ID);

                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    checkedOutStatus = reader["IsCheckedOut"].ToString();
                    bookName = reader["Title"].ToString();
                    dueBack = reader["DueBackDate"].ToString();
                }
                connection.Close();

                if (checkedOutStatus == "True")
                {
                    rv = $"Sorry, {bookName} is already checked out. It is due back {dueBack}.";
                }
                else if (checkedOutStatus == "False")
                {
                    cmd = new SqlCommand($"UPDATE Book " +
                        $"SET [IsCheckedOut] = @IsCheckedout, [LastCheckedOutDate] = @LastCheckedOutDate, [DueBackDate] = @DueDate " +
                        $"WHERE ID = @ID", connection);

                    cmd.Parameters.AddWithValue("@LastCheckedOutDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@DueDate", DateTime.Now.AddDays(10));
                    cmd.Parameters.AddWithValue("@ID", ID);
                    cmd.Parameters.AddWithValue("@IsCheckedOut", "True");

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                    rv = $"You checked out {bookName}. It is due back {DateTime.Now.AddDays(10)}";
                }
            }

            return rv;
        }
    }   
}