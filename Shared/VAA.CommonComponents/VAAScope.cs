using System;
using System.Transactions;

namespace VAA.CommonComponents
{
    /// <summary>
    /// Transaction Scope
    /// </summary>
    public class VAAScope : IDisposable
    {
        private TransactionScope scope;

        public VAAScope()
        {
            scope = new TransactionScope();
        }

        public void Complete()
        {
            scope.Complete();
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
