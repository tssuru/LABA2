using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;


namespace LABA2
{
    public static class MauiProgram
    {
        public class Publication
        {
            public string Name { get; set; }
            public string Faculty { get; set; }
            public string Department { get; set; }
            public string Data { get; set; }

            public Publication() { }
        }
        public interface IStrategy
        {
            List<Publication> Search(Publication publication);
        }
        public class Searcher
        {
            private Publication publication;
            private IStrategy strategy;

            public Searcher(Publication p, IStrategy str)
            {
                publication = p;
                strategy = str;
            }

            public List<Publication> SearchAlgorithm()
            {
                return strategy.Search(publication);
            }
        }

        public class Sax : IStrategy
        {
            public List<Publication> Search(Publication publication)
            {
                List<Publication> results = new List<Publication>();
                XmlTextReader xmlReader = new XmlTextReader(@"xmlhelp\XMLFile1.xml");
                while (xmlReader.Read())
                {
                    if (xmlReader.HasAttributes)
                    {
                        while (xmlReader.MoveToNextAttribute())
                        {
                            string name = "";
                            string faculty = "";
                            string department = "";
                            string data = "";

                            if (xmlReader.Name.Equals("NAME") && (xmlReader.Value.Equals(publication.Name) || publication.Name == null))
                            {
                                name = xmlReader.Value;
                                xmlReader.MoveToNextAttribute();
                                if (xmlReader.Name.Equals("FACULTY") && (xmlReader.Value.Equals(publication.Faculty) || publication.Faculty == null))
                                {
                                    faculty = xmlReader.Value;
                                    xmlReader.MoveToNextAttribute();
                                    if (xmlReader.Name.Equals("DEPARTMENT") && (xmlReader.Value.Equals(publication.Department) || publication.Department == null))
                                    {
                                        department = xmlReader.Value;
                                        xmlReader.MoveToNextAttribute();
                                        if (xmlReader.Name.Equals("DATA") && (xmlReader.Value.Equals(publication.Data) || publication.Data == null))
                                        {
                                            data = xmlReader.Value;
                                        }
                                    }
                                }
                            }

                            if (name != "" && faculty != "" && department != "" && data != "")
                            {
                                Publication newPublication = new Publication { Name = name, Department = department, Faculty = faculty, Data = data };
                                results.Add(newPublication);
                            }
                        }
                    }
                }
                xmlReader.Close();
                return results;
            }
        }

        public class Dom : IStrategy
        {
            public List<Publication> Search(Publication publication)
            {
                List<Publication> results = new List<Publication>();
                XmlDocument doc = new XmlDocument();
                doc.Load(@"xmlhelp\XMLFile1.xml");
                XmlNode node = doc.DocumentElement;
                foreach (XmlNode n in node.ChildNodes)
                {
                    string name = "";
                    string department = "";
                    string faculty = "";
                    string data = "";

                    foreach (XmlAttribute attribute in n.Attributes)
                    {
                        if (attribute.Name.Equals("NAME") && (attribute.Value.Equals(publication.Name) || publication.Name == null))
                            name = attribute.Value;
                        if (attribute.Name.Equals("FACULTY") && (attribute.Value.Equals(publication.Faculty) || publication.Faculty == null))
                            faculty = attribute.Value;
                        if (attribute.Name.Equals("DEPARTMENT") && (attribute.Value.Equals(publication.Department) || publication.Department == null))
                            department = attribute.Value;
                        if (attribute.Name.Equals("DATA") && (attribute.Value.Equals(publication.Data) || publication.Data == null))
                            data = attribute.Value;
                    }


                    if (name != "" && department != "" && faculty != "" && data != "")
                    {
                        Publication newPublication = new Publication { Name = name, Department = department, Faculty = faculty, Data = data };
                        results.Add(newPublication);
                    }
                }
                return results;
            }
        }

        public class Linq : IStrategy
        {
            public List<Publication> Search(Publication publication)
            {
                List<Publication> results = new List<Publication>();
                var doc = XDocument.Load(@"xmlhelp\XMLFile1.xml");
                var result = from obj in doc.Descendants("publications")
                             where
                             (
                             (obj.Attribute("NAME").Value.Equals(publication.Name) || publication.Name == null) &&
                             (obj.Attribute("FACULTY").Value.Equals(publication.Faculty) || publication.Faculty == null) &&
                             (obj.Attribute("DEPARTMENT").Value.Equals(publication.Department) || publication.Department == null) &&
                             (obj.Attribute("DATA").Value.Equals(publication.Data) || publication.Data == null)
                             )
                             select new
                             {
                                 name = (string)obj.Attribute("NAME"),
                                 faculty = (string)obj.Attribute("FACULTY"),
                                 department = (string)obj.Attribute("DEPARTMENT"),
                                 data = (string)obj.Attribute("DATA"),
                             };

                foreach (var p in result)
                {
                    Publication newPublication = new Publication { Name = p.name, Department = p.department, Faculty = p.faculty, Data = p.data };
                    results.Add(newPublication);
                }
                return results;
            }
        }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}