using System.ComponentModel;

namespace AhorraYa.WebClient.ViewModels.Categories
{
    public class CategoryEditVm
    {
        public int Id { get; set; }
        [DisplayName("Category Name")]
        public string CategoryName { get; set; } = null!;
    }
}
