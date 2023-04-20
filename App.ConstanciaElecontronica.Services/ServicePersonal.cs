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
    public class ServicePersonal
    {

		private readonly string _APIconstancia;
		private HttpClient clientAPI;

		public ServicePersonal(string APIconstancia) {
			
			_APIconstancia = APIconstancia;

		}

		
		public  ResponseBase<Personal> GetPersonal(string usuarioWin,string token)
        {
			var reponsePatient = new ResponseBase<Personal>();
			try {
				HttpClientHandler clientHandler = new HttpClientHandler();
				clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
				clientAPI = new HttpClient(clientHandler, false);

				using (clientAPI)
				{
					clientAPI.DefaultRequestHeaders.Accept.Clear();
					clientAPI.DefaultRequestHeaders.Authorization
						 = new AuthenticationHeaderValue("Bearer", token);
					var reponse = clientAPI.GetAsync($"{_APIconstancia}/api/v1/Personal/" + usuarioWin).Result;
					reponsePatient.code = (int)reponse.StatusCode;
					
						string result = reponse.Content.ReadAsStringAsync().Result;
						var responseBase = JsonConvert.DeserializeObject<ResponseBase<Personal>>(result);
						reponsePatient.data = responseBase.data;
					
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				reponsePatient.code = (int)HttpStatusCode.InternalServerError;
				reponsePatient.message = $"Se ha presenciado una excepcion :: {e.Message}";
			}
			return reponsePatient;
        }		

	}
}
