using ENBOrganizer.Util.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace ENBOrganizer.App.ViewModels
{
    // TODO: tons of repeated code in dialog ViewModels
    public abstract class DialogViewModelBase : ViewModelBase, IDataErrorInfo
    {
        private bool IsValid
        {
            get
            {
                if (ValidatedProperties == null)
                    return true;

                foreach (string property in ValidatedProperties)
                    if (GetValidationError(property) != string.Empty)
                        return false;

                return true;
            }
        }

        protected readonly DialogService _dialogService;
        protected readonly SettingsService _settingsService;
        protected List<string> ValidatedProperties { get; set; }
        
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        
        private string _name;

        public string Name
        {
            get { return _name; }
            set { Set(nameof(Name), ref _name, value); }
        }

        public string Error { get { throw new NotImplementedException(); } }

        public string this[string columnName] { get { return GetValidationError(columnName); } }

        public DialogViewModelBase() 
            : this(SimpleIoc.Default.GetInstance<DialogService>(), SimpleIoc.Default.GetInstance<SettingsService>()) { }

        public DialogViewModelBase(DialogService dialogService, SettingsService settingService)
        {
            _dialogService = dialogService;
            _settingsService = settingService;

            SaveCommand = new RelayCommand(Save, () => IsValid);
            CancelCommand = new RelayCommand(Close);
        }

        protected abstract string GetValidationError(string propertyName);
        protected abstract void Save();
        protected abstract void Close();

        protected string ValidateFileSystemName()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "名称不能为空。";

            if (!PathUtil.IsValidFileSystemName(Name))
                return "名称中包含有无效字符。";

            return string.Empty;
        }

        protected string ValidatePath(string path)
        {
            return (Directory.Exists(path) || File.Exists(path)) ? string.Empty : "文件夹/档案不存在。";
        }
    }
}