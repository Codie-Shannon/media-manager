using System.Windows;
using System.Windows.Controls;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    // Element Template
    // ====================================================
    // ====================================================
    [TemplatePart(Name = ContentWrapper, Type = typeof(Button))]


    public class viewIconButton : IconButtonBase
    {
        // Content Wrapper Variables
        // ====================================================
        // ====================================================
        public const string ContentWrapper = "PART_Content";
        private Button ContentWrapperButton { get; set; }


        // Constructor
        // ====================================================
        // ====================================================
        static viewIconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(viewIconButton), new FrameworkPropertyMetadata(typeof(viewIconButton)));
        }


        // Apply Template
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Element's Button
            ContentWrapperButton = (Button)GetTemplateChild(ContentWrapper);

            //Set Event Handlers
            ContentWrapperButton.Click += ContentWrapperButton_Click;
        }


        // Event Handlers
        // ====================================================
        // ====================================================
        private void ContentWrapperButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if the secondary icon has been set and if the current set icon is set to the secondary icon
            //Else check if the secondary icon has been set
            if(!string.IsNullOrEmpty(SecondaryIcon) && Icon == SecondaryIcon)
            {
                //Set Icon to PrimaryIcon
                Icon = PrimaryIcon;
            }
            else if(!string.IsNullOrEmpty(SecondaryIcon))
            {
                //Set Icon to SecondaryIcon
                Icon = SecondaryIcon;
            }
        }


        // Methods
        // ====================================================
        // ====================================================
        public void SetPrimaryIcon()
        {
            //Set Icon to PrimaryIcon
            Icon = PrimaryIcon;
        }

        public void SetSecondaryIcon()
        {
            //Set Icon to SecondaryIcon
            Icon = SecondaryIcon;
        }
    }
}