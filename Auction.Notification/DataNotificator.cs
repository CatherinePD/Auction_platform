using System;
using System.Threading.Tasks;
using Auction.Notification.EventsModels;
using SqlDependency.Core;

namespace Auction.Notification
{
    public sealed class DataNotificator: IDataNotificator
    {
        public event EventHandler<DataUpdatedEventArgs> OnDataUpdated;

        private readonly SqlDependencyEx _sqlDependency;
        private static string _connectionString;
        private static string _databaseName;
        private static string _tableName;
        private static int _identity;

        private DataNotificator()
        {
            _sqlDependency = new SqlDependencyEx(_connectionString, _databaseName, _tableName,
                    listenerType:SqlDependencyEx.NotificationTypes.Insert,
                    identity:_identity);
            _sqlDependency.TableChanged += SqlDependencyOnTableChanged;
        }

        private static Lazy<DataNotificator> _instance = new Lazy<DataNotificator>(() => new DataNotificator());

        public static DataNotificator Instance => _instance.Value;

        private void SqlDependencyOnTableChanged(object sender, SqlDependencyEx.TableChangedEventArgs e)
        {
            OnDataUpdated?.Invoke(this, new DataUpdatedEventArgs
            {
                Action = (ActionType)e.NotificationType,
                Data = e.Data
            });
        }

        public async Task Listen()
        {
            await _sqlDependency.Start();
        }

        public void Stop()
        {
            _sqlDependency.Stop();
            _instance = null;
            _instance = new Lazy<DataNotificator>(() => new DataNotificator());
        }

        public static void Register(string connectionString, string databaseName, string tableName, int identity = 1)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _tableName = tableName;
            _identity = identity;
        }
    }
}
