using System.Collections.Generic;
using Machine.Specifications;

namespace ActiveRecord.Specs
{
    public class When_loading_back_contents_of_simple_table_into_entity
    {
        Because of = () => contact = new Repository().FindBySql<Contact>("Select * from Contact");
        It should_return_values = () => ((string)contact[0].FirstName).ShouldEqual("Andy");
        It should_populate_full_name = () => ((string)contact[0].FullName).ShouldEqual("Mr Andy");

        private static dynamic contact;
    }

    public class When_loading_back_contents_of_generic_sql
    {
        Because of = () => contact = new Repository().FindBySql("Select Contact.Id, Contact.FirstName, Company.Name from Contact inner join Company on (Company.Id=Contact.Company_Id)");
        It should_return_values_from_contact_table = () => ((string)contact[0].FirstName).ShouldEqual("Andy");
        It should_return_values_from_company_table = () => ((string)contact[0].Name).ShouldEqual("Company Name");

        private static dynamic contact;
    }

    public class When_saving_new_entity
    {
        Establish context = () => contact = new Contact();

        private Because of = () =>
                                 {
                                     contact.FirstName = "First Name";
                                     contact.Surname = "Surname";
                                     repository.Save(contact);

                                     contactsFound = repository.GetById<Contact>(contact.Id);
                                 };

        It Should_set_id = () => ((int)contactsFound.Id).ShouldNotEqual(0);
        It Should_save_contacts_first_name = () => ((string)contactsFound.FirstName).ShouldEqual("First Name");
        It Should_save_contacts_surname = () => ((string)contactsFound.Surname).ShouldEqual("Surname");
        
        private static dynamic contact;
        private static Repository repository = new Repository();
        private static dynamic contactsFound;
    }

    public class When_updating_existing_entity
    {
        Establish context = () => contact = new Contact();

        private Because of = () =>
        {
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
        private static Repository repository = new Repository();
        private static dynamic contactsFound;
    }

    public class When_loading_back_existing_entity_by_id
    {
        Establish context = () =>
                                        {
                                            contact = new Contact();
                                            contact.FirstName = "Bob Dylan";
                                            repository = new Repository();
                                            repository.Save(contact);
                                        };

        Because of = () => dbContact = repository.GetById<Contact>(contact.Id);
        It Should_return_correct_entity_id = () => ((decimal)dbContact.Id).ShouldEqual((decimal)contact.Id);
        It Should_return_correct_entity_contents = () => ((string)dbContact.FirstName).ShouldEqual((string)contact.FirstName);

        private static dynamic contact;
        private static Repository repository;
        private static dynamic dbContact;
    }

    public class When_deleting_an_entity
    {
        Establish context = () =>
        {
            contact = new Contact();
            contact.FirstName = "Bob Dylan";
            repository = new Repository();
            repository.Save(contact);
        };

        Because of = () => repository.Delete(contact);

        It Should_return_null_when_querying_for_deleted_entity = () => ((object)repository.GetById<Contact>(contact.Id)).ShouldBeNull();

        private static dynamic contact;
        private static Repository repository;
    }

    public class Contact : Entity
    {
        public string FullName
        {
            get
            {
                return "Mr " + Self.FirstName;
            }
        }
    }
}
