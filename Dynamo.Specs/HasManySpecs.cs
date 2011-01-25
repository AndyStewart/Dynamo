using Dynamo.Specs.Fixtures;
using Machine.Specifications;

namespace Dynamo.Specs
{
    public class When_adding_an_entity_to_a_has_many_relationship : with_fresh_database
    {
        Because of = () =>
                                 {
                                     company = new Company();
                                     company.Name = "";
                                     repository.Save(company);

                                     contact = new Contact();
                                     contact.FirstName = "Andy";
                                     company.Contacts.Add(contact);
                                 };

        It Should_save_and_set_the_id = () => ((int)contact.Id).ShouldNotEqual(0);
        It Should_save_entire_entity = () => ((string)repository.GetById<Contact>(contact.Id).FirstName).ShouldEqual("Andy");
        It Should_return_entity_in_collection = () => ((int)company.Contacts[0].Id).ShouldEqual((int)contact.Id);

        private static dynamic contact;
        private static dynamic company;
    }
}