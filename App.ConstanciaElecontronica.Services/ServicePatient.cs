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
    public class ServicePatient : IServicePatient
    {

		private readonly string _APIconstancia;
		private  HttpClient clientAPI;

		public ServicePatient(string APIconstancia) {

			_APIconstancia = APIconstancia;
			

		}

		public ResponseBase<List<Instruccion>> GetInstrucciones(string atencion,string token)
		{
			var response = new ResponseBase<List<Instruccion>>();
			try
			{
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);

				using (clientAPI)
				{
					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Authorization
						 = new AuthenticationHeaderValue("Bearer", token);
					var reponse = clientAPI.GetAsync($"{_APIconstancia}/api/v1/Instrucciones/" + atencion).Result;
					response.code = (int)reponse.StatusCode;
					if (reponse.StatusCode == HttpStatusCode.OK)
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						var responseBase = JsonConvert.DeserializeObject<ResponseBase<List<Instruccion>>>(result);
						return responseBase;
					}
				}
			}
			catch (Exception e)
			{
				response.code = (int)HttpStatusCode.InternalServerError;
				response.message = $"Se ha presenciado una excepcion :: {e.Message}";
			}
			return response;
		}

		public  ResponseBase<Patient> GetPatient(string atencion,string token)
        {
			var reponsePatient = new ResponseBase<Patient>();
			try {

				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);

				using (clientAPI)
				{
					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Authorization
						 = new AuthenticationHeaderValue("Bearer", token);
					var reponse = clientAPI.GetAsync($"{_APIconstancia}/api/v1/Cliente/" + atencion).Result;
					reponsePatient.code = (int)reponse.StatusCode;
					if (reponse.StatusCode == HttpStatusCode.OK)
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						var responseBase = JsonConvert.DeserializeObject<ResponseBase<Patient>>(result);
						reponsePatient.data = responseBase.data;
					}
				}

			
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				reponsePatient.code = (int)HttpStatusCode.InternalServerError;
				reponsePatient.message = $"Se ha presenciado una excepcion :: {e.Message}";
			}
			return reponsePatient;
        }

		public ResponseBase<Patient> GetPatient(int TipoDoc,string NumDoc,string token)
		{
			var reponsePatient = new ResponseBase<Patient>();
			try
			{
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);

				using (clientAPI)
				{
					clientAPI.DefaultRequestHeaders.Accept.Clear();

					clientAPI.DefaultRequestHeaders.Authorization
										= new AuthenticationHeaderValue("Bearer", token);

					var reponse = clientAPI.GetAsync($"{_APIconstancia}/api/v1/Cliente/" + TipoDoc + "/" + NumDoc).Result;
					reponsePatient.code = (int)reponse.StatusCode;
					if (reponse.StatusCode == HttpStatusCode.OK)
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						var responseBase = JsonConvert.DeserializeObject<ResponseBase<Patient>>(result);
						reponsePatient.data = responseBase.data;
					}
					
				}
				
			}
			catch (Exception e)
			{
				reponsePatient.code = (int)HttpStatusCode.InternalServerError;
				reponsePatient.message = $"Se ha presenciado una excepcion :: {e.Message}";
				Console.WriteLine(e.Message);
			}

			clientAPI.Dispose();

			return reponsePatient;
		}



	}
}
