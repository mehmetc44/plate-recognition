using System;
using System.Runtime.CompilerServices;

namespace PlakaTanima.Application.Abstract.Services;
public interface ICameraService
{
    IAsyncEnumerable<byte[]> GetLiveStreamAsync([EnumeratorCancellation] CancellationToken ct);
}
