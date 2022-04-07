using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WpfApp1.model;

namespace WpfApp1.Service
{
    internal class UserDbService : INotifyPropertyChanged
    {
        public User User { get; set; }

        public int RecordNumber { get; private set; }
        public int TotalRecords { get; private set; }

        public UserDbService()
        {
            LoadInitialView();
        }

        private void LoadInitialView()
        {
            RecordNumber = 1;
            TotalRecords = GetTotalRecordFromDb();

            SelectFromDb();
        }

        public void Insert()
        {
            RecordMode = RecordMode.Insert;
            User = new User();

            NotifyPersonRecordChanged();
        }

        public void Update()
        {
            RecordMode = RecordMode.Update;
        }

        public void Save()
        {
            if (RecordMode == RecordMode.Insert)
            {
                InsertIntoDb();
                TotalRecords++;
                RecordNumber = TotalRecords;
            }
            else if (RecordMode == RecordMode.Update)
                UpdateInDb();
            else
                return;

            StopEditing();
        }

        public void StopEditing()
        {
            if (RecordMode == RecordMode.Insert)
                SelectFromDb();

            RecordMode = RecordMode.View;
        }

        private void UpdateInDb()
        {
            var query = $"UpdatePerson";
            using var connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            using var command = new SqlCommand(query, connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Username", User.Username);
            command.Parameters.AddWithValue("@Password", User.Password);
            command.Parameters.AddWithValue("@Age", User.Age);
            command.Parameters.AddWithValue("@Email", User.Email);

            connection.Open();

            command.ExecuteNonQuery();
        }

        private void InsertIntoDb()
        {
            var query = "InsertUser";

            using var connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            using var command = new SqlCommand(query, connection);

            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            command.Parameters.AddWithValue("@Username", User.Username);
            command.Parameters.AddWithValue("@Password", User.Password);
            command.Parameters.AddWithValue("@Age", User.Age);
            command.Parameters.AddWithValue("@Email", User.Email);

            command.ExecuteNonQuery();
        }

        private static int GetTotalRecordFromDb()
        {
            var query = $"select count(Username) from Form";
            using var connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            using var command = new SqlCommand(query, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            reader.Read();

            var recordCount = (int)reader[0];
            return recordCount;
        }

        public void SelectFromDb()
        {
            using var connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            using var command = new SqlCommand("SelectUser", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@RecordNumber", RecordNumber);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                User = new User
                {
                    Username = reader["Username"].ToString(),
                    Password = reader["Password"].ToString(),
                    Age = (int)reader["Age"],
                    Email = reader["Email"].ToString()
                };
            }
            else
                User = new User();

            NotifyPersonRecordChanged();
        }

        private void NotifyPersonRecordChanged()
        {
            OnPropertyChanged(nameof(User));
            OnPropertyChanged(nameof(RecordNumber));
            OnPropertyChanged(nameof(TotalRecords));
        }

        public void First()
        {
            if (RecordNumber != 1)
            {
                RecordNumber = 1;
                SelectFromDb();
            }
        }

        public void Previous()
        {
            if (RecordNumber > 1)
            {
                RecordNumber--;
                SelectFromDb();
            }
        }

        public void Next()
        {
            if (RecordNumber < TotalRecords)
            {
                RecordNumber++;
                SelectFromDb();
            }
        }

        public void Last()
        {
            if (RecordNumber != TotalRecords)
            {
                RecordNumber = TotalRecords;
                SelectFromDb();
            }
        }

        #region RecordViewState

        private RecordMode _recordMode = RecordMode.View;
        public RecordMode RecordMode
        {
            get
            {
                return _recordMode;
            }
            set
            {
                _recordMode = value;
                OnPropertyChanged(nameof(InputIsReadOnly));
                OnPropertyChanged(nameof(InputIsEditable));
            }
        }

        public bool InputIsReadOnly
        {
            get
            {
                return RecordMode != RecordMode.Insert && RecordMode != RecordMode.Update;
            }
        }

        public bool InputIsEditable => !InputIsReadOnly;

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }
}
