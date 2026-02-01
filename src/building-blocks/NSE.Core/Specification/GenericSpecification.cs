using System.Linq.Expressions;

namespace NSE.Core.Specification;

internal sealed class GenericSpecification<T>
{
    private Expression<Func<T, bool>> Expression { get; }

    public GenericSpecification(Expression<Func<T, bool>> expression)
    {
        Expression = expression;
    }

    public bool IsSatisfiedBy(T entity)
    {
        return Expression.Compile().Invoke(entity);
    }
}