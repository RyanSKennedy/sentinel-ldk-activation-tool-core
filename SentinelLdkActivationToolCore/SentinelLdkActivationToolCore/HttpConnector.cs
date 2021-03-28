using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using MyLogClass;
using SentinelLdkActivationToolCore;
using SentinelLdkActivationToolCore.Controllers;
using SentinelLdkActivationToolCore.Models;

namespace SentinelLdkActivationToolCore
{
    public class HttpConnector
    {
        private HttpClientHandler handler;  
        public HttpClient httpClient;
        public HttpResponseMessage httpClientResponse;
        public string httpClientResponseStr;
        public string httpClientResponseStatus;

        public HttpConnector(HttpClient newClient = null, HttpResponseMessage newResponse = null, string newResponseStr = null, string newResponseStatus = null)
        {
            httpClient = newClient;
            httpClientResponse = newResponse;
            httpClientResponseStr = newResponseStr;
            httpClientResponseStatus = newResponseStatus;
        }

        public HttpConnector GetRequest(string myAction, HttpMethod method = null, string myPlaceholder = null, KeyValuePair<string, string> rData = new KeyValuePair<string, string>(), HttpConnector client = null)
        {
            handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            string fullRequestUrl = SentinelMethods.UrlBuilder(SentinelSettings.actionsList[myAction], myPlaceholder);
            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Get Url for request: " + fullRequestUrl);

            if (client == null) {
                client = new HttpConnector(null, new HttpResponseMessage(), "", "");
            }

            if (method == null) {
                method = HttpMethod.Get;
            }

            switch (myAction)
            {
                case "login":
                    try
                    {
                        if (SentinelSettings.ignoreSslCertStatus) client.httpClient = new HttpClient(handler: handler);
                        else client.httpClient = new HttpClient();

                        var content = new StringContent(rData.Value, Encoding.UTF8, "application/xml");
                        client.httpClientResponse = client.httpClient.PostAsync(fullRequestUrl, content).Result;
                        client.httpClientResponseStr = client.httpClientResponse.Content.ReadAsStringAsync().Result;
                        client.httpClientResponseStatus = client.httpClientResponse.StatusCode.ToString();
                    }
                    catch (System.AggregateException e)
                    {
                        client.httpClientResponseStatus += e.InnerException.InnerException.Message;

                    }
                    catch (HttpRequestException hE)
                    {
                        client.httpClientResponseStatus += hE.Message;
                    }
                    break;

                case "loginpk":
                    try
                    {
                        if (SentinelSettings.ignoreSslCertStatus) client.httpClient = new HttpClient(handler: handler);
                        else client.httpClient = new HttpClient();

                        var content = new FormUrlEncodedContent(new[] { rData });
                        client.httpClientResponse = client.httpClient.PostAsync(fullRequestUrl, content).Result;
                        client.httpClientResponseStr = client.httpClientResponse.Content.ReadAsStringAsync().Result;
                        client.httpClientResponseStatus = client.httpClientResponse.StatusCode.ToString();
                    }
                    catch (System.AggregateException e)
                    {
                        client.httpClientResponseStatus += e.InnerException.InnerException.Message;

                    }
                    catch (HttpRequestException hE)
                    {
                        client.httpClientResponseStatus += hE.Message;
                    }
                    break;

                case "logout":
                    if (client != null && client.httpClient != null)
                    {
                        if (client.httpClientResponseStatus == "OK")
                        {
                            try
                            {
                                var content = new FormUrlEncodedContent(new[] { rData });

                                client.httpClientResponse = client.httpClient.PostAsync(fullRequestUrl, content).Result;
                                client.httpClientResponseStr = client.httpClientResponse.Content.ReadAsStringAsync().Result;
                                client.httpClientResponseStatus = client.httpClientResponse.StatusCode.ToString();
                            }
                            catch (System.AggregateException e)
                            {
                                client.httpClientResponseStatus += e.InnerException.InnerException.Message + " | in get info request after login by PK.";
                            }
                            catch (HttpRequestException hE)
                            {
                                client.httpClientResponseStatus += hE.Message + " | in get info request after login by PK.";
                            }
                        }
                    }
                    else
                    {
                        client.httpClientResponseStatus = "Not set HttpClient instance.";
                    }
                    break;

                case "getinfo":
                    if (client != null && client.httpClient != null)
                    {
                        if (client.httpClientResponseStatus == "OK")
                        {
                            try
                            {
                                if (method == HttpMethod.Get)
                                {
                                    client.httpClientResponse = client.httpClient.GetAsync(fullRequestUrl).Result;
                                }
                                else if (method == HttpMethod.Post)
                                {
                                    var content = new StringContent(rData.Value, Encoding.UTF8, "application/xml");
                                    client.httpClientResponse = client.httpClient.PostAsync(fullRequestUrl, content).Result;
                                }

                                client.httpClientResponseStr = client.httpClientResponse.Content.ReadAsStringAsync().Result;
                                client.httpClientResponseStatus = client.httpClientResponse.StatusCode.ToString();
                            }
                            catch (System.AggregateException e)
                            {
                                client.httpClientResponseStatus += e.InnerException.InnerException.Message + " | in get info request after login by PK.";
                            }
                            catch (HttpRequestException hE)
                            {
                                client.httpClientResponseStatus += hE.Message + " | in get info request after login by PK.";
                            }
                        }
                    }
                    else
                    {
                        client.httpClientResponseStatus = "Not set HttpClient instance.";
                    }
                    break;

                case "getact":
                    if (client != null && client.httpClient != null)
                    {
                        if (client.httpClientResponseStatus == "OK" || client.httpClientResponseStatus == "Created")
                        {
                            try
                            {
                                var content = new StringContent(rData.Value, Encoding.UTF8, "application/xml");
                                client.httpClientResponse = client.httpClient.PostAsync(fullRequestUrl, content).Result;
                                client.httpClientResponseStr = client.httpClientResponse.Content.ReadAsStringAsync().Result;
                                client.httpClientResponseStatus = client.httpClientResponse.StatusCode.ToString();
                            }
                            catch (System.AggregateException e)
                            {
                                client.httpClientResponseStatus += e.InnerException.InnerException.Message + " | in activate request after login by PK.";
                            }
                            catch (HttpRequestException hE)
                            {
                                client.httpClientResponseStatus += hE.Message + " | in activate request after login by PK.";
                            }
                        }
                    }
                    else
                    {
                        client.httpClientResponseStatus = "Not set HttpClient instance.";
                    }
                    break;

                case "getfpu":
                    try
                    {
                        if (SentinelSettings.ignoreSslCertStatus) client.httpClient = new HttpClient(handler: handler);
                        else client.httpClient = new HttpClient();

                        var content = new StringContent(rData.Value, Encoding.UTF8, "application/xml");
                        client.httpClientResponse = client.httpClient.PostAsync(fullRequestUrl, content).Result;
                        client.httpClientResponseStr = client.httpClientResponse.Content.ReadAsStringAsync().Result;
                        client.httpClientResponseStatus = client.httpClientResponse.StatusCode.ToString();
                    }
                    catch (System.AggregateException e)
                    {
                        client.httpClientResponseStatus += e.InnerException.InnerException.Message + " | in get update by C2V request.";
                    }
                    catch (HttpRequestException hE)
                    {
                        client.httpClientResponseStatus += hE.Message + " | in get update by C2V request.";
                    }
                    break;

                case "getlic":
                    if (client != null && client.httpClient != null)
                    {
                        if (client.httpClientResponseStatus == "OK")
                        {
                            try
                            {
                                client.httpClientResponse = client.httpClient.GetAsync(fullRequestUrl).Result;
                                client.httpClientResponseStr = client.httpClientResponse.Content.ReadAsStringAsync().Result;
                                client.httpClientResponseStatus = client.httpClientResponse.StatusCode.ToString();
                            }
                            catch (System.AggregateException e)
                            {
                                client.httpClientResponseStatus += e.InnerException.InnerException.Message;

                            }
                            catch (HttpRequestException hE)
                            {
                                client.httpClientResponseStatus += hE.Message;
                            }
                        }
                    }
                    else
                    {
                        client.httpClientResponseStatus = "Not set HttpClient instance.";
                    }
                    break;

                default:
                    // Передали в качестве запроса что-то невразумительное
                    client.httpClientResponseStatus = "Something whrong...";
                    break;
            }

            if (Startup.myAppSettings.LogIsEnabled) Log.Write(@"Response request status: " + client.httpClientResponseStatus);

            return client;
        }
    }
}
