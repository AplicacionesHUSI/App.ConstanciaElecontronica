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
    public class ServiceCuidadosEnfermeria
    {
		private readonly string _APICuidadosEnfermeria;
		private HttpClient clientAPI;

		public ServiceCuidadosEnfermeria(string APIconstancia)
		{
			_APICuidadosEnfermeria = APIconstancia;
		}

		public ResponseBase<List<Pregunta>> GetPreguntas(string tipo,string token)
		{
			var response = new ResponseBase<List<Pregunta>>();
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
					var reponse = clientAPI.GetAsync($"{_APICuidadosEnfermeria}/api/v1/Enfermeria/preguntas/"+tipo).Result;
						if (reponse.StatusCode == HttpStatusCode.OK)
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<List<Pregunta>>>(result);
					}
					else
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<List<Pregunta>>>(result);
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

		public ResponseBase<FirmaPaciente> GetFirma(string atencion, string tipo, string token)
		{
			var response = new ResponseBase<FirmaPaciente>();
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
					var reponse = clientAPI.GetAsync($"{_APICuidadosEnfermeria}/api/v1/FirmaPacientes/" +atencion+"/" + tipo).Result;
						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<FirmaPaciente>>(result);
					
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



		public async Task<ResponseBase<bool>> SendFirma(RequestFirmaEnfermeria requestFirma, string token)
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
					var reponse = await clientAPI.PostAsync($"{_APICuidadosEnfermeria}/api/v1/Enfermeria", content);

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


	}
}
