﻿using System.Threading.Tasks;

namespace GSM_Designer.Pages
{
    public interface iImageWidgetController
    {
        void SetSource(object BitmapImage, int containerIndex);
        void Reset();
        void UpdateUI(bool isProcessing);
    }

    public interface iLayoutUpdater
    {
        Task UpdateOutputLayout(string patternName);
        void ShowWindow();
    }
}