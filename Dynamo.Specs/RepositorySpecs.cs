using Dynamo.Specs.Fixtures;
using Machine.Specifications;

namespace Dynamo.Specs
{
    public class When_loading_back_contents_of_simple_table_into_entity : with_fresh_database
    {
        private Because of = () =>
                                 {
                                     dynamic andyContact = new Contact();
                                     andyContact.FirstName = "Andy";
                                     repository.Save(andyContact);

                                     contact = repository.FindBySql<Contact>("Select * from Contact");
                                 };

        It should_return_values = () => ((string)contact[0].FirstName).ShouldEqual("Andy");
        It should_populate_full_name = () => ((string)contact[0].FullName).ShouldEqual("Mr Andy");

        private static dynamic contact;
    }


    public class When_loading_back_contents_of_generic_sql : with_fresh_database
    {
        Because of = () =>
                         {
                             dynamic company = new Company();
                             company.Name = "Company Name";
                             repository.Save(company);

                             dynamic andyContact = new Contact();
                             andyContact.FirstName = "Andy";
                             andyContact.Company_Id = company.Id;
                             repository.Save(andyContact);

                             contact =repository.FindBySql("Select Contact.Id, Contact.FirstName, Company.Name from Contact inner join Company on (Company.Id=Contact.Company_Id)");
                         };

        It should_return_values_from_contact_table = () => ((string)contact[0].FirstName).ShouldEqual("Andy");
        It should_return_values_from_company_table = () => ((string)contact[0].Name).ShouldEqual("Company Name");

        private static dynamic contact;
    }

    public class When_saving_new_entity : with_fresh_database
    {
        Because of = () =>
                        {
                            contact = new Contact();
                            contact.FirstName = "First Name";
                            contact.Surname = "Surname";
                            repository.Save(contact);

                            contactsFound = repository.GetById<Contact>(contact.Id);
                        };

        It Should_set_id = () => ((int)contactsFound.Id).ShouldNotEqual(0);
        It Should_save_contacts_first_name = () => ((string)contactsFound.FirstName).ShouldEqual("First Name");
        It Should_save_contacts_surname = () => ((string)contactsFound.Surname).ShouldEqual("Surname");
        
        private static dynamic contact;
        private static dynamic contactsFound;
    }

    public class When_updating_existing_entity : with_fresh_database
    {
        private Because of = () =>
                                 {
                                    contact = new Contact();
                                    contact.FirstName = "First Name";
                                    contact.Surname = "Surname";
                                    repository.Save(contact);

                                    contact.FirstName = "My New First Name";
                                    contact.Surname= "My New Surname";
                                    repository.Save(contact);

                                    contactsFound = repository.GetById<Contact>(contact.Id);
                                };

        It Should_set_id = () => ((int)contactsFound.Id).ShouldNotEqual(0);
        It Should_save_contacts_first_name = () => ((string)contactsFound.FirstName).ShouldEqual("My New First Name");
        It Should_save_contacts_surname = () => ((string)contactsFound.Surname).ShouldEqual("My New Surname");

        private static dynamic contact;
        private static dynamic contactsFound;
    }

    public class When_loading_back_existing_entity_by_id : with_fresh_database
    {
        private Because of = () =>
                                 {
                                     contact = new Contact();
                                     contact.FirstName = "Bob Dylan";
                                     repository.Save(contact);

                                     dbContact = repository.GetById<Contact>(contact.Id);
                                 };

        It Should_return_correct_entity_id = () => ((decimal)dbContact.Id).ShouldEqual((decimal)contact.Id);
        It Should_return_correct_entity_contents = () => ((string)dbContact.FirstName).ShouldEqual((string)contact.FirstName);

        private static dynamic contact;
        private static dynamic dbContact;
    }

    public class When_deleting_an_entity : with_fresh_database
    {
        private Because of = () =>
                                 {
                                     contact = new Contact();
                                     contact.FirstName = "Bob Dylan";
                                     repository.Save(contact);
                                     repository.Delete(contact);
                                 };

        It Should_return_null_when_querying_for_deleted_entity = () => ((object)repository.GetById<Contact>(contact.Id)).ShouldBeNull();

        private static dynamic contact;
    }

    public class with_fresh_database
    {
        Establish context = () =>
        {
            TestDatabase.Initialise();
            repository = new Repository();
        };

        public static Repository repository;

    }


    public class Company : Entity
    {
        public string FullName
        {
            get
            {
                return "Mr " + Self.FirstName;
            }
        }
    }

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
