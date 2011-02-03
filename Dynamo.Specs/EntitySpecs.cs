using System.Collections.Generic;
using System.Linq;
using Dynamo.Specs.Fixtures;
using Machine.Specifications;

namespace Dynamo.Specs
{
    public class Whem_mapping_entity_with_has_many
    {
        Establish context = () => entity = new HasManyEntity();
        Because of = () => property = entity.Properties.FirstOrDefault(q => q.PropertyName == "Company");
        It should_set_property_name_to_type_name = () => property.PropertyName.ShouldEqual("Company");
        It should_set_column_name = () => property.ColumnName.ShouldEqual("HasManyEntity_Id");
        It should_set_type__to_Ilist_of_Company = () => property.Type.ShouldEqual(typeof(List<Company>));
        It should_set_property_type= () => property.PropertyType.ShouldEqual(PropertyType.HasMany);
        
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

    public class Whem_mapping_entity_with_has_many_and_custom_property_name
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

    public class Whem_mapping_entity_with_has_many_and_custom_column_name
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
}