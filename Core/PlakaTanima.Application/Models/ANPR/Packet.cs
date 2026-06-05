
namespace PlakaTanima.Application.Models.ANPR;

public sealed class Packet
{
    public PacketType Type { get; init; }

    public byte[] Data { get; init; } = [];
}