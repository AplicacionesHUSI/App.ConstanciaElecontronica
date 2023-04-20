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
    public class ServiceConfiguracion
    {

		private readonly string _APIconstancia;
		private  HttpClient clientAPI;

		public ServiceConfiguracion(string APIconstancia) {

			_APIconstancia = APIconstancia;

		}

		public ResponseBase<List<TipoDocumento>> GetTiposDocumento(string token)
		{
			var response = new ResponseBase<List<TipoDocumento>>();
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
					var reponse = clientAPI.GetAsync($"{_APIconstancia}/api/v1/Configuracion/tiposDocumento").Result;
					response.code = (int)reponse.StatusCode;
					if (reponse.StatusCode == HttpStatusCode.OK)
					{
						string result = reponse.Content.ReadAsStringAsync().Result;
						var responseBase = JsonConvert.DeserializeObject<ResponseBase<List<TipoDocumento>>>(result);
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

		


	}
}
