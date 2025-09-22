// ReSharper disable ALL

namespace SOLID;

// O - Open/Closed Principle (OCP)
// The Open/Closed Principle states that classes should be open for extension but closed for modification.
// You should be able to add new functionality without changing existing code.
// BAD EXAMPLE
public class DiscountCalculator
{
	public double CalculateDiscount(string customerType)
	{
		if (customerType == "Regular") return 0.1;
		if (customerType == "Premium") return 0.2;
		return 0.0;
	}
}

// GOOD EXAMPLE
public interface IDiscount
{
	double GetDiscount();
}

public class RegularCustomer : IDiscount
{
	public double GetDiscount() => 0.1;
}

public class PremiumCustomer : IDiscount
{
	public double GetDiscount() => 0.2;
}

// New extension without modifying existing code
public class UltimateCustomer : IDiscount
{
	public double GetDiscount() => 0.3;
}

public class DiscountCalculator2
{
	public double CalculateDiscount(IDiscount customer)
	{
		return customer.GetDiscount();
	}
}

