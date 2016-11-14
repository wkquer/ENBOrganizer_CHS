using ENBOrganizer.App.Messages;
using ENBOrganizer.Domain.Entities;
using ENBOrganizer.Domain.Exceptions;
using ENBOrganizer.Domain.Services;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace ENBOrganizer.App.ViewModels.Binaries
{
    public class AddBinaryViewModel : DialogViewModelBase
    {
        private readonly FileSystemService<Binary> _binaryService;
        
        public ICommand BrowseForDirectoryCommand { get; set; }
        public ICommand BrowseForArchiveCommand { get; set; }

        private string _sourcePath;

        public string SourcePath
        {
            get { return _sourcePath; }
            set { Set(nameof(SourcePath), ref _sourcePath, value); }
        }

        public AddBinaryViewModel(FileSystemService<Binary> binaryService)
        {
            _binaryService = binaryService;

            BrowseForDirectoryCommand = new RelayCommand(BrowseForDirectory);
            BrowseForArchiveCommand = new RelayCommand(BrowseForArchive);

            ValidatedProperties = new List<string>
            {
                nameof(Name),
                nameof(SourcePath)
            };
        }

        private void BrowseForDirectory()
        {
            string path = _dialogService.ShowFolderBrowserDialog("请选择enb核心的文件夹...");

            if (string.IsNullOrWhiteSpace(path))
                return;

            SourcePath = path;
            Name = new DirectoryInfo(path).Name;
        }

        private void BrowseForArchive()
        {
            string archivePath = _dialogService.ShowOpenFileDialog("请选择档案文件", "ZIP 文件(*.zip) | *.zip");

            if (string.IsNullOrWhiteSpace(archivePath))
                return;

            SourcePath = archivePath;
            Name = Path.GetFileNameWithoutExtension(SourcePath);
        }

        protected override void Save()
        {
            try
            {
                _binaryService.Import(new Binary(Name.Trim(), _settingsService.CurrentGame), SourcePath);
            }
            catch (DuplicateEntityException)
            {
                _dialogService.ShowErrorDialog("当前游戏中已经存在具有此名称的enb核心。");
            }
            catch (Exception exception)
            {
                _dialogService.ShowErrorDialog(exception.Message);
            }
            finally
            {
                Close();
            }
        }

        protected override void Close()
        {
            Name = string.Empty;
            SourcePath = string.Empty;

            _dialogService.CloseDialog(DialogName.AddBinary);
        }

        protected override string GetValidationError(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Name):
                    return ValidateFileSystemName();
                case nameof(SourcePath):
                    return ValidatePath(SourcePath);
            }

            return string.Empty;
        }
    }
}