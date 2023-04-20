using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Entities.Request;
using App.ConstanciaElecontronica.Entities.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace App.ConstanciaElecontronica.Services
{
    public class ServiceTratamientoFarmacologico
    {
		private readonly string _APICuidadosEnfermeria;
		private HttpClient clientAPI;

		public ServiceTratamientoFarmacologico(string APIconstancia)
		{
			_APICuidadosEnfermeria = APIconstancia;
		}

	


		public ResponseBase<bool> SendFirma(RequestFirmaTratamiento requestFirma, string token)
		{
			var response = new ResponseBase<bool>();
			try
			{

				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler);

				using (clientAPI)
				{
					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					var json = JsonConvert.SerializeObject(requestFirma);
					var content = new StringContent(json, Encoding.UTF8, "application/json");
					clientAPI.DefaultRequestHeaders.Authorization
						= new AuthenticationHeaderValue("Bearer", token);
					var reponse = clientAPI.PostAsync($"{_APICuidadosEnfermeria}/api/v1/TratamientoFarmacologico", content).Result;

					if (reponse.StatusCode == HttpStatusCode.OK)
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<bool>>(result);
					}
					else
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<bool>>(result);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				response.code = 500;
				response.message = "Se ha presentado una excepcion :: " + e.Message;
			}
			return response;
		}


		public ResponseBase<InfoTratamiento> GetInfo(int IdAtencion, string token)
		{
			var response = new ResponseBase<InfoTratamiento>();
			try
			{
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler);

				using (clientAPI)
				{
					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					clientAPI.DefaultRequestHeaders.Authorization
						= new AuthenticationHeaderValue("Bearer", token);
					var reponse = clientAPI.GetAsync($"{_APICuidadosEnfermeria}/api/v1/TratamientoFarmacologico/"+IdAtencion).Result;

						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<InfoTratamiento>>(result);
				
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				response.code = 500;
				response.message = "Se ha presentado una excepcion :: " + e.Message;
			}
			return response;
		}

	}
}
