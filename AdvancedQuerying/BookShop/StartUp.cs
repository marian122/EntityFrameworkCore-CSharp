namespace BookShop
{
    using Data;
    using Initializer;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);
                var input = Console.ReadLine();
                var result = GetBooksByCategory(db, input);
                Console.WriteLine(result);
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var sb = new StringBuilder();

            var books = context
                    .Books
                    .Where(b => b.AgeRestriction.ToString().ToLower() == command.ToLower())
                    .Select(b => new
                    {
                        b.Title
                    })
                    .OrderBy(x => x.Title)
                    .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var sb = new StringBuilder();

            var books = context
                    .Books
                    .Where(b => b.EditionType.ToString() == "Gold" && b.Copies < 5000 )
                    .Select(b => new
                    {
                        b.Title,
                        b.BookId
                    })
                    .OrderBy(b => b.BookId)
                    .ToList();


            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }


            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var splittedInput = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

            var books = context
                .Books
                .Where(b => b.BookCategories.Any(c => splittedInput.Contains(c.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title);

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
    }
}
