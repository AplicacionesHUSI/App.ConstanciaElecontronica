using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class TipoDocumento
    {
        public int idTipoDoc { get; set; }
        
        public string nomTipoDoc { get; set; }
        
        public string codTipoDoc { get; set; }
        
        public int maxLongitud { get; set; }
        
        public string tip_docu { get; set; }

        public bool indHabilitado{ get; set; }

    }
}
