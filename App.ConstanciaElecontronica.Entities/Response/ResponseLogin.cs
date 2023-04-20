using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.Response
{
    public class ResponseLogin
    {
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public string User { get; set; }
    }
}

