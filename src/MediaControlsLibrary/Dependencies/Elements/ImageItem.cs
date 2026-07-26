using System.Windows;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class ImageItem : ElementBase
    {
        // Constructor
        // =========================================================
        // =========================================================
        static ImageItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageItem), new FrameworkPropertyMetadata(typeof(ImageItem)));
        }


        // Apply Template
        // =========================================================
        // =========================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Set Content
            this.Content = string.IsNullOrEmpty(MCustomName) ? MName : MCustomName;
        }
    }
}