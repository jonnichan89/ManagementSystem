namespace ManagementSystem.WPF.UI.Data
{
    public class LoginUser : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        // Helper method to trigger the event
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private string _UserName;

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        private string _UserPassword;

        public string UserPassword
        {
            get
            {
                return _UserPassword;
            }
            set
            {
                _UserPassword = value;
                OnPropertyChanged("UserPassword");
            }
        }
    }

    public class LoginUserResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string AccessList { get; set; }
    }


}
