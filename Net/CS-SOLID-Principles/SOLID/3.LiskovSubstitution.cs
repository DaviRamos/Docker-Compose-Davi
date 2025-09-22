// ReSharper disable ALL

namespace SOLID;

// L - Liskov Substitution Principle (LSP)
// The Liskov Substitution Principle states that objects of a superclass should be replaceable
// with objects of a subclass without breaking the application.
public class Bird
{
    public virtual void Eat()
    {
        Console.WriteLine($"Bird is eating.");
    }
    public virtual void Fly()
    {
        Console.WriteLine("Flying");
    }
}

public class Ostrich : Bird
{
    public override void Fly()
    {
        // This violates LSP because Ostrich cannot fly
        throw new Exception("Ostriches can't fly!");
    }
}

// GOOD EXAMPLE
public class Bird2
{
    public virtual void Eat()
    {
        Console.WriteLine($"Bird is eating.");
    }
}

public interface IFlyingBird
{
    void Fly();
}

public class Sparrow : Bird2, IFlyingBird
{
    public void Fly()
    {
        Console.WriteLine("Sparrow flies");
    }
}

public class Ostrich2 : Bird2
{
    // No Fly method, as ostrich can't fly
}