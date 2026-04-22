using System.Collections.Generic;

namespace project.Models
{
    public class ProductHomeViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<string> SelectedCategories { get; set; } = new List<string>();
        public List<string> SelectedSizes { get; set; } = new List<string>();
        public List<string> SelectedColors { get; set; } = new List<string>();
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
