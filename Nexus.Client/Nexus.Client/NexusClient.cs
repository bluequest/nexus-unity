using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Nexus.Client
{
    /// <summary>
    /// Access to the Nexus.gg APIs.
    /// </summary>
    public sealed class NexusClient
    {
        /// <summary>
        /// Base URL for Nexus. 
        /// </summary>
        private const string NexusUrl = "https://nexus.gg/";
        
        /// <summary>
        /// Base URL for Nexus API.
        /// </summary>
        private const string NexusServiceUrl = "https://attribution.nexus.gg/";
        
        /// <summary>
        /// URL for Nexus/Creators.
        /// </summary>
        private const string NexusCreatorsUrl = NexusClient.NexusServiceUrl + "/creators";

        private const string NexusSharedSecretHeader = "x-shared-secret";

        private static readonly NexusCreators EmptyResult = new NexusCreators();
        
        private readonly INexusSettings settings;

        private readonly HttpClient client;

        /// <summary>
        /// Creates a new instance of the NexusClient.
        /// </summary>
        public NexusClient(INexusSettings settings) : this(settings, new HttpClient())
        {
        }
        
        internal NexusClient(INexusSettings settings, HttpClient client)
        {
            this.settings = settings;
            this.client = client;
        }

        /// <summary>
        /// Retrieve the <see cref="NexusCreators"/>.
        /// </summary>
        /// <returns>The list of creators for this user.</returns>
        public async Task<NexusCreators> GetCreators()
        {
            if (!this.TryGetSecret(out string secret))
            {
                // no settings or no secret, exit
                return NexusClient.EmptyResult;
            }
            
            // Retrieve creators from the server
            NexusCreators result;
            try
            {
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, NexusClient.NexusCreatorsUrl);
                
                // set the secret message header, used to identify the client
                request.Headers.Add(NexusClient.NexusSharedSecretHeader, secret);
                using HttpResponseMessage response = await this.client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    // success, convert response to NexusCreators
                    using Stream stream = await response.Content.ReadAsStreamAsync();
                    DataContractJsonSerializer serialize = new DataContractJsonSerializer(typeof(NexusCreators));
                    result = serialize.ReadObject(stream) as NexusCreators;

                    if (result != null && result.Creators != null)
                    {
                        // convert abbreviated `nexusUrl` from server to full url
                        foreach (NexusCreator creator in result.Creators)
                        {
                            creator.NexusURL = NexusClient.NexusUrl + creator.nexusUrl;
                        }
                    }
                }
                else
                {
                    // failure, log and exit
                    Trace.TraceError("Failed to get Nexus creators: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                    result = NexusClient.EmptyResult;
                }
            }
            catch (Exception e)
            {
                // something went wrong
                result = NexusClient.EmptyResult;
                Trace.TraceError("Failed to get Nexus creators: {0}", e.Message);
            }

            return result;
        }

        public async Task<bool> AttributeCreator()
        {
            if (!this.TryGetSecret(out string secret))
            {
                // no settings or no secret, exit
                return false;
            }
            
            bool result;
            try
            {
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, NexusClient.NexusCreatorsUrl);
                
                // set the secret message header, used to identify the client
                request.Headers.Add(NexusClient.NexusSharedSecretHeader, secret);
                using HttpResponseMessage response = await this.client.SendAsync(request);
                result = response.IsSuccessStatusCode;
                if (!result)
                {
                    // failure, log and exit
                    Trace.TraceError("Failed to attribute Nexus creator: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                // something went wrong
                result = false;
                Trace.TraceError("Failed to attribute Nexus creator: {0}", e.Message);
            }

            return result;
        }

        /// <summary>
        /// Retrieve and validate shared secret from current settings.
        /// </summary>
        private bool TryGetSecret(out string secret)
        {
            secret = string.Empty;
            if (this.settings == null)
            {
                Trace.TraceError("NexusSettings is not defined.");
                return false;
            }
            else if (string.IsNullOrEmpty(this.settings.SharedSecret))
            {
                Trace.TraceError("NexusSettings.SharedSecret is not defined.");
                return false;
            }

            secret = this.settings.SharedSecret;
            return true;
        }
    }
}