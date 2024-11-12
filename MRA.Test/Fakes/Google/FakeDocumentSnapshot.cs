using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.WebApi.Test.Fakes.Google
{
    public class FakeDocumentSnapshot
    {
        public string Id { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public FakeDocumentSnapshot(string id, Dictionary<string, object> data)
        {
            Id = id;
            Data = data;
        }

        public T ConvertTo<T>() where T : new()
        {
            // Simulamos la conversión a un tipo T, aquí deberías ajustar a tu lógica de negocio
            return new T();
        }

        public object GetFieldValue(string field)
        {
            return Data.TryGetValue(field, out var value) ? value : null;
        }
    }

}
