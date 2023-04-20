using App.ConstanciaElecontronica.Entities;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Entities.Response;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace App.ConstanciaElecontronica.Services
{
	public class LoginService
	{
		private readonly string _APILogin;
		private HttpClient clientAPI;

		public LoginService(string APIlogin)
		{
			_APILogin = APIlogin;

		}

		public ResponseBase<ResponseLogin> ValidationLoggin(Credentials credentials)
		{
			var response = new ResponseBase<ResponseLogin>();
			try
			{
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);

				using (clientAPI)
				{

					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					var json = JsonConvert.SerializeObject(credentials);
					var data = new StringContent(json, Encoding.UTF8, "application/json");
					string url = $"{_APILogin}/api/v2/Autentication";
					var reponse = clientAPI.PostAsync(url, data).Result;
					response.code = (int)reponse.StatusCode;

					string result = reponse.Content.ReadAsStringAsync().Result;
					response = JsonConvert.DeserializeObject<ResponseBase<ResponseLogin>>(result);

				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				response.code = 500;
				response.message = "Se ha presentado una excepcion en login :: " + e.Message;
			}

			return response;
		}

		public async  Task<ResponseBase<Application>> GetApplicationInfo(string applicationId)
		{
			var response = new ResponseBase<Application>();
			try
			{
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);


				using (clientAPI)
				{

					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					string url = $"{_APILogin}/api/aplication/" + applicationId;
					var reponse = await clientAPI.GetAsync(url);
					response.code = (int)reponse.StatusCode;

					string result = await reponse.Content.ReadAsStringAsync();
					response = JsonConvert.DeserializeObject<ResponseBase<Application>>(result);


				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				response.code = 500;
				response.message = "Se ha presentado una excepcion en login :: " + e.Message;
			}

			return response;
		}

		public async Task<byte[]> getFile(string applicationId)
		{

			
			try
			{
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);

				using (clientAPI)
				{
					string url = $"{_APILogin}/api/aplication/Download/" + applicationId;
					var httpResponse = await clientAPI.GetAsync(url);
					
					if (httpResponse.StatusCode == HttpStatusCode.OK)
					{
						return await httpResponse.Content.ReadAsByteArrayAsync();
					}
					else
					{
						//Url is Invalid
						return null;
					}
				}
			}
			catch (Exception)
			{
				//Handle Exception
				return null;
			}
		}
	}
}
