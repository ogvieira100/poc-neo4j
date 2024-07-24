using System.Linq.Expressions;

namespace Api.Util
{

    public class Condition
    {
        public string Campo { get; set; }
        public string Valor { get; set; }
        public string Expressao { get; set; }
    }
    public static class UtilClass
    {

        public static List<Condition> ExtractConditions<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            var conditions = new List<Condition>();
            if (predicate.Body is BinaryExpression binaryExpression)
            {
                ExtractBinaryExpression(binaryExpression, conditions);
            }
            return conditions;
        }
        private static void ExtractBinaryExpression(BinaryExpression binaryExpression, List<Condition> conditions)
        {
            if (binaryExpression.Left is MemberExpression memberExpression &&
                binaryExpression.Right is ConstantExpression constantExpression)
            {
                conditions.Add(new Condition
                {
                    Campo = memberExpression.Member.Name,
                    Valor = constantExpression.Value?.ToString(),
                    Expressao = binaryExpression.NodeType.ToString()
                });
            }
            else if (binaryExpression.Left is BinaryExpression leftBinary)
            {
                ExtractBinaryExpression(leftBinary, conditions);
            }

            if (binaryExpression.Right is BinaryExpression rightBinary)
            {
                ExtractBinaryExpression(rightBinary, conditions);
            }
        }
    }
}

