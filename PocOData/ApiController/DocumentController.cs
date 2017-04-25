using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using PocOData.Data.EF;
using PocOData.Data.Models;
using Document = PocOData.Models.Document;
using PocContext = PocOData.Models.PocContext;

namespace PocOData.ApiController
{
    public class DocumentController : ODataController
    {
        private PocContext _db = new PocContext();

        public IHttpActionResult Get()
        {
            return Ok(_db.Documents);
        }

        [HttpPost]
        public IHttpActionResult CheckOut(int key)
        {
            var document = _db.Documents.FirstOrDefault(m => m.Id == key);
            if (document == null)
            {
                return BadRequest(ModelState);
            }

            if (!TryCheckoutDocument(document))
            {
                return BadRequest("The document is already checked out.");
            }

            return Ok(document);
        }

        [HttpPost]
        public IHttpActionResult Return(int key)
        {
            var document = _db.Documents.FirstOrDefault(m => m.Id == key);
            if (document == null)
            {
                return BadRequest(ModelState);
            }

            document.IsCheckedOut = false;

            return Ok(document);
        }
        
        [HttpPost]
        public IHttpActionResult CheckOutMany(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var documentIDs = new HashSet<int>(parameters["DocumentIDs"] as IEnumerable<int>);
            
            var results = new List<Document>();
            foreach (var document in _db.Documents.Where(m => documentIDs.Contains(m.Id)))
            {
                if (TryCheckoutDocument(document))
                {
                    results.Add(document);
                }
            }
            
            return Ok(results);
        }

        [HttpPost]
        [ODataRoute("CreateDocument")]
        public IHttpActionResult CreateDocument(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string title = parameters["Title"] as string;

            var document = new Document
            {
                Title = title,
                Id = _db.Documents.Count + 1,
            };

            _db.Documents.Add(document);

            return Created(document);
        }

        protected Document GetDocumentByKey(int key)
        {
            return _db.Documents.FirstOrDefault(m => m.Id == key);
        }

        private bool TryCheckoutDocument(Document document)
        {
            if (document.IsCheckedOut)
            {
                return false;
            }
            
            document.IsCheckedOut = true;
            return true;
        }
    }
}