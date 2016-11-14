﻿namespace ENBOrganizer.App.Messages
{
    public enum DialogName
    {
        GameDetail,
        AddMasterListItem,
        AddBinary,
        AddPreset,
        EditPreset,
        GlobalEnbLocal,
        AddENBoostPreset,
        Input
    }

    public enum DialogAction
    {
        Open,
        Close
    }

    public class DialogMessage
    {
        public DialogName DialogName { get; set; }
        public DialogAction DialogAction { get; set; }

        public DialogMessage(DialogName dialogName, DialogAction dialogAction)
        {
            DialogName = dialogName;
            DialogAction = dialogAction;
        }
    }
}