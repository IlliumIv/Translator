using System;

namespace TranslatorHelper.Message
{
    [Flags]
    public enum ChatType
    {
        // -2 = NPC Exclamations


        LogColorMagenta = 0,    // Whisper
        LogColorWhite = 2,      // Say
        LogColorBrown = 4,      // Zone
        LogColorCian = 5,       // Shout
        LogColorLightGreen = 9,  // Guild
        LogColorYellow = 100,  // System

        // LogColorGreen = ?,   // Officer
        // LogColorPink = ?,   // Spouse
        // LogColorBlue = ?,   // Party
        // LogColorOrange = ?,   // Raid
        // LogColorGold = ?,   // World
        // LogColorGold = ?,   // Telephaty
    }

    /*
    "LogColorLightRed"
    "LogColorBlack"
    "LogColorSlateBlue"
    "LogColorRed"
    "LogColorYellow"
    "LogColorViolet"
    */
}
