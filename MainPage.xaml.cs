using System.Xml.Xsl;
using System.Xml;
using System.Reflection;
using Microsoft.Maui.Controls;


namespace LABA2
{
    public partial class MainPage : ContentPage
    {
        private string XslPath = "";
        public MainPage()
        {
            InitializeComponent();
            GetAllAuthors();
        }

        private void GetAllAuthors()
        {
            XmlDocument doc = new XmlDocument();
            var appLocation = Assembly.GetEntryAssembly().Location;
            var appPath = Path.GetDirectoryName(appLocation);
            Directory.SetCurrentDirectory(appPath);
            doc.Load(@"xmlhelp\XMLFile1.xml");

            XmlElement xRoot = doc.DocumentElement;
            XmlNodeList childNodes = xRoot.SelectNodes("publications");


            for (int i = 0; i < childNodes.Count; i++)
            {
                XmlNode n = childNodes.Item(i);
                addItems(n);
            }

        }

        private void addItems(XmlNode n)
        {
            if (!NamePicker.Items.Contains(n.SelectSingleNode("@NAME").Value))
                NamePicker.Items.Add(n.SelectSingleNode("@NAME").Value);
            if (!FacultyPicker.Items.Contains(n.SelectSingleNode("@FACULTY").Value))
                FacultyPicker.Items.Add(n.SelectSingleNode("@FACULTY").Value);
            if (!DepartmentPicker.Items.Contains(n.SelectSingleNode("@DEPARTMENT").Value))
                DepartmentPicker.Items.Add(n.SelectSingleNode("@DEPARTMENT").Value);
            if (!DataPicker.Items.Contains(n.SelectSingleNode("@DATA").Value))
                DataPicker.Items.Add(n.SelectSingleNode("@DATA").Value);
        }


        private void SearchBtnHandler(object sender, EventArgs e)
        {
            editor.Text = "";

            MauiProgram.Publication publication = GetSelectedParameters();
            MauiProgram.IStrategy analyzer = GetSelectedAnalyzer();
            PerformSearch(publication, analyzer);
        }

        private MauiProgram.Publication GetSelectedParameters()
        {
            MauiProgram.Publication publication = new MauiProgram.Publication();

            if (NameCheckBox.IsChecked)
            {
                publication.Name = NamePicker.SelectedItem.ToString();
            }
            if (FacultyCheckBox.IsChecked)
            {
                publication.Faculty = FacultyPicker.SelectedItem.ToString();
            }
            if (DepartmentCheckBox.IsChecked)
            {
                publication.Department = DepartmentPicker.SelectedItem.ToString();
            }
            if (DataCheckBox.IsChecked)
            {
                publication.Data = DataPicker.SelectedItem.ToString();
            }

            return publication;
        }

        private MauiProgram.IStrategy GetSelectedAnalyzer()
        {
            MauiProgram.IStrategy analyzer = new MauiProgram.Sax();

            if (DomBtn.IsChecked)
            {
                analyzer = new MauiProgram.Dom();
            }
            if (LinqBtn.IsChecked)
            {
                analyzer = new MauiProgram.Linq();
            }

            return analyzer;
        }

        private void PerformSearch(MauiProgram.Publication publication, MauiProgram.IStrategy analyzer)
        {
            MauiProgram.Searcher search = new MauiProgram.Searcher(publication, analyzer);
            List<MauiProgram.Publication> results = search.SearchAlgorithm();

            foreach (MauiProgram.Publication p in results)
            {
                editor.Text += "Name: " + p.Name + "\n";
                editor.Text += "Faculty: " + p.Faculty + "\n";
                editor.Text += "Department: " + p.Department + "\n";
                editor.Text += "Data: " + p.Data + "\n";
                editor.Text += "\n";
            }
        }

        private void ClearFields(object sender, EventArgs e)
        {
            editor.Text = "";
            SaxBtn.IsChecked = false;
            DomBtn.IsChecked = false;
            LinqBtn.IsChecked = false;
            NameCheckBox.IsChecked = false;
            FacultyCheckBox.IsChecked = false;
            DepartmentCheckBox.IsChecked = false;
            DataCheckBox.IsChecked = false;
            NamePicker.SelectedItem = null;
            FacultyPicker.SelectedItem = null;
            DepartmentPicker.SelectedItem = null;
            DataPicker.SelectedItem = null;
        }

        private void OnTransformToHTMLBtnClicked(object sender, EventArgs e)
        {
            XslCompiledTransform xct = LoadXSLT();
            string xmlPath = @"./XMLFile1.xml";
            string htmlPath = @"./XMLFile1.html";

            XsltArgumentList xslArgs = CreateXSLTArguments();

            TransformXMLToHTML(xct, xslArgs, xmlPath, htmlPath);
        }

        private async void OnOpenFileButton(object sender, EventArgs e)
        {
            var fileResult = await FilePicker.PickAsync();

            if (fileResult != null)
            {
                XslPath = fileResult.FullPath;
            }
        }
        private XslCompiledTransform LoadXSLT()
        {
            XslCompiledTransform xct = new XslCompiledTransform();
            xct.Load(@"" + XslPath);
            return xct;
        }
        private XsltArgumentList CreateXSLTArguments()
        {
            XsltArgumentList xslArgs = new XsltArgumentList();

            string name = NameCheckBox.IsChecked ? NamePicker.SelectedItem.ToString() : null;
            string faculty = FacultyCheckBox.IsChecked ? FacultyPicker.SelectedItem.ToString() : null;
            string department = DepartmentCheckBox.IsChecked ? DepartmentPicker.SelectedItem.ToString() : null;
            string data = DataCheckBox.IsChecked ? DataPicker.SelectedItem.ToString() : null;

            if (name != null)
            {
                xslArgs.AddParam("name", "", name);
            }
            if (faculty != null)
            {
                xslArgs.AddParam("faculty", "", faculty);
            }
            if (department != null)
            {
                xslArgs.AddParam("department", "", department);
            }
            if (data != null)
            {
                xslArgs.AddParam("data", "", data);
            }

            return xslArgs;
        }

        private void TransformXMLToHTML(XslCompiledTransform xct, XsltArgumentList xslArgs, string xmlPath, string htmlPath)
        {
            using (XmlReader xr = XmlReader.Create(xmlPath))
            {
                using (XmlWriter xw = XmlWriter.Create(htmlPath))
                {
                    xct.Transform(xr, xslArgs, xw);
                }
            }
        }

        private async void OnExitBtnClicked(object sender, EventArgs e)
        {
            var result = await Application.Current.MainPage.DisplayAlert("Exit", "Are you sure you want to exit the program?", "Yes", "No");
            if (result)
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            int textLength = editor.Text.Length;
            int fontSize = CalculateFontSize(textLength);
            editor.FontSize = fontSize;
        }

        private int CalculateFontSize(int textLength)
        {
            if (textLength < 100)
            {
                return 18;
            }
            else if (textLength < 500)
            {
                return 14;
            }
            else
            {
                return 10;
            }
        }
    }
}