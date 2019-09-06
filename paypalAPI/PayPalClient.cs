/*
 * https://developer.paypal.com/docs/checkout/reference/server-integration/setup-sdk/#
 * 
 */

using System;
using PayPalCheckoutSdk.Core;
using BraintreeHttp;

using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace Samples
{
    public class PayPalClient
    {
        /**
            Set up PayPal environment with sandbox credentials.
            In production, use LiveEnvironment.
         */
        public static PayPalEnvironment environment()
        {
            // return new SandboxEnvironment("PAYPAL-SANDBOX-CLIENT-ID", "PAYPAL-SANDBOX-CLIENT-SECRET");
            return new SandboxEnvironment("AVMmmgQ68CuoEnnEOH1ItGcGL3jnHMAq-l3g3Gm70X0el7x56oLCZqugr9vdxRGQ2eI9OlTVxEfuKjU1", "ENS498SmIprbOmSCS3qHb8to0R2nBlczdhtzUe4-G-HzufkGZiMh7GnSdEq3c-wwbKTy4TxcLbmV7YUF");
        }

        /**
            Returns PayPalHttpClient instance to invoke PayPal APIs.
         */
        public static HttpClient client()
        {
            return new PayPalHttpClient(environment());
        }

        public static HttpClient client(string refreshToken)
        {
            return new PayPalHttpClient(environment(), refreshToken);
        }

        /**
            Use this method to serialize Object to a JSON string.
        */
        public static String ObjectToJSONString(Object serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = JsonReaderWriterFactory.CreateJsonWriter(
                        memoryStream, Encoding.UTF8, true, true, "  ");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(serializableObject.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            ser.WriteObject(writer, serializableObject);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();
        }
    }
}