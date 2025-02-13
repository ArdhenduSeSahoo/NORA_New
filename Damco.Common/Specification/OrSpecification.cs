using System;
using System.Linq.Expressions;

namespace Damco.Common.Specification
{
    public class OrSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> IsSatisfiedBy()
        {
            return _left.IsSatisfiedBy().OrElse(_right.IsSatisfiedBy());
        }
    }
}