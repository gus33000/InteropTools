using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppPlugin
{
    public interface IPlugin<TIn, Tout, TProgress>
    {
        Task<Tout> ExecuteAsync(TIn input, IProgress<TProgress> progress = null, CancellationToken cancelTokem = default);
    }

    public interface IPlugin<TIn, Tout, TOption, TProgress>
    {
        Task<Tout> ExecuteAsync(TIn input, TOption options, IProgress<TProgress> progress = null, CancellationToken cancelTokem = default);

        Task<TOption> PrototypeOptions { get; }

    }
}
