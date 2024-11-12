using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.WebApi.Test.Fakes.Google
{
    public class FakeCollectionReference
    {
        private readonly List<FakeDocumentSnapshot> _documents;

        public FakeCollectionReference(List<FakeDocumentSnapshot> documents)
        {
            _documents = documents;
        }

        public FakeQuery OrderByDescending(string field)
        {
            return new FakeQuery(_documents.OrderByDescending(doc => doc.GetFieldValue(field)).ToList());
        }
    }

}
