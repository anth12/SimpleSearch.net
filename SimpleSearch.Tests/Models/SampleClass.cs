
namespace SimpleSearch.Tests.Models
{
    public class SampleClass
    {
        public SampleClass(string company, string city, string country)
        {
            Company = company;
            City = city;
            Country = country;
        }

        public string Company { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
