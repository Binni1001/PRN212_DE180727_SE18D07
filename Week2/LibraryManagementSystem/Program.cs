using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LibraryManagementSystem
{
    // Abstract base class LibraryItem
    public abstract class LibraryItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }

        protected LibraryItem(int id, string title, int publicationYear)
        {
            Id = id;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            PublicationYear = publicationYear;
        }

        public abstract void DisplayInfo();

        public virtual decimal CalculateLateReturnFee(int daysLate)
        {
            return daysLate * 0.50m;
        }
    }

    // Interface IBorrowable
    public interface IBorrowable
    {
        DateTime? BorrowDate { get; set; }
        DateTime? ReturnDate { get; set; }
        bool IsAvailable { get; set; }
        void Borrow();
        void Return();
    }

    // Book class
    public class Book : LibraryItem, IBorrowable
    {
        public string Author { get; set; }
        public int Pages { get; set; }
        public string? Genre { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Book(int id, string title, int publicationYear, string author)
            : base(id, title, publicationYear)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Book - ID: {Id}, Title: {Title}, Author: {Author}, Year: {PublicationYear}, " +
                $"Pages: {Pages}, Genre: {Genre ?? "N/A"}, Available: {IsAvailable}");
        }

        public override decimal CalculateLateReturnFee(int daysLate)
        {
            return daysLate * 0.75m;
        }

        public void Borrow()
        {
            if (IsAvailable)
            {
                IsAvailable = false;
                BorrowDate = DateTime.Now;
                ReturnDate = null;
                Console.WriteLine($"Book '{Title}' has been borrowed on {BorrowDate}");
            }
            else
            {
                Console.WriteLine($"Book '{Title}' is not available for borrowing.");
            }
        }

        public void Return()
        {
            if (!IsAvailable)
            {
                IsAvailable = true;
                ReturnDate = DateTime.Now;
                Console.WriteLine($"Book '{Title}' has been returned on {ReturnDate}");
            }
            else
            {
                Console.WriteLine($"Book '{Title}' was not borrowed.");
            }
        }
    }

    // DVD class
    public class DVD : LibraryItem, IBorrowable
    {
        public string Director { get; set; }
        public int Runtime { get; set; }
        public string? AgeRating { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsAvailable { get; set; } = true;

        public DVD(int id, string title, int publicationYear, string director)
            : base(id, title, publicationYear)
        {
            Director = director ?? throw new ArgumentNullException(nameof(director));
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"DVD - ID: {Id}, Title: {Title}, Director: {Director}, Year: {PublicationYear}, " +
                $"Runtime: {Runtime} minutes, Age Rating: {AgeRating ?? "N/A"}, Available: {IsAvailable}");
        }

        public override decimal CalculateLateReturnFee(int daysLate)
        {
            return daysLate * 1.00m;
        }

        public void Borrow()
        {
            if (IsAvailable)
            {
                IsAvailable = false;
                BorrowDate = DateTime.Now;
                ReturnDate = null;
                Console.WriteLine($"DVD '{Title}' has been borrowed on {BorrowDate}");
            }
            else
            {
                Console.WriteLine($"DVD '{Title}' is not available for borrowing.");
            }
        }

        public void Return()
        {
            if (!IsAvailable)
            {
                IsAvailable = true;
                ReturnDate = DateTime.Now;
                Console.WriteLine($"DVD '{Title}' has been returned on {ReturnDate}");
            }
            else
            {
                Console.WriteLine($"DVD '{Title}' was not borrowed.");
            }
        }
    }

    // Magazine class
    public class Magazine : LibraryItem
    {
        public int IssueNumber { get; set; }
        public string? Publisher { get; set; }

        public Magazine(int id, string title, int publicationYear, int issueNumber)
            : base(id, title, publicationYear)
        {
            IssueNumber = issueNumber;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"Magazine - ID: {Id}, Title: {Title}, Year: {PublicationYear}, " +
                $"Issue: {IssueNumber}, Publisher: {Publisher ?? "N/A"}");
        }
    }

    // Library class
    public class Library
    {
        private List<LibraryItem> items = new List<LibraryItem>();

        public void AddItem(LibraryItem item)
        {
            items.Add(item);
            Console.WriteLine($"Item '{item.Title}' added to the library.");
        }

        public LibraryItem? SearchByTitle(string title)
        {
            return items.FirstOrDefault(item => item.Title.ContainsIgnoreCase(title));
        }

        public void DisplayAllItems()
        {
            Console.WriteLine("\n===== All Library Items =====");
            if (items.Count == 0)
            {
                Console.WriteLine("No items in the library.");
                return;
            }
            foreach (var item in items)
            {
                item.DisplayInfo();
            }
        }

        public bool UpdateItemTitle(int id, ref string newTitle)
        {
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                string oldTitle = item.Title;
                item.Title = newTitle;
                newTitle = oldTitle; // Update the ref parameter to old title
                return true;
            }
            return false;
        }

        public LibraryItem? GetItemReference(int id)
        {
            return items.FirstOrDefault(i => i.Id == id);
        }
    }

    // Advanced: Record type for borrowing history
    public record BorrowRecord(int ItemId, string Title, DateTime? BorrowDate, DateTime? ReturnDate, string BorrowerName)
    {
        public string? LibraryLocation { get; init; }
        public override string ToString()
        {
            return $"Borrow Record: Item ID: {ItemId}, Title: {Title}, Borrower: {BorrowerName}, " +
                   $"Borrowed: {BorrowDate?.ToString("yyyy-MM-dd") ?? "N/A"}, " +
                   $"Returned: {ReturnDate?.ToString("yyyy-MM-dd") ?? "N/A"}, Location: {LibraryLocation ?? "N/A"}";
        }
    }

    // Advanced: Extension method for string
    public static class StringExtensions
    {
        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(toCheck))
                return false;
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    // Advanced: Generic collection
    public class LibraryItemCollection<T> where T : LibraryItem
    {
        private List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public T? GetItem(int id)
        {
            return items.FirstOrDefault(item => item.Id == id);
        }

        public int Count => items.Count;
    }

    class Program
    {
        static void Main()
        {
            // Create library
            var library = new Library();

            // Add items
            var book1 = new Book(1, "The Great Gatsby", 1925, "F. Scott Fitzgerald")
            {
                Genre = "Classic Fiction",
                Pages = 180
            };

            var book2 = new Book(2, "Clean Code", 2008, "Robert C. Martin")
            {
                Genre = "Programming",
                Pages = 464
            };

            var dvd1 = new DVD(3, "Inception", 2010, "Christopher Nolan")
            {
                Runtime = 148,
                AgeRating = "PG-13"
            };

            var magazine1 = new Magazine(4, "National Geographic", 2023, 56)
            {
                Publisher = "National Geographic Partners"
            };

            library.AddItem(book1);
            library.AddItem(book2);
            library.AddItem(dvd1);
            library.AddItem(magazine1);

            // Display all items
            library.DisplayAllItems();

            // Borrow and return demonstration
            Console.WriteLine("\n===== Borrowing Demonstration =====");
            book1.Borrow();
            dvd1.Borrow();

            // Try to borrow again
            book1.Borrow();

            // Display changed status
            Console.WriteLine("\n===== Updated Status =====");
            book1.DisplayInfo();
            dvd1.DisplayInfo();

            // Return item
            Console.WriteLine("\n===== Return Demonstration =====");
            book1.Return();

            // Search for an item
            Console.WriteLine("\n===== Search Demonstration =====");
            var foundItem = library.SearchByTitle("Clean");
            if (foundItem != null)
            {
                Console.WriteLine("Found item:");
                foundItem.DisplayInfo();
            }
            else
            {
                Console.WriteLine("Item not found");
            }

            // Advanced features demonstration
            if (ShouldRunAdvancedFeatures())
            {
                // Boxing/Unboxing performance comparison
                Console.WriteLine("\n===== ADVANCED: Boxing/Unboxing Performance =====");
                
                var standardList = new ArrayList();
                var genericList = new List<int>();
                var customCollection = new LibraryItemCollection<Book>();
                
                const int iterations = 1_000_000;
                
                // Measure ArrayList performance (with boxing)
                var stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < iterations; i++)
                {
                    standardList.Add(i);
                }
                
                int sum1 = 0;
                foreach (int i in standardList)
                {
                    sum1 += i;
                }
                stopwatch.Stop();
                Console.WriteLine($"ArrayList time (with boxing): {stopwatch.ElapsedMilliseconds}ms");
                
                // Measure generic List<T> performance (no boxing)
                stopwatch.Restart();
                for (int i = 0; i < iterations; i++)
                {
                    genericList.Add(i);
                }
                
                int sum2 = 0;
                foreach (int i in genericList)
                {
                    sum2 += i;
                }
                stopwatch.Stop();
                Console.WriteLine($"Generic List<T> time (no boxing): {stopwatch.ElapsedMilliseconds}ms");
                
                // Add books to custom collection
                var book3 = new Book(5, "The Hobbit", 1937, "J.R.R. Tolkien") { Pages = 310, Genre = "Fantasy" };
                var book4 = new Book(6, "1984", 1949, "George Orwell") { Pages = 328, Genre = "Dystopian" };
                
                customCollection.Add(book1);
                customCollection.Add(book3);
                customCollection.Add(book4);
                
                Console.WriteLine($"Books in custom collection: {customCollection.Count}");
                
                // Pattern matching demonstration
                Console.WriteLine("\n===== ADVANCED: Pattern Matching =====");
                
                var item = library.SearchByTitle("Clean");
                
                string description = item switch
                {
                    Book b when b.Pages > 400 => $"Long book: {b.Title} with {b.Pages} pages",
                    Book b => $"Book: {b.Title} by {b.Author}",
                    DVD d => $"DVD: {d.Title} directed by {d.Director}",
                    Magazine m => $"Magazine: {m.Title}, Issue #{m.IssueNumber}",
                    null => "No item found",
                    _ => $"Unknown type: {item.Title}"
                };
                
                Console.WriteLine(description);
                
                // Ref returns demonstration
                Console.WriteLine("\n===== ADVANCED: Ref Returns =====");
                
                try
                {
                    var itemRef = library.GetItemReference(1);
                    if (itemRef != null)
                    {
                        Console.WriteLine($"Before modification: {itemRef.Title}");
                        itemRef.Title += " (Modified)";
                        Console.WriteLine($"After modification: {itemRef.Title}");
                    }
                    else
                    {
                        Console.WriteLine("Item not found.");
                    }
                    
                    string title = "New Title";
                    if (library.UpdateItemTitle(2, ref title))
                    {
                        Console.WriteLine($"Updated title from '{title}' to 'New Title'");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
                // Nullable types demonstration
                Console.WriteLine("\n===== ADVANCED: Nullable Types =====");
                
                Book? nullableBook = null;
                string bookTitle = nullableBook?.Title ?? "No title available";
                Console.WriteLine($"Nullable book title: {bookTitle}");
                
                var borrowedBook = library.SearchByTitle("gatsby") as Book;
                DateTime? dueDate = borrowedBook?.BorrowDate?.AddDays(14);
                Console.WriteLine($"Due date: {dueDate?.ToString("yyyy-MM-dd") ?? "Not borrowed"}");
                
                // Record type demonstration
                Console.WriteLine("\n===== ADVANCED: Record Type =====");
                
                var borrowRecord = new BorrowRecord(
                    1,
                    "The Great Gatsby",
                    DateTime.Now.AddDays(-7),
                    null,
                    "John Smith")
                {
                    LibraryLocation = "Main Branch"
                };
                
                Console.WriteLine(borrowRecord);
                
                var returnedRecord = borrowRecord with { ReturnDate = DateTime.Now };
                Console.WriteLine($"Original record: {borrowRecord}");
                Console.WriteLine($"Modified record: {returnedRecord}");
                
                // Test extension method
                string searchTerm = "code";
                bool contains = "Clean Code".ContainsIgnoreCase(searchTerm);
                Console.WriteLine($"Does 'Clean Code' contain '{searchTerm}'? {contains}");
            }
        }

        static bool ShouldRunAdvancedFeatures()
        {
            Console.WriteLine("\nWould you like to run the advanced features? (y/n)");
            return Console.ReadLine()?.ToLower() == "y";
        }
    }
}