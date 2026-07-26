using System.Windows;
using System.Windows.Controls;
using MediaControlsLibrary.Models;

namespace Media_Manager.Selectors
{
    public class ElementSelector : DataTemplateSelector
    {
        // Data Template Selector
        // ======================================================
        // ======================================================
        public override DataTemplate SelectTemplate(object item, DependencyObject owner)
        {
            //Get Owner
            FrameworkElement container = (FrameworkElement)owner;

            //Check if the Owner was Parsed Successfully
            if (container != null && item != null)
            {
                //Check if the Element's Model was Movie or Game
                if (item is MovieSearch s)
                {
                    //Get and Return DataTemplate
                    return container.FindResource("MovieTemplate") as DataTemplate;
                }
                else
                {
                    //Get and Return DataTemplate
                    return container.FindResource("GameTemplate") as DataTemplate;
                }
            }

            //Return Null
            return null;
        }
    }
}