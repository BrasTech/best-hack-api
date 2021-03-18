using DrovoAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Classes
{
    public static class Categories
    {
        private static List<Category> categories { get; set; }
        public static void Load()
        {
            string categoriesData = File.ReadAllText(@"Files/categories.json");
            categories = JsonConvert.DeserializeObject<List<Category>>(categoriesData);
        }
        public static Category Get(int categoryId)
        {
            return categories.FirstOrDefault(u => u.Id == categoryId);
        }
        public static Category Get(string name)
        {
            return categories.FirstOrDefault(u => u.Name == name);
        }
        public static int GetByName(string name)
        {
            var category = categories.FirstOrDefault(u => u.Name == name);
            if (category != null)
                return category.Id;
            return -1;
        }
        public static string GetById(int id)
        {
            var category = categories.FirstOrDefault(u => u.Id == id);
            if (category != null)
                return category.Name;
            return "Не распознана";
        }

    }
}
