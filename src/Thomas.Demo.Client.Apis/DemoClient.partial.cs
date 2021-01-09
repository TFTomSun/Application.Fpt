using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thomas.Demo.Client.Apis
{
    partial class NumbersClient
    {

        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
        {
            // Required, because there seems to be a problem in some cases with reading streams in Blazor WebAssembly.
            this.ReadResponseAsString = true;

        }

    }
}
