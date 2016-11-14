﻿using ENBOrganizer.App.Messages;
using ENBOrganizer.Domain.Entities;
using ENBOrganizer.Domain.Services;
using ENBOrganizer.Util;
using System.Linq;

namespace ENBOrganizer.App.ViewModels.Binaries
{
    public class BinariesViewModel : FileSystemViewModel<Binary>
    {
        private readonly PresetService _presetService;
        protected override DialogName DialogName { get { return DialogName.AddBinary; } }
        protected override string DialogHostName { get { return "RenameBinaryDialog"; } }

        public BinariesViewModel(FileSystemService<Binary> binaryService, PresetService presetService)
            : base(binaryService)
        {
            _presetService = presetService;
        }

        protected override async void Edit(Binary binary)
        {
            string newName = ((string)await _dialogService.ShowInputDialog("请输入enb核心的名称：", DialogHostName, binary.Name)).Trim();

            if (!ShouldEdit(binary, newName))
                return;
            
            foreach (Preset preset in _presetService.Items.Where(preset => binary.Equals(preset.Binary)))
                preset.Binary = new Binary(newName, SettingsService.CurrentGame);

            _presetService.SaveChanges();            

            DataService.Rename(binary, newName);
        }

        private bool ShouldEdit(Binary binary, string newName)
        {
            if (binary.Name.EqualsIgnoreCase(newName))
                return false;

            if (DataService.GetByGame(SettingsService.CurrentGame).Any(b => b.Name.EqualsIgnoreCase(newName)))
            {
                _dialogService.ShowErrorDialog("具有此名称的enb核心已存在。");
                return false;
            }
                        
            return true;
        }
    }
}