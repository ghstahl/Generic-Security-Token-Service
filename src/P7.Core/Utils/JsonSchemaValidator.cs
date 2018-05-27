using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace P7.Core.Utils
{
    public class JsonSchemaValidator
    {
       
        public static JsonValidateResponse Validate(JsonValidateRequest request)
        {
            // load schema
            JSchema schema = JSchema.Parse(request.Schema);
            JToken json = JToken.Parse(request.Json);

            // validate json
            IList<ValidationError> errors;
            bool valid = json.IsValid(schema, out errors);

            // return error messages and line info to the browser
            return new JsonValidateResponse
            {
                Valid = valid,
                Errors = errors
            };
        }
    }
}