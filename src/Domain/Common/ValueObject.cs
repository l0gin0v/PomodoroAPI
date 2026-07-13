namespace Domain.Common;

public abstract class ValueObject : IEquatable<ValueObject>, IComparable<ValueObject>
{
    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null)
            return false;
        
        return left?.Equals(right!) != false;
    }

    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
        => !EqualOperator(left, right);

    protected abstract IEnumerable<object> GetEqualityComponents();
    
    protected virtual IEnumerable<IComparable> GetComparisonComponents() 
        => GetEqualityComponents().OfType<IComparable>();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public bool Equals(ValueObject? other)
        => Equals((object?)other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var component in GetEqualityComponents())
            hash.Add(component);
        
        return hash.ToHashCode();
    }

    public int CompareTo(ValueObject? other)
    {
        if (other is null) return 1;
        if (ReferenceEquals(this, other)) return 0;
        if (GetType() != other.GetType()) 
            throw new ArgumentException($"Cannot compare {GetType().Name} with {other.GetType().Name}");

        var thisComponents = GetComparisonComponents().ToList();
        var otherComponents = other.GetComparisonComponents().ToList();
        
        for (var i = 0; i < Math.Min(thisComponents.Count, otherComponents.Count); i++)
        {
            var comparison = thisComponents[i].CompareTo(otherComponents[i]);
            if (comparison != 0) return comparison;
        }
        
        return thisComponents.Count.CompareTo(otherComponents.Count);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is not ValueObject other)
            throw new ArgumentException($"Object must be of type {GetType().Name}", nameof(obj));
        
        return CompareTo(other);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
        => EqualOperator(left, right);

    public static bool operator !=(ValueObject? left, ValueObject? right)
        => NotEqualOperator(left, right);
    
    public static bool operator <(ValueObject? left, ValueObject? right)
        => left is null ? right is not null : left.CompareTo(right) < 0;
    
    public static bool operator <=(ValueObject? left, ValueObject? right)
        => left is null || left.CompareTo(right) <= 0;
    
    public static bool operator >(ValueObject? left, ValueObject? right)
        => left is not null && left.CompareTo(right) > 0;
    
    public static bool operator >=(ValueObject? left, ValueObject? right)
        => left is null ? right is null : left.CompareTo(right) >= 0;
}