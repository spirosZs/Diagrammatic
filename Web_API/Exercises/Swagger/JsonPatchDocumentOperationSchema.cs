using System.ComponentModel.DataAnnotations;

namespace Exercises.Swagger
{
    public class JsonPatchDocumentOperationSchema
    {
        [Required] 
        public string Path { get; set; }

        [Required] 
        public JsonPatchDocumentOperation Op { get; set; }

        public string Value { get; set; }
    }
}