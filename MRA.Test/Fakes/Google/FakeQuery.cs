using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.WebApi.Test.Fakes.Google
{
    public class FakeQuery
    {
        private readonly List<FakeDocumentSnapshot> _documents;

        public FakeQuery(List<FakeDocumentSnapshot> documents)
        {
            _documents = documents;
        }

        public Task<FakeQuerySnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
        {
            var snapshot = new FakeQuerySnapshot(_documents);
            return Task.FromResult(snapshot);
        }
    }

}
