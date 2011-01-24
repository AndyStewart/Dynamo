namespace Dynamo.Specs.Fixtures
{
    public class Company : Entity
    {
        public Company()
        {
            HasMany<Contact>("Contacts");
        }
        public string FullName
        {
            get
            {
                return "Mr " + Self.FirstName;
            }
        }
    }
}