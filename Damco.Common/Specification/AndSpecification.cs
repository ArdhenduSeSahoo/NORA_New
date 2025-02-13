using System;
using System.Linq.Expressions;

namespace Damco.Common.Specification
{
    public class AndSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> IsSatisfiedBy()
        {
            return _left.IsSatisfiedBy().AndAlso(_right.IsSatisfiedBy());
        }
    }
}