﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Totten.Solutions.WolfMonitor.Client.Infra.Data.Https.Base;
using Totten.Solutions.WolfMonitor.Client.Infra.Data.Https.Features.Users;
using Totten.Solutions.WolfMonitor.Client.Infra.Data.Https.Features.Users.ViewModels;
using Totten.Solutions.WolfMonitor.Infra.CrossCutting.Structs;

namespace Totten.Solutions.WolfMonitor.WpfApp.Applications.Users
{
    public class UserService : IUserService
    {
        private UserEndPoint _endPoint;
        public UserService(UserEndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        public async Task<Result<Exception, UserBasicInformationViewModel>> GetInfo()
        {
            return await _endPoint.GetInfo();
        }
        public string GetClientCredentials()
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes($"postman:postmanSecret"));
        }
        public string Authentication()
        {
            var request = _endPoint.Client.CreateRequest(HttpMethod.Post, "identityserver/connect/token");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", GetClientCredentials());
            request.Headers.Host = _endPoint.Client.UrlApi.Replace("http://", "");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "password"},
                { "username",  _endPoint.Client.User.Login },
                { "password",  _endPoint.Client.User.Password},
                { "scope", "Agents Monitoring Users"}
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
    }
}
