using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TaskManager.Controllers
{
    public class TokenManager
    {
        private static IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
        private static IJsonSerializer serializer = new JsonNetSerializer();
        private static IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        private static JwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
        private static string secret = "supersecret";
        private static JwtValidator validator = new JwtValidator(serializer, new UtcDateTimeProvider());
        private static JwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

        public static string CreateTokenFor(string id)
        {
            Dictionary<string, string> payload = new Dictionary<string, string>{{"_id", id}};
            return encoder.Encode(payload, secret);
        }

        public static string Verify(string token)
        {
            try
            {
                var json = decoder.Decode(token, secret, verify: true);
                Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                return payload["_id"];
            }
            catch (SignatureVerificationException)
            {
                return null;
            }
            catch (Exception genericException)
            {
                Console.WriteLine(genericException.Message);
                return null;
            }
        }
    
    }
}
