namespace Dynamo.Specs.Fixtures
{
    public class Contact : Entity
    {
        public Contact()
        {
            BelongsTo("Company");
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