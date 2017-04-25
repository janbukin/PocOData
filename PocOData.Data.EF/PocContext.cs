using System.Collections.Generic;
using PocOData.Data.Models;

namespace PocOData.Data.EF
{
    public class PocContext
    {
        static List<Document> _documents;
        static PocContext()
        {
            _documents = new List<Document>
            {
                new Document { Id = 1, Title = "Maximum Payback" }
            };
        }

        public List<Document> Documents => _documents;
    }
}
