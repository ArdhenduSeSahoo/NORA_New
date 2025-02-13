using System;
using System.Linq.Expressions;

namespace Damco.Common.Specification
{
    public class NotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _specification;

        public NotSpecification(ISpecification<T> specification)
        {
            _specification = specification;
        }

        public override Expression<Func<T, bool>> IsSatisfiedBy()
        {
            var isSatisfiedBy = _specification.IsSatisfiedBy();
            return Expression.Lambda<Func<T, bool>>(Expression.Not(isSatisfiedBy.Body), isSatisfiedBy.Parameters);
        }
    }
}