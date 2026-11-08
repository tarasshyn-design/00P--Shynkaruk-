using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        /// <summary>
        /// Розбиває текст на слова, видаляє пунктуацію, переводить у нижній регістр
        /// </summary>
        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Enumerable.Empty<string>();

            return Regex.Matches(text.ToLowerInvariant(), @"\w+")
                        .Cast<Match>()
                        .Select(m => m.Value);
        }

        /// <summary>
        /// Видаляє стоп-слова (a, an, the, is, are, etc.)
        /// </summary>
        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            var stopWords = new HashSet<string>
            {
                "a", "an", "the",
                "is", "are",
                "was", "were",
                "in", "on", "at",
                "to", "for",
                "of", "and", "or"
            };
            return words.Where(w => !stopWords.Contains(w));
        }
    }

    public class InvertedIndex
    {
        // Слово > Множина ID документів
        private Dictionary<string, HashSet<int>> _index;
        // ID -> Документ (для швидкого отримання)
        private Dictionary<int, Document> _documents;

        public InvertedIndex()
        {
            _index = new Dictionary<string, HashSet<int>>();
            _documents = new Dictionary<int, Document>();
        }

        /// <summary>
        /// Додає документ до індексу
        /// </summary>
        public void AddDocument(Document doc)
        {
            if (doc == null) return;

            // 1. Зберегти документ у _documents
            _documents[doc.Id] = doc;

            // 2. Розбити Content на слова (toLowerCase, видалити пунктуацію через Tokenize)
            var tokens = TextProcessor.Tokenize(doc.Content);

            // 3. Для кожного слова додати doc.Id до _index[word]
            foreach (var word in tokens)
            {
                if (!_index.ContainsKey(word))
                {
                    _index[word] = new HashSet<int>();
                }
                _index[word].Add(doc.Id);
            }
        }

        /// <summary>
        /// Шукає документи, що містять слово
        /// </summary>
        public List<Document> SearchSingleWord(string word)
        {
            var normalizedWord = word.ToLowerInvariant();
            if (_index.TryGetValue(normalizedWord, out var docIds))
            {
                return docIds.Select(id => _documents[id]).ToList();
            }
            return new List<Document>();
        }

        /// <summary>
        /// Шукає документи, що містять ВСІ слова (AND)
        /// </summary>
        public List<Document> SearchMultipleWords(params string[] words)
        {
            if (words == null || words.Length == 0) return new List<Document>();

            HashSet<int> resultIds = null;

            foreach (var rawWord in words)
            {
                var word = rawWord.ToLowerInvariant();
                if (!_index.TryGetValue(word, out var docIds))
                {
                    return new List<Document>(); // Якщо хоча б одного слова немає, спільних документів немає
                }

                if (resultIds == null)
                {
                    resultIds = new HashSet<int>(docIds);
                }
                else
                {
                    resultIds.IntersectWith(docIds); // Перетин множин (AND)
                }

                if (resultIds.Count == 0) break;
            }

            return resultIds?.Select(id => _documents[id]).ToList() ?? new List<Document>();
        }

        /// <summary>
        /// Шукає документи, що містять БУДЬ-ЯКЕ слово (OR)
        /// </summary>
        public List<Document> SearchAnyWord(params string[] words)
        {
            if (words == null || words.Length == 0) return new List<Document>();

            var resultIds = new HashSet<int>();

            foreach (var rawWord in words)
            {
                var word = rawWord.ToLowerInvariant();
                if (_index.TryGetValue(word, out var docIds))
                {
                    resultIds.UnionWith(docIds); // Об'єднання множин (OR)
                }
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;
    }

    public class PerformanceTest
    {
        public static void CompareSearchMethods(List<Document> documents, string searchWord)
        {
            var sw = new Stopwatch();

            // 1. Лінійний пошук
            sw.Start();
            var normalizedWord = searchWord.ToLowerInvariant();
            var linearResults = documents.Where(d =>
                TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).ToList();
            sw.Stop();
            long linearTicks = sw.ElapsedTicks;

            // 2. Inverted Index
            var index = new InvertedIndex();
            foreach (var doc in documents)
            {
                index.AddDocument(doc);
            }

            sw.Restart();
            var indexResults = index.SearchSingleWord(searchWord);
            sw.Stop();
            long indexTicks = sw.ElapsedTicks;

            Console.WriteLine($"--- Тест продуктивності (Кількість документів: {documents.Count}, Слово: '{searchWord}') ---");
            Console.WriteLine($"Лінійний пошук:  {linearTicks} ticks");
            Console.WriteLine($"Inverted Index: {indexTicks} ticks");
            Console.WriteLine($"Лінійний знайшов: {linearResults.Count}, Індекс знайшов: {indexResults.Count}\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Базові тестові дані
            var documents = new List<Document>
            {
                new Document { Id = 1, Title = "Introduction to C#", Content = "C# is a modern object-oriented programming language developed by Microsoft." },
                new Document { Id = 2, Title = "Java Basics", Content = "Java is a popular programming language known for its portability and performance." },
                new Document { Id = 3, Title = "Python Overview", Content = "Python is a high-level programming language with simple syntax." },
                new Document { Id = 4, Title = "C# Advanced Features", Content = "C# supports LINQ, async/await, and many modern programming paradigms." },
                new Document { Id = 5, Title = "Comparing Languages", Content = "C#, Java, and Python are among the most popular programming languages today." }
            };

            // Вимірювання продуктивності для 10 документів
            var docs10 = new List<Document>(documents);
            for (int i = 0; i < 5; i++)
            {
                docs10.Add(new Document { Id = documents[i].Id + 5, Title = documents[i].Title + " (Copy)", Content = documents[i].Content });
            }
            PerformanceTest.CompareSearchMethods(docs10, "programming");

            // Вимірювання продуктивності для 1000 документів
            var docs1000 = new List<Document>();
            for (int i = 1; i <= 1000; i++)
            {
                var template = documents[(i - 1) % documents.Count];
                docs1000.Add(new Document
                {
                    Id = i,
                    Title = $"{template.Title} #{i}",
                    Content = $"{template.Content} Additional random text identifier {i}"
                });
            }
            PerformanceTest.CompareSearchMethods(docs1000, "programming");

            // Демонстрація роботи індексу та множинного пошуку (AND / OR)
            var index = new InvertedIndex();
            foreach (var doc in documents)
            {
                index.AddDocument(doc);
            }

            Console.WriteLine($"Завантажено документів: {index.DocumentCount}");
            Console.WriteLine($"Унікальних слів в індексі: {index.UniqueWords}\n");

            // AND пошук
            var andWords = new[] { "programming", "language" };
            Console.WriteLine($"Пошук (AND) за словами: {string.Join(", ", andWords)}");
            var andResults = index.SearchMultipleWords(andWords);
            foreach (var doc in andResults)
            {
                Console.WriteLine($"  {doc}");
            }
            Console.WriteLine($"Знайдено документів: {andResults.Count}\n");

            // OR пошук
            var orWords = new[] { "async", "syntax" };
            Console.WriteLine($"Пошук (OR) за словами: {string.Join(", ", orWords)}");
            var orResults = index.SearchAnyWord(orWords);
            foreach (var doc in orResults)
            {
                Console.WriteLine($"  {doc}");
            }
            Console.WriteLine($"Знайдено документів: {orResults.Count}\n");
        }
    }
}
