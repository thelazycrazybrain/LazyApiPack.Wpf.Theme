# About this project
This project enables you to theme your WPF Application.

# How to use the ThemeManager
First, you need to initialize the ThemeManager.

```csharp
  public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Start the theme manager and add supported themes to it
            ThemeManager.Start(this,
               SkeumorphDarkTheme.CreateSkeumorphNightTheme(null, null, null,
                   new[] { "SkeuTest1", "SkeuTest2" },
                   new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.UITest;component/Themes/CustomSkeumorphTheme.xaml", UriKind.Absolute)),
               SkeumorphBrightTheme.CreateSkeumorphBrightBlueTheme(null, null, null,
                   new[] { "SkeuTest3", "SkeuTest4" },
                   new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.UITest;component/Themes/CustomSkeumorphTheme.xaml", UriKind.Absolute))
               // ...
               );
            // Then set the first theme
            ThemeManager.Instance.SetTheme("Skeumorph Dark Night");

            // if you store the theme in a variable or select it via ThemeManager.Instance.AvailableThemes
            // you can simply set the theme with ThemeManager.Instance.CurrentTheme = myTheme;
        }
    }
```

# How to switch a theme
You can set the theme in code with 
```csharp
ThemeManager.Instance.SetTheme("ThemeName");
```
or
```csharp
ThemeManager.Instance.CurrentTheme = myTheme;
```

or you can bind the Theme Manager in the UI

```xaml
<ComboBox 
    SelectedItem="{Binding CurrentTheme, Source={x:Static themes:ThemeManager.Instance}}" 
    ItemsSource="{Binding AvailableThemes, Source={x:Static themes:ThemeManager.Instance}}" />
```

# How to add predefined themes
To add predefined themes, simply install a packed from the LazyApiPack.Wpf.Theme.* namespace and use the Theme-Classes
eg:
```csharp
var theme = SkeumorphDarkTheme.CreateSkeumorphNightTheme("Title to override", "Description to override", 
    new BitmapImage(/*Screenshot for the user*/),
    new[] { "Search Tag", "Another Search Tag" },
    new Uri("Additional Resource", UriKind.Absolute));
```

# How to include custom control themes to a theme
To add custom themes to a theme, simply call the Create*Theme function or the Theme constructor with additional resources

```csharp
SkeumorphDarkTheme.CreateSkeumorphNightTheme("Title to override", "Description to override", 
    new BitmapImage(/*Screenshot for the user*/),
    new[] { "Search Tag", "Another Search Tag" },
    // This line adds the CustomSkeumorphTheme to this theme.
    new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.UITest;component/Themes/CustomSkeumorphTheme.xaml", UriKind.Absolute));
```
the XAML of CustomSkeumorphTheme could look like this;
```xaml
<ControlTemplate x:Key="SpecialControlTemplate" TargetType="{x:Type local:SpecialControl}" >
    <Border BorderBrush="{DynamicResource PrimaryForegroundColorBrush}" Background="{DynamicResource PrimaryBackgroundColorBrush}" BorderThickness="5">
        <Grid>
            <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center" Text="{TemplateBinding Title}"/>
        </Grid>
    </Border>
</ControlTemplate>

<Style TargetType="{x:Type local:SpecialControl}">
    <Setter Property="Template" Value="{StaticResource SpecialControlTemplate}"/>
</Style>
```

Ensure you have a Generic.xaml in your project with the default theme and it is mapped to the Application.Resources.MergedResourceDictionaries

Please consider the LazyApiPack.Wpf.Theme.UITest solution as a reference on how to implement this pack.