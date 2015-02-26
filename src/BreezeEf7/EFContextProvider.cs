using System;
using System.Data;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Query;

namespace Breeze.ContextProvider.EF7
{

    public class EFContextProvider<T> : ContextProvider where T : DbContext
    {

        private T _context;

        public T Context
        {
            get
            {
                if (_context == null)
                {
                    _context = CreateContext();
                }
                return _context;
            }
        }

        protected virtual T CreateContext()
        {
            return null;
        }

        public override IDbConnection GetDbConnection()
        {
            return _context.Database.AsSqlServer().Connection.DbConnection;
        }

        protected override void OpenDbConnection()
        {
            var connection = GetDbConnection();
            if (connection.State == ConnectionState.Closed) connection.Open();
        }

        protected override void CloseDbConnection()
        {
            if (_context != null)
            {
                var c = GetDbConnection();
                c.Close();
                c.Dispose();
            }
        }

        public override object[] GetKeyValues(EntityInfo entityInfo)
        {
            return GetKeyValues(entityInfo.Entity);
        }

        public object[] GetKeyValues(object entity)
        {
            var entry = _context.Entry(entity);
            return entry.Metadata.GetPrimaryKey().Properties.Select(p => entry.Property(p.Name).CurrentValue).ToArray();
        }

        protected override string BuildJsonMetadata()
        {
            throw new NotImplementedException();
        }

        protected override void SaveChangesCore(SaveWorkState saveWorkState)
        {
            throw new NotImplementedException();
        }
    }
}
