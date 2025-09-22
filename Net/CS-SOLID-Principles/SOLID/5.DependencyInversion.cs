// ReSharper disable ALL

namespace SOLID;

// D - Dependency Inversion Principle (DIP)
// The Dependency Inversion Principle has two parts:
//    1. High-level modules should not depend on low-level modules.Both should depend on abstractions.
//    2. Abstractions should not depend on details. Details should depend on abstractions.
// BAD EXAMPLE
public class LightBulb
{
    public void TurnOn() => Console.WriteLine("LightBulb On");
}

public class Switch
{
    private readonly LightBulb _bulb = new LightBulb();
    public void Operate()
    {
        _bulb.TurnOn();
    }
}

// GOOD EXAMPLE
public interface IDevice
{
    void TurnOn();
}

public class LightBulb2 : IDevice
{
    public void TurnOn() => Console.WriteLine("LightBulb On");
}

public class Switch2
{
    private readonly IDevice _device;
    public Switch2(IDevice device)
    {
        _device = device;
    }
    public void Operate()
    {
        _device.TurnOn();
    }
}