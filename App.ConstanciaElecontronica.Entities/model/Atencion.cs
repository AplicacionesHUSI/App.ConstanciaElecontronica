using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class Atencion
    {
        public int IdAtencion { get; set; }
        
        public short IdAtencionTipo { get; set; }        
        public string TipoAtencion { get; set; }
    }
}
