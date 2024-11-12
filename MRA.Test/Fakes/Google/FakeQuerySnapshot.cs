using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.WebApi.Test.Fakes.Google
{
    public class FakeQuerySnapshot
    {
        private readonly List<FakeDocumentSnapshot> _documents;

        public FakeQuerySnapshot(List<FakeDocumentSnapshot> documents)
        {
            _documents = documents;
        }

        public List<FakeDocumentSnapshot> Documents => _documents;
    }

}
