namespace PocOData.Models
{
    public class Document
    {
        public int Id { get; set; }

        public string Title { get; set; }
        
        public bool IsCheckedOut { get; set; }

        public string Status { get; set; }
    }
}