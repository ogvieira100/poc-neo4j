using Api.Models;
using Neo4j.Driver;
using System.Linq.Expressions;

namespace Api.Util
{

    public class Condition
    {
        public string Campo { get; set; }
        public object? Valor { get; set; }
        public ExpressionType Expressao { get; set; }
    }
    public static class UtilClass
    {

        public static Expression<Func<T, bool>> AddConditionEqual<T>(Expression<Func<T, bool>> predicate, string fieldName, string value) where T : class
        {
            // Acessa a propriedade do objeto
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, fieldName);

            // Cria a expressão para a condição
            var constant = Expression.Constant(value);
            var condition = Expression.Equal(property, constant);

            // Combina a condição com o predicado existente
            var combined = Expression.AndAlso(predicate.Body, condition);

            // Retorna uma nova expressão com a condição adicionada
            return Expression.Lambda<Func<T, bool>>(combined, predicate.Parameters[0]);
        }

        public static DateTime ConvertZonedDateTimeToDateTime(ZonedDateTime zonedDateTime)
        {
            // Obtenha o DateTimeOffset do ZonedDateTime
            DateTimeOffset dateTimeOffset = zonedDateTime.ToDateTimeOffset();

            // Converta para DateTime
            DateTime dateTime = dateTimeOffset.UtcDateTime; // Ou .LocalDateTime para a hora local

            return dateTime;
        }

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
                    Valor = constantExpression.Value,
                    Expressao = binaryExpression.NodeType
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

