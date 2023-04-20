using App.ConstanciaElecontronica.Entities.model;
using App.ConstanciaElecontronica.Entities.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App.ConstanciaElecontronica.Entities.Interfaces
{
    public interface IServicePatient
    {
        ResponseBase<Patient> GetPatient(string atencion,string token);
        ResponseBase<List<Instruccion>> GetInstrucciones(string atencion,string token);
    }
}
