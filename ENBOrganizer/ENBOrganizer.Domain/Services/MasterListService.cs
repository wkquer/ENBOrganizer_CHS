using ENBOrganizer.Domain.Entities;
using ENBOrganizer.Domain.Exceptions;
using ENBOrganizer.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ENBOrganizer.Domain.Services
{
    public class MasterListService : DataService<MasterListItem>
    {
        private static List<MasterListItem> _defaultMasterListItems
        {
            get
            {
                return new List<MasterListItem>
                {
                    new MasterListItem("enbseries", MasterListItemType.文件夹),
                    new MasterListItem("injFX_Shaders", MasterListItemType.文件夹),
                    new MasterListItem("SweetFX", MasterListItemType.文件夹),
                    new MasterListItem("d3d9_smaa.dll", MasterListItemType.文件),
                    new MasterListItem("d3d9.fx", MasterListItemType.文件),
                    new MasterListItem("d3d9injFX.dll", MasterListItemType.文件),
                    new MasterListItem("d3d9SFX.dll", MasterListItemType.文件),
                    new MasterListItem("dxgi.dll", MasterListItemType.文件),
                    new MasterListItem("dxgi.fx", MasterListItemType.文件),
                    new MasterListItem("effect.txt", MasterListItemType.文件),
                    new MasterListItem("enbbloom.fx", MasterListItemType.文件),
                    new MasterListItem("enbeffect.fx", MasterListItemType.文件),
                    new MasterListItem("enbeffectprepass.fx", MasterListItemType.文件),
                    new MasterListItem("enblens.fx", MasterListItemType.文件),
                    new MasterListItem("enblensmask.png", MasterListItemType.文件),
                    new MasterListItem("enblocal.ini", MasterListItemType.文件),
                    new MasterListItem("enbpalette.bmp", MasterListItemType.文件),
                    new MasterListItem("enbraindrops.tga", MasterListItemType.文件),
                    new MasterListItem("enbraindrops_small.tga", MasterListItemType.文件),
                    new MasterListItem("enbseries.ini", MasterListItemType.文件),
                    new MasterListItem("enbsunsprite.fx", MasterListItemType.文件),
                    new MasterListItem("enbsunsprite.tga", MasterListItemType.文件),
                    new MasterListItem("FXAA_Tool.exe", MasterListItemType.文件),
                    new MasterListItem("injector.ini", MasterListItemType.文件),
                    new MasterListItem("shader.fx", MasterListItemType.文件),
                    new MasterListItem("SMAA.fx", MasterListItemType.文件),
                    new MasterListItem("SMAA.h", MasterListItemType.文件),
                    new MasterListItem("SweetFX readme.txt", MasterListItemType.文件),
                    new MasterListItem("SweetFX_preset.txt", MasterListItemType.文件),
                    new MasterListItem("SweetFX_settings.txt", MasterListItemType.文件),
                    new MasterListItem("d3d11.dll", MasterListItemType.文件),
                    new MasterListItem("d3dcompiler_46e.dll", MasterListItemType.文件),
                    new MasterListItem("enbadaptation.fx", MasterListItemType.文件),
                    new MasterListItem("enbdepthoffield.fx", MasterListItemType.文件),
                    new MasterListItem("enbeffectpostpass.fx", MasterListItemType.文件),
                };
            }
        }

        public override void Add(MasterListItem masterListItem)
        {
            if (FileSystemNames.EssentialNames.Any(name => name.EqualsIgnoreCase(masterListItem.Name)))
                return;

            base.Add(masterListItem);
        }

        public void CreateMasterListItems(DirectoryInfo directory)
        {
            foreach (FileSystemInfo fileSystemInfo in directory.GetFileSystemInfos())
            {
                MasterListItemType masterListItemType = fileSystemInfo is DirectoryInfo ? MasterListItemType.文件夹 : MasterListItemType.文件;

                MasterListItem masterListItem = new MasterListItem(fileSystemInfo.Name, masterListItemType);

                try
                {
                    Add(masterListItem);
                }
                catch (DuplicateEntityException) { }
            }
        }

        public void AddDefaultItems()
        {
            foreach (MasterListItem masterListItem in _defaultMasterListItems)
                Add(masterListItem);
        }
    }
}
