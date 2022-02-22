using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

[assembly: ExportFont("Font Awesome 6 Free-Solid-900.otf", Alias = "FA")]

// Barker: I've read the comment at...
// https://docs.microsoft.com/xamarin/xamarin-forms/app-fundamentals/localization/text?pivots=windows#specify-the-default-culture
// and I'm assuming that using en-GB is the most appropriate language
// to specify here given that the AppResources.resx strings are en-GB.
[assembly: NeutralResourcesLanguage("en-GB")]