using System;
using System.Linq.Expressions;

namespace Damco.Common.Specification
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> IsSatisfiedBy();
        Func<T, bool> IsSatisfiedByCompile();
        Expression<Func<T, bool>> Predicate { get; }
        ISpecification<T> And(ISpecification<T> specification);
        ISpecification<T> Or(ISpecification<T> specification);
        ISpecification<T> Not(ISpecification<T> specification);
    }
}