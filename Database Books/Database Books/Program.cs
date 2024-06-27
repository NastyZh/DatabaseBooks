using Database_Books;

class Program
{
    static void Main(string[] args)
    {
        var dbHelper = new DatabaseHelper();

        // Примеры использования

        // Добавление книги
        dbHelper.AddBook("1984", "George Orwell", 1949, "Dystopian", true);

        // Добавление пользователя
        dbHelper.AddUser("John Doe", "john.doe@example.com");

        // Получение первой книги и пользователя из базы данных
        var book = dbHelper.GetAllBooks().FirstOrDefault();
        var user = dbHelper.GetAllUsers().FirstOrDefault();

        if (book != null && user != null)
        {
            try
            {
                dbHelper.LoanBook(book.Id, user.Id, DateTime.Now);
                Console.WriteLine("Book loaned successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loaning book: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Book or User not found.");
        }

        // Получение списка всех книг
        var books = dbHelper.GetAllBooks();
        foreach (var b in books)
        {
            Console.WriteLine($"Title: {b.BookName}, Author: {b.Author}, Year: {b.Year}, Genre: {b.Genre}, Available: {b.IsFree}");
        }

        // Получение списка всех пользователей
        var users = dbHelper.GetAllUsers();
        foreach (var u in users)
        {
            Console.WriteLine($"Name: {u.UserName}, Contact: {u.ContactInfo}");
        }

        // Получение списка всех записей о выдаче книг
        var loans = dbHelper.GetAllLoans();
        foreach (var l in loans)
        {
            Console.WriteLine($"Loan ID: {l.Id}, Book ID: {l.BookId}, User ID: {l.UserId}, Loan Date: {l.LoanDate}, Return Date: {l.ReturnDate}");
        }
    }
}