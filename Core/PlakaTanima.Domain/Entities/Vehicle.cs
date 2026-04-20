using System;
using PlakaTanima.Domain.Enums;
namespace PlakaTanima.Domain.Entities;

public class Vehicle
{
    public string PlateNumber;
    public string Color;
    public string Model;
    public string Brand;
    public string Location;
    public DateTime FirstSeen;
    public VehiclePlateType PlateType;
}
