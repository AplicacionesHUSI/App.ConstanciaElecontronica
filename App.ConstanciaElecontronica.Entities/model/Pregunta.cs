using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.model
{
    public class Pregunta
    {
        public int IdPregunta { get; set; }
        [JsonProperty(PropertyName = "Pregunta")]
        public string Descripcion { get; set; }
        public bool Acepta { get; set; }
    }
}
