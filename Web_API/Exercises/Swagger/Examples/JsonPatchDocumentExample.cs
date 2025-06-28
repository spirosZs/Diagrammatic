using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Exercises.Swagger.Examples
{
    public class JsonPatchDocumentExample: IExampleProvider<JsonPatchDocumentSchema>
    {
        public object GetExample()
        {
            return new Operation[]
            {
                new Operation()
                {
                    op = "replace",
                    path = "/property",
                    value = "New Value"
                },
                new Operation()
                {
                    op = "add",
                    path = "/property",
                    value = "New Value"
                },
                new Operation()
                {
                    op = "remove",
                    path = "/property"
                },
                new Operation()
                {
                    op = "copy",
                    from = "/fromProperty",
                    path = "/toProperty"
                },
                new Operation()
                {
                    op = "move",
                    from = "/fromProperty",
                    path = "/toProperty"
                },
                new Operation()
                {
                    op = "test",
                    path = "/property",
                    value = "Has Value"
                },
                new Operation()
                {
                    op = "add",
                    path = "/arrayProperty/-",
                    value = "Add Item to Array"
                },
                new Operation()
                {
                    op = "replace",
                    path = "/arrayProperty/0",
                    value = "Replace First Array Item"
                },
                new Operation()
                {
                    op = "replace",
                    path = "/arrayProperty/-",
                    value = "Replace Last Array Item"
                }
            };
        }
    }
}