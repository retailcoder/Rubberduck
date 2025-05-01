using Rubberduck.Parsing.VBA;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.CodeAnalysis.Inspections
{
    public interface IInspector : IDisposable
    {
        Task<IEnumerable<IInspectionResult>> FindIssuesAsync(RubberduckParserState state, CancellationToken token);
        Task<IEnumerable<IInspectionResult>> FindIssuesAsync(RubberduckParserState state, CodeInspectionSeverity minSeverity, CancellationToken token);
    }
}
