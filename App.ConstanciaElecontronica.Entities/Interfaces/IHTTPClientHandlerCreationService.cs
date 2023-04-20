using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.Interfaces
{
    public interface IHTTPClientHandlerCreationService
    {
        HttpClientHandler GetInsecureHandler();
    }
}
