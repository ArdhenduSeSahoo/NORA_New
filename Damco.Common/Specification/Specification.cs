using System;
using System.Linq.Expressions;

namespace Damco.Common.Specification
{
    public abstract class Specification<T> : ISpecification<T>
    {
        protected readonly Expression<Func<T, bool>> Default = x => true;

        protected Expression<Func<T, bool>> predicate;

        protected Specification()
        {
            predicate = Default;
        }

        public virtual Expression<Func<T, bool>> IsSatisfiedBy()
        {
            return Expression.Lambda<Func<T, bool>>(predicate.Body, predicate.Parameters);
        }

        public virtual Func<T, bool> IsSatisfiedByCompile()
        {
            return Expression.Lambda<Func<T, bool>>(predicate.Body, predicate.Parameters).Compile();
        }

        public Expression<Func<T, bool>> Predicate => predicate;

        public ISpecification<T> And(ISpecification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        public ISpecification<T> Or(ISpecification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        public ISpecification<T> Not(ISpecification<T> specification)
        {
            return new NotSpecification<T>(specification);
        } 
    }
}