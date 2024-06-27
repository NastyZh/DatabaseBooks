namespace Database_Books;

public class Book
{
    public int Id { get; set; }
    public string BookName { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
    public string Genre { get; set; }
    public bool IsFree { get; set; }
}