using StoryTranslatorReactDotnet.Models;

namespace StoryTranslatorReactDotnet.Database;

public static class InitializeData {
    public static void CreateInitialBookData(ApplicationDbContext db)
    {
        if(db.Books.Any() || db.Paragraphs.Any()) return;

        Book book1 = new Book("Book 1");

        db.Books.Add(book1);

        Book book2 = new Book("Book 2");

        db.Books.Add(book2);

        Paragraph paragraph1 = new Paragraph(
                                    "Book 1 paragraph 1 English", 
                                    "Book 1 paragraph 1 Spanish",
                                    "Book 1 paragraph 1 French",
                                    "Book 1 paragraph 1 German",
                                    book1);
        db.Paragraphs.Add(paragraph1);

        Paragraph paragraph2 = new Paragraph(
                                    "Book 1 paragraph 2 English", 
                                    "Book 1 paragraph 2 Spanish",
                                    "Book 1 paragraph 2 French",
                                    "Book 1 paragraph 2 German",
                                    book1);
        db.Paragraphs.Add(paragraph2);

        Paragraph paragraph3 = new Paragraph(
                                    "Book 1 paragraph 3 English", 
                                    "Book 1 paragraph 3 Spanish",
                                    "Book 1 paragraph 3 French",
                                    "Book 1 paragraph 3 German",
                                    book1);
        db.Paragraphs.Add(paragraph3);

        Paragraph paragraph4 = new Paragraph(
                                    "Book 2 paragraph 1 English", 
                                    "Book 2 paragraph 1 Spanish",
                                    "Book 2 paragraph 1 French",
                                    "Book 2 paragraph 1 German",
                                    book2);
        db.Paragraphs.Add(paragraph4);

        Paragraph paragraph5 = new Paragraph(
                                    "Book 2 paragraph 2 English", 
                                    "Book 2 paragraph 2 Spanish",
                                    "Book 2 paragraph 2 French",
                                    "Book 2 paragraph 2 German",
                                    book2);
        db.Paragraphs.Add(paragraph5);

        Paragraph paragraph6 = new Paragraph(
                                    "Book 3 paragraph 3 English", 
                                    "Book 3 paragraph 3 Spanish",
                                    "Book 3 paragraph 3 French",
                                    "Book 3 paragraph 3 German",
                                    book2);
        db.Paragraphs.Add(paragraph6);

        db.SaveChanges();
    }
}