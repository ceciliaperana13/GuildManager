using System.Windows;
 using System.Windows.Controls;

 namespace GuildManager.Client.View.Controls;
 public partial class FantasyTextBox : UserControl
 {
     public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(FantasyTextBox),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


     public string Text
    {
         get => (string)GetValue(TextProperty);
         set => SetValue(TextProperty, value);
    }

    public FantasyTextBox()
    {
        InitializeComponent();
        InputBox.SetBinding(TextBox.TextProperty,
        new System.Windows.Data.Binding(nameof(Text))
         {
               Source = this,
               Mode = System.Windows.Data.BindingMode.TwoWay,
               UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
         });

    }

 }