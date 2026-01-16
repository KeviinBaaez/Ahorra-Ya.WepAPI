using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Categories
{
    public class CategoryListVm
    {
        public int Id { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; } = null!;
    }
}
