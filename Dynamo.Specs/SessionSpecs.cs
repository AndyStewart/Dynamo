using System.Collections.Generic;
using System.Linq;
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
                                     Session.Save(andyContact);

                                     contact = Session.FindBySql<Contact>("Select * from Contact");
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
                             Session.Save(company);

                             dynamic andyContact = new Contact();
                             andyContact.FirstName = "Andy";
                             andyContact.Company = company;
                             Session.Save(andyContact);

                             contact = Session.FindBySql("Select Contact.Id, Contact.FirstName, Company.Name from Contact inner join Company on (Company.Id=Contact.Company_Id) Where Company.Id=@Id", new { Id=company.Id});
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
                            Session.Save(contact);

                            contactsFound = Session.GetById<Contact>(contact.Id);
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
                                    Session.Save(contact);

                                    contact.FirstName = "My New First Name";
                                    contact.Surname= "My New Surname";
                                    Session.Save(contact);

                                    contactsFound = Session.GetById<Contact>(contact.Id);
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
                                     Session.Save(contact);

                                     dbContact = Session.GetById<Contact>(contact.Id);
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
                                     Session.Save(contact);
                                     Session.Delete(contact);
                                 };

        It Should_return_null_when_querying_for_deleted_entity = () => ((object)Session.GetById<Contact>(contact.Id)).ShouldBeNull();

        private static dynamic contact;
    }

    public class When_a_find_is_performed_by_a_single_column_without_parameters : with_fresh_database
    {
        Because of = () =>
                         {
                             dynamic contact = new Contact();
                             contact.FirstName = "Andy";
                             contact.Surname = "Stewart";
                             Session.Save(contact);

                             dynamic contact2 = new Contact();
                             contact2.FirstName = "Bob";
                             contact2.Surname = "Stewart";
                             Session.Save(contact2);

                             dynamic contact3 = new Contact();
                             contact3.FirstName = "Andy";
                             contact3.Surname = "Smith";
                             Session.Save(contact3);

                             results = Session.Find<Contact>().Where("FirstName='Andy'").ToList();
                         };

        It should_return_2_records = () => results.Count().ShouldEqual(2);
        It should_return_first_record = () => results.Any(q => q.FirstName == "Andy" && q.Surname == "Stewart").ShouldBeTrue();
        It should_return_second_record = () => results.Any(q => q.FirstName == "Andy" && q.Surname == "Smith").ShouldBeTrue();
        private static IList<dynamic> results; 
    }

    public class When_a_find_is_performed_by_a_single_column_with_one_parameter : with_fresh_database
    {
        Because of = () =>
        {
            dynamic contact = new Contact();
            contact.FirstName = "Andy";
            contact.Surname = "Stewart";
            Session.Save(contact);

            dynamic contact2 = new Contact();
            contact2.FirstName = "Bob";
            contact2.Surname = "Stewart";
            Session.Save(contact2);

            dynamic contact3 = new Contact();
            contact3.FirstName = "Andy";
            contact3.Surname = "Smith";
            Session.Save(contact3);

            results = Session.Find<Contact>().Where("FirstName=@FirstName", new { FirstName="Andy"}).ToList();
        };

        It should_return_2_records = () => results.Count.ShouldEqual(2);
        It should_return_first_record = () => results.Any(q => q.FirstName == "Andy" && q.Surname == "Stewart").ShouldBeTrue();
        It should_return_second_record = () => results.Any(q => q.FirstName == "Andy" && q.Surname == "Smith").ShouldBeTrue();
        static IList<dynamic> results;
    }

    public class When_a_find_is_performed_by_a_single_column_with_multiple_parameters : with_fresh_database
    {
        Because of = () =>
        {
            dynamic contact = new Contact();
            contact.FirstName = "Andy";
            contact.Surname = "Stewart";
            Session.Save(contact);

            dynamic contact2 = new Contact();
            contact2.FirstName = "Bob";
            contact2.Surname = "Stewart";
            Session.Save(contact2);

            dynamic contact3 = new Contact();
            contact3.FirstName = "Andy";
            contact3.Surname = "Smith";
            Session.Save(contact3);

            results = Session.Find<Contact>().Where("FirstName=@FirstName and Surname=@Surname", new { FirstName = "Andy", Surname="Stewart" }).ToList();
        };

        It should_return_1_records = () => results.Count.ShouldEqual(1);
        It should_return_first_record = () => results.Any(q => q.FirstName == "Andy" && q.Surname == "Stewart").ShouldBeTrue();
        static IList<dynamic> results;
    }

    public class When_a_find_is_performed_by_a_single_column_with_single_parameter_without_condition_text_and_ordered : with_fresh_database
    {
        Because of = () =>
        {
            dynamic contact = new Contact();
            contact.FirstName = "Andy";
            contact.Surname = "Stewart";
            Session.Save(contact);

            dynamic contact2 = new Contact();
            contact2.FirstName = "Bob";
            contact2.Surname = "Stewart";
            Session.Save(contact2);

            dynamic contact3 = new Contact();
            contact3.FirstName = "Andy";
            contact3.Surname = "Smith";
            Session.Save(contact3);

            results = Session.Find<Contact>().Where(new { FirstName = "Andy" }).OrderBy("Surname").ToList();
        };

        It should_return_2_records = () => results.Count().ShouldEqual(2);
        It should_return_first_record_as_smith = () => ((string)results.ToList()[0].Surname).ShouldEqual("Smith");
        It should_return_first_record_as_stewart = () => ((string)results.ToList()[1].Surname).ShouldEqual("Stewart");
        static IEnumerable<dynamic> results;
    }

    public class When_find_is_performed_with_no_conditions_and_ordered : with_fresh_database
    {
        Because of = () =>
        {
            dynamic contact = new Contact();
            contact.FirstName = "Andy";
            contact.Surname = "Stewart";
            Session.Save(contact);

            dynamic contact2 = new Contact();
            contact2.FirstName = "Bob";
            contact2.Surname = "Stewart";
            Session.Save(contact2);

            dynamic contact3 = new Contact();
            contact3.FirstName = "Andy";
            contact3.Surname = "Smith";
            Session.Save(contact3);

            results = Session.Find<Contact>()
                                .OrderBy("Surname").ToList();
        };

        It should_return_2_records = () => results.Count().ShouldEqual(3);
        It should_return_first_record_as_smith = () => ((string)results.ToList()[0].Surname).ShouldEqual("Smith");
        It should_return_first_record_as_stewart = () => ((string)results.ToList()[1].Surname).ShouldEqual("Stewart");
        static IEnumerable<dynamic> results;
    }

    public class When_a_find_is_performed_by_a_single_column_with_single_parameter_and_ordered : with_fresh_database
    {
        Because of = () =>
        {
            dynamic contact = new Contact();
            contact.FirstName = "Andy";
            contact.Surname = "Stewart";
            Session.Save(contact);

            dynamic contact2 = new Contact();
            contact2.FirstName = "Bob";
            contact2.Surname = "Stewart";
            Session.Save(contact2);

            dynamic contact3 = new Contact();
            contact3.FirstName = "Andy";
            contact3.Surname = "Smith";
            Session.Save(contact3);

            results = Session.Find<Contact>().Where("FirstName=@FirstName", new { FirstName = "Andy" }).OrderBy("Surname").ToList();
        };

        It should_return_2_records = () => results.Count().ShouldEqual(2);
        It should_return_first_record_as_smith = () => ((string)results.ToList()[0].Surname).ShouldEqual("Smith");
        It should_return_first_record_as_stewart = () => ((string)results.ToList()[1].Surname).ShouldEqual("Stewart");
        static IEnumerable<dynamic> results;
    }

    public class When_a_find_is_performed_using_a_dynamic_finder_with_one_conditions_and_order_applied : with_fresh_database
    {
        Because of = () =>
        {
            dynamic contact = new Contact();
            contact.FirstName = "Andy";
            contact.Surname = "Stewart";
            Session.Save(contact);

            dynamic contact2 = new Contact();
            contact2.FirstName = "Bob";
            contact2.Surname = "Stewart";
            Session.Save(contact2);

            dynamic contact3 = new Contact();
            contact3.FirstName = "Andy";
            contact3.Surname = "Smith";
            Session.Save(contact3);

            results = Session.DynamicFind<Contact>().ByFirstName("Andy").OrderBy("Surname").ToList();
        };

        It should_return_1_records = () => results.Count().ShouldEqual(2);
        It should_return_correct_firstname = () => ((string)results[0].Surname).ShouldEqual("Smith");
        It should_return_correct_surname = () => ((string)results[1].Surname).ShouldEqual("Stewart");

        static IList<dynamic> results;
    }

    public class When_a_find_is_performed_using_a_dynamic_finder_with_two_conditions : with_fresh_database
    {
        Because of = () =>
        {
            dynamic contact = new Contact();
            contact.FirstName = "Andy";
            contact.Surname = "Stewart";
            Session.Save(contact);

            dynamic contact2 = new Contact();
            contact2.FirstName = "Bob";
            contact2.Surname = "Stewart";
            Session.Save(contact2);

            dynamic contact3 = new Contact();
            contact3.FirstName = "Andy";
            contact3.Surname = "Smith";
            Session.Save(contact3);

            results = Session.DynamicFind<Contact>().ByFirstNameAndSurname("Andy", "Stewart").ToList();
        };

        It should_return_1_records = () => results.Count().ShouldEqual(1);
        It should_return_correct_firstname = () => ((string)results[0].FirstName).ShouldEqual("Andy");
        It should_return_correct_surname = () => ((string)results[0].Surname).ShouldEqual("Stewart");

        static IList<dynamic> results;
    }

    public class When_a_find_is_performed_using_eager_loading  : with_fresh_database
    {
        Because of = () =>
        {
            dynamic company = new Company();
            company.Name = "New Name";
            Session.Save(company);

            dynamic contact1 = new Contact();
            contact1.FirstName = "Andy";
            contact1.Surname = "Stewart";
            company.Contacts.Add(contact1);

            dynamic contact2 = new Contact();
            contact2.FirstName = "Andy";
            contact2.Surname = "Stewart";
            company.Contacts.Add(contact2);

            sqlCount = Session.DbProvider.QueryCount;
            results = Session.Find<Contact>()
                                .Include<Company>()
                                .ToList();

            eagerLoadedCompany = results[0].Company;
        };

        It Should_only_execute_two_queries = () => (Session.DbProvider.QueryCount - sqlCount).ShouldEqual(2);
        It Should_return_company = () => ((string)eagerLoadedCompany.Name).ShouldEqual("New Name");

        static IList<dynamic> results;
        static int sqlCount;
        static dynamic eagerLoadedCompany;
    }

    public class When_a_find_is_used_but_returns_a_count : with_fresh_database
    {
        Because of = () =>
        {
            dynamic contact = new Contact();
            contact.FirstName = "Andy";
            contact.Surname = "Stewart";
            Session.Save(contact);

            dynamic contact2 = new Contact();
            contact2.FirstName = "Bob";
            contact2.Surname = "Stewart";
            Session.Save(contact2);

            dynamic contact3 = new Contact();
            contact3.FirstName = "Andy";
            contact3.Surname = "Smith";
            Session.Save(contact3);
        };

        It Should_return_correct_count_of_records = () => Session.Find<Contact>().Count("FirstName").ShouldEqual(3);
    }

    public class When_reading_data_from_customised_class : with_fresh_database
    {
        Because of = () =>
        {
            dynamic company = new CustomCompany();
            company.CompanyName = "Company Name";
            contact = new CustomContact();
            contact.FirstName = "Andy";
            company.Contacts = new List<Contact>();
            company.Contacts.Add(contact);
            
            Session.Save(company);

            companyFound = Session.GetById<CustomCompany>(company.Id);
        };

        private static dynamic companyFound;
        private static dynamic contact;

        It should_return_company_name = () => ((string)companyFound.CompanyName).ShouldEqual("Company Name");
        It should_load_back_company_for_contact = () => ((string)contact.ParentCompany.CompanyName).ShouldEqual("Company Name");
        
        public class CustomCompany : Entity
        {
            public CustomCompany()
            {
                TableName = "Company";
                Property<string>("CompanyName", "Name");
                HasMany<CustomContact>("Contacts", "Company_Id");
            }
        }

        public class CustomContact : Entity
        {
            public CustomContact()
            {
                TableName = "Contact";
                BelongsTo<CustomCompany>("ParentCompany", "Company_Id");
            }
        }
    }



    public class with_fresh_database
    {
        Establish context = () =>
        {
            TestDatabase.Initialise();
            Session = new Session(@"Data Source=.\sqlexpress;Initial Catalog=Dynamo_Test;Integrated Security=True");
        };

        public static ISession Session;

    }
}
