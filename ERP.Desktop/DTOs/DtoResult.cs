using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.DTOs
{
    public class DtoResult
    {
        public bool IsSuccessed { get; set; }
        public string Message { get; set; }
    }
    public class DtoResultObj<T>
    {
        public T Object { get; set; }
        public bool IsSuccessed { get; set; }
        public string Message { get; set; }
    }
}
