using System.Collections.Generic;
using System.Data;
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
                                     repository.Save(contact);
                                     dbContact = repository.FindBySql<Contact>("Select * from Contact where Id=" + contact.Id);
                                 };

        It Should_save_contacts_first_name = () => ((string)dbContact[0].FirstName).ShouldEqual("First Name");

        private static dynamic contact;
        private static Repository repository = new Repository();
        private static IList<dynamic> dbContact;
    }

    public class Contact : Entity
    {
        public string FullName
        {
            get
            {
                return "Mr " + self.FirstName;
            }
        }
    }
}
