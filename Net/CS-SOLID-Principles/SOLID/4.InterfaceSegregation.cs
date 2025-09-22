// ReSharper disable ALL

namespace SOLID;

// I - Interface Segregation Principle (ISP)
// The Interface Segregation Principle states that clients should not be forced to depend on interfaces they don't use.
// It's better to have many small, specific interfaces than one large, general-purpose interface.
// BAD EXAMPLE
public interface IActivity
{
    void Work();
    void Eat();
}

public class Human : IActivity
{
    public void Work() { }
    public void Eat() { }
}

public class Robot : IActivity
{
    public void Work() { }

    // This method is not applicable for Robot, violating the Interface Segregation Principle
    public void Eat() { throw new NotImplementedException(); }
}

// GOOD EXAMPLE
public interface IWork
{
    void Work();
}

public interface IEat
{
    void Eat();
}

public class Human2 : IWork, IEat
{
    public void Work() { }
    public void Eat() { }
}

public class Robot2 : IWork
{
    public void Work() { }
}