﻿using ENBOrganizer.App.Messages;
using ENBOrganizer.Domain;
using ENBOrganizer.Domain.Entities;
using ENBOrganizer.Domain.Services;
using ENBOrganizer.Util;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ENBOrganizer.App.ViewModels
{
    public abstract class PageViewModelBase<TEntity> : ViewModelBase, IPageViewModel where TEntity : EntityBase
    {
        protected virtual DataService<TEntity> DataService { get; set; }
        protected readonly DialogService _dialogService;
        protected abstract DialogName DialogName { get; }
        
        public ObservableCollection<TEntity> Models { get; set; }
        public SettingsService SettingsService { get; set; }
        public ICommand OpenAddDialogCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public PageViewModelBase(DataService<TEntity> dataService)
            : this(dataService, SimpleIoc.Default.GetInstance<DialogService>(), SimpleIoc.Default.GetInstance<SettingsService>()) { }
        
        public PageViewModelBase(DataService<TEntity> dataService, DialogService dialogService, SettingsService settingsService)
        {
            DataService = dataService;
            DataService.ItemsChanged += _dataService_ItemsChanged;

            _dialogService = dialogService;
            SettingsService = settingsService;

            OpenAddDialogCommand = new RelayCommand(() => _dialogService.ShowDialog(DialogName), CanAdd);
            DeleteCommand = new RelayCommand<TEntity>(entity => DataService.Delete(entity));

            Models = new ObservableCollection<TEntity>();

            PopulateModels();
        }

        protected abstract bool CanAdd();

        protected virtual void PopulateModels()
        {
            Models.Clear();
            Models.AddAll(DataService.Items);
        }

        protected virtual void _dataService_ItemsChanged(object sender, RepositoryChangedEventArgs eventArgs)
        {
            TEntity entity = eventArgs.Entity as TEntity;

            if (eventArgs.RepositoryActionType == RepositoryActionType.Added)
                Models.Add(entity);
            else
                Models.Remove(entity);
        }
    }
}
