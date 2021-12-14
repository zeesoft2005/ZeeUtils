using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZeeUtils
{
    /// <summary>
    /// A HttpClient wrapper; can further be enhanced for scoped clients by https://stackoverflow.com/questions/42235677/httpclient-this-instance-has-already-started
    /// </summary>
    public static class HttpUtils
    {
        
        public static async Task<string> Request(this HttpClient client, string api, string method = "GET", StringContent payload = null, HttpClient otherClient = null)
        {
            string res = string.Empty;
            Task<HttpResponseMessage> response = null;
            try
            {
                    response = method == "POST" ? client.PostAsync(api, payload) :
                        (method == "PUT" ? client.PutAsync(api, payload) : client.GetAsync(api)); //if not POST/PUT, then default to GET

                response.Wait();                
                if (response.Result.IsSuccessStatusCode)
                {
                    res = await response.Result.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new ApplicationException(response.Result.ReasonPhrase);
                }
            }
            catch (Exception error)
            {
                throw error;//TODO: log it later
            }
            return res;
        }
        
        public static async Task<string> RequestDelete(this HttpClient client, string api, HttpClient otherClient=null)
        {
            string res = string.Empty;
            try
            {
                var response = otherClient==null ? client.DeleteAsync(api) : otherClient.DeleteAsync(api);
                response.Wait();
                if (response.Result.IsSuccessStatusCode)
                {
                    res = await response.Result.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new ApplicationException(response.Result.ReasonPhrase);
                }
            }
            catch (Exception error)
            {
                throw error;//TODO: log it later
            }
            return res;
        }

        #region RestSharp utils

        //public static async Task<T> Request<T>(this RestClient client, string api, 
        //    string method = "GET", StringContent payload = null) where T: class
        //{
        //    T response = null;
        //    try
        //    {
        //        RestRequest req = new RestRequest(api, method == "POST" ? Method.POST : (method == "PUT" ? Method.PUT : Method.GET));

        //        response = method == "POST" ? await client.PostAsync<T>(req) :
        //            (method == "PUT" ? await client.PutAsync<T>(req) : await client.GetAsync<T>(req)); //if not POST/PUT, then default to GET

                
        //    }
        //    catch (Exception error)
        //      {
        //        throw error;//TODO: log it later
        //    }
        //    return response;
        //}
        //public static async Task<string> RequestDelete(this RestClient client, string api)
        //{
        //    string response = string.Empty;
        //    try
        //    {
        //        RestRequest req = new RestRequest(api, Method.DELETE);
        //        response = await client.DeleteAsync<string>(req);
        //    }
        //    catch (Exception error)
        //    {
        //        throw error;//TODO: log it later
        //    }
        //    return response;
        //}


        #endregion

    }
}
