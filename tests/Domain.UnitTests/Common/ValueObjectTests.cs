using Domain.Common;

namespace Domain.UnitTests.Common;

public abstract class ValueObjectTests<T> where T : ValueObject
{
    protected abstract T CreateInstance();
    protected abstract T CreateDifferentInstance();
    protected abstract (T first, T second) CreateEqualInstances();

    #region Equality Tests

    [Fact]
    public void Equals_EqualObjects_ReturnsTrue()
    {
        var (first, second) = CreateEqualInstances();
        
        first.Should().Be(second);
        (first == second).Should().BeTrue();
        (first != second).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentObjects_ReturnsFalse()
    {
        var first = CreateInstance();
        var second = CreateDifferentInstance();
        
        first.Should().NotBe(second);
        (first == second).Should().BeFalse();
        (first != second).Should().BeTrue();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var instance = CreateInstance();
        
        instance.Equals(null).Should().BeFalse();
        (instance == null).Should().BeFalse();
        (instance != null).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var instance = CreateInstance();
        
        instance.Equals("string").Should().BeFalse();
        instance.Equals(123).Should().BeFalse();
        instance.Equals(new object()).Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldBeReflexive()
    {
        var instance = CreateInstance();
        
        instance.Equals(instance).Should().BeTrue();
        instance.Should().Be(instance);
        (instance == instance).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldBeSymmetric()
    {
        var (first, second) = CreateEqualInstances();
        
        first.Equals(second).Should().Be(second.Equals(first));
        (first == second).Should().Be(second == first);
    }

    [Fact]
    public void Equals_ShouldBeTransitive()
    {
        var (first, _) = CreateEqualInstances();
        var (second, third) = CreateEqualInstances();
        
        if (first.Equals(second) && second.Equals(third))
        {
            first.Equals(third).Should().BeTrue();
        }
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    public void GetHashCode_EqualObjects_HasSameHashCode()
    {
        var (first, second) = CreateEqualInstances();
        
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentObjects_HasDifferentHashCode()
    {
        var first = CreateInstance();
        var second = CreateDifferentInstance();
        
        first.GetHashCode().Should().NotBe(second.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeStableAcrossCalls()
    {
        var instance = CreateInstance();
        
        var firstHash = instance.GetHashCode();
        var secondHash = instance.GetHashCode();
        
        firstHash.Should().Be(secondHash);
    }

    #endregion

    #region CompareTo Tests

    [Fact]
    public void CompareTo_EqualObjects_ReturnsZero()
    {
        var (first, second) = CreateEqualInstances();
        
        first.CompareTo(second).Should().Be(0);
        second.CompareTo(first).Should().Be(0);
    }

    [Fact]
    public void CompareTo_SameInstance_ReturnsZero()
    {
        var instance = CreateInstance();
        
        instance.CompareTo(instance).Should().Be(0);
    }

    [Fact]
    public void CompareTo_Null_ReturnsPositive()
    {
        var instance = CreateInstance();
        
        instance.CompareTo(null).Should().BePositive();
        (instance > null!).Should().BeTrue();
        (instance >= null!).Should().BeTrue();
        (instance < null!).Should().BeFalse();
        (instance <= null!).Should().BeFalse();
    }

    [Fact]
    public void CompareTo_DifferentType_ThrowsArgumentException()
    {
        var instance = CreateInstance();
        
        instance.Invoking(x => x.CompareTo("string"))
            .Should().Throw<ArgumentException>()
            .WithMessage($"*{instance.GetType().Name}*");
        
        instance.Invoking(x => x.CompareTo(123))
            .Should().Throw<ArgumentException>();
        
        instance.Invoking(x => x.CompareTo(new object()))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CompareTo_DifferentObjects_ReturnsCorrectComparison()
    {
        var first = CreateInstance();
        var second = CreateDifferentInstance();
        var comparison = first.CompareTo(second);
        
        if (comparison < 0)
        {
            first.Should().BeLessThan(second);
            second.Should().BeGreaterThan(first);
            
            (first < second).Should().BeTrue();
            (first > second).Should().BeFalse();
            (first <= second).Should().BeTrue();
            (first >= second).Should().BeFalse();
            
            (second > first).Should().BeTrue();
            (second < first).Should().BeFalse();
            (second >= first).Should().BeTrue();
            (second <= first).Should().BeFalse();
        }
        else if (comparison > 0)
        {
            first.Should().BeGreaterThan(second);
            second.Should().BeLessThan(first);
            
            (first < second).Should().BeFalse();
            (first > second).Should().BeTrue();
            (first <= second).Should().BeFalse();
            (first >= second).Should().BeTrue();
            
            (second > first).Should().BeFalse();
            (second < first).Should().BeTrue();
            (second >= first).Should().BeFalse();
            (second <= first).Should().BeTrue();
        }
    }

    [Fact]
    public void CompareTo_ShouldBeTransitive()
    {
        var first = CreateInstance();
        var second = CreateDifferentInstance();
        var third = CreateDifferentInstance();
        
        if (first.CompareTo(second) < 0 && second.CompareTo(third) < 0)
        {
            first.CompareTo(third).Should().BeNegative();
            first.Should().BeLessThan(third);
        }
        else if (first.CompareTo(second) > 0 && second.CompareTo(third) > 0)
        {
            first.CompareTo(third).Should().BePositive();
            first.Should().BeGreaterThan(third);
        }
    }

    #endregion

    #region CompareTo Object Overload Tests

    [Fact]
    public void CompareTo_ObjectOverload_WithNull_ReturnsPositive()
    {
        var instance = CreateInstance();
        
        instance.CompareTo((object?)null).Should().BePositive();
    }

    [Fact]
    public void CompareTo_ObjectOverload_WithWrongType_ThrowsArgumentException()
    {
        var instance = CreateInstance();
        
        instance.Invoking(x => x.CompareTo((object)"string"))
            .Should().Throw<ArgumentException>()
            .WithMessage($"*{instance.GetType().Name}*");
        
        instance.Invoking(x => x.CompareTo((object)123))
            .Should().Throw<ArgumentException>();
        
        instance.Invoking(x => x.CompareTo((object)new object()))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CompareTo_ObjectOverload_WithValueObject_ReturnsSameAsTypedCompareTo()
    {
        var instance = CreateInstance();
        var different = CreateDifferentInstance();
        
        var expected = instance.CompareTo(different);
        instance.CompareTo((object)different).Should().Be(expected);
    }

    [Fact]
    public void CompareTo_ObjectOverload_WithEqualValueObject_ReturnsZero()
    {
        var (first, second) = CreateEqualInstances();
        
        first.CompareTo((object)second).Should().Be(0);
        second.CompareTo((object)first).Should().Be(0);
    }

    [Fact]
    public void CompareTo_ObjectOverload_WithSameInstance_ReturnsZero()
    {
        var instance = CreateInstance();
        
        instance.CompareTo((object)instance).Should().Be(0);
    }

    #endregion

    #region Operator Overloads Tests

    [Fact]
    public void OperatorOverloads_ShouldBeConsistentWithEquals()
    {
        var (first, second) = CreateEqualInstances();
        
        (first == second).Should().Be(first.Equals(second));
        (first != second).Should().Be(!first.Equals(second));
    }

    [Fact]
    public void OperatorOverloads_ShouldBeConsistentWithCompareTo()
    {
        var first = CreateInstance();
        var second = CreateDifferentInstance();
        var comparison = first.CompareTo(second);
        
        if (comparison < 0)
        {
            (first < second).Should().BeTrue();
            (first > second).Should().BeFalse();
            (first <= second).Should().BeTrue();
            (first >= second).Should().BeFalse();
        }
        else if (comparison > 0)
        {
            (first < second).Should().BeFalse();
            (first > second).Should().BeTrue();
            (first <= second).Should().BeFalse();
            (first >= second).Should().BeTrue();
        }
        else
        {
            (first == second).Should().BeTrue();
            (first != second).Should().BeFalse();
            (first < second).Should().BeFalse();
            (first > second).Should().BeFalse();
            (first <= second).Should().BeTrue();
            (first >= second).Should().BeTrue();
        }
    }

    #endregion
}