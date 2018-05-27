using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Schema;

namespace P7.Core.Utils
{
    public class JsonValidateResponse
    {
        public bool Valid { get; set; }
        public IList<ValidationError> Errors { get; set; }
    }
}
