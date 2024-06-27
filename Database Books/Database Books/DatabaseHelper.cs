using System.Data.SQLite;
using Dapper;

namespace Database_Books;

public class DatabaseHelper
{
    
        private SQLiteConnection _connection;

        public DatabaseHelper()
        {
            _connection = new SQLiteConnection("Data Source=identifier.sqlite");
            _connection.Open();

            CreateTables();
        }

        private void CreateTables()
        {
            _connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Books (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            BookName TEXT NOT NULL,
            Author TEXT NOT NULL,
            Year INTEGER NOT NULL,
            Genre TEXT NOT NULL,
            IsFree INTEGER NOT NULL DEFAULT 1
        );
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserName TEXT NOT NULL,
            ContactInfo TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS Loans (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            BookId INTEGER NOT NULL,
            UserId INTEGER NOT NULL,
            LoanDate DATE NOT NULL,
            ReturnDate DATE,
            FOREIGN KEY (BookId) REFERENCES Books(Id),
            FOREIGN KEY (UserId) REFERENCES Users(Id)
        );
    ");
        }

        public void AddBook(string bookName, string author, int year, string genre, bool isFree)
        {
            var query = "INSERT INTO Books (BookName, Author, Year, Genre, IsFree) VALUES (@BookName, @Author, @Year, @Genre, @IsFree)";
            _connection.Execute(query, new { BookName = bookName, Author = author, Year = year, Genre = genre, IsFree = isFree ? 1 : 0 });
        }

        public void UpdateBook(int id, string bookName, string author, int year, string genre, bool isFree)
        {
            var query = "UPDATE Books SET BookName = @BookName, Author = @Author, Year = @Year, Genre = @Genre, IsFree = @IsFree WHERE Id = @Id";
            _connection.Execute(query, new { Id = id, BookName = bookName, Author = author, Year = year, Genre = genre, IsFree = isFree ? 1 : 0 });
        }

        public void DeleteBook(int id)
        {
            var query = "DELETE FROM Books WHERE Id = @Id";
            _connection.Execute(query, new { Id = id });
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _connection.Query<Book>("SELECT * FROM Books");
        }

        public void AddUser(string userName, string contactInfo)
        {
            var query = "INSERT INTO Users (UserName, ContactInfo) VALUES (@UserName, @ContactInfo)";
            _connection.Execute(query, new { UserName = userName, ContactInfo = contactInfo });
        }

        public void UpdateUser(int id, string userName, string contactInfo)
        {
            var query = "UPDATE Users SET UserName = @UserName, ContactInfo = @ContactInfo WHERE Id = @Id";
            _connection.Execute(query, new { Id = id, UserName = userName, ContactInfo = contactInfo });
        }

        public void DeleteUser(int id)
        {
            var query = "DELETE FROM Users WHERE Id = @Id";
            _connection.Execute(query, new { Id = id });
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _connection.Query<User>("SELECT * FROM Users");
        }

        public void LoanBook(int bookId, int userId, DateTime loanDate)
        {
            var book = _connection.QuerySingleOrDefault<Book>("SELECT * FROM Books WHERE Id = @Id", new { Id = bookId });
            if (book == null)
            {
                throw new Exception("Book not found.");
            }

            if (book.IsFree)
            {
                _connection.Execute("INSERT INTO Loans (BookId, UserId, LoanDate) VALUES (@BookId, @UserId, @LoanDate)",
                    new { BookId = bookId, UserId = userId, LoanDate = loanDate });
                _connection.Execute("UPDATE Books SET IsFree = 0 WHERE Id = @Id", new { Id = bookId });
            }
            else
            {
                throw new Exception("Book is not available.");
            }
        }

        public void ReturnBook(int loanId, DateTime returnDate)
        {
            var loan = _connection.QuerySingleOrDefault<Loan>("SELECT * FROM Loans WHERE Id = @Id", new { Id = loanId });
            if (loan != null)
            {
                _connection.Execute("UPDATE Loans SET ReturnDate = @ReturnDate WHERE Id = @Id", new { ReturnDate = returnDate, Id = loanId });
                _connection.Execute("UPDATE Books SET IsFree = 1 WHERE Id = @Id", new { Id = loan.BookId });
            }
            else
            {
                throw new Exception("Loan not found.");
            }
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            return _connection.Query<Loan>("SELECT * FROM Loans");
        }
    }