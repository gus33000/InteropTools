using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppPlugin
{
    public interface IPlugin<TIn, Tout, TProgress>
    {
        Task<Tout> ExecuteAsync(TIn input, IProgress<TProgress> progress = null, CancellationToken cancelTokem = default(CancellationToken));
    }

    public interface IPlugin<TIn, Tout, TOption, TProgress>
    {
        Task<Tout> ExecuteAsync(TIn input, TOption options, IProgress<TProgress> progress = null, CancellationToken cancelTokem = default(CancellationToken));

        Task<TOption> PrototypeOptions { get; }

    }
}
