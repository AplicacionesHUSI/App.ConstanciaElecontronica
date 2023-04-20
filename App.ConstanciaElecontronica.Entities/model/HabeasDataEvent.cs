using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class HabeasDataEvent
    {
        public int IdTipoDoc { get; set; }
        public string NumDoc { get; set; }
        public string UsrWindowsReg { get; set; }
        public bool IndMismoPaciente { get; set; }
        public string Idioma { get; set; }  // es/en
        //responsable
        public string Responsable { get; set; } //Nombre
        public int IdTipoDocResponsable { get; set; }
        public string NumDocResponsable { get; set; }
        public string Parentesco { get; set; }

    }
}
