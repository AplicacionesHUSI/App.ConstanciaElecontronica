using App.ConstanciaElecontronica.Entities.Interfaces;
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
using System.Threading.Tasks;

namespace App.ConstanciaElecontronica.Services
{
    public class ServiceVacunacion
    {
		private readonly string _APIVacunacion;
		private HttpClient clientAPI;

		public ServiceVacunacion(string APIconstancia)
		{
			_APIVacunacion = APIconstancia;
			
		}

		public ResponseBase<List<Vacuna>> GetVacunas(string token)
		{
			var response = new ResponseBase<List<Vacuna>>();
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
					var reponse = clientAPI.GetAsync($"{_APIVacunacion}/api/v1/Vacunas").Result;
					
						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<List<Vacuna>>>(result);
						response.code = reponse.StatusCode.GetHashCode();
					
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



		public async Task<ResponseBase<bool>> SendFirma(RequestFirmaVacuna requestFirma, string token)
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
					var reponse = await clientAPI.PostAsync($"{_APIVacunacion}/api/v1/Vacunas", content);

					
						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<bool>>(result);
					
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
