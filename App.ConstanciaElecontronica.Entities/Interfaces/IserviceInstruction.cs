using App.ConstanciaElecontronica.Entities.Request;
using App.ConstanciaElecontronica.Entities.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.Interfaces
{
    public interface IserviceInstruction
    {
        ResponseBase<bool> SendInstruction(RequestFirmaEgreso requestFirma,string token);
    }
}
