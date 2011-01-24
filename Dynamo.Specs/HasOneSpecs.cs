using Machine.Specifications;

namespace Dynamo.Specs
{
    public class When_saving_an_entity_that_has_one_related_entity : with_fresh_database
    {
        Because of = () =>
                        {
                            company = new Company();
                            company.Name = "Company Name";
                            repository.Save(company);

                            contact = new Contact();
                            contact.FirstName = "Andy";
                            contact.Company = company;
                            repository.Save(contact);
                        };

        It should_correctly_store_the_entity_id_in_the_table = () => ((int)repository.GetById<Contact>(contact.Id).Company_Id).ShouldEqual((int)company.Id);
        
        static dynamic contact;
        static dynamic company;
    }

    public class When_lazy_loading_an_entity_from_a_has_one_relationship : with_fresh_database
    {
        Because of = () =>
        {
            company = new Company();
            company.Name = "Company Name";
            repository.Save(company);

            contact = new Contact();
            contact.FirstName = "Andy";
            contact.Company = company;
            repository.Save(contact);
        };

        It should_correctly_load_back_entity = () => ((int)repository.GetById<Contact>(contact.Id).Company.Id).ShouldEqual((int)company.Id);

        static dynamic contact;
        static dynamic company;
    }

    public class When_saving_an_entity_that_has_one_related_entity_which_is_null : with_fresh_database
    {
        Because of = () =>
        {
            contact = new Contact();
            contact.FirstName = "Andy";
            repository.Save(contact);
        };

        It should_return_null_when_company_isnt_set = () => ((Company)repository.GetById<Contact>(contact.Id).Company).ShouldBeNull();

        static dynamic contact;
    }

}