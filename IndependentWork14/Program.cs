using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Shynkaruk
{
    public class FileMetadata
    {
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; }
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new Dictionary<string, FileMetadata>();

        public void Add(string path, FileMetadata metadata)
        {
            _cache[path] = metadata;
        }

        public FileMetadata Get(string path)
        {
            return _cache.TryGetValue(path, out var metadata) ? metadata : null;
        }

        public void Remove(string path)
        {
            _cache.Remove(path);
        }

        public bool Contains(string path)
        {
            return _cache.ContainsKey(path);
        }
    }

    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new Dictionary<string, HashSet<string>>();

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.ContainsKey(articleId))
            {
                _articleTags[articleId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            _articleTags[articleId].Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.ContainsKey(articleId))
            {
                _articleTags[articleId].Remove(tag);
            }
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tags in _articleTags.Values)
            {
                allTags.UnionWith(tags);
            }
            return allTags;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (!_articleTags.ContainsKey(articleId1) || !_articleTags.ContainsKey(articleId2))
            {
                return new HashSet<string>();
            }

            var common = new HashSet<string>(_articleTags[articleId1], StringComparer.OrdinalIgnoreCase);
            common.IntersectWith(_articleTags[articleId2]);
            return common;
        }
    }

    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            var words = content.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (!_invertedIndex.ContainsKey(word))
                {
                    _invertedIndex[word] = new HashSet<int>();
                }
                _invertedIndex[word].Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            return _invertedIndex.TryGetValue(word, out var docs) ? docs : new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words == null || words.Length == 0) return new HashSet<int>();

            HashSet<int> result = null;
            foreach (var word in words)
            {
                var docs = Search(word);
                if (result == null)
                {
                    result = new HashSet<int>(docs);
                }
                else
                {
                    result.IntersectWith(docs);
                }
            }
            return result ?? new HashSet<int>();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №14: Dictionary та HashSet ===\n");

            var fileCache = new FileMetadataCache();
            fileCache.Add("docs/report.pdf", new FileMetadata { FileName = "report.pdf", Size = 2048, LastModified = DateTime.Now, Hash = "abc123hash" });
            var meta = fileCache.Get("docs/report.pdf");
            Console.WriteLine($"Знайдено файл: {meta?.FileName}, Розмір: {meta?.Size} байт\n");

            var tagManager = new TagManager();
            tagManager.AddTag("art1", "C#");
            tagManager.AddTag("art1", "dotnet");
            tagManager.AddTag("art2", "C#");
            tagManager.AddTag("art2", "Performance");

            Console.WriteLine($"Усі унікальні теги: {string.Join(", ", tagManager.GetAllUniqueTags())}");
            Console.WriteLine($"Спільні теги art1 та art2: {string.Join(", ", tagManager.FindCommonTags("art1", "art2"))}\n");

            var index = new SearchIndex();
            index.AddDocument(1, "C# колекції та словники у dotnet");
            index.AddDocument(2, "Висока продуктивність C# коду та словників");
            index.AddDocument(3, "Основи програмування на C#");

            var results = index.Search("C#");
            Console.WriteLine($"Документи зі словом 'C#': {string.Join(", ", results)}");
            var multiResults = index.SearchMultiple("C#", "словників");
            Console.WriteLine($"Документи зі словами 'C#' та 'словників': {string.Join(", ", multiResults)}");
        }
    }
}
