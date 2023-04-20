using System;
using System.Collections.Generic;
using System.Text;

namespace App.ConstanciaElecontronica.Entities.Response
{
    public class ResponseBase<T>
    {
        public DateTime date;
        public int code;
        public string message;
        public T data { set; get; }
    }
}
