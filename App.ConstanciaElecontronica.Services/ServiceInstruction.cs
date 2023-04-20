using App.ConstanciaElecontronica.Entities.Interfaces;
using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Entities.Request;
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
    public class ServiceInstruction : IserviceInstruction
    {
		private readonly string _APIconstancia;
		private  HttpClient clientAPI;

		public ServiceInstruction(string APIconstancia)
		{
			_APIconstancia = APIconstancia;
			
		}

		public ResponseBase<bool> SendInstruction(RequestFirmaEgreso requestFirma,string token)
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
					var reponse = clientAPI.PostAsync($"{_APIconstancia}/api/v1/Instrucciones", content).Result;

					if (reponse.StatusCode == HttpStatusCode.OK)
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<bool>>(result);
					}
					else {
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

		public async Task<ResponseBase<bool>> SendHabeasData(FirmaHabeasData Firma,string token)
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

					var json = JsonConvert.SerializeObject(Firma);
					var content = new StringContent(json, Encoding.UTF8, "application/json");
					clientAPI.DefaultRequestHeaders.Authorization
					 = new AuthenticationHeaderValue("Bearer", token);
					var reponse = await clientAPI.PostAsync($"{_APIconstancia}/api/v1/HabeasData", content);
					response.code = (int)reponse.StatusCode;
					if (reponse.StatusCode == HttpStatusCode.OK)
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						var responseBase = JsonConvert.DeserializeObject<ResponseBase<bool>>(result);
						return responseBase;
					}
					else
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						response = JsonConvert.DeserializeObject<ResponseBase<bool>>(result);
					}
				}

				//using (var client = new HttpClient())
				//{
				//client.DefaultRequestHeaders.Accept.Clear();
				//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				//var json = JsonConvert.SerializeObject(Firma);
				//var content = new StringContent(json, Encoding.UTF8, "application/json");
				//client.DefaultRequestHeaders.Authorization
				// = new AuthenticationHeaderValue("Bearer", token);
				//var reponse = await client.PostAsync($"{_APIconstancia}/api/v1/HabeasData", content);
				//response.code = (int)reponse.StatusCode;
				//if (reponse.StatusCode == HttpStatusCode.OK)
				//{
				//	string result = reponse.Content.ReadAsStringAsync().Result;
				//	var responseBase = JsonConvert.DeserializeObject<ResponseBase<bool>>(result);
				//	return responseBase;
				//}
				//else {
				//	string result = reponse.Content.ReadAsStringAsync().Result;
				//	response= JsonConvert.DeserializeObject<ResponseBase<bool>>(result);

				//}
				//}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				response.code = 500;
				response.message = "Se ha presentado una excepcion :: " + e.Message; 
			}
			return response ;
		}
	}
}
