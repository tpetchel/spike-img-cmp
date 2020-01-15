using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace spike_img_cmp
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];
            var imageFiles = new List<string>();

            imageFiles.AddRange(Directory.EnumerateFiles(path, "*.png", SearchOption.AllDirectories));
            int pngFileCount = imageFiles.Count;
            imageFiles.AddRange(Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories));
            int svgFileCount = imageFiles.Count - pngFileCount;

            var catalog = new Dictionary<string, List<string>>();

            foreach (var file in imageFiles)
            {
                string hash = string.Empty;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(file))
                    {
                        hash = Encoding.Default.GetString(md5.ComputeHash(stream));
                    }
                }

                if (!catalog.ContainsKey(hash))
                {
                    catalog.Add(hash, new List<string>());
                }
                catalog[hash].Add(file);
            }

            foreach (var kvp in catalog)
            {
                if (kvp.Value.Count > 1)
                {
                    Console.WriteLine();
                    Console.WriteLine($"These appear the same:");
                    foreach (var file in kvp.Value)
                    {
                        Console.WriteLine($"- {file.Substring(path.Length)}");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Examined {pngFileCount} .png files");
            Console.WriteLine($"Examined {svgFileCount} .svg files");
        }
    }
}
