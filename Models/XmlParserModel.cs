using System.Xml.Serialization;

namespace TextToXmlService.Models;

// using System.Xml.Serialization;
// XmlSerializer serializer = new XmlSerializer(typeof(People));
// using (StringReader reader = new StringReader(xml))
// {
//    var test = (People)serializer.Deserialize(reader);
// }
[XmlRoot(ElementName = "address")]
public class Address
{

    [XmlElement(ElementName = "street")]
    public string? Street { get; set; }

    [XmlElement(ElementName = "city")]
    public string? City { get; set; }

    [XmlElement(ElementName = "zipcode")]
    public string? Zipcode { get; set; }
}

[XmlRoot(ElementName = "phone")]
public class Phone
{

    [XmlElement(ElementName = "mobile")]
    public string? Mobile { get; set; }

    [XmlElement(ElementName = "landline")]
    public string? Landline { get; set; }
}

[XmlRoot(ElementName = "family")]
public class Family
{

    [XmlElement(ElementName = "name")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "born")]
    public string? Born { get; set; }

    [XmlElement(ElementName = "address")]
    public Address? Address { get; set; }

    [XmlElement(ElementName = "phone")]
    public Phone? Phone { get; set; }
}

[XmlRoot(ElementName = "person")]
public class Person
{

    [XmlElement(ElementName = "firstname")]
    public string? Firstname { get; set; }

    [XmlElement(ElementName = "lastname")]
    public string? Lastname { get; set; }

    [XmlElement(ElementName = "address")]
    public Address? Address { get; set; }

    [XmlElement(ElementName = "phone")]
    public Phone? Phone { get; set; }

    [XmlElement(ElementName = "family")]
    public List<Family> Family { get; set; }
}

[XmlRoot(ElementName = "people")]
public class People
{

    [XmlElement(ElementName = "person")]
    public List<Person>? Person { get; set; }
}

