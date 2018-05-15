using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PackageManager.Views
{
    public partial class Loading : UserControl
    {
        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(
            "CancelCommand", 
            typeof(ICommand), 
            typeof(Loading), 
            new PropertyMetadata(null, OnCancelCommandChanged)
        );

        private static void OnCancelCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Loading view = (Loading)d;
            view.btnCancel.Command = (ICommand)e.NewValue;
        }

        public Loading()
        {
            InitializeComponent();
        }
    }
}
