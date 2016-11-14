﻿using ENBOrganizer.App.Messages;
using ENBOrganizer.Domain;
using ENBOrganizer.Domain.Entities;
using ENBOrganizer.Domain.Exceptions;
using ENBOrganizer.Domain.Services;
using ENBOrganizer.Util;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace ENBOrganizer.App.ViewModels.Presets
{
    public class AddPresetViewModel : DialogViewModelBase
    {
        private readonly PresetService _presetService;
        private readonly FileSystemService<Binary> _binaryService;
        
        public ObservableCollection<Binary> Binaries { get; set; }
        public ICommand BrowseForDirectoryCommand { get; set; }
        public ICommand BrowseForArchiveCommand { get; set; }

        private string _sourcePath;

        public string SourcePath
        {
            get { return _sourcePath; }
            set { Set(nameof(SourcePath), ref _sourcePath, value); }
        }

        private bool _isGlobalENBLocalEnabled;

        public bool IsGlobalENBLocalEnabled
        {
            get { return _isGlobalENBLocalEnabled; }
            set { Set(nameof(IsGlobalENBLocalEnabled), ref _isGlobalENBLocalEnabled, value); }
        }
        
        private string _description;

        public string Description
        {
            get { return _description; }
            set { Set(nameof(Description), ref _description, value); }
        }

        private Binary _binary;

        public Binary Binary
        {
            get { return _binary; }
            set { Set(nameof(Binary), ref _binary, value); }
        }
        
        public AddPresetViewModel(PresetService presetService, FileSystemService<Binary> binaryService)
        {
            _presetService = presetService;
            _binaryService = binaryService;
            _binaryService.ItemsChanged += _binaryService_ItemsChanged;
            _settingsService.PropertyChanged += _settingsService_PropertyChanged;

            BrowseForDirectoryCommand = new RelayCommand(BrowseForDirectory);
            BrowseForArchiveCommand = new RelayCommand(BrowseForArchive);
            
            ValidatedProperties = new List<string>
            {
                nameof(Name),
                nameof(SourcePath)
            };

            LoadBinaries();
        }

        private void _settingsService_PropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName.EqualsIgnoreCase("CurrentGame"))
                LoadBinaries();
        }

        private void LoadBinaries()
        {
            if (Binaries == null)
                Binaries = new ObservableCollection<Binary>();

            Binaries.Clear();
            Binaries.AddAll(_binaryService.GetByGame(_settingsService.CurrentGame));
        }

        private void _binaryService_ItemsChanged(object sender, RepositoryChangedEventArgs repositoryChangedEventArgs)
        {
            Binary binary = repositoryChangedEventArgs.Entity as Binary;

            if (!binary.Game.Equals(_settingsService.CurrentGame))
                return;

            if (repositoryChangedEventArgs.RepositoryActionType == RepositoryActionType.Added)
                Binaries.Add(repositoryChangedEventArgs.Entity as Binary);
            else
                Binaries.Remove(repositoryChangedEventArgs.Entity as Binary);
        }

        protected override void Save()
        {
            try
            {
                Preset preset = new Preset(Name.Trim(), _settingsService.CurrentGame)
                {
                    Description = Description?.Trim(),
                    IsGlobalENBLocalEnabled = IsGlobalENBLocalEnabled
                };

                // Detect whether the user has selected the default value in the ComboBox.
                if (Binary != null && Binary.Name != "-- None --" && Binary.Game != null)
                    preset.Binary = Binary;

                _presetService.Import(preset, SourcePath);
            }
            catch (DuplicateEntityException)
            {
                _dialogService.ShowErrorDialog("当前游戏已存在此名称的预设。");
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
            IsGlobalENBLocalEnabled = false;
            Description = string.Empty;
            Binary = null;

            _dialogService.CloseDialog(DialogName.AddPreset);
        }

        private void BrowseForDirectory()
        {
            string path = _dialogService.ShowFolderBrowserDialog("请选择预设的文件夹...");

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
