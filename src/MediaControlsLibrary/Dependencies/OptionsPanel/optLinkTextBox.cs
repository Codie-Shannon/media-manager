using System.Windows;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Navigation;
using MediaControlsLibrary.Dependencies;

namespace MediaControlsLibrary
{
    public class optLinkTextBox : TextBoxBase
    {
        // Variables
        // ====================================================
        // ====================================================
        private const string str_Link = "PART_Link";


        // Constructor
        // ====================================================
        // ====================================================
        static optLinkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(optLinkTextBox), new FrameworkPropertyMetadata(typeof(optLinkTextBox)));
        }


        // Apply Template
        // ====================================================
        // ====================================================
        public override void OnApplyTemplate()
        {
            //Fire When the Element's Template is Applied
            base.OnApplyTemplate();

            //Get Element
            Hyperlink hyperlink = GetHyperLink();

            //Set Event Handlers
            hyperlink.RequestNavigate += HyperLink_Navigate;
        }


        // Event Handlers
        // ====================================================
        // ====================================================
        private void HyperLink_Navigate(object sender, RequestNavigateEventArgs e)
        {
            OpenUri((Hyperlink)sender);
        }


        // Methods
        // ====================================================
        // ====================================================
        private Hyperlink GetHyperLink()
        {
            //Get Hyperlink From Template
            Hyperlink link = (Hyperlink)this.Template.FindName(str_Link, this);

            //Return Hyperlink
            return link;
        }

        private void OpenUri(Hyperlink link)
        {
            //Open Web Address
            Process.Start(link.NavigateUri.ToString());
        }
    }
}