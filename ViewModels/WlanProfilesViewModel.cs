using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using NetworkSharp.Services;

namespace NetworkSharp.ViewModels
{
    /// <summary>
    /// ViewModel for the WLAN Profiles feature
    /// </summary>
    public class WlanProfilesViewModel : INotifyPropertyChanged
    {
        private readonly IWlanProfileService _wlanProfileService;
        private bool _hasAdminPrivileges;


        public ObservableCollection<WlanProfile> Profiles { get; set; }
        
        public WlanProfilesViewModel(IWlanProfileService wlanProfileService)
        {
            _wlanProfileService = wlanProfileService;
            
            Profiles = new ObservableCollection<WlanProfile>();
            _hasAdminPrivileges = _wlanProfileService.HasAdministratorPrivileges();
            
            // Load profiles asynchronously
            _ = LoadProfilesAsync();
        }

        public async Task LoadProfilesAsync()
        {
            IsLoading = true;
            
            try
            {
                var profiles = await _wlanProfileService.GetProfilesAsync();
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Profiles.Clear();
                    foreach (var profile in profiles)
                    {
                        Profiles.Add(profile);
                    }
                });
            }
            catch
            {
                // Handle error
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Profiles.Clear();
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task ShowPasswordAsync()
        {
            if (SelectedProfile == null)
                return;
                
            try
            {
                var password = await _wlanProfileService.GetProfilePasswordAsync(SelectedProfile.Name);
                Password = password;
                ShowPassword = true;
            }
            catch (UnauthorizedAccessException)
            {
                Password = "Administrator-Rechte erforderlich";
                ShowPassword = true;
            }
            catch (Exception)
            {
                Password = "Fehler beim Laden des Passworts";
                ShowPassword = true;
            }
        }

        public void HidePassword()
        {
            Password = "";
            ShowPassword = false;
        }

        public async Task CopyPasswordAsync()
        {
            if (string.IsNullOrEmpty(Password) || Password == "Administrator-Rechte erforderlich")
                return;
                
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Clipboard.SetText(Password);
                });
            }
            catch
            {
                // Clipboard copy failed
            }
        }

        public async Task DeleteProfileAsync()
        {
            if (SelectedProfile == null)
                return;
                
            try
            {
                var success = await _wlanProfileService.DeleteProfileAsync(SelectedProfile.Name);
                if (success)
                {
                    Profiles.Remove(SelectedProfile);
                    SelectedProfile = null;
                    Password = "";
                    ShowPassword = false;
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Show error message
            }
            catch
            {
                // Show error message
            }
        }

        public async Task RefreshSignalStrengthAsync()
        {
            if (SelectedProfile == null)
                return;
                
            try
            {
                var signalStrength = await _wlanProfileService.GetSignalStrengthAsync(SelectedProfile.Name);
                SelectedProfile.SignalStrength = signalStrength;
            }
            catch
            {
                // Ignore errors
            }
        }

        public bool IsLoading
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public bool HasAdminPrivileges
        {
            get => _hasAdminPrivileges;
            set
            {
                _hasAdminPrivileges = value;
                OnPropertyChanged();
            }
        }

        public WlanProfile? SelectedProfile
        {
            get;
            set
            {
                field = value;
                Password = "";
                ShowPassword = false;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "";

        public bool ShowPassword
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
