using System;

namespace PlakaTanima.Domain.DTO.Camera;

public class CreateCameraDto
{
    public string Name { get; set; } = default!;
    public string GateName { get; set; } = default!;
    public string Ip { get; set; } = default!;
    public int Port { get; set; } = 554;
    public int StreamChannel { get; set; } = 1;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
