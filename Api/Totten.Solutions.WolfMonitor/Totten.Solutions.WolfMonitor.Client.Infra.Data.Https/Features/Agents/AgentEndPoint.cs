﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Totten.Solutions.WolfMonitor.Client.Infra.Data.Https.Base;
using Totten.Solutions.WolfMonitor.Infra.CrossCutting.Structs;

namespace Totten.Solutions.WolfMonitor.Client.Infra.Data.Https.Features.Authentication
{
    public class AgentEndPoint : BaseEndPoint 
    {
        public AgentEndPoint(CustomHttpCliente customHttpCliente) : base(customHttpCliente)
        {

        }
        public string GetClientCredentials()
            => Convert.ToBase64String(Encoding.ASCII.GetBytes($"postman:postmanSecret"));

        public string Login()
        {
            var request = base.Client.CreateRequest(HttpMethod.Post, "identityserver/connect/token");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", GetClientCredentials());
            request.Headers.Host = base.Client.UrlApi.Replace("http://", "");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password"},
                { "username",  base.Client.User.Login },
                { "password",  base.Client.User.Password},
                { "scope", "Agents Monitoring"}
            });

            using (var httpClient = new HttpClient())
            using (request)
            using (var response = httpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro na comunicação com a API, não foi possivel obter o token. status de erro: {response.StatusCode}");
                }

                dynamic content = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
                return content.access_token.ToString();
            }
        }

        public async Task<Result<Exception, T>> GetDetail<T>(Guid id)
            => await InnerGetAsync<T>($"agents/{id}");

        public async Task<Result<Exception, Unit>> Delete(Guid agentId)
            => await InnerAsync<Unit, object>($"agents/{agentId}", null, HttpMethod.Delete);

        public async Task<Result<Exception, PageResult<T>>> GetAllAgents<T>()
            => await InnerGetAsync<PageResult<T>>($"agents");

        private async Task<Result<Exception, Unit>> InnerAsync<T>(T agent, HttpMethod httpMethod)
            => await InnerAsync<Unit, T>("agents", agent, httpMethod);

        public bool Update<T>(T agent)
            => InnerAsync<T>(agent, HttpMethod.Patch).ConfigureAwait(false).GetAwaiter().GetResult().IsSuccess;

        public Result<Exception, T> GetInfo<T>()
            => InnerGetAsync<T>("agents/info").ConfigureAwait(false).GetAwaiter().GetResult();

    }
}
