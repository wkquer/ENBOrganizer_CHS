﻿using ENBOrganizer.App.Messages;
using ENBOrganizer.Domain.Entities;
using ENBOrganizer.Domain.Services;

namespace ENBOrganizer.App.ViewModels.Master
{
    public class MasterListViewModel : PageViewModelBase<MasterListItem>
    {
        protected override DialogName DialogName { get { return DialogName.AddMasterListItem; } }
        
        public MasterListViewModel(MasterListService dataService) 
            : base(dataService) { }

        protected override bool CanAdd()
        {
            return true;
        }
    }
}
