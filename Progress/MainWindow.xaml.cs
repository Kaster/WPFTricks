using System.Windows;

namespace Progress
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private void dxTextEdit_Validate(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        //{
        //    if (e.Value == null) return;
        //    if (e.Value.ToString().Length > 4) return;
        //    e.IsValid = false;
        //    e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Information;
        //    e.ErrorContent = "User ID is less than five symbols. Please correct.";
        //}
    }
}
