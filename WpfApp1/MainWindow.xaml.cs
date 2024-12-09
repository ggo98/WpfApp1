using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.IO;
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
//using System.Windows.Shapes;
using System.Xml;
using System.Reflection;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string xshdFilePath = Path.Combine(GetAssemblyDirectory(Assembly.GetExecutingAssembly()), @"xshd\TSQL-Mode.xshd");

            // Load the XSHD file
            var highlightingDefinition = LoadHighlightingFromFile(xshdFilePath);

            // Apply the syntax highlighting to the editor
            texteditor_SQL.SyntaxHighlighting = highlightingDefinition;
        }

        private IHighlightingDefinition LoadHighlightingFromFile(string filePath)
        {
            // Ensure the file exists before trying to load it
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The specified XSHD file was not found: {filePath}");

            // Load the syntax highlighting definition from the XSHD file
            using (var reader = XmlReader.Create(filePath))
            {
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        public static string GetAssemblyDirectory(Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.Location);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hello wpf");

            this.FontSize = 80;
            return;


            foreach (var control in FindVisualChildren<Control>(this))
            {
                control.FontSize = 30;
            }
            return;

            // marche pas...
            var existingStyle = (Style)this.Resources["ControlStyle"];
            if (existingStyle != null)
            {
                /*
                var setter = new Setter(Control.FontSizeProperty, 30);
                //System.InvalidOperationException: 'After a 'SetterBaseCollection' is in use (sealed), it cannot be modified.'
                style.Setters.Clear();  // Clear existing setters
                style.Setters.Add(setter);  // Add new setter for FontSize
                */
                var newStyle = new Style(typeof(Control)); // or target a specific type like Button or TextBox

                // Copy the existing Setters (if needed) to preserve previous ones
                foreach (var setter in existingStyle.Setters)
                {
                    newStyle.Setters.Add(setter);
                }
                newStyle.Setters.Add(new Setter(Control.FontSizeProperty, 80));



                // Replace the existing style in the resources with the new one
                this.Resources["ControlStyle"] = newStyle;
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    yield return (T)child;
                }

                // Recursively find children in the visual tree
                foreach (var descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}
