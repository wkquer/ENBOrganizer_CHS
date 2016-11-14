﻿using ENBOrganizer.App.Messages;
using ENBOrganizer.Domain.Entities;
using ENBOrganizer.Domain.Exceptions;
using ENBOrganizer.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ENBOrganizer.App.ViewModels.Master
{
    public class AddMasterListItemViewModel : DialogViewModelBase
    {
        private readonly MasterListService _masterListService;

        private MasterListItemType _selectedMasterListItemType;

        public MasterListItemType SelectedMasterListItemType
        {
            get { return _selectedMasterListItemType; }
            set { Set(nameof(SelectedMasterListItemType), ref _selectedMasterListItemType, value); }
        }

        public IEnumerable<MasterListItemType> MasterListItemTypes
        {
            get { return Enum.GetValues(typeof(MasterListItemType)).Cast<MasterListItemType>(); }
        }
        
        public AddMasterListItemViewModel(MasterListService masterListService)
        {
            _masterListService = masterListService;

            ValidatedProperties = new List<string> { nameof(Name) };
        }
        
        protected override void Save()
        {
            try
            {
                _masterListService.Add(new MasterListItem(Name.Trim(), SelectedMasterListItemType));
            }
            catch (DuplicateEntityException)
            {
                _dialogService.ShowErrorDialog("无法添加重复的主列表项。");
            }
            finally
            {
                Close();
            }
        }

        protected override void Close()
        {
            Name = string.Empty;

            _dialogService.CloseDialog(DialogName.AddMasterListItem);
        }

        protected override string GetValidationError(string propertyName)
        {
            return ValidateFileSystemName();
        }
    }
}
