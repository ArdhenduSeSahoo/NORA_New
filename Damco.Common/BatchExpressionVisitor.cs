using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public class BatchExpressionVisitor : ExpressionVisitor
    {
        private ExpressionType? previousType = null;
        private List<int> EntityKeys = new List<int>();
        private int OperationMode = 0;
        private bool FoundIds = false;
        /// <summary>
        /// 0-To retrieve list if unique value
        /// 1-Apply inserted list of unique value to expression
        /// -1 - default do nothing
        /// </summary>
        /// <param name="OperationMode"></param>
        public BatchExpressionVisitor(int OperationMode = -1)
        {
            this.OperationMode = OperationMode;
        }

        public List<int> EntityUniqueKeys { get { return EntityKeys; } set { EntityKeys = value; } }

        public override Expression Visit(Expression node)
        {
            //Debug.WriteLine(node?.ToString() + "---" + node?.NodeType.ToString());

            if (node?.NodeType == ExpressionType.Constant && previousType == ExpressionType.Call && !FoundIds)
            {

                if (this.OperationMode == 0)
                {
                    EntityKeys = (List<int>)node.GetConstantValue();
                    FoundIds = true;
                    if (node != null) { previousType = node?.NodeType; }
                    return base.Visit(node);
                }
                else if (this.OperationMode == 1)
                {
                    Expression newexpression = Expression.Constant(EntityKeys);
                    FoundIds = true;
                    previousType = node?.NodeType;
                    return newexpression;
                }
                else
                {
                    if (node != null) { previousType = node?.NodeType; }
                    return base.Visit(node);
                }
            }
            else
            {
                if (node != null) { previousType = node?.NodeType; }
                return base.Visit(node);
            }
        }
    }
}
