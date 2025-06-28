using System.Collections.Generic;

namespace Exercises.Swagger
{
    
    public class JsonPatchDocumentSchema
    {
        /// <summary>
        /// An array of operations to apply to the specified resource. For more information visit http://jsonpatch.com/.
        /// </summary>
        public IEnumerable<JsonPatchDocumentOperationSchema> Operations { get; set; }
    }
}