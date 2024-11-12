using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.WebApi.Test.Fakes.Google
{
    public class FakeFirestoreDb
    {
        private readonly Dictionary<string, FakeCollectionReference> _collections;

        public FakeFirestoreDb()
        {
            _collections = new Dictionary<string, FakeCollectionReference>();
        }

        public void AddCollection(string name, List<FakeDocumentSnapshot> documents)
        {
            _collections[name] = new FakeCollectionReference(documents);
        }

        public FakeCollectionReference Collection(string name)
        {
            return _collections.TryGetValue(name, out var collection) ? collection : null;
        }
    }

}
