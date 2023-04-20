using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class CartaCompromisoCovid19
    {

        public string TipoFirma { get; set; }
        public int IdCliente { get; set; }
        public int IdAtencion { get; set; }
        public string UsrWindowsReg { get; set; }
        public bool IndMismoPaciente { get; set; }
        
        //Acompañante 1
        public bool IndAcompananteUno { get; set; }
        public int? IdTipoDocResponsableUno { get; set; }
        public string NumDocResponsableUno { get; set; }
        public string ParentescoUno { get; set; }
        public string NomResponsableUno { get; set; }
        public string TelefonoUno { get; set; }

        //Acompañante 2
        public bool IndAcompananteDos { get; set; }
        public int? IdTipoDocResponsableDos { get; set; }
        public string NumDocResponsableDos { get; set; }
        public string ParentescoDos { get; set; }
        public string NomResponsableDos { get; set; }
        public string TelefonoDos { get; set; }
        public DateTime FecRegistro { get; set; }

        public string ImageUno { get; set; }

        public string ImageDos { get; set; }
        public string SingImage { get; set; }

    }
}
