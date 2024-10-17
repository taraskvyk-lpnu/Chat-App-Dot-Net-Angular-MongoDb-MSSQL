using User.Management.API.DataAccess.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace User.Management.API.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository UserRepository { get; }
        private readonly UsersDbContext context;
        private IDbContextTransaction _currentTransaction;

        public UnitOfWork(UsersDbContext context, IUserRepository userRepository)
        {
            this.context = context;
            UserRepository = userRepository;
        }
        
        public async Task<int> SaveChangesAsync()
        {
            int result = 0;

            if (_currentTransaction == null)
            {
                _currentTransaction = await context.Database.BeginTransactionAsync();
            }

            try
            {
                result = await context.SaveChangesAsync();

                await _currentTransaction.CommitAsync();
            }
            catch (Exception e)
            {
                await RollbackTransactionAsync();
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }

            return result;
        }

        private async Task RollbackTransactionAsync()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while trying to rollback the transaction: " + ex.Message);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
            }

            context.Dispose();
        }
    }
}