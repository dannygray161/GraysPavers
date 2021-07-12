using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GraysPavers.Utilities
{
    public static class SessionExtensionMethods
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }





        #region Session Extension Method to store more than integers or strings SET

        /*
         * has to be a static class in order to be an extension method. we will have a public static void
         * set method on generic type T, and the first param should be the property that you want this extension method
         * (Isession) then we add a string of key and a value that is generic.
         * So: so far we have set<t>(this Isession session, string key, T value).
         *
         * after that we have to define what the method is going to do
         * so we want to set the string so we will use set string and then pass the params of key and json
         * serializer dot serializer (with the argument of value).
         *
         * this allows us to serialize the object and store it as a string. 
         *
         */

        #endregion





        #region Session Extension Method GET
        /*
         * In order to retreive the object it first has to be deserialized
         * we accomplish this with a GET method
         * so same as before we need a type of T for generic, and then get<T>
         * and we want the parameters of this Isession session, and the string key like before, but no value because
         * we have yet to retreive it. now we set a var as value and in order to get it we use
         * session.getstring(key) to retreive the value
         * now we have to deserialize it so we return value
         * and if it is null we assign it a default value and if it is not null
         * we use json serializer dot deserialize (on generic type) <T> **this is the generic
         * ((value) as a parameter of deserialize
         */
        

        #endregion









    }
}
