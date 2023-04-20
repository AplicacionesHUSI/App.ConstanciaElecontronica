using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Entities.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace App.ConstanciaElecontronica.Services
{
    public class ServiceCartaCompromiso
	{

		private readonly string _APIconstancia;
		private HttpClient clientAPI;

		public ServiceCartaCompromiso(string APIconstancia) {
			_APIconstancia = APIconstancia;
		}


		public async Task<ResponseBase<bool>> EnviarFirma(CartaCompromisoCovid19 requestFirma, string token)
		{
			var response = new ResponseBase<bool>();
			try
			{
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);

				using (clientAPI)
				{
					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					var json = JsonConvert.SerializeObject(requestFirma);
					var content = new StringContent(json, Encoding.UTF8, "application/json");
					clientAPI.DefaultRequestHeaders.Authorization
						= new AuthenticationHeaderValue("Bearer", token);
					var reponse = await clientAPI.PostAsync($"{_APIconstancia}/api/v1/CartaCompromiso/covid", content);


					string result = reponse.Content.ReadAsStringAsync().Result;
					response = JsonConvert.DeserializeObject<ResponseBase<bool>>(result);
					response.code = (int)reponse.StatusCode;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				response.code = 500;
				
				throw new Exception("Se ha presentado una excepcion :: " + e.Message);

			}
			return response;
		}

		public async Task<ResponseBase<string>> CancelarFirma(string token)
		{
			var response = new ResponseBase<string>();
			try
			{
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);

				using (clientAPI)
				{
					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					clientAPI.DefaultRequestHeaders.Authorization
						= new AuthenticationHeaderValue("Bearer", token);
					var reponse = clientAPI.GetAsync($"{_APIconstancia}/api/v1/Eventos/cancelar").Result;


					string result = reponse.Content.ReadAsStringAsync().Result;
					response = JsonConvert.DeserializeObject<ResponseBase<string>>(result);
					response.code = (int)reponse.StatusCode;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				response.code = 500;

				throw new Exception("Se ha presentado una excepcion :: " + e.Message);

			}
			return response;
		}
		public async Task<ResponseBase<string>> ValidarTablet()
		{
			var response = new ResponseBase<string>();
			try
			{
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);

				using (clientAPI)
				{
					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				
					var reponse = clientAPI.GetAsync($"{_APIconstancia}/api/v1/Eventos/statusTablet").Result;


					string result = reponse.Content.ReadAsStringAsync().Result;
					response = JsonConvert.DeserializeObject<ResponseBase<string>>(result);
					response.code = (int)reponse.StatusCode;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				response.code = 500;

				throw new Exception("Se ha presentado una excepcion :: " + e.Message);

			}
			return response;
		}

	}
}
