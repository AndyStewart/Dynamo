using System.Collections.Generic;
using System.Linq;
using Dynamo.Specs.Fixtures;
using Machine.Specifications;

namespace Dynamo.Specs
{
    public class When_mapping_entity_with_has_many
    {
        Establish context = () => entity = new HasManyEntity();
        Because of = () => property = entity.Properties.FirstOrDefault(q => q.PropertyName == "Company");

        It should_set_property_name_to_type_name = () => property.PropertyName.ShouldEqual("Company");
        It should_set_column_name = () => property.ColumnName.ShouldEqual("HasManyEntity_Id");
        It should_set_type__to_Ilist_of_Company = () => property.Type.ShouldEqual(typeof(List<Company>));
        It should_set_property_type = () => property.PropertyType.ShouldEqual(PropertyType.HasMany);

        private static HasManyEntity entity;
        private static Property property;

        public class HasManyEntity : Entity
        {
            public HasManyEntity()
            {
                HasMany<Company>();
            }
        }
    }

    public class When_mapping_entity_with_has_many_and_custom_property_name
    {
        Establish context = () => entity = new HasManyEntity();
        Because of = () => property = entity.Properties.FirstOrDefault(q => q.PropertyName == "Company1");
        It should_set_property_name_to_type_name = () => property.PropertyName.ShouldEqual("Company1");
        It should_set_column_name = () => property.ColumnName.ShouldEqual("HasManyEntity_Id");
        It should_set_type__to_Ilist_of_Company = () => property.Type.ShouldEqual(typeof(List<Company>));
        It should_set_property_type = () => property.PropertyType.ShouldEqual(PropertyType.HasMany);

        private static HasManyEntity entity;
        private static Property property;

        public class HasManyEntity : Entity
        {
            public HasManyEntity()
            {
                HasMany<Company>("Company1");
            }
        }
    }

    public class When_mapping_entity_with_has_many_and_custom_column_name
    {
        Establish context = () => entity = new HasManyEntity();
        Because of = () => property = entity.Properties.FirstOrDefault(q => q.PropertyName == "Company");
        It should_set_property_name_to_type_name = () => property.PropertyName.ShouldEqual("Company");
        It should_set_column_name = () => property.ColumnName.ShouldEqual("custom_key");
        It should_set_type__to_Ilist_of_Company = () => property.Type.ShouldEqual(typeof(List<Company>));
        It should_set_property_type = () => property.PropertyType.ShouldEqual(PropertyType.HasMany);

        private static HasManyEntity entity;
        private static Property property;

        public class HasManyEntity : Entity
        {
            public HasManyEntity()
            {
                HasMany<Company>(columnName: "custom_key");
            }
        }
    }

    public class Whem_mapping_entity_with_belongs_to
    {
        Establish context = () => entity = new BelongsToEntity();
        Because of = () => property = entity.Properties.FirstOrDefault(q => q.PropertyName == "Company");
        It should_set_property_name_to_type_name = () => property.PropertyName.ShouldEqual("Company");
        It should_set_column_name = () => property.ColumnName.ShouldEqual("Company_Id");
        It should_set_type__to_Company = () => property.Type.ShouldEqual(typeof(Company));
        It should_set_property_type = () => property.PropertyType.ShouldEqual(PropertyType.BelongsTo);

        private static BelongsToEntity entity;
        private static Property property;

        public class BelongsToEntity : Entity
        {
            public BelongsToEntity()
            {
                BelongsTo<Company>();
            }
        }
    }

    public class When_mapping_a_property_to_a_different_column_name
    {
        Establish context = () => entity = new PropertyEntity();
        Because of = () => property = entity.Properties.FirstOrDefault(q => q.PropertyName == "Surname");

        It should_set_table_name_automatically = () => entity.TableName.ShouldEqual("PropertyEntity");
        It should_set_property_name_to_property_specified = () => property.PropertyName.ShouldEqual("Surname");
        It should_set_column_name = () => property.ColumnName.ShouldEqual("PersonsSurname");
        It should_set_type_to_string = () => property.Type.ShouldEqual(typeof(string));
        It should_set_property_type = () => property.PropertyType.ShouldEqual(PropertyType.Property);

        private static PropertyEntity entity;
        private static Property property;

        public class PropertyEntity : Entity
        {
            public PropertyEntity()
            {
                base.Property<string>("Surname", "PersonsSurname");
            }
        }
    }

    public class When_mapping_to_a_different_table_name
    {
        Because of = () => entity = new CustomTableEntity();
        It should_set_property_name_to_property_specified = () => entity.TableName.ShouldEqual("CustomTable");

        private static CustomTableEntity entity;

        public class CustomTableEntity : Entity
        {
            public CustomTableEntity()
            {
                TableName = "CustomTable";
            }
        }
    }
}